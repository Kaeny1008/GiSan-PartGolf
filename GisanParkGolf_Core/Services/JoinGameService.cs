using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GisanParkGolf_Core.Services
{
    public class JoinGameService : IJoinGameService
    {
        public class JoinGameResult
        {
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
        }

        private readonly MyDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JoinGameService(MyDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<Game>> GetGamesAsync(
            string? searchField, string? searchQuery, int pageIndex, int pageSize, string? status = null)
        {
            var query = _dbContext.Games.AsQueryable();

            var statusCode = GameStatusHelper.ToStatusCode(status);
            query = query.Where(g => g.GameStatus == statusCode);

            // 검색조건
            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "GameName":
                        query = query.Where(g => (g.GameName ?? "").Contains(searchQuery));
                        break;
                    case "StadiumName":
                        query = query.Where(g => (g.StadiumName ?? "").Contains(searchQuery));
                        break;
                    case "GameHost":
                        query = query.Where(g => (g.GameHost ?? "").Contains(searchQuery));
                        break;
                }
            }
            var ordered = query.OrderByDescending(g => g.GameDate);
            return await PaginatedList<Game>.CreateAsync(ordered, pageIndex, pageSize);
        }

        public async Task<JoinGameResult> JoinGameAsync(string gameCode, ClaimsPrincipal user)
        {
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            // 기존 참가 엔터티 찾기 (취소 포함)
            var participant = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId);

            if (participant != null)
            {
                if (participant.IsCancelled)
                {
                    // 참가 취소 이력이 있으면 복구
                    participant.IsCancelled = false;
                    participant.CancelDate = null;
                    participant.CancelReason = null;
                    participant.CancelledBy = null;
                    participant.JoinDate = DateTime.Now;
                    participant.JoinIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                    participant.JoinStatus = "Join";
                    // 참가자 수 증가
                    game.ParticipantNumber += 1;

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        return new JoinGameResult { Success = true };
                    }
                    catch (Exception ex)
                    {
                        return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
                    }
                }
                else
                {
                    // 이미 참가중인 경우
                    return new JoinGameResult { Success = false, ErrorMessage = "이미 참가 신청하셨습니다." };
                }
            }
            else
            {
                // 신규 참가
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                string joinId = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 12);

                var join = new GameParticipant
                {
                    JoinId = joinId,
                    GameCode = gameCode,
                    UserId = userId,
                    JoinDate = DateTime.Now,
                    JoinIp = ip,
                    JoinStatus = "Join",
                    IsCancelled = false,
                    CancelDate = null,
                    CancelReason = null,
                    CancelledBy = null
                };
                _dbContext.GameParticipants.Add(join);

                game.ParticipantNumber += 1;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return new JoinGameResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
                }
            }
        }

        public async Task<JoinGameResult> LeaveGameAsync(string gameCode, ClaimsPrincipal user)
        {
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            // 참가 엔터티 찾기 (GameCode + UserId로)
            var joinEntity = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId && !gp.IsCancelled);

            if (joinEntity == null)
                return new JoinGameResult { Success = false, ErrorMessage = "참가 내역이 없습니다." };

            joinEntity.JoinStatus = "Cancel";
            joinEntity.IsCancelled = true;
            joinEntity.CancelDate = DateTime.Now;
            joinEntity.CancelReason = "사용자 요청";

            // 참가자 수 감소 (0보다 작아지지 않도록 방어 코드)
            if (game.ParticipantNumber > 0)
                game.ParticipantNumber -= 1;

            try
            {
                await _dbContext.SaveChangesAsync();
                return new JoinGameResult { Success = true };
            }
            catch (Exception ex)
            {
                return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
            }
        }

        public IQueryable<GameParticipant> GameParticipants
            => _dbContext.GameParticipants.AsQueryable();
    }
}