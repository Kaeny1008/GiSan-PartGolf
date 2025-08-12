using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services;
using GisanParkGolf_Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace GiSanParkGolf.Pages.PlayerPage
{
    [Authorize]
    public class MyGameDetailModel : PageModel
    {
        private readonly IJoinGameService _gameService;

        public MyGameDetailModel(IJoinGameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty(SupportsGet = true)]
        public string? GameCode { get; set; }

        public MyGameDetailViewModel? Game { get; set; }

        public async Task<IActionResult> OnGetAsync(string? gameCode)
        {
            if (string.IsNullOrEmpty(gameCode))
                return RedirectToPage("MyGame");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Game = await _gameService.GetMyGameInformationAsync(gameCode, userId);

            if (Game == null)
                return RedirectToPage("MyGame");

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(string gameCode, string CancelReason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(CancelReason))
            {
                TempData["ErrorMessage"] = "취소 사유를 입력하세요.";
                return RedirectToPage(new { GameCode = gameCode });
            }

            var result = await _gameService.MyGameCancelAsync(gameCode, userId, CancelReason);

            if (result.Success)
                TempData["SuccessMessage"] = "참가 취소가 정상적으로 처리되었습니다.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "참가 취소 중 문제가 발생했습니다.";

            return RedirectToPage(new { GameCode = gameCode });
        }

        public async Task<IActionResult> OnPostRejoinAsync(string gameCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _gameService.MyGameRejoinAsync(gameCode, userId);

            if (result.Success)
                TempData["SuccessMessage"] = "재참가가 정상적으로 처리되었습니다.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "재참가 처리 중 문제가 발생했습니다.";

            return RedirectToPage(new { GameCode = gameCode });
        }
    }
}