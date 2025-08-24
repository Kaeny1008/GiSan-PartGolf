using System;

namespace GisanParkGolf.Pages.Admin.ViewModels
{
    /// <summary>
    /// 관리자 대회 목록 화면에 표시되는 대회 정보 ViewModel
    /// </summary>
    public class GameListViewModel
    {
        public string? GameCode { get; set; } = string.Empty;      // 대회 고유 코드
        public string? GameName { get; set; } = string.Empty;      // 대회명
        public DateTime GameDate { get; set; }                    // 대회 일자/시간
        public string? StadiumName { get; set; } = string.Empty;   // 경기장명
        public string? GameHost { get; set; } = string.Empty;      // 주최자명
        public string? GameStatus { get; set; } = string.Empty;    // 대회 상태 (예: 진행중/종료/취소)
        public int ParticipantNumber { get; set; }                // 참가 인원 수
        public int HoleMaximum { get; set; }                      // 홀당 최대 인원
        public string? Note { get; set; } = string.Empty;
        public string? PlayMode { get; set; } = string.Empty;
        // UI에만 필요한 추가 속성 (예: 포맷팅)
        public string GameDateText => GameDate.ToString("yyyy-MM-dd HH:mm");
    }
}