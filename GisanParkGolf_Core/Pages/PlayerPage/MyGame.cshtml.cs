using GisanParkGolf_Core.Services;
using GisanParkGolf_Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.PlayerPage
{
    [Authorize]
    public class MyGameModel : PageModel
    {
        private readonly IPlayerGameService _playerGameService;

        public MyGameModel(IPlayerGameService playerGameService)
        {
            _playerGameService = playerGameService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public PaginatedList<PlayerGameListItemViewModel> GameList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            GameList = await _playerGameService.GetMyGamesAsync(
                userId, SearchField, SearchKeyword, PageIndex, PageSize
            );

            return Page();
        }
    }
}