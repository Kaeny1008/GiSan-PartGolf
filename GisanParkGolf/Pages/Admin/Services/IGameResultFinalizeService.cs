using GisanParkGolf.Pages.Admin.ViewModels;
using GisanParkGolf.Pages.Manager.ViewModels;

namespace GisanParkGolf.Pages.Admin.Services
{
    public interface IGameResultFinalizeService
    {
        List<GameInfoViewModel> GetScoreConfirmedGames();
        List<ParticipantResultViewModel> GetGameResults(string gameCode);
        void EndGame(string gameCode);
    }
}