using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;

namespace GisanParkGolf_Core.Services
{
    public interface IGameService
    {
        // 페이징/검색 기능이 포함된 대회 목록 조회
        Task<PaginatedList<Game>> GetGamesAsync(string? searchField, string? searchQuery, int pageIndex, int pageSize);

        // 특정 대회 정보 조회
        Task<Game?> GetGameByIdAsync(string gameCode);

        // 신규 대회 생성
        Task CreateGameAsync(Game game);

        // 대회 정보 수정
        Task UpdateGameAsync(Game game);

        // 대회 상태 변경 (예: 취소)
        Task UpdateGameStatusAsync(string gameCode, string status);
    }
}