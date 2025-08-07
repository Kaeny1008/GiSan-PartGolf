using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.Data
{
    [Table("sys_users")]
    public class SYS_Users
    {
        [Key]
        [StringLength(15)]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [StringLength(25)]
        [Required]
        [Column("user_name")]
        public string UserName { get; set; } = string.Empty;

        [StringLength(100)]
        [Required]
        [Column("user_password")]
        public string UserPassword { get; set; } = string.Empty;

        [Required]
        [Column("user_number")]
        public int UserNumber { get; set; }

        [Required]
        [Column("user_gender")]
        public int UserGender { get; set; }

        [StringLength(70)]
        [Required]
        [Column("user_address")]
        public string UserAddress { get; set; } = string.Empty;

        [StringLength(70)]
        [Column("user_address2")]
        public string? UserAddress2 { get; set; }

        [Required]
        [Column("user_registration_date")]
        public DateTime UserRegistrationDate { get; set; }

        [StringLength(255)]
        [Column("user_note")]
        public string? UserNote { get; set; }

        [StringLength(8)]
        [Column("user_wclass")]
        public string UserWClass { get; set; } = "승인대기"; // 기본값을 "승인대기"로 설정

        [Column("user_class")]
        public int UserClass { get; set; } = 3; // 기본값을 3으로 설정 (Member)

        public virtual SYS_UserHandicaps? Handicap { get; set; }
    }
}