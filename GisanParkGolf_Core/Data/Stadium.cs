using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.Data
{
    // 클래스 이름을 단수형으로 변경 (C# Naming Convention)
    [Table("sys_stadium_list")]
    public class Stadium
    {
        [Key]
        [Column("stadium_code")]
        public string StadiumCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "경기장 이름은 필수 항목입니다.")]
        [Column("stadium_name")]
        public string StadiumName { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        // 서버 시간은 UTC 기준으로 통일하여 시간대 문제를 원천 차단
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("note")]
        public string? Note { get; set; }

        [Column("region_name")]
        public string? RegionName { get; set; }

        [Column("city_name")]
        public string? CityName { get; set; }

        // Navigation Property: 이 경기장에 속한 코스 목록
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}