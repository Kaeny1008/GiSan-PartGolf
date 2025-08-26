using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("sys_handicap_change_logs")]
    public class Player_Handicap_ChangeLog
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }

        [Required]
        [Column("user_id")]
        [StringLength(15)]
        public string UserId { get; set; } = string.Empty;

        [Column("age")]
        public int Age { get; set; }

        [Required]
        [Column("prev_handicap")]
        public int PrevHandicap { get; set; }

        [Required]
        [Column("new_handicap")]
        public int NewHandicap { get; set; }

        [Required]
        [Column("prev_source")]
        [StringLength(10)]
        public string PrevSource { get; set; } = string.Empty;

        [Required]
        [Column("new_source")]
        [StringLength(10)]
        public string NewSource { get; set; } = string.Empty;

        [Column("changed_by")]
        [StringLength(50)]
        public string? ChangedBy { get; set; }

        [Required]
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }

        [Column("reason")]
        [StringLength(100)]
        public string? Reason { get; set; }

        // --- Navigation Property ---
        [ForeignKey("UserId")]
        public virtual Member User { get; set; } = null!;
    }
}