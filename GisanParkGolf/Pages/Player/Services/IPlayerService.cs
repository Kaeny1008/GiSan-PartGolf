using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GisanParkGolf.Services.Player
{
    public interface IPlayerService
    {
        Task<PaginatedList<Member>> GetPlayersAsync(string? searchField, string? searchQuery, bool readyUserOnly, int pageIndex, int pageSize);
        Task<List<Member>> GetPlayersForExcelAsync(string? searchField, string? searchQuery, bool readyUserOnly);
        Task ApproveReadyUsersAsync();
    }
}