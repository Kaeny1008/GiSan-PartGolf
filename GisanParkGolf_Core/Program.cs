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

        // �α��� �ý��� ����
        // ������ Identity �ý��� ���, �����ϰ� ���� '��Ű ����' �ý����� ���
        builder.Services.AddAuthentication("Identity.Application") // �� �̸��� ��Ű�� ���
            .AddCookie("Identity.Application", options =>
            {
                // �α������� ���� ����ڰ� ������ �ʿ��� �������� ����, �����
                options.LoginPath = "/Account/Login";

                // ������ ������ ����� ���Ƽ� ���� ���� ��, �����
                options.AccessDeniedPath = "/Account/AccessDenied";

                // ��Ű �̸��� ��ȿ�ð� ����
                options.Cookie.Name = "GisanParkGolf.AuthCookie";
                options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8�ð� ���� �α��� ����
            });

        // '������' ��å
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

        // �߿�: ����(Authentication)�� �㰡(Authorization)���� �׻� ����
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}