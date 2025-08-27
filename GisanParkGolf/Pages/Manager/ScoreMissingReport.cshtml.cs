using GisanParkGolf.Pages.Manager.Services;
using GisanParkGolf.Pages.Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf.Pages.Manager
{
    public class ScoreMissingReportModel : PageModel
    {
        private readonly IScoreMissingReportServece _reportService;

        public ScoreMissingReportModel(IScoreMissingReportServece reportService)
        {
            _reportService = reportService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SelectedGameCode { get; set; }

        public List<GameInfoViewModel> GameList { get; set; } = new List<GameInfoViewModel>();
        public List<MissingScoreInfoViewModel> MissingScoreList { get; set; } = new List<MissingScoreInfoViewModel>();

        public void OnGet()
        {
            GameList = _reportService.GetRunningOrPendingGames();
            if (string.IsNullOrWhiteSpace(SelectedGameCode))
            {
                MissingScoreList = new List<MissingScoreInfoViewModel>();
            }
            else
            {
                MissingScoreList = _reportService.GetMissingScoreList(SelectedGameCode);
            }
        }
    }
}