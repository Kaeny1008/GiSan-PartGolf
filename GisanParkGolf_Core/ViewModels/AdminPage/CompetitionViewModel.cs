namespace GisanParkGolf_Core.ViewModels.AdminPage
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
    }
}
