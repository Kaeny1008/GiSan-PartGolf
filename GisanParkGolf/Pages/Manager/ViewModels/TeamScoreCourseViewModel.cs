using static GiSanParkGolf.Pages.Manager.TeamScoreInputModel;

namespace GisanParkGolf.Pages.Manager.ViewModels
{
    public class TeamScoreCourseViewModel
    {
        public GameInformation? GameInformations { get; set; }
        public string? CourseName { get; set; }
        public int CourseCode { get; set; }
        public List<TeamScoreRow> TeamRows { get; set; } = new();
        public List<HoleInfo> Holes { get; set; } = new();
    }

    public class TeamScoreRow
    {
        public int CourseOrder { get; set; }
        public string? TeamNumber { get; set; }
        public string? ParticipantName { get; set; }
        public string? ParticipantId { get; set; }
        public Dictionary<int, int?> Scores { get; set; } = new(); // 반드시 int?로!
    }

    public class GameInformation
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
    }

    public class HoleInfo
    {
        public int HoleId { get; set; }
        public string? HoleName { get; set; }
    }
}