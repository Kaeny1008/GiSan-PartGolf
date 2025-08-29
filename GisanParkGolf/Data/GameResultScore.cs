using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("game_result_score")]
    public class GameResultScore
    {
        [Key]
        [Column("score_id")]
        public long ScoreId { get; set; }

        [Required]
        [Column("game_code", TypeName = "varchar(13)")]
        public string GameCode { get; set; } = string.Empty;

        [ForeignKey("GameCode")]
        public virtual Game? Game { get; set; } // Navigation property

        [Required]
        [Column("user_id", TypeName = "varchar(50)")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual Member? User { get; set; }

        [Required]
        [Column("course_code")]
        public int CourseCode { get; set; }

        [ForeignKey("CourseCode")]
        public virtual Course? Course { get; set; }

        [Required]
        [Column("hole_id")]
        public int HoleId { get; set; }

        [ForeignKey("HoleId")]
        public virtual Hole? Hole { get; set; }

        
        [Column("score")]
        public int? Score { get; set; }

        [Required]
        [Column("input_date")]
        public DateTime InputDate { get; set; } = DateTime.Now;

        [Required]
        [Column("input_by", TypeName = "varchar(25)")]
        public string InputBy { get; set; } = string.Empty;

        [Column("memo", TypeName = "varchar(255)")]
        public string? Memo { get; set; }

        [Column("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [Column("last_updated_by", TypeName = "varchar(25)")]
        public string? LastUpdatedBy { get; set; }
    }
}