using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GiSanParkGolf.Pages.Manager
{
    public class GameResultModel : PageModel
    {
        [BindProperty] public string GameCode { get; set; }
        [BindProperty] public string CourseName { get; set; }
        [BindProperty] public int HoleNumber { get; set; }

        // �˻� �� ����¡
        [BindProperty(SupportsGet = true)] public string SearchParticipant { get; set; }
        [BindProperty(SupportsGet = true)] public string SearchTeam { get; set; }
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public List<SelectListItem> GameOptions { get; set; } = new();
        public List<SelectListItem> CourseOptions { get; set; } = new();
        public List<SelectListItem> HoleOptions { get; set; } = new();

        public List<HoleResultRow> HoleResults { get; set; } = new();
        public List<HoleResultRow> PagedHoleResults { get; set; } = new();

        public void OnGet()
        {
            // ����: �ɼ�, ��ü ������ ����
            GameOptions = new List<SelectListItem> { new("2024�� ��ȸ", "G2024"), new("2025�� ��ȸ", "G2025") };
            CourseOptions = new List<SelectListItem> { new("A�ڽ�", "A"), new("B�ڽ�", "B") };
            HoleOptions = new List<SelectListItem> { new("1��Ȧ", "1"), new("2��Ȧ", "2") };

            HoleResults = new List<HoleResultRow>
        {
            new() { CourseName="A", HoleNumber=1, TeamNumber="T1", CourseOrder=1, ParticipantName="ȫ�浿", ParticipantId="U1", Score=3, Note="" },
            new() { CourseName="A", HoleNumber=1, TeamNumber="T2", CourseOrder=2, ParticipantName="��ö��", ParticipantId="U2", Score=4, Note="" },
            new() { CourseName="B", HoleNumber=2, TeamNumber="T3", CourseOrder=3, ParticipantName="�̿���", ParticipantId="U3", Score=5, Note="" }
        };

            // �˻� ����
            var filtered = HoleResults
                .Where(r => (string.IsNullOrEmpty(SearchParticipant) || r.ParticipantName.Contains(SearchParticipant))
                         && (string.IsNullOrEmpty(SearchTeam) || r.TeamNumber.Contains(SearchTeam)))
                .ToList();

            TotalCount = filtered.Count;
            PagedHoleResults = filtered.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }

        public IActionResult OnPost()
        {
            // ���� ����
            TempData["SuccessMessage"] = "����� ����Ǿ����ϴ�.";
            return Page();
        }

        public JsonResult OnGetCourses(string gameCode)
        {
            // ���� DB���� �ش� ��ȸ �ڽ� ��� ��ȸ
            var list = new List<SelectListItem> {
        new("A�ڽ�", "A"),
        new("B�ڽ�", "B")
    };
            return new JsonResult(list.Select(x => new { value = x.Value, text = x.Text }));
        }

        public JsonResult OnGetHoles(string gameCode, string courseName)
        {
            // ���� DB���� �ش� �ڽ��� Ȧ ��� ��ȸ
            var list = new List<SelectListItem> {
        new("1��Ȧ", "1"),
        new("2��Ȧ", "2")
    };
            return new JsonResult(list.Select(x => new { value = x.Value, text = x.Text }));
        }

        public class HoleResultRow
        {
            public string CourseName { get; set; }
            public int HoleNumber { get; set; }
            public string TeamNumber { get; set; }
            public int CourseOrder { get; set; }
            public string ParticipantName { get; set; }
            public string ParticipantId { get; set; }
            public int Score { get; set; }
            public string Note { get; set; }
        }
    }
}