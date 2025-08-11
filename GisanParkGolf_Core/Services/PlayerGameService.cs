using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace GisanParkGolf_Core.Services
{
    public class PlayerGameService : IPlayerGameService
    {
        private readonly MyDbContext _db;

        public PlayerGameService(MyDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<PlayerGameListItem>> GetMyGamesAsync(string userId, string searchField, string searchKeyword, int pageIndex, int pageSize)
        {
            var query = _db.GameParticipants
                .Include(x => x.Game)
                .Include(x => x.Game.Stadium)
                .Where(x => x.UserId == userId);

            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchKeyword))
            {
                string kw = searchKeyword.ToLower();
                query = searchField switch
                {
                    "GameName" => query.Where(x => x.Game.GameName.ToLower().Contains(kw)),
                    "StadiumName" => query.Where(x => x.Game.Stadium.StadiumName.ToLower().Contains(kw)),
                    "GameHost" => query.Where(x => x.Game.GameHost.ToLower().Contains(kw)),
                    _ => query
                };
            }

            // 정렬 및 페이징
            query = query.OrderByDescending(x => x.Game.GameDate);

            return await PaginatedList<PlayerGameListItem>.CreateAsync(
                query.Select(x => new PlayerGameListItem
                {
                    GameCode = x.GameCode,
                    GameName = x.Game.GameName,
                    StadiumName = x.Game.Stadium.StadiumName,
                    GameHost = x.Game.GameHost,
                    GameDate = x.Game.GameDate,
                    StartRecruiting = x.Game.StartRecruiting,
                    EndRecruiting = x.Game.EndRecruiting,
                    GameStatus = x.Game.GameStatus,
                    IsCancelledText = x.IsCancelled == 1 ? "취소" : "참가",
                }),
                pageIndex, pageSize
            );
        }

        public async Task<PlayerGameDetailViewModel> GetMyGameDetailAsync(string userId, string gameCode)
        {
            var participant = await _db.GameParticipants
                .Include(x => x.Game)
                .Include(x => x.Game.Stadium)
                .FirstOrDefaultAsync(x => x.GameCode == gameCode && x.UserId == userId);

            if (participant == null) return null;

            return new PlayerGameDetailViewModel
            {
                GameCode = participant.GameCode,
                GameName = participant.Game.GameName,
                GameDate = participant.Game.GameDate,
                StadiumName = participant.Game.Stadium.StadiumName,
                GameHost = participant.Game.GameHost,
                PlayMode = participant.Game.PlayMode,
                HoleMaximum = participant.Game.HoleMaximum,
                StartRecruiting = participant.Game.StartRecruiting,
                EndRecruiting = participant.Game.EndRecruiting,
                GameNote = participant.Game.Note,
                ParticipantNumber = participant.Game.ParticipantNumber,
                CancelDate = participant.CancelDate,
                CancelReason = participant.CancelReason,
                CancelledBy = participant.CancelledBy,
                AssignmentStatus = participant.AssignmentStatus,
                IsCancelled = participant.IsCancelled,
            };
        }

        public async Task<bool> CancelGameAsync(string userId, string gameCode, string reason)
        {
            var participant = await _db.GameParticipants.FirstOrDefaultAsync(x => x.GameCode == gameCode && x.UserId == userId);
            if (participant == null) return false;
            participant.IsCancelled = 1;
            participant.CancelReason = reason;
            participant.CancelDate = DateTime.Now;
            participant.CancelledBy = "User";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejoinGameAsync(string userId, string gameCode)
        {
            var participant = await _db.GameParticipants.FirstOrDefaultAsync(x => x.GameCode == gameCode && x.UserId == userId);
            if (participant == null) return false;
            if (participant.CancelledBy == "Admin" || participant.AssignmentStatus == "Cancelled") return false;
            participant.IsCancelled = 0;
            participant.CancelReason = null;
            participant.CancelDate = null;
            participant.CancelledBy = null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}