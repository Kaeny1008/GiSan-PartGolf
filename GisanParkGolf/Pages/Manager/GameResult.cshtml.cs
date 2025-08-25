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

        // 검색 및 페이징
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
            // 예시: 옵션, 전체 데이터 생성
            GameOptions = new List<SelectListItem> { new("2024년 대회", "G2024"), new("2025년 대회", "G2025") };
            CourseOptions = new List<SelectListItem> { new("A코스", "A"), new("B코스", "B") };
            HoleOptions = new List<SelectListItem> { new("1번홀", "1"), new("2번홀", "2") };

            HoleResults = new List<HoleResultRow>
        {
            new() { CourseName="A", HoleNumber=1, TeamNumber="T1", CourseOrder=1, ParticipantName="홍길동", ParticipantId="U1", Score=3, Note="" },
            new() { CourseName="A", HoleNumber=1, TeamNumber="T2", CourseOrder=2, ParticipantName="김철수", ParticipantId="U2", Score=4, Note="" },
            new() { CourseName="B", HoleNumber=2, TeamNumber="T3", CourseOrder=3, ParticipantName="이영희", ParticipantId="U3", Score=5, Note="" }
        };

            // 검색 필터
            var filtered = HoleResults
                .Where(r => (string.IsNullOrEmpty(SearchParticipant) || r.ParticipantName.Contains(SearchParticipant))
                         && (string.IsNullOrEmpty(SearchTeam) || r.TeamNumber.Contains(SearchTeam)))
                .ToList();

            TotalCount = filtered.Count;
            PagedHoleResults = filtered.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }

        public IActionResult OnPost()
        {
            // 저장 로직
            TempData["SuccessMessage"] = "결과가 저장되었습니다.";
            return Page();
        }

        public JsonResult OnGetCourses(string gameCode)
        {
            // 실제 DB에서 해당 대회 코스 목록 조회
            var list = new List<SelectListItem> {
        new("A코스", "A"),
        new("B코스", "B")
    };
            return new JsonResult(list.Select(x => new { value = x.Value, text = x.Text }));
        }

        public JsonResult OnGetHoles(string gameCode, string courseName)
        {
            // 실제 DB에서 해당 코스의 홀 목록 조회
            var list = new List<SelectListItem> {
        new("1번홀", "1"),
        new("2번홀", "2")
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