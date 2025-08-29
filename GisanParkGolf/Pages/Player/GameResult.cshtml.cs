using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Player.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace GisanParkGolf.Pages.Player
{
    public class GameResultModel : PageModel
    {
        private readonly IGameResutService _gameService;

        public GameResultModel(IGameResutService gameService)
        {
            _gameService = gameService;
        }

        public PaginatedList<GameResultViewModel>? AllResults { get; set; }
        public GameResultViewModel? MyResult { get; set; }
        public List<HoleResultViewModel> HoleResults { get; set; } = new();
        public List<CourseViewModel> Courses { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? SearchField { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)] public string? GameCode { get; set; }

        public async Task OnGetAsync(string gameCode)
        {
            if (string.IsNullOrEmpty(SearchQuery) && string.IsNullOrEmpty(SearchField))
            {
                SearchField = null;
                SearchQuery = null;
                PageIndex = 1;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                MyResult = _gameService.GetMyResult(gameCode, userId);
                HoleResults = _gameService.GetHoleResults(gameCode, userId);
                Courses = _gameService.GetCourses(gameCode);
            }

            AllResults = await _gameService.GetFilteredResults(gameCode, SearchField, SearchQuery, PageIndex, PageSize);
        }
    }
}
