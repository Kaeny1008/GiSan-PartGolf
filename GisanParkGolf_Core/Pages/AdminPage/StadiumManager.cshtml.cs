using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Pages.AdminPage.AdminPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GiSanParkGolf.Pages.AdminPage
{
    [Authorize(Policy = "AdminOnly")]
    public class StadiumManagerModel : PageModel
    {
        // ★★★ 이제 서비스 하나로 모든 것을 처리! ★★★
        private readonly IStadiumService _stadiumService;

        public StadiumManagerModel(IStadiumService stadiumService)
        {
            _stadiumService = stadiumService;
            StadiumList = new PaginatedList<Stadium>(new List<Stadium>(), 0, 1, 10);
        }

        public PaginatedList<Stadium> StadiumList { get; set; }
        public Stadium? SelectedStadium { get; set; }
        public Course? SelectedCourse { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public Stadium NewStadium { get; set; } = new();
        [BindProperty]
        public Course NewCourse { get; set; } = new();
        [BindProperty]
        public List<Hole> HoleDetails { get; set; } = [];

        public async Task OnGetAsync(string? stadiumCode, int? courseCode, bool showNewForm = false)
        {
            StadiumList = await _stadiumService.GetStadiumsAsync(SearchField, SearchQuery, PageIndex, PageSize);

            if (!string.IsNullOrEmpty(stadiumCode))
            {
                SelectedStadium = await _stadiumService.GetStadiumByIdAsync(stadiumCode);
            }

            if (courseCode.HasValue)
            {
                SelectedCourse = await _stadiumService.GetCourseByIdAsync(courseCode.Value);
                if (SelectedCourse != null)
                {
                    HoleDetails = SelectedCourse.Holes.ToList();
                }
            }

            if (showNewForm)
            {
                ViewData["ShowNewStadiumForm"] = "True";
                SelectedStadium = null;
                SelectedCourse = null;
            }
        }

        public IActionResult OnGetNewStadium()
        {
            return RedirectToPage(new { showNewForm = true, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostCreateStadiumAsync()
        {
            ModelState.Clear();
            TryValidateModel(NewStadium);

            if (!ModelState.IsValid)
            {
                TempData["ErrorTitle"] = "저장 실패";
                TempData["ErrorMessage"] = "경기장 정보를 다시 확인해주세요.";
                await OnGetAsync(null, null, showNewForm: true);
                return Page();
            }
            await _stadiumService.CreateStadiumAsync(NewStadium);

            TempData["SuccessTitle"] = "저장확인";
            TempData["SuccessMessage"] = "신규 경기장 등록 완료.";
            return RedirectToPage(new { stadiumCode = NewStadium.StadiumCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostCreateCourseAsync(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(NewCourse.CourseName) || NewCourse.HoleCount <= 0)
            {
                TempData["ErrorTitle"] = "저장 실패";
                TempData["ErrorMessage"] = "코스 이름과 홀 수를 다시 확인해주세요.";
                await OnGetAsync(stadiumCode, null);
                return Page();
            }
            NewCourse.StadiumCode = stadiumCode;
            await _stadiumService.CreateCourseAsync(NewCourse);

            TempData["SuccessTitle"] = "저장확인";
            TempData["SuccessMessage"] = "코스 등록 완료! 이제 홀 정보를 입력하세요.";
            return RedirectToPage(new { stadiumCode, courseCode = NewCourse.CourseCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostSaveHolesAsync(int courseCode, string stadiumCode)
        {
            if (HoleDetails == null || !HoleDetails.Any() || HoleDetails.Any(h => string.IsNullOrWhiteSpace(h.HoleName) || h.Distance <= 0 || h.Par <= 0))
            {
                TempData["ErrorTitle"] = "저장 실패";
                TempData["ErrorMessage"] = "홀 정보를 모두 입력해주세요.";
                await OnGetAsync(stadiumCode, courseCode);
                return Page();
            }

            await _stadiumService.SaveHolesAsync(courseCode, HoleDetails);

            TempData["SuccessTitle"] = "저장확인";
            TempData["SuccessMessage"] = "홀 정보가 정상적으로 저장되었습니다.";
            return RedirectToPage(new { stadiumCode, courseCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostDeleteStadiumAsync(string stadiumCode)
        {
            try
            {
                await _stadiumService.DeleteStadiumAsync(stadiumCode);
                TempData["SuccessTitle"] = "삭제 확인";
                TempData["SuccessMessage"] = "경기장이 정상적으로 삭제되었습니다.";
            }
            catch (Exception ex)
            {
                TempData["ErrorTitle"] = "삭제 실패";
                TempData["ErrorMessage"] = $"경기장 삭제 중 오류가 발생했습니다: {ex.Message}";
                // 필요시 로그 ex
                await OnGetAsync(null, null);
                return Page();
            }

            return RedirectToPage(new { PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostDeleteCourseAsync(int courseCode, string stadiumCode)
        {
            try
            {
                await _stadiumService.DeleteCourseAsync(courseCode);
                TempData["SuccessTitle"] = "삭제 확인";
                TempData["SuccessMessage"] = "코스가 정상적으로 삭제되었습니다.";
            }
            catch (Exception ex)
            {
                TempData["ErrorTitle"] = "삭제 실패";
                TempData["ErrorMessage"] = $"코스 삭제 중 오류가 발생했습니다: {ex.Message}";
                await OnGetAsync(stadiumCode, null);
                return Page();
            }

            return RedirectToPage(new { stadiumCode, PageIndex, PageSize, SearchField, SearchQuery });
        }
    }
}