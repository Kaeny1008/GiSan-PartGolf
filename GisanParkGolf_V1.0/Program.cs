using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 웹앱 빌더 생성 (환경설정, 서비스등록 등)
            var builder = WebApplication.CreateBuilder(args);

            // ApplicationDbContext를 DI에 등록 (Identity, 인증 관련 DB컨텍스트)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MyDbContext도 DI에 등록 (별도 데이터 관리용 DB컨텍스트)
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ASP.NET Core Identity(사용자 인증/관리) 서비스 등록
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                // 이메일 인증 없이 바로 로그인 가능
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>(); // ApplicationDbContext를 사용자 저장소로 사용

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // 권한(Authorization) 서비스 등록 (예: [Authorize] 어트리뷰트 사용 가능)
            builder.Services.AddAuthorization();

            // Razor Pages(뷰) 서비스 등록
            builder.Services.AddRazorPages();

            // 웹앱 생성
            var app = builder.Build();

            // 개발환경이 아니면 에러 핸들러 및 HSTS 적용
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // HTTPS 리다이렉트 및 정적 파일(이미지, css 등) 제공
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); // 라우팅 미들웨어 (요청 URL 분석)

            // 인증 미들웨어 (로그인/쿠키 확인)
            app.UseAuthentication();

            // 권한 미들웨어 ([Authorize] 등 체크)
            app.UseAuthorization();

            // Razor Pages 라우팅 매핑
            app.MapRazorPages();

            // 앱 실행
            app.Run();
        }
    }
}