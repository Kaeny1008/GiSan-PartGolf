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
            await HttpContext.SignOutAsync("Identity.Application"); // �Ǵ� ��Ȯ�� ��Ŵ�� ����
            return RedirectToPage("/Account/Login");
        }
    }
}