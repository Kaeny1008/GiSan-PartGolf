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
        private readonly IStadiumService _stadiumService; // ����� ����� ���� ����

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
                // ���� ���: �⺻�� ����
                ViewData["Title"] = "�ű� ��ȸ ����";
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
                // ���� ���: ������ �ε�
                ViewData["Title"] = "��ȸ ���� ����";
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
                throw new InvalidOperationException("���� ������ �����ϴ�.");
            }

            var selectedStadium = (await _stadiumService.GetStadiumsAsync(null, null, 1, 1000))
                .FirstOrDefault(s => s.StadiumCode == Game.StadiumCode);

            Game.StadiumName = selectedStadium?.StadiumName ?? "�� �� ����";
            Game.PostIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (IsCreateMode)
            {
                await _gameService.CreateGameAsync(Game);
                TempData["SuccessMessage"] = "��ȸ�� ���������� ��ϵǾ����ϴ�.";
            }
            else
            {
                if (!string.IsNullOrEmpty(Game.GameCode))
                {
                    // ���� ��ƼƼ�� DB���� ��ȸ
                    var existingGame = await _gameService.GetGameByIdAsync(Game.GameCode);
                    if (existingGame != null)
                    {
                        // ���� �����ڼ� ����
                        Game.ParticipantNumber = existingGame.ParticipantNumber;

                        // ���� ��ƼƼ�� ���� �����ؼ� ������Ʈ (EF Core �浹 ����)
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
                        // �ʿ��� ������ �ʵ� ����

                        await _gameService.UpdateGameAsync(existingGame);
                    }
                    else
                    {
                        // ���� ��ƼƼ�� �� ã���� ���� ó��
                        TempData["ErrorMessage"] = "������ ��ȸ�� ã�� �� �����ϴ�.";
                        await LoadStadiumsAsync();
                        return Page();
                    }
                }
                TempData["SuccessMessage"] = "��ȸ ������ ���������� �����Ǿ����ϴ�.";
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
            var stadiums = await _stadiumService.GetStadiumsAsync(null, null, 1, 1000); // ��ü ����� ��������
            StadiumSelectList = new SelectList(stadiums, "StadiumCode", "StadiumName");
        }
    }
}