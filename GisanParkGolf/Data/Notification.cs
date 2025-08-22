using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf.Data
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; } // PK

        [Required]
        [StringLength(15)]
        [Column("user_id")]
        public string UserId { get; set; } = null!; // FK (sys_users.user_id)

        [Required]
        [StringLength(30)]
        [Column("type")]
        public string Type { get; set; } = null!; // 알림 종류

        [Required]
        [StringLength(100)]
        [Column("title")]
        public string Title { get; set; } = null!; // 알림 제목

        [Required]
        [StringLength(255)]
        [Column("message")]
        public string Message { get; set; } = null!; // 알림 상세 내용

        [Required]
        [Column("is_read")]
        public bool IsRead { get; set; } = false; // 읽음 여부

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // 생성일시

        [StringLength(255)]
        [Column("link_url")]
        public string? LinkUrl { get; set; } // 상세페이지 링크 (선택)

        // 관계: Player (Navigation property)
        [ForeignKey("UserId")]
        public virtual Player? User { get; set; }
    }

    public static class NotificationTypes
    {
        public const string CancelApproved = "참가취소 승인";
        public const string RejoinApproved = "재참가 승인";
    }
}