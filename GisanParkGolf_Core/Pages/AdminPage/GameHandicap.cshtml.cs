using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Services.AdminPage;
using GisanParkGolf_Core.ViewModels.AdminPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.AdminPage
{
    [Authorize(Policy = "AdminOnly")]
    public class GameHandicapModel : PageModel
    {
        private readonly IHandicapService _handicapService;

        public GameHandicapModel(IHandicapService handicapService)
        {
            _handicapService = handicapService;
        }

        // 페이지에 표시될 데이터 (PaginatedList 사용)
        public PaginatedList<HandicapViewModel> Handicaps { get; set; } = null!;

        // 검색 필드 (이름 또는 ID)
        [BindProperty(SupportsGet = true)]
        public string SearchField { get; set; } = "UserName"; // 기본 검색 조건은 '이름'

        // 검색어
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        // 페이지당 보여줄 행 수
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10; // 기본 10개

        // 현재 페이지 번호
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1; // 기본 1페이지

        // 성공/실패 메시지를 전달하기 위한 TempData
        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        // 페이지가 GET 방식으로 요청될 때 실행되는 메서드
        public async Task OnGetAsync()
        {
            // 서비스 계층을 통해 페이징 및 검색 조건에 맞는 데이터를 가져옴
            Handicaps = await _handicapService.GetPlayerHandicapsAsync(SearchField, SearchQuery, PageIndex, PageSize);
        }

        // 개별 핸디캡을 업데이트할 때 실행되는 메서드
        public async Task<IActionResult> OnPostUpdateAsync(string userId, int age, int newHandicap, string newSource)
        {
            var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";

            try
            {
                var result = await _handicapService.UpdateHandicapAsync(userId, age, newHandicap, newSource, adminName);
                if (result)
                {
                    SuccessMessage = $"({userId}) 님의 핸디캡이 성공적으로 업데이트되었습니다.";
                }
                else
                {
                    SuccessMessage = "변경된 내용이 없어 업데이트하지 않았습니다.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"업데이트 중 오류가 발생했습니다: {ex.Message}";
            }

            // 현재 페이지 및 검색 상태를 유지하며 페이지를 새로고침
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }

        // 모든 핸디캡을 일괄 재계산할 때 실행되는 메서드
        public async Task<IActionResult> OnPostRecalculateAllAsync()
        {
            var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
            try
            {
                var updatedCount = await _handicapService.RecalculateAllHandicapsAsync(adminName);
                SuccessMessage = $"총 {updatedCount}명의 핸디캡이 자동으로 재계산되었습니다.";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"전체 재계산 중 오류가 발생했습니다: {ex.Message}";
            }

            // 현재 페이지 및 검색 상태를 유지하며 페이지를 새로고침
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }

        // .cshtml 파일에서 '자동' 핸디캡 값을 미리 보여주기 위한 헬퍼 메서드
        public int CalculateHandicapByAge(int age)
        {
            return HandicapCalculator.CalculateByAge(age);
        }
    }
}