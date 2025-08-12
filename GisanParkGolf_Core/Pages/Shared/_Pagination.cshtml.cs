using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf_Core.Pages.Shared
{
    public class PaginationModel
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public Func<int, string> GetPageUrl { get; set; } = i => "#";
    }
}
