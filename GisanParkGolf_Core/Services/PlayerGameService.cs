using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.ViewModels;
using GisanParkGolf_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public class PlayerGameService : IPlayerGameService
    {
        private readonly MyDbContext _db;

        public PlayerGameService(MyDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<PlayerGameListItemViewModel>> GetMyGamesAsync(
            string userId, string searchField, string searchKeyword, int pageIndex, int pageSize)
        {
            var query = _db.GameParticipants
                .Include(x => x.Game)
                .Where(x => x.UserId == userId);

            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchKeyword))
            {
                string kw = searchKeyword.ToLower();
                query = searchField switch
                {
                    "GameName" => query.Where(x => x.Game != null && x.Game.GameName.ToLower().Contains(kw)),
                    "GameHost" => query.Where(x => x.Game != null && x.Game.GameHost.ToLower().Contains(kw)),
                    _ => query
                };
            }

            query = query.OrderByDescending(x => x.Game.GameDate);

            return await PaginatedList<PlayerGameListItemViewModel>.CreateAsync(
                query.Select(x => new PlayerGameListItemViewModel
                {
                    GameCode = x.GameCode,
                    GameName = x.Game != null ? x.Game.GameName : "",
                    GameHost = x.Game != null ? x.Game.GameHost : "",
                    GameDate = x.Game != null ? x.Game.GameDate : default(DateTime),
                    IsCancelledText = x.IsCancelled ? "취소" : "참가",
                    // 실제 ViewModel에 있는 필드만 넣기
                }),
                pageIndex, pageSize
            );
        }

        public async Task<PlayerGameDetailViewModel> GetMyGameDetailAsync(string userId, string gameCode)
        {
            var participant = await _db.GameParticipants
                .Include(x => x.Game)
                .FirstOrDefaultAsync(x => x.GameCode == gameCode && x.UserId == userId);

            if (participant == null) return null;

            var game = participant.Game;

            return new PlayerGameDetailViewModel
            {
                GameCode = participant.GameCode,
                GameName = game?.GameName ?? "",
                GameDate = game?.GameDate,
                GameHost = game?.GameHost ?? "",
                PlayMode = game?.PlayMode,
                HoleMaximum = game?.HoleMaximum,
                StartRecruiting = game?.StartRecruiting,
                EndRecruiting = game?.EndRecruiting,
                GameNote = game?.Note,
                ParticipantNumber = game?.ParticipantNumber,
                CancelDate = participant.CancelDate,
                CancelReason = participant.CancelReason,
                AssignmentStatus = null,   // 실제 구조에 맞게
                IsCancelled = participant.IsCancelled,
                CancelledBy = null,        // 실제 구조에 맞게
            };
        }

        public async Task<bool> CancelGameAsync(string userId, string gameCode, string reason)
        {
            var participant = await _db.GameParticipants
                .FirstOrDefaultAsync(x => x.GameCode == gameCode && x.UserId == userId);
            if (participant == null) return false;
            participant.IsCancelled = true;
            participant.CancelReason = reason;
            participant.CancelDate = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejoinGameAsync(string userId, string gameCode)
        {
            var participant = await _db.GameParticipants
                .FirstOrDefaultAsync(x => x.GameCode == gameCode && x.UserId == userId);
            if (participant == null) return false;
            participant.IsCancelled = false;
            participant.CancelReason = null;
            participant.CancelDate = null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}