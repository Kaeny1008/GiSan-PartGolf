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

        public List<CourseScoreCard> ScoreCardCourses { get; set; } = new();

        public void OnGet()
        {
            // 예시 데이터: 실제로는 DB에서 대회/코스/홀/참가자 정보 불러옴
            ScoreCardCourses = new List<CourseScoreCard>
            {
                new CourseScoreCard
                {
                    CourseName = "A 코스",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    Participants = new List<ScoreCardParticipant>
                    {
                        new ScoreCardParticipant { CourseOrder=1, ParticipantName="회원2", ParticipantId="A122" },
                        new ScoreCardParticipant { CourseOrder=2, ParticipantName="회원4", ParticipantId="A125" },
                        new ScoreCardParticipant { CourseOrder=3, ParticipantName="회원3", ParticipantId="A123" },
                        new ScoreCardParticipant { CourseOrder=4, ParticipantName="회원44", ParticipantId="A124" },
                        new ScoreCardParticipant { CourseOrder=5, ParticipantName="관리자", ParticipantId="1" }
                    }
                },
                new CourseScoreCard
                {
                    CourseName = "B 코스",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    Participants = new List<ScoreCardParticipant>
                    {
                        new ScoreCardParticipant { CourseOrder=1, ParticipantName="회원2", ParticipantId="A122" },
                        new ScoreCardParticipant { CourseOrder=2, ParticipantName="회원4", ParticipantId="A125" },
                        new ScoreCardParticipant { CourseOrder=3, ParticipantName="회원3", ParticipantId="A123" },
                        new ScoreCardParticipant { CourseOrder=4, ParticipantName="회원44", ParticipantId="A124" },
                        new ScoreCardParticipant { CourseOrder=5, ParticipantName="관리자", ParticipantId="1" }
                    }
                }
            };
        }

        public class CourseScoreCard
        {
            public string CourseName { get; set; }
            public List<int> HoleNumbers { get; set; } = new();
            public List<ScoreCardParticipant> Participants { get; set; } = new();
        }

        public class ScoreCardParticipant
        {
            public int CourseOrder { get; set; }
            public string ParticipantName { get; set; }
            public string ParticipantId { get; set; }
            public Dictionary<int, int> Scores { get; set; } = new(); // key: 홀번호, value: 점수
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