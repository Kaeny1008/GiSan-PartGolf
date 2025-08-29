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

        public GameResultViewModel? MyResult { get; set; }
        public List<GameResultViewModel> AllResults { get; set; } = new();

        public void OnGet(string gameCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) { return; }

            // 본인 결과 가져오기
            MyResult = _gameService.GetMyResult(gameCode, userId);

            // 전체 결과 가져오기
            AllResults = _gameService.GetAllResults(gameCode);
        }
    }
}
