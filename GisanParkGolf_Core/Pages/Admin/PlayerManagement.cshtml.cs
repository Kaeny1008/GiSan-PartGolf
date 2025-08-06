using ClosedXML.Excel; // ���� ���̺귯�� ���ӽ����̽�
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

        // --- �˻�/����¡/������ �Ӽ��� ---
        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ReadyUserOnly { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 20; // ���� PageSize 20���� ����
        public int TotalUsers { get; set; }
        public IList<SYS_Users> Users { get; set; } = new List<SYS_Users>();

        // ������ �ε� �� �˻�/����¡ ó��
        public async Task OnGetAsync()
        {
            // 1. ���� ������ ���� ���� �޼��� ȣ��
            var usersIQ = GetFilteredUsersQuery();

            // 2. �� ���� ���
            TotalUsers = await usersIQ.CountAsync();

            // 3. ����¡ ó���Ͽ� ������ ��������
            Users = await usersIQ.OrderByDescending(u => u.UserRegistrationDate)
                                 .Skip((CurrentPage - 1) * PageSize)
                                 .Take(PageSize)
                                 .ToListAsync();
        }

        // ���� ���� ��ư Ŭ�� �� ȣ��� �ڵ鷯
        public async Task<IActionResult> OnGetExcelAsync()
        {
            // 1. ����¡ ���� ���͸��� ��� �����͸� �����ɴϴ�.
            var usersIQ = GetFilteredUsersQuery();
            var users = await usersIQ.OrderByDescending(u => u.UserRegistrationDate).ToListAsync();

            // 2. ClosedXML�� ����Ͽ� ���� ���� ����
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("���� ���");

                // ��� �߰�
                worksheet.Cell(1, 1).Value = "���̵�";
                worksheet.Cell(1, 2).Value = "�̸�";
                worksheet.Cell(1, 3).Value = "����";
                worksheet.Cell(1, 4).Value = "�����";

                // ������ �߰�
                int currentRow = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(currentRow, 1).Value = user.UserId;
                    worksheet.Cell(currentRow, 2).Value = user.UserName;
                    worksheet.Cell(currentRow, 3).Value = user.UserWClass;
                    worksheet.Cell(currentRow, 4).Value = user.UserRegistrationDate;
                    currentRow++;
                }

                // ��Ÿ�� ���� (���� ����)
                worksheet.Columns().AdjustToContents();

                // 3. �޸� ��Ʈ���� ���� ������ ����
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"GisanParkGolf_Players_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    // 4. ���� �ٿ�ε� Result ��ȯ
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // �˻� ���ǿ� ���� IQueryable�� ��ȯ�ϴ� ���� �޼��� (�ڵ� �ߺ� ����)
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