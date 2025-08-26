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
            // ����: GameList�� DB���� ��ȸ
            GameList = GetGameList();

            SelectedGameCode = gameCode;

            // ����: ���� ��Ȳ�� DB���� ��ȸ, gameCode�� ����
            MissingScoreList = GetMissingScores(gameCode);
        }

        // TODO: ���� DB���� ��ȸ�ϵ��� ����
        private List<GameInfo> GetGameList() => new List<GameInfo>
        {
            new GameInfo { GameCode = "G001", GameName = "ĥ��δ�ȸ" },
            new GameInfo { GameCode = "G002", GameName = "���̽����" },
        };

        private List<MissingScoreInfo> GetMissingScores(string gameCode)
        {
            // ���� ������
            var all = new List<MissingScoreInfo>
            {
                new MissingScoreInfo {
                    GameCode = "G001", GameName = "ĥ��δ�ȸ", TeamNumber = "T01",
                    ParticipantName = "ȫ�浿", ParticipantId = "user01", MissingCoursesAndHoles = new List<string> { "A�ڽ�-3Ȧ" }
                },
                new MissingScoreInfo {
                    GameCode = "G002", GameName = "���̽����", TeamNumber = "T02",
                    ParticipantName = "��ö��", ParticipantId = "user02", MissingCoursesAndHoles = new List<string> { "B�ڽ�-5Ȧ", "C�ڽ�-1Ȧ" }
                },
            };
            if (string.IsNullOrEmpty(gameCode))
                return all;
            return all.Where(x => x.GameCode == gameCode).ToList();
        }
    }
}