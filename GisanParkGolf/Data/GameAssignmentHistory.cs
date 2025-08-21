using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("game_assignment_history")]
    public class GameAssignmentHistory
    {
        [Key]
        [Column("history_id")]
        public int HistoryId { get; set; }

        [Required]
        [Column("game_code", TypeName = "varchar(50)")]
        public string? GameCode { get; set; }

        [Column("changed_by", TypeName = "varchar(50)")]
        public string? ChangedBy { get; set; }

        [Column("changed_at")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        // 예: "Save", "Finalize", "Unlock", "CancelAssignment", "ForceAssign"
        [Column("change_type", TypeName = "varchar(50)")]
        public string? ChangeType { get; set; }

        // 상세는 JSON 또는 텍스트로 보관(예: 요약 또는 diff JSON)
        [Column("details", TypeName = "text")]
        public string? Details { get; set; }
    }
}