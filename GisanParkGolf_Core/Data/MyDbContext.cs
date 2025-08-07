using GisanParkGolf_Core.Data;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<SYS_Users> SYS_Users { get; set; } = null!;
        public DbSet<SYS_UserHandicaps> SYS_UserHandicaps { get; set; } = null!;
        public DbSet<SYS_HandicapChangeLog> SYS_HandicapChangeLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SYS_Users와 SYS_UserHandicaps 간의 1:1 관계 설정
            // 한 명의 유저는 하나의 핸디캡 정보를 가짐
            modelBuilder.Entity<SYS_Users>()
                .HasOne(u => u.Handicap)
                .WithOne(h => h.User)
                .HasForeignKey<SYS_UserHandicaps>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 유저가 삭제되면 핸디캡 정보도 함께 삭제

            // SYS_HandicapChangeLog와 SYS_Users 간의 1:N 관계 설정
            // 한 명의 유저는 여러 개의 핸디캡 변경 로그를 가질 수 있음
            modelBuilder.Entity<SYS_HandicapChangeLog>()
                .HasOne(l => l.User)
                .WithMany() // SYS_Users 쪽에서 로그 목록을 참조할 필요가 없으므로 비워둠
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 유저가 삭제되어도 로그는 보존
        }
    }
}