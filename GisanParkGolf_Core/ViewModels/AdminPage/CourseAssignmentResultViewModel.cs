namespace GisanParkGolf_Core.ViewModels.AdminPage
{
    public class CourseAssignmentResultViewModel
    {
        public string? CourseName { get; set; }
        public string? HoleNumber { get; set; }
        public string? TeamNumber { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? GenderText { get; set; }
        public string? AgeGroupText { get; set; }
        public int HandicapValue { get; set; }
        public int AwardCount { get; set; }
        // 필요하다면 수상 상세 리스트도 추가 가능
    }
}
