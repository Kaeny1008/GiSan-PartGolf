using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace GiSanParkGolf
{
    public class Global : HttpApplication
    {
        public static UserViewModel uvm = new UserViewModel();
        public static SelectUserViewModel suvm = new SelectUserViewModel();

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
    }
}