using GisanParkGolf_Core.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.Data
{
    [Table("game_participants")]
    public class GameParticipant
    {
        [Key]
        [Column("join_id", TypeName = "varchar(27)")]
        public string? JoinId { get; set; }

        [Required]
        [Column("game_code", TypeName = "varchar(6)")]
        public string? GameCode { get; set; }

        [Required]
        [Column("user_id", TypeName = "varchar(15)")]
        public string? UserId { get; set; }

        [Column("join_date")]
        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Required]
        [Column("join_ip", TypeName = "varchar(15)")]
        public string? JoinIp { get; set; }

        [Column("join_status", TypeName = "varchar(6)")]
        public string JoinStatus { get; set; } = "Join";

        [Column("is_cancelled")]
        public bool IsCancelled { get; set; } = false;

        [Column("cancel_date")]
        public DateTime? CancelDate { get; set; }

        [Column("cancel_reason", TypeName = "varchar(200)")]
        public string? CancelReason { get; set; }

        [Column("approval", TypeName = "varchar(15)")]
        public string? Approval { get; set; }

        // Navigation Properties
        [ForeignKey("GameCode")]
        public virtual Game? Game { get; set; }

        // SYS_Users 테이블과 연결될 Navigation Property
        [ForeignKey("UserId")]
        public virtual Player? User { get; set; }
    }
}