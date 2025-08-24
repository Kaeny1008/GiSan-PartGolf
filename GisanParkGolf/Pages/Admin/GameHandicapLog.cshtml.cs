using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Admin.Admin;
using GisanParkGolf.Pages.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GisanParkGolf.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class GameHandicapLogModel : PageModel
    {
        private readonly IHandicapService _handicapService;

        public GameHandicapLogModel(IHandicapService handicapService)
        {
            _handicapService = handicapService;
        }

        public PaginatedList<HandicapChangeLogViewModel> Logs { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public string SearchField { get; set; } = "UserName";

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;


        public async Task OnGetAsync()
        {
            Logs = await _handicapService.GetHandicapLogsAsync(SearchField, SearchQuery, PageIndex, PageSize);
        }
    }
}