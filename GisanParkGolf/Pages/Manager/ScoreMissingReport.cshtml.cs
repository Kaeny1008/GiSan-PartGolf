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
            // 1. 대회 목록 조회
            GameList = _scoreMissingReportService.GetRunningOrPendingGames();

            // 2. gameCode가 없으면, 서버에서만 변수 세팅 (리다이렉트 없이)
            SelectedGameCode = string.IsNullOrEmpty(gameCode) && GameList.Any()
                ? GameList[0].GameCode
                : gameCode;

            // 3. 누락 점수 목록 조회
            MissingScoreList = _scoreMissingReportService.GetMissingScoreList(SelectedGameCode);
        }
    }
}