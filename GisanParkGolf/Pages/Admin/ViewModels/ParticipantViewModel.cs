namespace GisanParkGolf.Pages.Admin.ViewModels
{
    public class ParticipantViewModel
    {
        public string JoinId { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime? JoinDate { get; set; }
        public string JoinDateText =>
            JoinDate == null || JoinDate == DateTime.MinValue
            ? "확인불가"
            : JoinDate.Value.ToString("yyyy-MM-dd");
        public string JoinStatus { get; set; } = "";
        public bool IsCancelled { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        public string? Approval { get; set; }
        public string? GenderText { get; set; }
        public int HandicapValue { get; set; } = 0;
        public string AgeGroupText { get; set; } = "";
        public int AwardCount { get; set; } = 0;
        public int? UserNumber { get; set; }
        public int? UserGender { get; set; }
        public HandicapViewModel? Handicap { get; set; }
    }
}