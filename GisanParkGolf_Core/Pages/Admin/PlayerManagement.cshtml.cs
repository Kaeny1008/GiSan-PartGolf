using ClosedXML.Excel; // 엑셀 라이브러리 네임스페이스
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GisanParkGolf_Core.Pages.Admin
{
    public class PlayerManagementModel : PageModel
    {
        private readonly MyDbContext _context;

        public PlayerManagementModel(MyDbContext context)
        {
            _context = context;
        }

        // --- 검색/페이징/데이터 속성들 ---
        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ReadyUserOnly { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 20; // 기존 PageSize 20으로 변경
        public int TotalUsers { get; set; }
        public IList<SYS_Users> Users { get; set; } = new List<SYS_Users>();

        // 페이지 로드 및 검색/페이징 처리
        public async Task OnGetAsync()
        {
            // 1. 재사용 가능한 쿼리 빌더 메서드 호출
            var usersIQ = GetFilteredUsersQuery();

            // 2. 총 개수 계산
            TotalUsers = await usersIQ.CountAsync();

            // 3. 페이징 처리하여 데이터 가져오기
            Users = await usersIQ.OrderByDescending(u => u.UserRegistrationDate)
                                 .Skip((CurrentPage - 1) * PageSize)
                                 .Take(PageSize)
                                 .ToListAsync();
        }

        // 엑셀 저장 버튼 클릭 시 호출될 핸들러
        public async Task<IActionResult> OnGetExcelAsync()
        {
            // 1. 페이징 없이 필터링된 모든 데이터를 가져옵니다.
            var usersIQ = GetFilteredUsersQuery();
            var users = await usersIQ.OrderByDescending(u => u.UserRegistrationDate).ToListAsync();

            // 2. ClosedXML을 사용하여 엑셀 파일 생성
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("선수 목록");

                // 헤더 추가
                worksheet.Cell(1, 1).Value = "아이디";
                worksheet.Cell(1, 2).Value = "이름";
                worksheet.Cell(1, 3).Value = "상태";
                worksheet.Cell(1, 4).Value = "등록일";

                // 데이터 추가
                int currentRow = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(currentRow, 1).Value = user.UserId;
                    worksheet.Cell(currentRow, 2).Value = user.UserName;
                    worksheet.Cell(currentRow, 3).Value = user.UserWClass;
                    worksheet.Cell(currentRow, 4).Value = user.UserRegistrationDate;
                    currentRow++;
                }

                // 스타일 조정 (선택 사항)
                worksheet.Columns().AdjustToContents();

                // 3. 메모리 스트림에 엑셀 파일을 저장
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"GisanParkGolf_Players_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    // 4. 파일 다운로드 Result 반환
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // 검색 조건에 따라 IQueryable을 반환하는 헬퍼 메서드 (코드 중복 방지)
        private IQueryable<SYS_Users> GetFilteredUsersQuery()
        {
            IQueryable<SYS_Users> usersIQ = _context.SYS_Users.AsQueryable();

            if (ReadyUserOnly)
            {
                usersIQ = usersIQ.Where(u => u.UserWClass == "W");
            }

            if (!string.IsNullOrEmpty(SearchQuery) && !string.IsNullOrEmpty(SearchField))
            {
                switch (SearchField)
                {
                    case "UserName":
                        usersIQ = usersIQ.Where(u => u.UserName.Contains(SearchQuery));
                        break;
                    case "UserId":
                        usersIQ = usersIQ.Where(u => u.UserId.Contains(SearchQuery));
                        break;
                    case "UserWClass":
                        usersIQ = usersIQ.Where(u => u.UserWClass.Contains(SearchQuery));
                        break;
                }
            }
            return usersIQ;
        }
    }
}