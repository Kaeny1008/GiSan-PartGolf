namespace GisanParkGolf.Pages.Player.ViewModels
{
    public class GameResultViewModel
    {
        public string UserId { get; set; } = string.Empty; // 참가자 ID
        public string UserName { get; set; } = string.Empty; // 참가자 이름
        public int TotalScore { get; set; } // 총 점수
        public int Rank { get; set; } // 순위
        public string? Award { get; set; } // 수상 내역 (선택 사항)
    }
}