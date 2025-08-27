using GisanParkGolf.Data;
using GisanParkGolf.Pages.Manager.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf.Pages.Manager.Services
{
    public class ScoreMissingReportServece : IScoreMissingReportServece
    {
        private readonly MyDbContext _dbContext;

        public ScoreMissingReportServece(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 진행중 또는 종료대기중 대회만 반환
        public List<GameInfoViewModel> GetRunningOrPendingGames()
        {
            var statusList = new[] { "Running", "PendingClose" };

            return _dbContext.Games
                .Where(g => statusList.Contains(g.GameStatus))
                .OrderByDescending(g => g.GameDate)
                .Select(g => new GameInfoViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName
                })
                .ToList();
        }

        public List<MissingScoreInfoViewModel> GetMissingScoreList(string? gameCode)
        {
            var sql = @"
                SELECT
                    g.game_code AS GameCode,
                    g.game_name AS GameName,
                    u.team_number AS TeamNumber,
                    u.user_id AS UserId,
                    m.user_name AS UserName,
                    GROUP_CONCAT(CONCAT(c.course_name, '-', h.hole_name)) AS MissingCoursesAndHolesRaw
                FROM game_list g
                JOIN game_user_assignments u ON u.game_code = g.game_code
                JOIN sys_users m ON m.user_id = u.user_id
                CROSS JOIN sys_course_list c
                CROSS JOIN sys_hole_info h
                LEFT JOIN game_result_score s ON
                    s.game_code = g.game_code AND
                    s.user_id = u.user_id AND
                    s.course_code = c.course_code AND
                    s.hole_id = h.hole_id
                WHERE s.score_id IS NULL
                  AND (@gameCode IS NULL OR g.game_code = @gameCode)
                  AND (g.game_status = 'Running' OR g.game_status = 'PendingClose')
                GROUP BY g.game_code, g.game_name, u.team_number, u.user_id, m.user_name
            ";

            var result = _dbContext.MissingScoreInfoViewModel
                .FromSqlRaw(sql, new object[] { gameCode ?? "" })
                .AsEnumerable()
                .Select(x =>
                {
                    x.MissingCoursesAndHoles = !string.IsNullOrEmpty(x.MissingCoursesAndHolesRaw)
                        ? x.MissingCoursesAndHolesRaw
                            .Split(',')
                            .Select(s => s.Trim())
                            .Select(s => { // ★ 추가!
                                var regex = new System.Text.RegularExpressions.Regex(@"([AB])(?:\s*코스)?-(\d+)번 홀");
                                var match = regex.Match(s);
                                if (match.Success)
                                    return $"{match.Groups[1].Value}-{int.Parse(match.Groups[2].Value)}번 홀";
                                return s;
                            })
                            .Distinct()
                            .OrderBy(s => GetCourseSortKey(s))
                            .ToList()
                        : new List<string>();
                    return x;
                }).ToList();

            return result;
        }

        // 코스명/홀번호 기준 정렬용 키 추출 함수
        private static string GetCourseSortKey(string value)
        {
            // "A 코스-1번 홀" → "A-1", "B-7번 홀" → "B-7"
            var regex = new System.Text.RegularExpressions.Regex(@"([AB])(?:\s*코스)?-(\d+)번 홀");
            var match = regex.Match(value);
            if (match.Success)
            {
                var course = match.Groups[1].Value;
                var hole = int.Parse(match.Groups[2].Value);
                // 코스명(A, B) + 홀번호(2자리로)
                return $"{course}-{hole:D2}";
            }
            return value;
        }
    }
}