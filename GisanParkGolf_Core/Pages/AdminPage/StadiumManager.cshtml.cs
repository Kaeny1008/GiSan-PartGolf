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
        // �ڡڡ� ���� ���� �ϳ��� ��� ���� ó��! �ڡڡ�
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
                TempData["ErrorTitle"] = "���� ����";
                TempData["ErrorMessage"] = "����� ������ �ٽ� Ȯ�����ּ���.";
                await OnGetAsync(null, null, showNewForm: true);
                return Page();
            }
            await _stadiumService.CreateStadiumAsync(NewStadium);

            TempData["SuccessTitle"] = "����Ȯ��";
            TempData["SuccessMessage"] = "�ű� ����� ��� �Ϸ�.";
            return RedirectToPage(new { stadiumCode = NewStadium.StadiumCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostCreateCourseAsync(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(NewCourse.CourseName) || NewCourse.HoleCount <= 0)
            {
                TempData["ErrorTitle"] = "���� ����";
                TempData["ErrorMessage"] = "�ڽ� �̸��� Ȧ ���� �ٽ� Ȯ�����ּ���.";
                await OnGetAsync(stadiumCode, null);
                return Page();
            }
            NewCourse.StadiumCode = stadiumCode;
            await _stadiumService.CreateCourseAsync(NewCourse);

            TempData["SuccessTitle"] = "����Ȯ��";
            TempData["SuccessMessage"] = "�ڽ� ��� �Ϸ�! ���� Ȧ ������ �Է��ϼ���.";
            return RedirectToPage(new { stadiumCode, courseCode = NewCourse.CourseCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostSaveHolesAsync(int courseCode, string stadiumCode)
        {
            if (HoleDetails == null || !HoleDetails.Any() || HoleDetails.Any(h => string.IsNullOrWhiteSpace(h.HoleName) || h.Distance <= 0 || h.Par <= 0))
            {
                TempData["ErrorTitle"] = "���� ����";
                TempData["ErrorMessage"] = "Ȧ ������ ��� �Է����ּ���.";
                await OnGetAsync(stadiumCode, courseCode);
                return Page();
            }

            await _stadiumService.SaveHolesAsync(courseCode, HoleDetails);

            TempData["SuccessTitle"] = "����Ȯ��";
            TempData["SuccessMessage"] = "Ȧ ������ ���������� ����Ǿ����ϴ�.";
            return RedirectToPage(new { stadiumCode, courseCode, PageIndex, PageSize, SearchField, SearchQuery });
        }

        public async Task<IActionResult> OnPostDeleteStadiumAsync(string stadiumCode)
        {
            try
            {
                await _stadiumService.DeleteStadiumAsync(stadiumCode);
                TempData["SuccessTitle"] = "���� Ȯ��";
                TempData["SuccessMessage"] = "������� ���������� �����Ǿ����ϴ�.";
            }
            catch (Exception ex)
            {
                TempData["ErrorTitle"] = "���� ����";
                TempData["ErrorMessage"] = $"����� ���� �� ������ �߻��߽��ϴ�: {ex.Message}";
                // �ʿ�� �α� ex
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
                TempData["SuccessTitle"] = "���� Ȯ��";
                TempData["SuccessMessage"] = "�ڽ��� ���������� �����Ǿ����ϴ�.";
            }
            catch (Exception ex)
            {
                TempData["ErrorTitle"] = "���� ����";
                TempData["ErrorMessage"] = $"�ڽ� ���� �� ������ �߻��߽��ϴ�: {ex.Message}";
                await OnGetAsync(stadiumCode, null);
                return Page();
            }

            return RedirectToPage(new { stadiumCode, PageIndex, PageSize, SearchField, SearchQuery });
        }
    }
}