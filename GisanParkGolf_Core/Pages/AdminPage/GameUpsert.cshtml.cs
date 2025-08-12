using DocumentFormat.OpenXml.Wordprocessing;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services.AdminPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GiSanParkGolf.Pages.AdminPage
{
    [Authorize(Policy = "AdminOnly")]
    public class GameUpsertModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IStadiumService _stadiumService; // 경기장 목록을 위해 주입

        public GameUpsertModel(IGameService gameService, IStadiumService stadiumService)
        {
            _gameService = gameService;
            _stadiumService = stadiumService;
        }

        [BindProperty]
        public Game? Game { get; set; } = new();

        public SelectList? StadiumSelectList { get; set; }

        public bool IsCreateMode => string.IsNullOrEmpty(Game?.GameCode);

        public async Task<IActionResult> OnGetAsync(string? gameCode)
        {
            await LoadStadiumsAsync();

            if (string.IsNullOrEmpty(gameCode))
            {
                // 생성 모드: 기본값 설정
                ViewData["Title"] = "신규 대회 개최";
                Game = new Game
                {
                    GameDate = DateTime.Now.Date.AddHours(10),
                    StartRecruiting = DateTime.Now.Date.AddHours(9),
                    EndRecruiting = DateTime.Now.Date.AddDays(15).AddHours(17),
                    HoleMaximum = 4,
                    PlayMode = "Stroke"
                };
            }
            else
            {
                // 수정 모드: 데이터 로드
                ViewData["Title"] = "대회 정보 수정";
                Game = await _gameService.GetGameByIdAsync(gameCode);
                if (Game == null) return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadStadiumsAsync();
                return Page();
            }

            // Game이 null이면 바로 예외 처리 또는 반환(원하는 방식으로!)
            if (Game == null)
            {
                // 사용자에게 오류 메시지를 보여주거나, 로그를 남기고 return 등
                throw new InvalidOperationException("게임 정보가 없습니다.");
                // 또는: return; // 메서드 종료
            }

            // StadiumCode가 null일 수 있으니, 필요한 경우 추가 체크
            var selectedStadium = (await _stadiumService.GetStadiumsAsync(null, null, 1, 1000))
                .FirstOrDefault(s => s.StadiumCode == Game.StadiumCode);

            Game.StadiumName = selectedStadium?.StadiumName ?? "알 수 없음";
            Game.PostIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (IsCreateMode)
            {
                await _gameService.CreateGameAsync(Game);
                TempData["SuccessMessage"] = "대회가 성공적으로 등록되었습니다.";
            }
            else
            {
                await _gameService.UpdateGameAsync(Game);
                TempData["SuccessMessage"] = "대회 정보가 성공적으로 수정되었습니다.";
            }
            return RedirectToPage("./GameList");
        }

        public async Task<IActionResult> OnPostCancelGameAsync(string gameCode)
        {
            await _gameService.UpdateGameStatusAsync(gameCode, "Cancelled");
            return RedirectToPage("./GameList");
        }

        private async Task LoadStadiumsAsync()
        {
            var stadiums = await _stadiumService.GetStadiumsAsync(null, null, 1, 1000); // 전체 경기장 가져오기
            StadiumSelectList = new SelectList(stadiums, "StadiumCode", "StadiumName");
        }
    }
}