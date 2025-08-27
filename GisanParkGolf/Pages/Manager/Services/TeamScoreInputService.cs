using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Manager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf.Pages.Manager.Services
{
    public class TeamScoreInputService : ITeamScoreInputService
    {

        private readonly MyDbContext _dbContext;

        public TeamScoreInputService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TeamScoreCourseViewModel> GetTeamScoreCourses(string gameCode, string teamNumber)
        {
            var game = _dbContext.Games.FirstOrDefault(g => g.GameCode == gameCode);
            if (game == null) return new List<TeamScoreCourseViewModel>();

            var stadiumCode = game.StadiumCode;
            var courses = _dbContext.Courses
                .Where(c => c.StadiumCode == stadiumCode && c.IsActive)
                .ToList();

            // 팀 인원 조회 (팀번호, 게임코드 기준)
            var teamAssignments = _dbContext.GameUserAssignments
                .Where(a => a.GameCode == gameCode && a.TeamNumber == teamNumber)
                .Include(a => a.User)
                .ToList();

            var result = new List<TeamScoreCourseViewModel>();
            foreach (var course in courses)
            {
                var courseCode = course.CourseCode;
                var holes = _dbContext.Holes
                    .Where(h => h.CourseCode == courseCode)
                    .OrderBy(h => h.HoleId)
                    .Select(h => new HoleInfo
                    {
                        HoleId = h.HoleId,
                        HoleName = h.HoleName
                    }).ToList();

                var teamRows = teamAssignments.Select(a =>
                {
                    var scores = _dbContext.GameResultScores
                        .Where(s => s.GameCode == gameCode
                                 && s.UserId == a.UserId
                                 && s.CourseCode == courseCode)
                        .ToDictionary(s => s.HoleId, s => s.Score);

                    foreach (var hole in holes)
                    {
                        if (!scores.ContainsKey(hole.HoleId))
                            scores[hole.HoleId] = 0;
                    }

                    return new TeamScoreRow
                    {
                        CourseOrder = a.CourseOrder ?? 0,
                        TeamNumber = a.TeamNumber,
                        ParticipantName = a.User != null ? a.User.UserName : "",
                        ParticipantId = a.UserId,
                        Scores = scores
                    };
                }).ToList();

                result.Add(new TeamScoreCourseViewModel
                {
                    CourseName = course.CourseName,
                    CourseCode = course.CourseCode,
                    TeamRows = teamRows,
                    GameInformations = new GameInformation
                    {
                        GameCode = game.GameCode,
                        GameName = game.GameName
                    },
                    Holes = holes
                });
            }
            return result;
        }

        /// <summary>
        /// 입력받은 점수 데이터를 저장(기존은 수정, 없으면 신규 생성)
        /// </summary>
        /// <param name="gameCode">게임코드</param>
        /// <param name="inputBy">입력자</param>
        /// <param name="scores">점수 데이터: key는 "courseCode_userId_holeId", value는 점수</param>
        public async Task SaveScoresAsync(string gameCode, string inputBy, Dictionary<string, int> scores)
        {
            foreach (var key in scores.Keys)
            {
                // key: "courseCode_userId_holeId"
                var parts = key.Split('_');
                if (parts.Length != 3) continue;

                if (!int.TryParse(parts[0], out int courseCode)) continue;
                string userId = parts[1];
                if (!int.TryParse(parts[2], out int holeId)) continue;
                int score = scores[key];

                var exist = await _dbContext.GameResultScores
                    .FirstOrDefaultAsync(s => s.GameCode == gameCode
                                           && s.CourseCode == courseCode
                                           && s.UserId == userId
                                           && s.HoleId == holeId);

                if (exist == null)
                {
                    _dbContext.GameResultScores.Add(new GameResultScore
                    {
                        GameCode = gameCode,
                        CourseCode = courseCode,
                        UserId = userId,
                        HoleId = holeId,
                        Score = score,
                        InputBy = inputBy,
                        InputDate = DateTime.Now
                    });
                }
                else
                {
                    exist.Score = score;
                    exist.LastUpdated = DateTime.Now;
                    exist.LastUpdatedBy = inputBy;
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
