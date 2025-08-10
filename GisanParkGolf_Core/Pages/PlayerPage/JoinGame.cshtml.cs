using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem�� ���� �߰�
using System.Security.Claims;

namespace GiSanParkGolf.Pages.PlayerPage
{
    public class JoinGameModel : PageModel
    {
        private readonly IGameService _gameService;
        //private readonly IGameJoinService _gameJoinService;

        public JoinGameModel(IGameService gameService/*, IGameJoinService gameJoinService*/)
        {
            _gameService = gameService;
            //_gameJoinService = gameJoinService;
        }

        // --- �信 ���ε��� �Ӽ��� ---
        public List<Game> GameList { get; set; } = new();
        public int TotalCount { get; set; }
        public List<SelectListItem> SearchFields { get; private set; } = new(); // �˻� �ʵ� ���

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; } = "GameName";
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        // --- �� ����� �Ӽ� ---
        [BindProperty(SupportsGet = true)]
        public string? GameCode { get; set; }
        public Game? GameDetail { get; set; }
        public bool IsDetailView => !string.IsNullOrEmpty(GameCode);


        public async Task<IActionResult> OnGetAsync()
        {
            // �˻� �ʵ� ��� �ʱ�ȭ (SearchViewComponent�� ������ ������)
            //SearchFields = new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "��ȸ��", Value = "GameName" },
            //    new SelectListItem { Text = "�����", Value = "StadiumName" },
            //    new SelectListItem { Text = "����", Value = "GameHost" },
            //};

            //if (IsDetailView)
            //{
            //    GameDetail = await _gameService.GetGameByCodeAsync(GameCode);
            //    if (GameDetail == null) return NotFound();
            //}
            //else
            //{
            //    var (games, total) = await _gameService.GetRecruitingGamesPaginatedAsync(SearchField, SearchQuery, PageIndex, PageSize);
            //    GameList = games;
            //    TotalCount = total;
            //}
            return Page();
        }

        //public async Task<IActionResult> OnPostJoinAsync(string gameCode)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var (success, message) = await _gameJoinService.TryJoinAsync(gameCode, userId, ipAddress);
        //    if (success) TempData["SuccessMessage"] = message;
        //    else TempData["ErrorMessage"] = message;
        //    return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        //}
    }
}