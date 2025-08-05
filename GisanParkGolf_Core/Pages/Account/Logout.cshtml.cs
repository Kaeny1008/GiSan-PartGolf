using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync("Identity.Application"); // 또는 정확한 스킴명 지정
            return RedirectToPage("/Account/Login");
        }
    }
}