using GisanParkGolf.Data;
using GisanParkGolf.Pages.Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            var results = _dbContext.MissingScoreInfoViewModel
                .FromSqlInterpolated($@"
                SELECT
                    g.game_code AS GameCode,
                    g.game_name AS GameName,
                    u.team_number AS TeamNumber,
                    u.user_id AS UserId,
                    m.user_name AS UserName,
                    GROUP_CONCAT(CONCAT(c.course_name, '-', h.hole_name) 
                        ORDER BY c.course_name, h.hole_id SEPARATOR ',') AS MissingCoursesAndHolesRaw
                FROM game_list g
                JOIN game_user_assignments u ON u.game_code = g.game_code
                JOIN sys_users m ON m.user_id = u.user_id
                JOIN sys_course_list c ON c.stadium_code = g.stadium_code
                JOIN sys_hole_info h ON h.course_code = c.course_code
                LEFT JOIN game_result_score s ON
                    s.game_code = g.game_code AND
                    s.user_id = u.user_id AND
                    s.course_code = c.course_code AND
                    s.hole_id = h.hole_id
                WHERE s.score_id IS NULL OR s.score IS NULL
                  AND ({gameCode} IS NULL OR g.game_code = {gameCode})
                  AND (g.game_status = 'Running' OR g.game_status = 'PendingClose')
                GROUP BY g.game_code, g.game_name, u.team_number, u.user_id, m.user_name;
                ")
                .AsNoTracking()
                .AsEnumerable()
                .Select(x =>
                {
                    if (!string.IsNullOrEmpty(x.MissingCoursesAndHolesRaw))
                    {
                        var items = x.MissingCoursesAndHolesRaw
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Select(s =>
                            {
                                // 정규화: "A 코스-1번 홀", "A-1번 홀" 등을 "A-1번 홀" 형식으로
                                var regex = new Regex(@"([A-Za-z]|[AB])(?:\s*코스)?-(\d+)(?:번\s*홀)?", RegexOptions.IgnoreCase);
                                var m = regex.Match(s);
                                if (m.Success)
                                {
                                    var course = m.Groups[1].Value.ToUpper();
                                    var holeNum = int.Parse(m.Groups[2].Value);
                                    return $"{course}-{holeNum}번 홀";
                                }
                                return s;
                            })
                            .Distinct()
                            .OrderBy(s => GetCourseSortKey(s))
                            .ToList();

                        x.MissingCoursesAndHoles = items;
                    }
                    else
                    {
                        x.MissingCoursesAndHoles = new List<string>();
                    }

                    return x;
                })
                .ToList();

            return results;
        }

        // 코스명/홀번호 기준 정렬용 키 추출 함수
        private static string GetCourseSortKey(string value)
        {
            var regex = new Regex(@"([A-Za-z]|[AB])(?:\s*코스)?-(\d+)번 홀", RegexOptions.IgnoreCase);
            var match = regex.Match(value);
            if (match.Success)
            {
                var course = match.Groups[1].Value.ToUpper();
                var hole = int.Parse(match.Groups[2].Value);
                return $"{course}-{hole:D2}";
            }
            return value;
        }
    }
}