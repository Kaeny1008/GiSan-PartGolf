using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Pages.Manager.ViewModels
{
    public class MissingScoreInfoViewModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public string? TeamNumber { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? MissingCoursesAndHolesRaw { get; set; }
        [NotMapped]
        public List<string>? MissingCoursesAndHoles { get; set; }
    }

    public class GameInfoViewModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public DateTime GameDate { get; set; }
    }
}
