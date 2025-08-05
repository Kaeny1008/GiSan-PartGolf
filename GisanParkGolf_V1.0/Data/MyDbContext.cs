using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<SYS_User> SYS_Users { get; set; }
}

public class SYS_User
{
    [Key]
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserPassword { get; set; }
}