using GisanParkGolf_Core.Services;
using GisanParkGolf_Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.PlayerPage
{
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

        public PaginatedList<PlayerGameListItem> GameList { get; set; }

        public async Task OnGetAsync()
        {
            // 실제 사용자 아이디 가져오기 (환경에 맞게 수정)
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("로그인 정보가 없습니다.");
            }

            GameList = await _playerGameService.GetMyGamesAsync(userId, SearchField, SearchKeyword, PageIndex, PageSize);
        }
    }
}