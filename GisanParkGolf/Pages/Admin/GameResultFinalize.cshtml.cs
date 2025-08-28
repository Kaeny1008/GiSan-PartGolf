using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GisanParkGolf.Pages.Admin.Services;
using GisanParkGolf.Pages.Manager.ViewModels;
using GisanParkGolf.Pages.Admin.ViewModels;

namespace GisanParkGolf.Pages.Admin
{
    public class GameResultFinalizeModel : PageModel
    {
        private readonly IGameResultFinalizeService _gameResultService;

        public GameResultFinalizeModel(IGameResultFinalizeService gameResultService)
        {
            _gameResultService = gameResultService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SelectedGameCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
        public List<GameInfoViewModel> PagedGames { get; set; } = new();
        public List<GameInfoViewModel> ScoreConfirmedGames { get; set; } = new();
        public List<ParticipantResultViewModel> Participants { get; set; } = new();
        public List<ParticipantResultViewModel> Top3 => Participants.OrderByDescending(p => p.UserScore).Take(3).ToList();

        public void OnGet()
        {
            // 검색 및 페이징 처리
            var allGames = _gameResultService.GetScoreConfirmedGames();

            if (!string.IsNullOrWhiteSpace(SearchField) && !string.IsNullOrWhiteSpace(SearchQuery))
            {
                allGames = allGames.Where(game =>
                    (SearchField == "GameName" && (game.GameName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false)) ||
                    (SearchField == "GameCode" && (game.GameCode?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false))
                ).ToList();
            }

            TotalPages = (int)Math.Ceiling(allGames.Count / (double)PageSize);
            PagedGames = allGames.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

            // 선택된 대회의 결과 가져오기
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