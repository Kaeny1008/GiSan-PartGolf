using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Web.UI;

namespace GiSanParkGolf
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Session["UserInfo"] as UserViewModel;

            lblName.Text = user != null ? user.UserName : "손님";
        }
    }
}