using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GisanParkGolf.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;

        // '역할 분석가(AuthorizationService)'를 이 페이지로 초대합니다.
        public IndexModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        // 분석 결과를 저장할 변수들을 준비합니다.
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; } // '매니저' 역할이 있다고 가정합니다.

        // 페이지가 열릴 때, 사용자의 역할을 분석합니다.
        public async Task OnGetAsync()
        {
            // 사용자가 로그인한 상태일 때만 분석을 시작합니다.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // 1. 최고 권한인 '관리자'인지 먼저 확인합니다.
                IsAdmin = (await _authorizationService.AuthorizeAsync(User, "AdminOnly")).Succeeded;

                // 2. 관리자가 아니라면, '매니저'인지 확인합니다.
                //    (참고: "ManagerOnly" 정책이 Program.cs에 정의되어 있어야 합니다!)
                if (!IsAdmin)
                {
                    IsManager = (await _authorizationService.AuthorizeAsync(User, "ManagerOnly")).Succeeded;
                }
            }
        }
    }
}