using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("MariaDb"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDb"))
                )
            );

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<MyDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddRazorPages();

            // 여기에서 IUserService 등록!
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllers();
            app.Run();
        }
    }
}