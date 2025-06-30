using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

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