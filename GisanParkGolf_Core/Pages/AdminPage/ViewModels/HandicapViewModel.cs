using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf_Core.Pages.AdminPage.AdminPage
{
    // 화면에 핸디캡 정보를 표시하기 위한 전용 모델
    public class HandicapViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int UserNumber { get; set; }
        public int Age { get; set; }

        [Required]
        public int AgeHandicap { get; set; }

        [Required]
        public string Source { get; set; } = string.Empty;
        public DateTime? LastUpdated { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}