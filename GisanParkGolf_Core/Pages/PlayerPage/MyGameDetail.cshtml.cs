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
                ResultMessage = "데이터가 없습니다.";
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
                ResultMessage = "데이터가 없습니다.";
                return RedirectToPage("./MyGame");
            }

            if (actionType == "cancel")
            {
                if (string.IsNullOrWhiteSpace(cancelReason))
                {
                    ModelState.AddModelError("", "취소 사유를 입력하세요.");
                }
                else
                {
                    var result = await _playerGameService.CancelGameAsync(userId, gameCode, cancelReason);
                    ResultMessage = result ? "참가가 성공적으로 취소되었습니다." : "참가취소 중 오류가 발생했습니다.";
                }
            }
            else if (actionType == "rejoin")
            {
                if (Detail.CancelledBy == "Admin" || Detail.AssignmentStatus == "Cancelled")
                {
                    ModelState.AddModelError("", "관리자에 의해 취소된 게임은 재참가가 불가합니다.");
                }
                else
                {
                    var result = await _playerGameService.RejoinGameAsync(userId, gameCode);
                    ResultMessage = result ? "재참가가 성공적으로 저장되었습니다." : "재참가 중 오류가 발생했습니다.";
                }
            }

            // 변경된 상태를 다시 조회
            Detail = await _playerGameService.GetMyGameDetailAsync(userId, gameCode);

            return Page();
        }
    }
}