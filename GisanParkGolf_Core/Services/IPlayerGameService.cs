using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.ViewModels;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public interface IPlayerGameService
    {
        Task<PaginatedList<PlayerGameListItem>> GetMyGamesAsync(string userId, string searchField, string searchKeyword, int pageIndex, int pageSize);
        Task<PlayerGameDetailViewModel> GetMyGameDetailAsync(string userId, string gameCode);
        Task<bool> CancelGameAsync(string userId, string gameCode, string reason);
        Task<bool> RejoinGameAsync(string userId, string gameCode);
    }
}