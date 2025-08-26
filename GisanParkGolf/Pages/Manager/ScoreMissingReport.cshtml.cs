using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf.Pages.Manager
{
    public class ScoreMissingReportModel : PageModel
    {
        public List<MissingScoreInfo> MissingScoreList { get; set; }

        public class MissingScoreInfo
        {
            public string GameCode { get; set; }
            public string GameName { get; set; }
            public string TeamNumber { get; set; }
            public string ParticipantName { get; set; }
            public string ParticipantId { get; set; }
            public List<string> MissingCoursesAndHoles { get; set; } // ����: ["A�ڽ�-3Ȧ", "B�ڽ�-5Ȧ"]
        }

        public void OnGet()
        {
        }
    }
}
