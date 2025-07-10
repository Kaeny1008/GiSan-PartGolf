using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Player
{
    public partial class MyGame : System.Web.UI.Page
    {
        private static string gamecode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // 참가취소로 온경우
                if (!string.IsNullOrEmpty(Request.QueryString["GameCancel"]))
                {
                    gamecode = Request.QueryString["GameCode"];
                    GameCancel();
                    return;
                }

                if (!string.IsNullOrEmpty(Request.QueryString["GameCode"]))
                {
                    MainContent.Visible = false;
                    GameContent.Visible = true;
                    gamecode = Request.QueryString["GameCode"];
                    LoadGame();
                }
                else
                {
                    MainContent.Visible = true;
                    GameContent.Visible = false;
                    GameList.DataSource = Global.dbManager.GetMyGameList(Global.uvm.UserID);
                    GameList.DataBind();
                }
            }
        }

        protected void GameCancel()
        {
            string dbWrite = Global.dbManager.MyGameCancel(gamecode, Global.uvm.UserID);
            if (dbWrite.Equals("Success"))
            {
                Response.Redirect(string.Format("~/Sites/Player/MyGame.aspx"));
            }
            else
            {
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += dbWrite;
                strAlarm += "');</script>";
                Response.Write(strAlarm);
            }
        }

        protected void LoadGame()
        {
            var gameinfo = (new DB_Management()).GetGameInformation(gamecode);

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
            TB_User.Text = gameinfo.ParticipantNumber.ToString();
        }

        protected string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
        }
    }
}