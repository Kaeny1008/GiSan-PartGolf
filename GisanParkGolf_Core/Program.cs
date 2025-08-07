using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Identity; // Identity ����� ���� �ʼ�!
using Microsoft.EntityFrameworkCore;
using T_Engine;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 1. �����ͺ��̽� �� ���� ��� ---
        var connectionString = builder.Configuration.GetConnectionString("MariaDb");
        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        // ���񽺵��� ���⿡ ��� (Singleton, Scoped ��)
        builder.Services.AddSingleton<Cryptography>();
        builder.Services.AddScoped<IUserService, UserService>();


        // --- 2. ���� �� ���� ���� (ASP.NET Core Identity ���) ---
        // UserManager, SignInManager ���� ��� �����ϴ� Identity �ý��� ���
        builder.Services.AddDefaultIdentity<IdentityUser>(options => {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<MyDbContext>();

        // Identity�� ����ϴ� ��Ű�� ������ �츮 �Ը��� �°� ����
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "GisanParkGolf.AuthCookie";
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        // ���� ��å ����: "IsAdmin" Ŭ������ "true"�� ����ڸ� ���
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("IsAdmin", "true"));
        });


        // --- 3. ��Ÿ �� ���� ---
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        // --- 4. ���ø����̼� ���� �� ���� ---
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // ���� �߿�: Authentication -> Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}