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

            if (Game == null)
            {
                throw new InvalidOperationException("게임 정보가 없습니다.");
            }

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
                if (!string.IsNullOrEmpty(Game.GameCode))
                {
                    // 기존 엔티티를 DB에서 조회
                    var existingGame = await _gameService.GetGameByIdAsync(Game.GameCode);
                    if (existingGame != null)
                    {
                        // 기존 참가자수 유지
                        Game.ParticipantNumber = existingGame.ParticipantNumber;

                        // 기존 엔티티에 값만 복사해서 업데이트 (EF Core 충돌 방지)
                        existingGame.GameName = Game.GameName;
                        existingGame.GameDate = Game.GameDate;
                        existingGame.StadiumCode = Game.StadiumCode;
                        existingGame.StadiumName = Game.StadiumName;
                        existingGame.GameHost = Game.GameHost;
                        existingGame.HoleMaximum = Game.HoleMaximum;
                        existingGame.PlayMode = Game.PlayMode;
                        existingGame.StartRecruiting = Game.StartRecruiting;
                        existingGame.EndRecruiting = Game.EndRecruiting;
                        existingGame.GameNote = Game.GameNote;
                        existingGame.PostIp = Game.PostIp;
                        // 필요한 나머지 필드 복사

                        await _gameService.UpdateGameAsync(existingGame);
                    }
                    else
                    {
                        // 기존 엔티티를 못 찾으면 오류 처리
                        TempData["ErrorMessage"] = "수정할 대회를 찾을 수 없습니다.";
                        await LoadStadiumsAsync();
                        return Page();
                    }
                }
                TempData["SuccessMessage"] = "대회 정보가 정상적으로 수정되었습니다.";
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