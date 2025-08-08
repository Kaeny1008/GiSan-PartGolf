using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;

namespace GisanParkGolf_Core.Services
{
    public interface IStadiumService
    {
        Task<PaginatedList<Stadium>> GetStadiumsAsync(string? searchField, string? searchQuery, int pageIndex, int pageSize);
    }
}
