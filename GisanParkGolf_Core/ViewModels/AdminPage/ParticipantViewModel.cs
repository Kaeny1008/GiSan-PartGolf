namespace GisanParkGolf_Core.ViewModels.AdminPage
{
    public class ParticipantViewModel
    {
        public string JoinId { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime JoinDate { get; set; }
        public string JoinStatus { get; set; } = "";
        public bool IsCancelled { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        public string? Approval { get; set; }
    }
}