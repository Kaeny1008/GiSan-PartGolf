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

            var teamRows = teamAssignments.Select(a => new TeamScoreRow
            {
                CourseOrder = a.CourseOrder ?? 0,
                TeamNumber = a.TeamNumber,
                ParticipantName = a.User != null ? a.User.UserName : "",
                ParticipantId = a.UserId,
                Scores = new Dictionary<int, int>()
            }).ToList();

            var result = new List<TeamScoreCourseViewModel>();
            foreach (var course in courses)
            {
                // *** 핵심! hole_count만큼 1~N 번호 리스트 ***
                var holeNumbers = Enumerable.Range(1, course.HoleCount).ToList();

                result.Add(new TeamScoreCourseViewModel
                {
                    CourseName = course.CourseName,
                    HoleNumbers = holeNumbers,
                    TeamRows = teamRows // 모든 코스에 동일 참가자!
                });
            }
            return result;
        }
    }
}
