using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("game_award_histories")]
    public class GameAwardHistory
    {
        [Key]
        [Column("award_id")]
        public int AwardId { get; set; }

        [Required]
        [Column("game_code", TypeName = "varchar(6)")]
        public string? GameCode { get; set; }

        [Required]
        [Column("user_id", TypeName = "varchar(15)")]
        public string? UserId { get; set; }

        [Required]
        [Column("award_type", TypeName = "varchar(50)")]
        public string? AwardType { get; set; }

        [Column("award_level", TypeName = "varchar(30)")]
        public string? AwardLevel { get; set; }

        [Column("award_date")]
        public DateTime AwardDate { get; set; }

        [Column("note", TypeName = "varchar(255)")]
        public string? Note { get; set; }

        // Navigation Properties
        [ForeignKey("GameCode")]
        public virtual Game? Game { get; set; }

        [ForeignKey("UserId")]
        public virtual Member? User { get; set; }
    }
}