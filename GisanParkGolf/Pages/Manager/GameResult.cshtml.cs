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

        public List<CourseScoreCard> ScoreCardCourses { get; set; } = new();

        public void OnGet()
        {
            // ���� ������: �����δ� DB���� ��ȸ/�ڽ�/Ȧ/������ ���� �ҷ���
            ScoreCardCourses = new List<CourseScoreCard>
            {
                new CourseScoreCard
                {
                    CourseName = "A �ڽ�",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    Participants = new List<ScoreCardParticipant>
                    {
                        new ScoreCardParticipant { CourseOrder=1, ParticipantName="ȸ��2", ParticipantId="A122" },
                        new ScoreCardParticipant { CourseOrder=2, ParticipantName="ȸ��4", ParticipantId="A125" },
                        new ScoreCardParticipant { CourseOrder=3, ParticipantName="ȸ��3", ParticipantId="A123" },
                        new ScoreCardParticipant { CourseOrder=4, ParticipantName="ȸ��44", ParticipantId="A124" },
                        new ScoreCardParticipant { CourseOrder=5, ParticipantName="������", ParticipantId="1" }
                    }
                },
                new CourseScoreCard
                {
                    CourseName = "B �ڽ�",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    Participants = new List<ScoreCardParticipant>
                    {
                        new ScoreCardParticipant { CourseOrder=1, ParticipantName="ȸ��2", ParticipantId="A122" },
                        new ScoreCardParticipant { CourseOrder=2, ParticipantName="ȸ��4", ParticipantId="A125" },
                        new ScoreCardParticipant { CourseOrder=3, ParticipantName="ȸ��3", ParticipantId="A123" },
                        new ScoreCardParticipant { CourseOrder=4, ParticipantName="ȸ��44", ParticipantId="A124" },
                        new ScoreCardParticipant { CourseOrder=5, ParticipantName="������", ParticipantId="1" }
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
            public Dictionary<int, int> Scores { get; set; } = new(); // key: Ȧ��ȣ, value: ����
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