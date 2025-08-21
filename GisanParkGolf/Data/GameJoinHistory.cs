using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("game_join_history")]
    public class GameJoinHistory
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("game_code")]
        public string GameCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("user_id")]
        public string UserId { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        [Column("action_type")]
        public string ActionType { get; set; } = null!;

        [Required]
        [Column("action_date")]
        public DateTime ActionDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("action_by")]
        public string ActionBy { get; set; } = null!;

        [MaxLength(45)]
        [Column("action_ip")]
        public string? ActionIp { get; set; }

        [MaxLength(200)]
        [Column("cancel_reason")]
        public string? CancelReason { get; set; }

        [MaxLength(50)]
        [Column("participant_id")]
        public string? ParticipantId { get; set; }

        [MaxLength(200)]
        [Column("memo")]
        public string? Memo { get; set; }
    }
}