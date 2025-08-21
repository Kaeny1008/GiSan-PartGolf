using System;

namespace GisanParkGolf.Pages.AdminPage.ViewModels
{
    /// <summary>
    /// 대회 참가자 배정(코스, 팀, 순번 등) 결과를 표시하는 ViewModel
    /// </summary>
    public class AssignedPlayerViewModel
    {
        public string UserId { get; set; } = string.Empty;        // 참가자 ID
        public string UserName { get; set; } = string.Empty;      // 이름
        public int AgeHandicap { get; set; }                      // 핸디캡 점수
        public string GameCode { get; set; } = string.Empty;      // 대회 코드
        public string CourseName { get; set; } = string.Empty;    // 배정된 코스명
        public int CourseOrder { get; set; }                      // 코스 내 순번 (옵션)
        public int GroupNumber { get; set; }                      // 배정된 조/그룹 번호
        public string TeamNumber { get; set; } = string.Empty;    // 팀 번호(문자)
        public string AssignmentStatus { get; set; } = string.Empty; // 배정 상태(정상/취소 등)
        public string HoleNumber { get; set; } = string.Empty;    // 홀 번호(옵션)
        public bool IsCancelled => AssignmentStatus == "취소";    // 취소 여부(간편 확인)

        // UI에서 쓸 추가 속성 예시
        public string StatusText => IsCancelled ? "취소" : "정상 배정";
    }
}