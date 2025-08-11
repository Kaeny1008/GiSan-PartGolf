using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public class PlayerGameService : IPlayerGameService
    {
        private readonly MyDbContext _context;

        public PlayerGameService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<PlayerGameListItemViewModel>> GetMyGamesAsync(
            string userId, string searchField, string searchKeyword, int pageIndex, int pageSize)
        {
            var query = _context.Games
                .Where(g => g.Participants.Any(p => p.UserId == userId));

            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchKeyword))
            {
                switch (searchField)
                {
                    case "GameName":
                        query = query.Where(g => g.GameName.Contains(searchKeyword));
                        break;
                    case "StadiumName":
                        query = query.Where(g => g.StadiumName.Contains(searchKeyword));
                        break;
                    case "GameHost":
                        query = query.Where(g => g.GameHost.Contains(searchKeyword));
                        break;
                }
            }

            var totalCount = await query.CountAsync();
            var games = await query
                .OrderByDescending(g => g.GameDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new PlayerGameListItemViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName,
                    StadiumName = g.StadiumName,
                    GameHost = g.GameHost,
                    GameDate = g.GameDate,
                    StartRecruiting = g.StartRecruiting,
                    EndRecruiting = g.EndRecruiting,
                    GameStatus = g.GameStatus,
                    IsCancelledText = g.Participants
                        .Where(p => p.UserId == userId)
                        .Select(p => p.IsCancelled == true ? "취소" : "참가")
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new PaginatedList<PlayerGameListItemViewModel>(
                games, totalCount, pageIndex, pageSize
            );
        }

        public async Task<PlayerGameDetailViewModel> GetMyGameDetailAsync(string userId, string gameCode)
        {
            var game = await _context.Games
                .Where(g => g.GameCode == gameCode)
                .Select(g => new PlayerGameDetailViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName,
                    StadiumName = g.StadiumName,
                    GameHost = g.GameHost,
                    GameDate = g.GameDate,
                    PlayMode = g.PlayMode,
                    HoleMaximum = g.HoleMaximum,
                    StartRecruiting = g.StartRecruiting,
                    EndRecruiting = g.EndRecruiting,
                    GameNote = g.GameNote,
                    ParticipantNumber = g.Participants.Count(),
                    IsCancelled = g.Participants.Where(p => p.UserId == userId).Select(p => p.IsCancelled).FirstOrDefault(),
                    CancelDate = g.Participants.Where(p => p.UserId == userId).Select(p => p.CancelDate).FirstOrDefault(),
                    CancelReason = g.Participants.Where(p => p.UserId == userId).Select(p => p.CancelReason).FirstOrDefault(),
                    CancelledBy = g.Participants.Where(p => p.UserId == userId).Select(p => p.CancelledBy).FirstOrDefault(),
                    AssignmentStatus = g.Participants.Where(p => p.UserId == userId).Select(p => p.AssignmentStatus).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return game;
        }

        public async Task<bool> CancelGameAsync(string userId, string gameCode, string cancelReason)
        {
            var participant = await _context.GameParticipants
                .FirstOrDefaultAsync(p => p.GameCode == gameCode && p.UserId == userId);

            if (participant == null || participant.IsCancelled == 1)
                return false;

            participant.IsCancelled = 1;
            participant.CancelDate = DateTime.Now;
            participant.CancelReason = cancelReason;
            participant.CancelledBy = "User";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejoinGameAsync(string userId, string gameCode)
        {
            var participant = await _context.GameParticipants
                .FirstOrDefaultAsync(p => p.GameCode == gameCode && p.UserId == userId);

            if (participant == null || participant.IsCancelled == 0 || participant.CancelledBy == "Admin" || participant.AssignmentStatus == "Cancelled")
                return false;

            participant.IsCancelled = 0;
            participant.CancelReason = null;
            participant.CancelDate = null;
            participant.CancelledBy = null;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}