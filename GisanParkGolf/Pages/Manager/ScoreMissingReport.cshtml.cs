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
                MissingScoreList = _reportService.GetMissingScoreList(SelectedGameCode) ?? new List<MissingScoreInfoViewModel>();
            }
        }

        public async Task OnPostConfirmScoreAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SelectedGameCode))
                {
                    _reportService.ConfirmGameScore(SelectedGameCode);

                    await _reportService.SendNotificationToParticipantsAsync(SelectedGameCode);

                    TempData["SuccessMessage"] = "점수 확정이 완료되었으며, 참가자들에게 알림이 전송되었습니다.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"점수 확정 중 오류: {ex.Message}";
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "점수 확정 중 알 수 없는 오류가 발생했습니다.";
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }
    }
}