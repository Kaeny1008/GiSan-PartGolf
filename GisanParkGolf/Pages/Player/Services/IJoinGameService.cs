using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Player.ViewModels;
using GisanParkGolf.ViewModels.Player;
using GiSanParkGolf.Pages.Player;
using System.Security.Claims;
using static GisanParkGolf.Services.Player.JoinGameService;

namespace GisanParkGolf.Services.Player
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
        Task<JoinGameResult> MyGameCancelAsync(string gameCode, string? userId, string cancelReason);
        Task<JoinGameResult> MyGameRejoinAsync(string gameCode, string? userId);
        Task<bool> IsAssignmentLockedAsync(string gameCode);

        Task<JoinGameResult> MyGameCancelRequestlAsync(string gameCode, string? userId, string cancelReason);
        Task<JoinGameResult> MyGameRejoinRequestAsync(string gameCode, string? userId);

        Task<PaginatedList<AssignmentResultModel>> GetAssignmentResultAsync(
            string gameCode,
            string? searchField,
            string? searchQuery,
            int pageIndex,
            int pageSize
        );
    }
}
