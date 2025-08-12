using GiSanParkGolf.Services.AdminPage;
using GisanParkGolf_Core.ViewModels.AdminPage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace GiSanParkGolf.Pages.AdminPage
{
    public class GameSetupModel : PageModel
    {
        private readonly IGameSetupService _service;

        public GameSetupModel(IGameSetupService service)
        {
            _service = service;
            GameList = new List<GameListViewModel>();
            SelectedGame = new GameListViewModel();
            PlayerList = new List<GameJoinUserListViewModel>();
            AssignedPlayers = new List<AssignedPlayerViewModel>();
            CancelPlayers = new List<AssignedPlayerViewModel>();
            UnassignedPlayers = new List<GameJoinUserListViewModel>();
        }

        public List<GameListViewModel> GameList { get; set; }
        [BindProperty]
        public string? SelectedGameCode { get; set; }
        public GameListViewModel SelectedGame { get; set; }
        public List<GameJoinUserListViewModel> PlayerList { get; set; }
        public List<AssignedPlayerViewModel> AssignedPlayers { get; set; }
        public List<AssignedPlayerViewModel> CancelPlayers { get; set; }
        public List<GameJoinUserListViewModel> UnassignedPlayers { get; set; }

        public int TotalCount { get; set; }
        public int CancelledCount { get; set; }
        public int JoinedCount { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; } = "GameName";
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public void OnGet()
        {
            GameList = _service.GetGames(SearchQuery ?? "");
        }

        public IActionResult OnPostSelectGame(string selectedGameCode)
        {
            SelectedGameCode = selectedGameCode;
            GameList = _service.GetGames(SearchQuery);

            SelectedGame = _service.GetGameInformation(selectedGameCode);

            PlayerList = _service.GetGameUserList(selectedGameCode);

            //AssignedPlayers = _service.GetAssignmentResult(selectedGameCode)
            //    .FindAll(x => x.AssignmentStatus != "Cancelled");

            //CancelPlayers = _service.GetAssignmentResult(selectedGameCode)
            //    .FindAll(x => x.AssignmentStatus == "Cancelled");

            UnassignedPlayers = new List<GameJoinUserListViewModel>();

            return Page();
        }

        // 기타 POST 핸들러도 _service만 호출
    }
}