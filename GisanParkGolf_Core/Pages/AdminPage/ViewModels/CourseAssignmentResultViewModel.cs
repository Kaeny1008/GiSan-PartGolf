namespace GisanParkGolf_Core.Pages.AdminPage.AdminPage
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
        public int GroupNumber { get; set; }
        public int CourseOrder { get; set; }

        // 대회 정보 필드
        public string? GameName { get; set; }
        public string? GameDate { get; set; }
        public string? StadiumName { get; set; }
    }
}
