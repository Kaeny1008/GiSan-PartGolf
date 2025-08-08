using DocumentFormat.OpenXml.InkML;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core.Services
{
    public class StadiumService : IStadiumService
    {

        private readonly MyDbContext _dbContext;

        public StadiumService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<Stadium>> GetStadiumsAsync(string? searchField, string? searchQuery, int pageIndex, int pageSize)
        {
            var query = GetFilteredStadiumsQuery(searchField, searchQuery);
            var orderedQuery = query.OrderByDescending(u => u.StadiumCode);

            return await PaginatedList<Stadium>.CreateAsync(orderedQuery, pageIndex, pageSize);
        }

        private IQueryable<Stadium> GetFilteredStadiumsQuery(string? searchField, string? searchQuery)
        {
            IQueryable<Stadium> SelStadium = _dbContext.Stadiums.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "StadiumName":
                        SelStadium = SelStadium.Where(u => u.StadiumName.Contains(searchQuery));
                        break;
                    case "StadiumCode":
                        SelStadium = SelStadium.Where(u => u.StadiumCode.Contains(searchQuery));
                        break;
                }
            }
            return SelStadium;
        }
    }
}
