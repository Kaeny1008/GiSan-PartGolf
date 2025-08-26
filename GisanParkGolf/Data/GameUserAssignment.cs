using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("game_user_assignments")]
    public class GameUserAssignment
    {
        [Key]
        [Column("assignment_id")]
        public int AssignmentId { get; set; }

        [Required]
        [Column("game_code", TypeName = "varchar(50)")]
        public string? GameCode { get; set; }

        [Required]
        [Column("user_id", TypeName = "varchar(50)")]
        public string? UserId { get; set; }

        [Column("course_name", TypeName = "varchar(50)")]
        public string? CourseName { get; set; }

        [Column("hole_number", TypeName = "varchar(50)")]
        public string? HoleNumber { get; set; }

        [Column("team_number", TypeName = "varchar(10)")]
        public string? TeamNumber { get; set; }

        [Column("group_number")]
        public int? GroupNumber { get; set; }

        [Column("course_order")]
        public int? CourseOrder { get; set; }

        [Column("age_handicap")]
        public int? AgeHandicap { get; set; }

        [Column("assigned_date")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [Column("assignment_status", TypeName = "varchar(50)")]
        public string AssignmentStatus { get; set; } = "Assigned";

        // Navigation Properties
        [ForeignKey("GameCode")]
        public virtual Game? Game { get; set; }

        [ForeignKey("UserId")]
        public virtual Player_Handicap? Handicap { get; set; }

        [ForeignKey("UserId")]
        public virtual Member? User { get; set; }
    }
}