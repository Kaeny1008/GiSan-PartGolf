using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem을 위해 추가
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

        // --- 뷰에 바인딩될 속성들 ---
        public List<Game> GameList { get; set; } = new();
        public int TotalCount { get; set; }
        public List<SelectListItem> SearchFields { get; private set; } = new(); // 검색 필드 목록

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; } = "GameName";
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        // --- 상세 보기용 속성 ---
        [BindProperty(SupportsGet = true)]
        public string? GameCode { get; set; }
        public Game? GameDetail { get; set; }
        public bool IsDetailView => !string.IsNullOrEmpty(GameCode);


        public async Task<IActionResult> OnGetAsync()
        {
            // 검색 필드 목록 초기화 (SearchViewComponent에 전달할 데이터)
            //SearchFields = new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "대회명", Value = "GameName" },
            //    new SelectListItem { Text = "경기장", Value = "StadiumName" },
            //    new SelectListItem { Text = "주최", Value = "GameHost" },
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