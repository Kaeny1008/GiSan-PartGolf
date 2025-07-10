using BBS.Models;
using Dul;
using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace GiSanParkGolf.Sites.Player
{
    public partial class JoinGame : System.Web.UI.Page
    {
        private static string gameCode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["GameCode"]))
                {
                    MainContent.Visible = false;
                    GameContent.Visible = true;
                    gameCode = Request.QueryString["GameCode"];
                    EarlyJoin();
                }
                else
                {
                    MainContent.Visible = true;
                    GameContent.Visible = false;
                    GameList.DataSource = Global.dbManager.GetGameReadyList();
                    GameList.DataBind();
                }
            }
        }

        protected void LoadGame()
        {
            var gameinfo = (new DB_Management()).GetGameInformation(gameCode);

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
        }

        protected void EarlyJoin()
        {
            // 기존 참가 신청했는지 확인
            string earlyJoinCheck = Global.dbManager.GetEarlyJoin(gameCode, Global.uvm.UserID);
            if (!string.IsNullOrEmpty(earlyJoinCheck))
            {
                if (earlyJoinCheck.Equals("Join"))
                {
                    string strJs = "<script>alert('이미 대회참가 신청을 하셨습니다.'); location.href='javascript:history.go(-1)';</script>";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
                    return;
                }
            }

            LoadGame();
        }

        protected string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
        }

        protected void BTN_Save_Click(object sender, EventArgs e)
        {
            string ipaddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipaddr))
            {
                ipaddr = Request.ServerVariables["REMOTE_ADDR"];
            }

            GameJoinUserModel gjum = new GameJoinUserModel
            {
                UserId = Global.uvm.UserID
                , JoinIP = ipaddr
                , GameCode = gameCode
            };
           
            string dbWrite = Global.dbManager.GameJoin(gjum);
            if (dbWrite.Equals("Success"))
            {
                Response.Redirect(string.Format("~/Sites/Player/JoinGame.aspx"));
            }
            else
            {
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += dbWrite;
                strAlarm += "');</script>";
                Response.Write(strAlarm);
            }
        }
    }
}