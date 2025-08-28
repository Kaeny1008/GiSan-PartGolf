namespace GisanParkGolf.Pages.Admin.ViewModels
{
    public class ParticipantResultViewModel
    {
        public string UserId { get; set; } = string.Empty; // 참가자 ID
        public string UserName { get; set; } = string.Empty; // 참가자 이름
        public int UserScore { get; set; } // 참가자 총 점수
        public List<ScoreDetailViewModel> ScoreDetails { get; set; } = new(); // 점수 상세 정보
    }

    public class ScoreDetailViewModel
    {
        public string CourseName { get; set; } = string.Empty; // 코스 이름
        public string HoleName { get; set; } = string.Empty; // 홀 번호
        public int Score { get; set; } // 점수
    }
}