using GisanParkGolf.Data;

namespace GisanParkGolf.Pages.Admin.ViewModels
{
    public class ParticipantAwardInfo
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public List<GameAwardHistory>? AwardHistories { get; set; }
    }
}
