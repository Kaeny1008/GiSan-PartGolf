using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.Data
{
    // MariaDB의 'sys_PlayerHandicaps' 테이블과 정확히 일치하도록 설정
    [Table("sys_userhandicaps")]
    public class Player_Handicap
    {
        // Primary Key & Foreign Key
        [Key]
        [Column("user_id")]
        [StringLength(15)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("age_handicap")]
        public int AgeHandicap { get; set; }

        [Required]
        [Column("source")]
        [StringLength(10)]
        public string Source { get; set; } = "자동"; // 기본값 설정

        [Column("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [Column("last_updated_by")]
        [StringLength(50)]
        public string? LastUpdatedBy { get; set; }

        // --- Navigation Property ---
        // 이 핸디캡 정보가 어떤 사용자의 것인지 알려주는 '연결고리' 역할
        // 실제 데이터베이스 컬럼으로 생성되지는 않음
        [ForeignKey("UserId")]
        public virtual Player User { get; set; } = null!;
    }
}