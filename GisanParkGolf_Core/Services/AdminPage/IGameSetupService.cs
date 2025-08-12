using GiSanParkGolf.Pages.AdminPage;
using GisanParkGolf_Core.ViewModels.AdminPage;
using System.Collections.Generic;

namespace GiSanParkGolf.Services.AdminPage
{
    public interface IGameSetupService
    {
        List<GameListViewModel> GetGames(string keyword);
        GameListViewModel? GetGameInformation(string code);
        List<GameJoinUserListViewModel> GetGameUserList(string code);
        //List<AssignedPlayerViewModel> GetAssignmentResult(string code);
    }
}