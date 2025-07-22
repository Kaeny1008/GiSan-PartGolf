using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI;

namespace GiSanParkGolf
{
    public class Global : HttpApplication
    {
        public static UserViewModel uvm = new UserViewModel();
        public static SelectUserViewModel suvm = new SelectUserViewModel();
        public static DB_Management dbManager = new DB_Management();
        public static GameListModel gameList = new GameListModel();
        public static Search_Property searchProperty = new Search_Property();
        public static GameJoinUserModel GameJoinUser = new GameJoinUserModel();

        void Application_Start(object sender, EventArgs e)
        {
            // 애플리케이션 시작 시 실행되는 코드
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_End(object sender, EventArgs e)
        {
            Debug.WriteLine("로그아웃한다.");
            Response.Redirect("~/Sites/Login/Logout");
        }
    }
}