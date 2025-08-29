namespace GisanParkGolf.Pages.Player.ViewModels
{
    public class HoleResultViewModel
    {
        public string CourseName { get; set; } = string.Empty; // 코스 이름
        public int HoleId { get; set; } // 홀 ID
        public string HoleName { get; set; } = string.Empty; // 홀 이름
        public int Score { get; set; } // 점수
    }
}