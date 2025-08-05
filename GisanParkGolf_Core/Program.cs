using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ���� ���� ���� (ȯ�漳��, ���񽺵�� ��)
            var builder = WebApplication.CreateBuilder(args);

            // ApplicationDbContext�� DI�� ��� (Identity, ���� ���� DB���ؽ�Ʈ)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MyDbContext�� DI�� ��� (���� ������ ������ DB���ؽ�Ʈ)
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ASP.NET Core Identity(����� ����/����) ���� ���
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                // �̸��� ���� ���� �ٷ� �α��� ����
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>(); // ApplicationDbContext�� ����� ����ҷ� ���

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // ����(Authorization) ���� ��� (��: [Authorize] ��Ʈ����Ʈ ��� ����)
            builder.Services.AddAuthorization();

            // Razor Pages(��) ���� ���
            builder.Services.AddRazorPages();

            // ���� ����
            var app = builder.Build();

            // ����ȯ���� �ƴϸ� ���� �ڵ鷯 �� HSTS ����
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // HTTPS �����̷�Ʈ �� ���� ����(�̹���, css ��) ����
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); // ����� �̵���� (��û URL �м�)

            // ���� �̵���� (�α���/��Ű Ȯ��)
            app.UseAuthentication();

            // ���� �̵���� ([Authorize] �� üũ)
            app.UseAuthorization();

            // Razor Pages ����� ����
            app.MapRazorPages();

            // �� ����
            app.Run();
        }
    }
}