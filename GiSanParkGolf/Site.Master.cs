using GiSanParkGolf.Models;
using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DB_Management userRepo = new DB_Management();
            UserViewModel uvm = userRepo.GetUserByUserID(Page.User.Identity.Name);
            //인증 정보 저장
            
            if (uvm.UserName == null)
            {
                lblName.Text = "손님";
            } else
            {
                lblName.Text = uvm.UserName;
            }
        }
    }
}