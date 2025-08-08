using System.Security.Claims; // Claims를 사용하기 위해 추가

namespace GisanParkGolf_Core.Services
{
    public interface IPlayerservice
    {
        // 기존 메서드
        bool IsUserIdExist(string userId);

        // ★★★ 로그인 처리를 위한 새 메서드 추가 ★★★
        // 성공하면 신분증(ClaimsPrincipal)을, 실패하면 null을 반환
        Task<ClaimsPrincipal?> AuthenticateUserAsync(string userId, string password);
    }
}