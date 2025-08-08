using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public interface IPlayerService
    {
        Task<PaginatedList<Player>> GetPlayersAsync(string? searchField, string? searchQuery, bool readyUserOnly, int pageIndex, int pageSize);
        Task<List<Player>> GetPlayersForExcelAsync(string? searchField, string? searchQuery, bool readyUserOnly);
    }
}