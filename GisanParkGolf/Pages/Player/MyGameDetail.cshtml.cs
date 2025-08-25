using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Player.ViewModels;
using GisanParkGolf.Services.Player;
using GisanParkGolf.ViewModels.Player;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiSanParkGolf.Pages.Player
{
    [Authorize]
    public class MyGameDetailModel : PageModel
    {
        private readonly IJoinGameService _gameService;

        public MyGameDetailModel(IJoinGameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty(SupportsGet = true)] public string? GameCode { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchField { get; set; } = "UserName";
        [BindProperty(SupportsGet = true)] public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 5;

        public MyGameDetailViewModel? Game { get; set; }
        public PaginatedList<AssignmentResultModel>? AssignmentResult { get; set; }

        public async Task<IActionResult> OnGetAsync(string? gameCode)
        {
            if (string.IsNullOrEmpty(gameCode))
                return RedirectToPage("MyGame");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Game = await _gameService.GetMyGameInformationAsync(gameCode, userId);

            if (Game == null)
                return RedirectToPage("MyGame");

            AssignmentResult = await _gameService.GetAssignmentResultAsync(
                gameCode,
                SearchField,
                SearchQuery,
                PageIndex,
                PageSize
            );

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(string gameCode, string CancelReason)
        {
            if (await _gameService.IsAssignmentLockedAsync(gameCode))
            {
                TempData["ErrorMessage"] = "코스배치가 완료되어 참가자 변경이 불가합니다.<br><strong class='text-danger'>관리자에게 요청하여 주십시오.</strong>";
                return RedirectToPage(new { GameCode = gameCode });
            }

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
            if (await _gameService.IsAssignmentLockedAsync(gameCode))
            {
                TempData["ErrorMessage"] = "코스배치가 완료되어 참가자 변경이 불가합니다.<br><strong class='text-danger'>관리자에게 요청하여 주십시오.</strong>";
                return RedirectToPage(new { GameCode = gameCode });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _gameService.MyGameRejoinAsync(gameCode, userId);

            if (result.Success)
                TempData["SuccessMessage"] = "재참가가 정상적으로 처리되었습니다.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "재참가 처리 중 문제가 발생했습니다.";

            return RedirectToPage(new { GameCode = gameCode });
        }

        public async Task<IActionResult> OnPostCancelRequestAsync(string gameCode, string CancelRequestReason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(CancelRequestReason))
            {
                TempData["ErrorMessage"] = "취소 사유를 입력하세요.";
                return RedirectToPage(new { GameCode = gameCode });
            }

            var result = await _gameService.MyGameCancelRequestlAsync(gameCode, userId, CancelRequestReason);

            if (result.Success)
                TempData["SuccessMessage"] = "참가 취소요청을 정상적으로 처리되었습니다.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "참가 취소요청 중 문제가 발생했습니다.";

            return RedirectToPage(new { GameCode = gameCode });
        }

        public async Task<IActionResult> OnPostRejoinRequestAsync(string gameCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _gameService.MyGameRejoinRequestAsync(gameCode, userId);

            if (result.Success)
                TempData["SuccessMessage"] = "재참가 요청을 정상적으로 처리되었습니다.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "재참가 요청 처리 중 문제가 발생했습니다.";

            return RedirectToPage(new { GameCode = gameCode });
        }
    }
}