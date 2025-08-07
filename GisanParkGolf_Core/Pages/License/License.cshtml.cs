using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf_Core.Pages.License
{
    public class LicenseModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "라이선스 정보";
        }
    }
}