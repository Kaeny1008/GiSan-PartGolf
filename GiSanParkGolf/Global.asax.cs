using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace GiSanParkGolf
{
    public class Global : HttpApplication
    {
        public static UserViewModel uvm = new UserViewModel();
        public static SelectUserViewModel suvm = new SelectUserViewModel();
        public static DB_Management dbManager = new DB_Management();
        public static GameListModel gameList = new GameListModel();
        public static Search_Property searchProperty = new Search_Property();

        void Application_Start(object sender, EventArgs e)
        {
            // 애플리케이션 시작 시 실행되는 코드
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //ScriptResourceDefinition myScriptResDef = new ScriptResourceDefinition();
            //myScriptResDef.Path = "~/Scripts/jquery-3.7.1.min.js";
            //myScriptResDef.DebugPath = "~/Scripts/jquery-3.7.1.js";
            //myScriptResDef.CdnPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-3.7.1.min.js";
            //myScriptResDef.CdnDebugPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-3.7.1.js";

            //ScriptManager.ScriptResourceMapping.AddDefinition("jquery", null, myScriptResDef);

            //ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.WebForms;
            //위에 라인을 주석처리하면 유효성 검사 결과가 다르게 나타난다.
        }

        void Application_End(object sender, EventArgs e)
        {
            Debug.WriteLine("로그아웃한다.");
            Response.Redirect("~/Sites/Login/Logout");
        }
    }
}