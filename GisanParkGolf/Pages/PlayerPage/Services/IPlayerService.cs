using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GisanParkGolf.Services.PlayerPage
{
    public interface IPlayerService
    {
        Task<PaginatedList<Player>> GetPlayersAsync(string? searchField, string? searchQuery, bool readyUserOnly, int pageIndex, int pageSize);
        Task<List<Player>> GetPlayersForExcelAsync(string? searchField, string? searchQuery, bool readyUserOnly);
        Task ApproveReadyUsersAsync();
    }
}