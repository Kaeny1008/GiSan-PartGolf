using GisanParkGolf_Core.ViewModels;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public interface IPlayerGameService
    {
        Task<PaginatedList<PlayerGameListItemViewModel>> GetMyGamesAsync(
            string userId, string searchField, string searchKeyword, int pageIndex, int pageSize);

        Task<PlayerGameDetailViewModel> GetMyGameDetailAsync(string userId, string gameCode);

        Task<bool> CancelGameAsync(string userId, string gameCode, string cancelReason);

        Task<bool> RejoinGameAsync(string userId, string gameCode);
    }
}