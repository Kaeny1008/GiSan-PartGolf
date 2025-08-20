using GisanParkGolf_Core.Data;

namespace GisanParkGolf_Core.Pages.AdminPage.AdminPage
{
    public class ParticipantAwardInfo
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public List<GameAwardHistory>? AwardHistories { get; set; }
    }
}
