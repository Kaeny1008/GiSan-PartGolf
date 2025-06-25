using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.BBS
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["BBSSelect"]))
            {
                Debug.WriteLine("id값이다: " + Request.QueryString["BBSSelect"].ToString());
            } else
            {
                Debug.WriteLine("id값이 없는거 같다?: " + Request.QueryString["BBSSelect"].ToString());
            }
                
            Response.Redirect("BoardList.aspx?BBSSelect=" + Request.QueryString["BBSSelect"].ToString());
        }
    }
}