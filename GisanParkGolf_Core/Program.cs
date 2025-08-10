using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services;
using Microsoft.EntityFrameworkCore;
using T_Engine;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("MariaDb");

        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        builder.Services.AddSingleton<Cryptography>();
        builder.Services.AddScoped<IPlayerservice, Playerservice>();
        builder.Services.AddScoped<IHandicapService, HandicapService>();
        builder.Services.AddScoped<IPlayerService, PlayerService>();
        builder.Services.AddScoped<IStadiumService, StadiumService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IJoinGameService, JoinGameService>();

        // 로그인 시스템 설정
        // 복잡한 Identity 시스템 대신, 간단하고 빠른 '쿠키 인증' 시스템을 사용
        builder.Services.AddAuthentication("Identity.Application") // 이 이름의 쿠키를 사용
            .AddCookie("Identity.Application", options =>
            {
                // 로그인하지 않은 사용자가 권한이 필요한 페이지에 오면, 여기로
                options.LoginPath = "/Account/Login";

                // 권한은 있지만 등급이 낮아서 접근 못할 때, 여기로
                options.AccessDeniedPath = "/Account/AccessDenied";

                // 쿠키 이름과 유효시간 설정
                options.Cookie.Name = "GisanParkGolf.AuthCookie";
                options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8시간 동안 로그인 유지
            });

        // '관리자' 정책
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("IsAdmin", "true"));
            options.AddPolicy("ManagerOnly", policy =>
                policy.RequireClaim("IsManager", "true"));
            options.AddPolicy("MemberOnly", policy =>
                policy.RequireClaim("IsMember", "true"));
        });

        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // 중요: 인증(Authentication)이 허가(Authorization)보다 항상 먼저
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}