using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace GisanParkGolf_Core.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyDbContext _db;

        public IndexModel(MyDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }
    }
}
