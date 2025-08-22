using GisanParkGolf.Helpers;
using GisanParkGolf.Services.PlayerPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using GisanParkGolf.Pages.PlayerPage.ViewModels;

namespace GiSanParkGolf.Pages.PlayerPage
{
    [Authorize]
    public class MyGameModel : PageModel
    {
        private readonly IJoinGameService _gameService;

        public MyGameModel(IJoinGameService gameService)
        {
            _gameService = gameService;
        }

        public PaginatedList<MyGameListModel>? GameList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; } = "GameName";
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                GameList = new PaginatedList<MyGameListModel>(new List<MyGameListModel>(), 0, PageIndex, PageSize);
                return;
            }

            GameList = await _gameService.GetMyGameListAsync(userId, SearchField, SearchQuery, PageIndex, PageSize);
        }
    }
}