using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GisanParkGolf.Pages.Admin.Services;
using GisanParkGolf.Pages.Manager.ViewModels;
using GisanParkGolf.Pages.Admin.ViewModels;

namespace GisanParkGolf.Pages.Admin
{
    public class GameResultsModel : PageModel
    {
        private readonly IGameResultService _gameResultService;

        public GameResultsModel(IGameResultService gameResultService)
        {
            _gameResultService = gameResultService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SelectedGameCode { get; set; }

        public List<GameInfoViewModel> ScoreConfirmedGames { get; set; } = new();
        public List<ParticipantResultViewModel> Participants { get; set; } = new();
        public List<ParticipantResultViewModel> Top3 => Participants.OrderByDescending(p => p.UserScore).Take(3).ToList();

        public void OnGet()
        {
            // "Score Confirmed" 상태의 대회 리스트 가져오기
            ScoreConfirmedGames = _gameResultService.GetScoreConfirmedGames();

            // 선택된 대회 결과 가져오기
            if (!string.IsNullOrWhiteSpace(SelectedGameCode))
            {
                Participants = _gameResultService.GetGameResults(SelectedGameCode);
            }
        }

        public IActionResult OnPostEndGame()
        {
            if (string.IsNullOrWhiteSpace(SelectedGameCode))
            {
                TempData["ErrorMessage"] = "대회 코드가 유효하지 않습니다.";
                return RedirectToPage();
            }

            try
            {
                _gameResultService.EndGame(SelectedGameCode);
                TempData["SuccessMessage"] = "대회가 성공적으로 종료되었습니다.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"대회 종료 중 오류가 발생했습니다: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}