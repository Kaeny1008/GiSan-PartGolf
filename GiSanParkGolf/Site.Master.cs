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
            if (Session["UserInfo"] == null)
            {
                lblName.Text = "손님";
            }
            else
            {
                lblName.Text = Helper.CurrentUser?.UserName;
            }
        }
    }
}