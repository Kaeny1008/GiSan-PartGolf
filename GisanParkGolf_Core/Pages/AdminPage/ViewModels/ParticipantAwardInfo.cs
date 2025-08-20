using GisanParkGolf_Core.Data;

namespace GisanParkGolf_Core.Pages.AdminPage.ViewModels
{
    public class ParticipantAwardInfo
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public List<GameAwardHistory>? AwardHistories { get; set; }
    }
}
