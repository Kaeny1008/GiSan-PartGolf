using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Information : System.Web.UI.Page
    {
        private string selectID = Global.uvm.SelectID;
        protected void Page_Load(object sender, EventArgs e)
        {
            //string strJs = @"<script language='JavaScript'>window.alert('" + Global.uvm.SelectID + "');</script>";
            //Response.Write(strJs);
        }
    }
}