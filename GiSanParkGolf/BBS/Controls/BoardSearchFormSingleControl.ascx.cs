using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.BBS.Controls
{
    public partial class BoardSearchFormSingleControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
            SearchQuery.Attributes["onkeypress"] =
               "if (event.keyCode==13){" +
               Page.GetPostBackEventReference(btnSearch) + "; return false;}";
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string strQuery =
                String.Format(
                    "/BBS/BoardList.aspx?SearchField={0}&SearchQuery={1}&bbsId={2}",
                        SearchField.SelectedItem.Value, SearchQuery.Text, Request.QueryString["bbsId"]);
            Response.Redirect(strQuery);
        }
    }
}