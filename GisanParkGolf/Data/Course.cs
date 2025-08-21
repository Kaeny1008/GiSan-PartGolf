using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("sys_course_list")]
    public class Course
    {
        [Key]
        [Column("course_code")]
        public int CourseCode { get; set; }

        [Column("stadium_code")]
        public string StadiumCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "코스 이름은 필수 항목입니다.")]
        [Column("course_name")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "홀 수는 필수 항목입니다.")]
        [Range(1, int.MaxValue, ErrorMessage = "홀 수는 1 이상의 숫자로 입력해주세요.")]
        [Column("hole_count")]
        public int HoleCount { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("StadiumCode")]
        public virtual Stadium? Stadium { get; set; }
        public virtual ICollection<Hole> Holes { get; set; } = new List<Hole>();
    }
}