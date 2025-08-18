using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services.PlayerPage;
using GisanParkGolf_Core.ViewModels.PlayerPage;
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
            if (await _gameService.IsAssignmentLockedAsync(gameCode))
            {
                TempData["ErrorMessage"] = "�ڽ���ġ�� �Ϸ�Ǿ� ������ ������ �Ұ��մϴ�.<br><strong class='text-danger'>�����ڿ��� ��û�Ͽ� �ֽʽÿ�.</strong>";
                return RedirectToPage(new { GameCode = gameCode });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(CancelReason))
            {
                TempData["ErrorMessage"] = "��� ������ �Է��ϼ���.";
                return RedirectToPage(new { GameCode = gameCode });
            }

            var result = await _gameService.MyGameCancelAsync(gameCode, userId, CancelReason);

            if (result.Success)
                TempData["SuccessMessage"] = "���� ��Ұ� ���������� ó���Ǿ����ϴ�.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "���� ��� �� ������ �߻��߽��ϴ�.";

            return RedirectToPage(new { GameCode = gameCode });
        }

        public async Task<IActionResult> OnPostRejoinAsync(string gameCode)
        {
            if (await _gameService.IsAssignmentLockedAsync(gameCode))
            {
                TempData["ErrorMessage"] = "�ڽ���ġ�� �Ϸ�Ǿ� ������ ������ �Ұ��մϴ�.<br><strong class='text-danger'>�����ڿ��� ��û�Ͽ� �ֽʽÿ�.</strong>";
                return RedirectToPage(new { GameCode = gameCode });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _gameService.MyGameRejoinAsync(gameCode, userId);

            if (result.Success)
                TempData["SuccessMessage"] = "�������� ���������� ó���Ǿ����ϴ�.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage ?? "������ ó�� �� ������ �߻��߽��ϴ�.";

            return RedirectToPage(new { GameCode = gameCode });
        }
    }
}