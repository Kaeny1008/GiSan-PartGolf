using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf.Pages.Manager
{
    public class ScoreMissingReportModel : PageModel
    {
        public List<MissingScoreInfo> MissingScoreList { get; set; } = new();
        public List<GameInfo> GameList { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? SelectedGameCode { get; set; }

        public class MissingScoreInfo
        {
            public string? GameCode { get; set; }
            public string? GameName { get; set; }
            public string? TeamNumber { get; set; }
            public string? ParticipantName { get; set; }
            public string? ParticipantId { get; set; }
            public List<string>? MissingCoursesAndHoles { get; set; }
        }

        public class GameInfo
        {
            public string? GameCode { get; set; }
            public string? GameName { get; set; }
        }

        public void OnGet(string gameCode)
        {
            // 예시: GameList를 DB에서 조회
            GameList = GetGameList();

            SelectedGameCode = gameCode;

            // 예시: 누락 현황을 DB에서 조회, gameCode로 필터
            MissingScoreList = GetMissingScores(gameCode);
        }

        // TODO: 실제 DB에서 조회하도록 구현
        private List<GameInfo> GetGameList() => new List<GameInfo>
        {
            new GameInfo { GameCode = "G001", GameName = "칠곡도민대회" },
            new GameInfo { GameCode = "G002", GameName = "구미시장배" },
        };

        private List<MissingScoreInfo> GetMissingScores(string gameCode)
        {
            // 예시 데이터
            var all = new List<MissingScoreInfo>
            {
                new MissingScoreInfo {
                    GameCode = "G001", GameName = "칠곡도민대회", TeamNumber = "T01",
                    ParticipantName = "홍길동", ParticipantId = "user01", MissingCoursesAndHoles = new List<string> { "A코스-3홀" }
                },
                new MissingScoreInfo {
                    GameCode = "G002", GameName = "구미시장배", TeamNumber = "T02",
                    ParticipantName = "김철수", ParticipantId = "user02", MissingCoursesAndHoles = new List<string> { "B코스-5홀", "C코스-1홀" }
                },
            };
            if (string.IsNullOrEmpty(gameCode))
                return all;
            return all.Where(x => x.GameCode == gameCode).ToList();
        }
    }
}