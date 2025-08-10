using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
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
    }
}
