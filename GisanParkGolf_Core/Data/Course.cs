using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.Data
{
    [Table("sys_course_list")]
    public class Course
    {
        [Key]
        [Column("course_code")]
        public int CourseCode { get; set; }

        [Column("stadium_code")]
        public string StadiumCode { get; set; } = string.Empty;

        [Column("course_name")]
        public string CourseName { get; set; } = string.Empty;

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