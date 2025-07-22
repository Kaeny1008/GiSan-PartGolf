using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Controls
{
    public partial class NewSearchControl : System.Web.UI.UserControl
    {
        public event EventHandler SearchRequested;
        public event EventHandler ResetRequested;

        public string SelectedField => DDL_SearchField.SelectedValue;
        public string Keyword => TB_SearchQuery.Text.Trim();
        public bool ReadyOnly => CB_ReadyUser.Checked;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DDL_SearchField.Items.Clear();

                switch (Request.ServerVariables["SCRIPT_NAME"])
                {
                    case "/Sites/Admin/Player Management":
                        CheckControl.Visible = true;
                        DDL_SearchField.Items.Add(new ListItem("이름", "UserName"));
                        DDL_SearchField.Items.Add(new ListItem("ID", "UserId"));
                        break;

                    case "/Sites/Admin/GameUserSetting":
                        CheckControl.Visible = false;
                        DDL_SearchField.Items.Add(new ListItem("대회명", "GameName"));
                        DDL_SearchField.Items.Add(new ListItem("경기장", "StadiumName"));
                        DDL_SearchField.Items.Add(new ListItem("주최", "HGameHostost"));
                        break;

                    case "/Sites/Admin/GameHandicap":
                        CheckControl.Visible = false;
                        DDL_SearchField.Items.Add(new ListItem("이름", "UserName"));
                        DDL_SearchField.Items.Add(new ListItem("ID", "UserId"));
                        break;

                    case "/Sites/Admin/GameHandicapLog":
                        CheckControl.Visible = false;
                        DDL_SearchField.Items.Add(new ListItem("이름", "UserName"));
                        DDL_SearchField.Items.Add(new ListItem("ID", "UserId"));
                        break;

                    case "/Sites/Admin/StadiumManager":
                        CheckControl.Visible = false;
                        DDL_SearchField.Items.Add(new ListItem("경기장", "StadiumName"));
                        DDL_SearchField.Items.Add(new ListItem("코드", "StadiumCode"));
                        break;

                    default:
                        CheckControl.Visible = false;
                        DDL_SearchField.Items.Add(new ListItem("대회명", "GameName"));
                        DDL_SearchField.Items.Add(new ListItem("경기장", "StadiumName"));
                        DDL_SearchField.Items.Add(new ListItem("주최", "GameHost"));
                        break;
                }
            }
        }

        protected void BTN_Search_Click(object sender, EventArgs e)
        {
            SearchRequested?.Invoke(this, EventArgs.Empty);
        }

        protected void BTN_Reset_Click(object sender, EventArgs e)
        {
            TB_SearchQuery.Text = string.Empty;
            CB_ReadyUser.Checked = false;
            DDL_SearchField.SelectedIndex = 0;

            ResetRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}