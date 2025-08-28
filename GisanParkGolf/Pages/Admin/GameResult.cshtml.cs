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
            // "Score Confirmed" ������ ��ȸ ����Ʈ ��������
            ScoreConfirmedGames = _gameResultService.GetScoreConfirmedGames();

            // ���õ� ��ȸ ��� ��������
            if (!string.IsNullOrWhiteSpace(SelectedGameCode))
            {
                Participants = _gameResultService.GetGameResults(SelectedGameCode);
            }
        }

        public IActionResult OnPostEndGame()
        {
            if (string.IsNullOrWhiteSpace(SelectedGameCode))
            {
                TempData["ErrorMessage"] = "��ȸ �ڵ尡 ��ȿ���� �ʽ��ϴ�.";
                return RedirectToPage();
            }

            try
            {
                _gameResultService.EndGame(SelectedGameCode);
                TempData["SuccessMessage"] = "��ȸ�� ���������� ����Ǿ����ϴ�.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"��ȸ ���� �� ������ �߻��߽��ϴ�: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}