using static GiSanParkGolf.Pages.Manager.TeamScoreInputModel;

namespace GisanParkGolf.Pages.Manager.ViewModels
{
    public class TeamScoreCourseViewModel
    {
        public GameInformation? GameInformations { get; set; }
        public string? CourseName { get; set; }
        public int CourseCode { get; set; }
        public List<int> HoleNumbers { get; set; } = new(); // 홀 번호 리스트 (1~N)
        public List<TeamScoreRow> TeamRows { get; set; } = new(); //참가자별 점수
    }

    public class TeamScoreRow
    {
        public int CourseOrder { get; set; }
        public string? TeamNumber { get; set; }
        public string? ParticipantName { get; set; }
        public string? ParticipantId { get; set; }
        public Dictionary<int, int> Scores { get; set; } = new(); // key: 홀번호, value: 점수
    }

    public class GameInformation
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
    }
}
