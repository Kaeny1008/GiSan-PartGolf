using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public class HandicapService : IHandicapService
    {
        private readonly MyDbContext _dbContext;
        private readonly ILogger<HandicapService> _logger;

        public HandicapService(MyDbContext dbContext, ILogger<HandicapService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PaginatedList<HandicapViewModel>> GetUserHandicapsAsync(string? searchField, string? searchKeyword, int pageIndex, int pageSize)
        {
            var query = _dbContext.SYS_Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                if (searchField == "UserName")
                {
                    query = query.Where(u => u.UserName.Contains(searchKeyword));
                }
                else if (searchField == "UserId")
                {
                    query = query.Where(u => u.UserId.Contains(searchKeyword));
                }
            }

            var viewModelQuery = query
                .Include(u => u.Handicap)
                .Select(u => new HandicapViewModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    UserNumber = u.UserNumber,
                    Age = PersonInfoCalculator.CalculateAge(u.UserNumber, u.UserGender),
                    AgeHandicap = u.Handicap != null ? u.Handicap.AgeHandicap : 0,
                    Source = u.Handicap != null ? u.Handicap.Source : "미설정",
                    LastUpdated = u.Handicap != null ? u.Handicap.LastUpdated : null,
                    LastUpdatedBy = u.Handicap != null ? u.Handicap.LastUpdatedBy : null
                })
                .OrderBy(vm => vm.UserName);

            return await PaginatedList<HandicapViewModel>.CreateAsync(viewModelQuery, pageIndex, pageSize);
        }

        public async Task<bool> UpdateHandicapAsync(string userId, int age, int newHandicap, string newSource, string updatedBy)
        {
            var handicap = await _dbContext.SYS_UserHandicaps.FindAsync(userId);
            int prevHandicap = 0;
            string prevSource = "미설정";

            if (handicap != null)
            {
                prevHandicap = handicap.AgeHandicap;
                prevSource = handicap.Source;
                if (handicap.AgeHandicap == newHandicap && handicap.Source == newSource)
                {
                    return false;
                }
            }
            else
            {
                handicap = new SYS_UserHandicaps { UserId = userId };
                _dbContext.SYS_UserHandicaps.Add(handicap);
            }

            var log = new SYS_HandicapChangeLog
            {
                UserId = userId,
                Age = age,
                PrevHandicap = prevHandicap,
                NewHandicap = newHandicap,
                PrevSource = prevSource,
                NewSource = newSource,
                Reason = "수동 편집",
                ChangedBy = updatedBy,
                ChangedAt = DateTime.UtcNow
            };
            _dbContext.SYS_HandicapChangeLogs.Add(log);

            handicap.AgeHandicap = newHandicap;
            handicap.Source = newSource;
            handicap.LastUpdated = DateTime.UtcNow;
            handicap.LastUpdatedBy = updatedBy;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> RecalculateAllHandicapsAsync(string updatedBy)
        {
            var usersWithHandicaps = await _dbContext.SYS_Users
                                                    .Include(u => u.Handicap)
                                                    .ToListAsync();
            int updatedCount = 0;
            var logsToAdd = new List<SYS_HandicapChangeLog>();

            foreach (var user in usersWithHandicaps)
            {
                int age = PersonInfoCalculator.CalculateAge(user.UserNumber, user.UserGender);
                int newHandicap = HandicapCalculator.CalculateByAge(age);
                var handicap = user.Handicap;
                int prevHandicap = handicap?.AgeHandicap ?? 0;
                string prevSource = handicap?.Source ?? "미설정";

                if (handicap != null && handicap.AgeHandicap == newHandicap && handicap.Source == "자동")
                {
                    continue;
                }

                if (handicap == null)
                {
                    handicap = new SYS_UserHandicaps { UserId = user.UserId };
                    _dbContext.SYS_UserHandicaps.Add(handicap);
                }

                logsToAdd.Add(new SYS_HandicapChangeLog
                {
                    UserId = user.UserId,
                    Age = age,
                    PrevHandicap = prevHandicap,
                    NewHandicap = newHandicap,
                    PrevSource = prevSource,
                    NewSource = "자동",
                    Reason = "전체 자동 재계산",
                    ChangedBy = updatedBy,
                    ChangedAt = DateTime.UtcNow
                });

                handicap.AgeHandicap = newHandicap;
                handicap.Source = "자동";
                handicap.LastUpdated = DateTime.UtcNow;
                handicap.LastUpdatedBy = updatedBy;

                updatedCount++;
            }

            if (updatedCount > 0)
            {
                await _dbContext.SYS_HandicapChangeLogs.AddRangeAsync(logsToAdd);
                await _dbContext.SaveChangesAsync();
            }

            return updatedCount;
        }

        public async Task<PaginatedList<HandicapChangeLogViewModel>> GetHandicapLogsAsync(string? searchField, string? searchKeyword, int pageIndex, int pageSize)
        {
            // 1. 로그 테이블을 먼저 가져온다.
            var logsQuery = _dbContext.SYS_HandicapChangeLogs.AsQueryable();

            // 2. 검색 조건이 있으면 적용한다.
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                // UserId로 검색하는 경우는 간단하다.
                if (searchField == "UserId")
                {
                    logsQuery = logsQuery.Where(log => log.UserId.Contains(searchKeyword));
                }
                // UserName으로 검색하는 경우는, 연결된 User 테이블을 확인해야 한다.
                else if (searchField == "UserName")
                {
                    // User 테이블이 존재하고, 그 User의 이름에 검색어가 포함된 경우만 필터링
                    // 이렇게 하면 User가 삭제된 경우에도 에러가 발생하지 않는다.
                    logsQuery = logsQuery.Where(log => log.User != null && log.User.UserName.Contains(searchKeyword));
                }
            }

            // 3. 최종적으로 화면에 보여줄 ViewModel로 변환한다.
            var viewModelQuery = logsQuery
                .OrderByDescending(log => log.ChangedAt)
                .Select(log => new HandicapChangeLogViewModel
                {
                    LogId = log.LogId,
                    UserId = log.UserId,
                    // 여기서도 User가 null일 경우를 대비하여 '탈퇴한 사용자' 같은 기본값을 준다.
                    UserName = log.User != null ? log.User.UserName : "탈퇴한 사용자",
                    Age = log.Age,
                    PrevHandicap = log.PrevHandicap,
                    NewHandicap = log.NewHandicap,
                    PrevSource = log.PrevSource,
                    NewSource = log.NewSource,
                    ChangedBy = log.ChangedBy,
                    ChangedAt = log.ChangedAt,
                    Reason = log.Reason
                });

            // 4. PaginatedList 헬퍼를 사용하여 페이징 처리를 하고 결과를 반환한다.
            return await PaginatedList<HandicapChangeLogViewModel>.CreateAsync(viewModelQuery, pageIndex, pageSize);
        }
    }
}