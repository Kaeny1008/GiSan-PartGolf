using GisanParkGolf.Pages.Manager.ViewModels;

namespace GisanParkGolf.Pages.Manager.Services
{
    public interface ITeamScoreInputService
    {
        // 팀별 코스별 점수 입력을 위한 데이터 조회 메서드
        List<TeamScoreCourseViewModel> GetTeamScoreCourses(string gameCode, string teamNumber);
        // 점수 저장용 메서드
        Task SaveScoresAsync(string gameCode, string inputBy, Dictionary<string, int> scores);
    }
}
