using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.AdminPage;
using GisanParkGolf.Pages.AdminPage.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GisanParkGolf.Pages.AdminPage.AdminPage
{
    public interface IHandicapService
    {
        Task<PaginatedList<HandicapViewModel>> GetPlayerHandicapsAsync(string? searchField, string? searchKeyword, int pageIndex, int pageSize);
        Task<bool> UpdateHandicapAsync(string userId, int age, int newHandicap, string newSource, string updatedBy);
        Task<int> RecalculateAllHandicapsAsync(string updatedBy);
        Task<PaginatedList<HandicapChangeLogViewModel>> GetHandicapLogsAsync(string? searchField, string? searchKeyword, int pageIndex, int pageSize);
    }
}