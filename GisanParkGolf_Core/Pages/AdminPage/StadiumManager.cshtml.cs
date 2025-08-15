using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Services.AdminPage;
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
            if (!ModelState.IsValid)
            {
                await OnGetAsync(null, null, showNewForm: true);
                return Page();
            }
            await _stadiumService.CreateStadiumAsync(NewStadium);
            return RedirectToPage(new { stadiumCode = NewStadium.StadiumCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostCreateCourseAsync(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(NewCourse.CourseName) || NewCourse.HoleCount <= 0)
            {
                await OnGetAsync(stadiumCode, null);
                return Page();
            }
            NewCourse.StadiumCode = stadiumCode;
            await _stadiumService.CreateCourseAsync(NewCourse);
            return RedirectToPage(new { stadiumCode, courseCode = NewCourse.CourseCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostSaveHolesAsync(int courseCode, string stadiumCode)
        {
            await _stadiumService.SaveHolesAsync(courseCode, HoleDetails);
            return RedirectToPage(new { stadiumCode, courseCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostDeleteStadiumAsync(string stadiumCode)
        {
            await _stadiumService.DeleteStadiumAsync(stadiumCode);
            return RedirectToPage(new { PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostDeleteCourseAsync(int courseCode, string stadiumCode)
        {
            await _stadiumService.DeleteCourseAsync(courseCode);
            return RedirectToPage(new { stadiumCode, PageIndex, PageSize, SearchField, SearchQuery });
        }
    }
}