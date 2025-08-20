namespace GisanParkGolf_Core.Pages.AdminPage.AdminPage
{
    public class CompetitionViewModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public DateTime GameDate { get; set; }
        public string? Status { get; set; }
        public string? StadiumName { get; set; }
        public int TotalParticipants { get; set; }
        public string? GameHost { get; set; }
        public string? PlayMode { get; set; }
        public string? GameNote { get; set; }
        public List<ParticipantAwardInfo> ParticipantAwards { get; set; } = new();
        public int JoinedCount { get; set; }
        public int AssignmentCount { get; set; }
        public bool AssignmentLocked { get; set; } = false;
    }
}
