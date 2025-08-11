namespace GisanParkGolf_Core.ViewModels
{
    public class MyGameDetailViewModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public DateTime GameDate { get; set; }
        public string? StadiumName { get; set; }
        public string? PlayModeToText { get; set; }
        public string? GameHost { get; set; }
        public int HoleMaximum { get; set; }
        public DateTime StartRecruiting { get; set; }
        public DateTime EndRecruiting { get; set; }
        public string? GameNote { get; set; }
        public int ParticipantNumber { get; set; }
        public int? IsCancelled { get; set; } // 0:참가중, 1:취소, null:기록없음
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        public string? AssignmentStatus { get; set; }
        public string? Approval { get; set; } // 관리자 승인값
    }
}
