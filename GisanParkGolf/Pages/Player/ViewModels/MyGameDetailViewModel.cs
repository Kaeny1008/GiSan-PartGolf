using GisanParkGolf.Data;
using GisanParkGolf.Helpers; // PlayModeHelper 네임스페이스 추가!
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.ViewModels.Player
{
    public class MyGameDetailViewModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public DateTime GameDate { get; set; }
        public string? StadiumName { get; set; }
        public string? PlayMode { get; set; }

        [NotMapped]
        public string PlayModeDisplay => PlayModeHelper.ToKorDisplay(PlayMode);

        // 필요하다면 영어도 이렇게!
        [NotMapped]
        public string PlayModeDisplayEng => PlayModeHelper.ToEngDisplay(PlayMode);

        public string? GameHost { get; set; }
        public int HoleMaximum { get; set; }
        public DateTime StartRecruiting { get; set; }
        public DateTime EndRecruiting { get; set; }
        public string? GameNote { get; set; }
        public int ParticipantNumber { get; set; }
        public int? IsCancelled { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        public string? AssignmentStatus { get; set; }
        public string? Approval { get; set; }

        // 코스배치 결과 관련
        public string? AssignedCourseName { get; set; }
        public string? AssignedHoleNumber { get; set; }
        public string? AssignedTeamNumber { get; set; }
        public int? AssignedCourseOrder { get; set; }

        public List<GameAssignmentResultViewModel>? AllAssignments { get; set; }
    }

    public class GameAssignmentResultViewModel
    {
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? CourseName { get; set; }
        public string? HoleNumber { get; set; }
        public string? TeamNumber { get; set; }
        public int? CourseOrder { get; set; }
        public int? UserGender { get; set; }
        public int? UserHandicap { get; set; }
        public int? UserAge { get; set; }
    }
}