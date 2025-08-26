using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Services.Player;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiSanParkGolf.Pages.Player
{
    [Authorize]
    public class JoinGameModel : PageModel
    {
        private readonly IJoinGameService _joinGameService;

        public JoinGameModel(IJoinGameService joinGameService)
        {
            _joinGameService = joinGameService;
        }

        public PaginatedList<Game>? GameList { get; set; }
        public HashSet<string> JoinedGameCodes { get; set; } = new HashSet<string>();

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
            // 모집중 대회 목록 조회
            GameList = await _joinGameService.GetGamesAsync(SearchField, SearchQuery, PageIndex, PageSize, status: "모집중");

            // 내 참가중 게임코드 조회
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || _joinGameService.GameParticipants == null)
            {
                JoinedGameCodes = new HashSet<string>();
            }
            else
            {
                var joinedCodes = await _joinGameService.GameParticipants
                    .Where(gp => gp.UserId == userId && !gp.IsCancelled)
                    .Select(gp => gp.GameCode)
                    .ToListAsync();

                // null이 아닌 값만 HashSet<string>으로 변환
                JoinedGameCodes = joinedCodes
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToHashSet();
            }
        }

        public async Task<IActionResult> OnPostJoinAsync(string gameCode)
        {
            //로그인한 사용자 정보 필요(ClaimsPrincipal 등)
            var result = await _joinGameService.JoinGameAsync(gameCode, User);
            if (result.Success)
            {
                TempData["SuccessTitle"] = "완료";
                TempData["SuccessMessage"] = "참가신청이 완료되었습니다.";
            }
            else
            {
                TempData["ErrorTitle"] = "참가 불가";
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }

        public async Task<IActionResult> OnPostLeaveAsync(string gameCode)
        {
            //로그인한 사용자 정보 필요(ClaimsPrincipal 등)
            var result = await _joinGameService.LeaveGameAsync(gameCode, User);
            if (result.Success)
            {
                TempData["SuccessTitle"] = "완료";
                TempData["SuccessMessage"] = "참가취소가 완료되었습니다.";
            }
            else
            {
                TempData["ErrorTitle"] = "취소 불가";
                TempData["ErrorMessage"] = result.ErrorMessage ?? "참가취소 실패";
            }
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }
    }
}