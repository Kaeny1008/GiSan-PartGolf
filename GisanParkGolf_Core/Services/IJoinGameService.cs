using DocumentFormat.OpenXml.Spreadsheet;
using GiSanParkGolf.Pages.PlayerPage;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.ViewModels;
using System.Security.Claims;
using static GisanParkGolf_Core.Services.JoinGameService;

namespace GisanParkGolf_Core.Services
{
    public interface IJoinGameService
    {
        Task<PaginatedList<Game>> GetGamesAsync(
            string? searchField, string? searchQuery, int pageIndex, int pageSize, string? status = null);

        Task<JoinGameResult> JoinGameAsync(string gameCode, ClaimsPrincipal user);

        Task<JoinGameResult> LeaveGameAsync(string gameCode, ClaimsPrincipal user);

        IQueryable<GameParticipant> GameParticipants { get; }

        // [NEW] 내 대회 목록 조회 (검색/페이징 포함)
        Task<PaginatedList<MyGameListModel>> GetMyGameListAsync(
            string userId, string? searchField, string? searchQuery, int pageIndex, int pageSize
        );

        Task<MyGameDetailViewModel?> GetMyGameInformationAsync(string gameCode, string? userId);
        Task<bool> MyGameCancelAsync(string gameCode, string? userId, string cancelReason);
        Task<bool> MyGameRejoinAsync(string gameCode, string? userId);
    }
}
