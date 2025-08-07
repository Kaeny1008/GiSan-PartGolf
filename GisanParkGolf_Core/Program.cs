using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Identity; // Identity 사용을 위해 필수!
using Microsoft.EntityFrameworkCore;
using T_Engine;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 1. 데이터베이스 및 서비스 등록 ---
        var connectionString = builder.Configuration.GetConnectionString("MariaDb");
        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        // 서비스들을 여기에 등록 (Singleton, Scoped 등)
        builder.Services.AddSingleton<Cryptography>();
        builder.Services.AddScoped<IUserService, UserService>();


        // --- 2. 인증 및 권한 설정 (ASP.NET Core Identity 사용) ---
        // UserManager, SignInManager 등을 모두 제공하는 Identity 시스템 등록
        builder.Services.AddDefaultIdentity<IdentityUser>(options => {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<MyDbContext>();

        // Identity가 사용하는 쿠키의 설정을 우리 입맛에 맞게 변경
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "GisanParkGolf.AuthCookie";
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        // 권한 정책 설정: "IsAdmin" 클레임이 "true"인 사용자만 통과
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("IsAdmin", "true"));
        });


        // --- 3. 기타 웹 설정 ---
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        // --- 4. 애플리케이션 빌드 및 실행 ---
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // 순서 중요: Authentication -> Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}