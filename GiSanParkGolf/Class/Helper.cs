using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GiSanParkGolf.Class
{
    public static class Helper
    {
        public static void RequireAdmin(Page page)
        {
            if (Helper.CurrentUser?.UserClass != 1)
            {
                page.Response.Redirect("~/Sites/Login/Admin Alert.aspx");
            }
        }

        public static string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
        }

        public static UserViewModel CurrentUser
        {
            get
            {
                return HttpContext.Current.Session["UserInfo"] as UserViewModel;
            }
        }
    }
}