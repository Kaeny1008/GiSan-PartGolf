using GisanParkGolf.Pages.Manager.Services;
using GisanParkGolf.Pages.Manager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf.Pages.Manager
{
    [Authorize(Policy = "ManagerOnly")]
    public class ScoreMissingReportModel : PageModel
    {
        private readonly IScoreMissingReportServece _scoreMissingReportService;

        public ScoreMissingReportModel(IScoreMissingReportServece scoreMissingReportService)
        {
            _scoreMissingReportService = scoreMissingReportService;
        }

        public List<MissingScoreInfoViewModel> MissingScoreList { get; set; } = new();
        public List<GameInfoViewModel> GameList { get; set; } = new();
        [BindProperty(SupportsGet = true)] public string? SelectedGameCode { get; set; }

        public void OnGet(string? gameCode)
        {
            // 1. ��ȸ ��� ��ȸ
            GameList = _scoreMissingReportService.GetRunningOrPendingGames();

            // 2. gameCode�� ������, ���������� ���� ���� (�����̷�Ʈ ����)
            SelectedGameCode = string.IsNullOrEmpty(gameCode) && GameList.Any()
                ? GameList[0].GameCode
                : gameCode;

            // 3. ���� ���� ��� ��ȸ
            MissingScoreList = _scoreMissingReportService.GetMissingScoreList(SelectedGameCode);
        }
    }
}