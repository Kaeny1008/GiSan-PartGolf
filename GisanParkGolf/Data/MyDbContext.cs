using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        // 모델 이름을 반영하여 DbSet 속성명도 규칙에 맞게 변경 (복수형)
        public DbSet<Member> Players { get; set; } = null!;
        public DbSet<Player_Handicap> PlayerHandicaps { get; set; } = null!;
        public DbSet<Player_Handicap_ChangeLog> HandicapChangeLogs { get; set; } = null!;
        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Hole> Holes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameParticipant> GameParticipants { get; set; }
        public DbSet<GameAwardHistory> GameAwardHistories { get; set; }
        public DbSet<GameUserAssignment> GameUserAssignments { get; set; }
        public DbSet<GameAssignmentHistory> GameAssignmentHistory { get; set; }
        public DbSet<GameJoinHistory> GameJoinHistories { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<GameResultScore> GameResultScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>()
                .HasOne(u => u.Handicap)
                .WithOne(h => h.User)
                .HasForeignKey<Player_Handicap>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player_Handicap_ChangeLog>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stadium>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Stadium)
                .HasForeignKey(c => c.StadiumCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Holes)
                .WithOne(h => h.Course)
                .HasForeignKey(h => h.CourseCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Participants)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.AwardHistories)
                .WithOne(a => a.Game)
                .HasForeignKey(a => a.GameCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.UserAssignments)
                .WithOne(a => a.Game)
                .HasForeignKey(a => a.GameCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Stadium)
                .WithMany()
                .HasForeignKey(g => g.StadiumCode);
        }
    }
}