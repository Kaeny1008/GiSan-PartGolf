using GiSanParkGolf.Pages.Admin;
using GiSanParkGolf.Pages.Player;
using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Admin.ViewModels;

namespace GisanParkGolf.Pages.Admin.Admin
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

        Task<PaginatedList<CompetitionViewModel>> GetCompetitionsAsync(
            string? searchField, string? searchQuery, int pageIndex, int pageSize);

        Task<PaginatedList<ParticipantViewModel>> GetParticipantsAsync(
            string gameCode, string? searchQuery, int pageIndex, int pageSize);
    }
}