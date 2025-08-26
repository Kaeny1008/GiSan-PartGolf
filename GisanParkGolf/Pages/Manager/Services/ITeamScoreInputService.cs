using GisanParkGolf.Pages.Manager.ViewModels;

namespace GisanParkGolf.Pages.Manager.Services
{
    public interface ITeamScoreInputService
    {
        List<TeamScoreCourseViewModel> GetTeamScoreCourses(string gameCode, string teamNumber);
    }
}
