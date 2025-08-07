using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public interface IPlayerService
    {
        Task<PaginatedList<SYS_Users>> GetPlayersAsync(string? searchField, string? searchQuery, bool readyUserOnly, int pageIndex, int pageSize);
        Task<List<SYS_Users>> GetPlayersForExcelAsync(string? searchField, string? searchQuery, bool readyUserOnly);
    }
}