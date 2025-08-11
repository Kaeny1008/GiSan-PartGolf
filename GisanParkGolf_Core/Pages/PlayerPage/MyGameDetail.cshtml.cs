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
    public class MyGameDetailModel : PageModel
    {
        private readonly IPlayerGameService _playerGameService;

        public MyGameDetailModel(IPlayerGameService playerGameService)
        {
            _playerGameService = playerGameService;
        }

        [BindProperty(SupportsGet = true)]
        public string GameCode { get; set; }

        public PlayerGameDetailViewModel Detail { get; set; }

        [TempData]
        public string ResultMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string gameCode)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            Detail = await _playerGameService.GetMyGameDetailAsync(userId, gameCode);
            if (Detail == null)
            {
                ResultMessage = "�����Ͱ� �����ϴ�.";
                return RedirectToPage("./MyGame");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string gameCode, string actionType, string cancelReason)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            Detail = await _playerGameService.GetMyGameDetailAsync(userId, gameCode);

            if (Detail == null)
            {
                ResultMessage = "�����Ͱ� �����ϴ�.";
                return RedirectToPage("./MyGame");
            }

            if (actionType == "cancel")
            {
                if (string.IsNullOrWhiteSpace(cancelReason))
                {
                    ModelState.AddModelError("", "��� ������ �Է��ϼ���.");
                }
                else
                {
                    var result = await _playerGameService.CancelGameAsync(userId, gameCode, cancelReason);
                    ResultMessage = result ? "������ ���������� ��ҵǾ����ϴ�." : "������� �� ������ �߻��߽��ϴ�.";
                }
            }
            else if (actionType == "rejoin")
            {
                if (Detail.CancelledBy == "Admin" || Detail.AssignmentStatus == "Cancelled")
                {
                    ModelState.AddModelError("", "�����ڿ� ���� ��ҵ� ������ �������� �Ұ��մϴ�.");
                }
                else
                {
                    var result = await _playerGameService.RejoinGameAsync(userId, gameCode);
                    ResultMessage = result ? "�������� ���������� ����Ǿ����ϴ�." : "������ �� ������ �߻��߽��ϴ�.";
                }
            }

            // ����� ���¸� �ٽ� ��ȸ
            Detail = await _playerGameService.GetMyGameDetailAsync(userId, gameCode);

            return Page();
        }
    }
}