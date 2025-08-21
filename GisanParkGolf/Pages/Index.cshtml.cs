using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GisanParkGolf.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;

        // '���� �м���(AuthorizationService)'�� �� �������� �ʴ��մϴ�.
        public IndexModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        // �м� ����� ������ �������� �غ��մϴ�.
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; } // '�Ŵ���' ������ �ִٰ� �����մϴ�.

        // �������� ���� ��, ������� ������ �м��մϴ�.
        public async Task OnGetAsync()
        {
            // ����ڰ� �α����� ������ ���� �м��� �����մϴ�.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // 1. �ְ� ������ '������'���� ���� Ȯ���մϴ�.
                IsAdmin = (await _authorizationService.AuthorizeAsync(User, "AdminOnly")).Succeeded;

                // 2. �����ڰ� �ƴ϶��, '�Ŵ���'���� Ȯ���մϴ�.
                //    (����: "ManagerOnly" ��å�� Program.cs�� ���ǵǾ� �־�� �մϴ�!)
                if (!IsAdmin)
                {
                    IsManager = (await _authorizationService.AuthorizeAsync(User, "ManagerOnly")).Succeeded;
                }
            }
        }
    }
}