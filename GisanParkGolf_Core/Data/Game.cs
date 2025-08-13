using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.Data
{
    [Table("game_list")]
    public class Game
    {
        [Key]
        [Column("game_code", TypeName = "varchar(6)")]
        public string? GameCode { get; set; }

        [Required(ErrorMessage = "대회명을 입력하세요.")]
        [Column("game_name", TypeName = "varchar(50)")]
        public string? GameName { get; set; }

        [Column("game_date")]
        public DateTime GameDate { get; set; }

        [Required]
        [Column("stadium_code", TypeName = "varchar(10)")]
        public string? StadiumCode { get; set; }

        [Column("stadium_name", TypeName = "varchar(50)")]
        public string? StadiumName { get; set; }

        [Required(ErrorMessage = "주최자를 입력하세요.")]
        [Column("game_host", TypeName = "varchar(100)")]
        public string? GameHost { get; set; }

        [Column("start_recruiting")]
        public DateTime StartRecruiting { get; set; }

        [Column("end_recruiting")]
        public DateTime EndRecruiting { get; set; }

        [Column("hole_maximum")]
        [Required(ErrorMessage = "홀당 최대인원을 입력하세요.")]
        [Range(1, int.MaxValue, ErrorMessage = "홀당 최대인원은 1 이상이어야 합니다.")]
        public int HoleMaximum { get; set; }

        [Column("play_mode", TypeName = "varchar(20)")]
        public string? PlayMode { get; set; }

        [Column("participant_number")]
        public int ParticipantNumber { get; set; } = 0;

        [Column("game_status", TypeName = "varchar(10)")]
        public string GameStatus { get; set; } = "Ready";

        [Column("game_setting", TypeName = "varchar(255)")]
        public string? GameSetting { get; set; }

        [Column("game_note", TypeName = "varchar(255)")]
        public string? GameNote { get; set; }

        [Column("post_date")]
        public DateTime PostDate { get; set; } = DateTime.Now;

        [Column("post_ip", TypeName = "varchar(15)")]
        public string? PostIp { get; set; }

        [Column("post_user", TypeName = "varchar(25)")]
        public string? PostUser { get; set; }

        // Navigation Properties
        public virtual ICollection<GameParticipant> Participants { get; set; } = new List<GameParticipant>();
        public virtual ICollection<GameAwardHistory> AwardHistories { get; set; } = new List<GameAwardHistory>();
        public virtual ICollection<GameUserAssignment> UserAssignments { get; set; } = new List<GameUserAssignment>();

        [ForeignKey("StadiumCode")]
        public Stadium? Stadium { get; set; }
    }

    public class GameViewModel : Game
    {
        public bool IsParticipating { get; set; }
    }
}