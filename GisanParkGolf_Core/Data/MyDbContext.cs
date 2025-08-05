using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<SYS_USERS> SYS_USERS { get; set; }
}

public class SYS_USERS
{
    [Key]
    public string USER_ID { get; set; }
    public string USER_NAME { get; set; }
    public string USER_PASSWORD { get; set; }
}