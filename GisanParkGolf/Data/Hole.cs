using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("sys_hole_info")]
    public class Hole
    {
        [Key]
        [Column("hole_id")]
        public int HoleId { get; set; }

        [Column("course_code")]
        public int CourseCode { get; set; }

        [Column("hole_name")]
        public string HoleName { get; set; } = string.Empty;

        [Column("distance")]
        public int Distance { get; set; }

        [Column("par")]
        public int Par { get; set; }

        // Navigation Property
        [ForeignKey("CourseCode")]
        public virtual Course? Course { get; set; }
    }
}