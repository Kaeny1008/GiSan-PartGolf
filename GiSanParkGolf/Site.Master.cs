using System;
using System.Web.UI;

namespace GiSanParkGolf
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Global.uvm.UserName == null)
            {
                lblName.Text = "손님";
            }
            else
            {
                lblName.Text = Global.uvm.UserName;
            }
        }
    }
}