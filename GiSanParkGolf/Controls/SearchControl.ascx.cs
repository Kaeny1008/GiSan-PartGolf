using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Controls
{
    public partial class SearchControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine(Request.ServerVariables["SCRIPT_NAME"]);
            // 검색 Textbox에서 엔터키를 누를때 검색버튼이 누르는 효과
            TB_SearchQuery.Attributes["onkeypress"] =
               "if (event.keyCode==13){" +
               Page.GetPostBackEventReference(BTN_Search) + "; return false;}";

            //어떤 메뉴로 왔는지 따라 검색필드를 변경
            if (Request.ServerVariables["SCRIPT_NAME"].Contains("Player Management"))
            {
                CB_ReadyUser.Visible = true;
                DDL_SearchField.Items.Add(new ListItem("이름", "Name"));
                DDL_SearchField.Items.Add(new ListItem("ID", "Id"));
            } else
            {
                CB_ReadyUser.Visible = false;
                DDL_SearchField.Items.Add(new ListItem("대회명", "Name"));
                DDL_SearchField.Items.Add(new ListItem("개최지", "Stadium"));
            }

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["SearchField"]) &&
                    !string.IsNullOrEmpty(Request.QueryString["SearchQuery"]))
                {
                    DDL_SearchField.SelectedValue = Request.QueryString["SearchField"];
                    TB_SearchQuery.Text = Request.QueryString["SearchQuery"];
                }

                if (!string.IsNullOrEmpty(Request.QueryString["ReadyUser"]))
                {
                    if (Request.QueryString["ReadyUser"].Equals("True"))
                    {
                        CB_ReadyUser.Checked = true;
                    }
                    else
                    {
                        CB_ReadyUser.Checked = false;
                    }
                }
            }
        }

        protected void BTN_Search_Click(object sender, EventArgs e)
        {
            Response.Redirect(LinkMake());
        }

        protected string LinkMake()
        {
            string returnLink = String.Format("{0}", Request.ServerVariables["SCRIPT_NAME"]); //주소입력
            string returnLink2 = string.Empty; //옵션입력

            if (!TB_SearchQuery.Text.Equals(string.Empty))
            {
                returnLink2 = 
                    String.Format("?SearchField={0}&SearchQuery={1}",
                    DDL_SearchField.SelectedItem.Value, TB_SearchQuery.Text);
            }

            if (CB_ReadyUser.Checked)
            {
                if (returnLink2.Equals(string.Empty))
                {
                    returnLink2 += String.Format("?ReadyUser={0}", "True");
                }
                else
                {
                    returnLink2 += String.Format("&ReadyUser={0}", "True");
                }
            }

            returnLink += returnLink2;
            Debug.WriteLine("검색 URL : " + returnLink);
            return returnLink;
        }
    }
}