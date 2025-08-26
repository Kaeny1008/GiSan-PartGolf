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
            // ������ ��ȸ ��� ��ȸ
            GameList = await _joinGameService.GetGamesAsync(SearchField, SearchQuery, PageIndex, PageSize, status: "������");

            // �� ������ �����ڵ� ��ȸ
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

                // null�� �ƴ� ���� HashSet<string>���� ��ȯ
                JoinedGameCodes = joinedCodes
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToHashSet();
            }
        }

        public async Task<IActionResult> OnPostJoinAsync(string gameCode)
        {
            //�α����� ����� ���� �ʿ�(ClaimsPrincipal ��)
            var result = await _joinGameService.JoinGameAsync(gameCode, User);
            if (result.Success)
            {
                TempData["SuccessTitle"] = "�Ϸ�";
                TempData["SuccessMessage"] = "������û�� �Ϸ�Ǿ����ϴ�.";
            }
            else
            {
                TempData["ErrorTitle"] = "���� �Ұ�";
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }

        public async Task<IActionResult> OnPostLeaveAsync(string gameCode)
        {
            //�α����� ����� ���� �ʿ�(ClaimsPrincipal ��)
            var result = await _joinGameService.LeaveGameAsync(gameCode, User);
            if (result.Success)
            {
                TempData["SuccessTitle"] = "�Ϸ�";
                TempData["SuccessMessage"] = "������Ұ� �Ϸ�Ǿ����ϴ�.";
            }
            else
            {
                TempData["ErrorTitle"] = "��� �Ұ�";
                TempData["ErrorMessage"] = result.ErrorMessage ?? "������� ����";
            }
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }
    }
}