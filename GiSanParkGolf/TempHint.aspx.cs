using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf
{
    public partial class TempHint : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //팝업창 여는 방법
            //StringBuilder strBuilder = new StringBuilder();

            //strBuilder.Append("<script language='javascript'>");
            //strBuilder.Append("w=810;h=620;");
            //strBuilder.Append("x=Math.floor((screen.availWidth-(w+12))/2);y=Math.floor((screen.availHeight-(h+30))/2);");
            //strBuilder.Append("window.open('Default2.aspx', '', ");
            //strBuilder.Append("'height='+h+',width='+w+',left='+x+',scrollbars=no,resizable=no');");
            //strBuilder.Append("</script>");

            //if (!ClientScript.IsClientScriptBlockRegistered("PopupScript"))
            //{
            //    ClientScript.RegisterClientScriptBlock(this.GetType(), "PopupScript", strBuilder.ToString());
            //}
        }
    }
}