namespace GisanParkGolf.Pages.PlayerPage.ViewModels
{
    /// <summary>
    /// 전체 코스배정 결과 테이블의 단일 행 정보 모델
    /// </summary>
    public class AssignmentResultModel
    {
        public string UserId { get; set; } = "";   // 참가자 아이디
        public string UserName { get; set; } = ""; // 참가자 이름
        public string CourseName { get; set; } = "";// 배정된 코스명
        public string HoleNumber { get; set; } = "";// 배정된 홀번호
        public string TeamNumber { get; set; } = "";// 배정된 팀번호
        public int? CourseOrder { get; set; } = 1; // 배정된 코스 순서 (1, 2, 3 등)
    }
}
