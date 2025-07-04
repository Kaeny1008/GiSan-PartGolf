using BBS.Models;
using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameCreate : System.Web.UI.Page
    {
        private String gameCode = null;
        private string ipaddr = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Page.User.Identity.IsAuthenticated)
            //{
            //    if (!Global.uvm.UserClass.Equals(1))
            //    {
            //        Response.Redirect("~/Sites/Login/Admin Alert.aspx");
            //        return;
            //    }
            //}
            //else
            //{
            //    Response.Redirect("~/Sites/Login/Admin Alert.aspx");
            //    return;
            //}

            ipaddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyy-MM-dd");
            TB_GameDate.Text = formattedDate;
            TB_StartDate.Text = formattedDate;

            DateTime addDay = now.AddDays(15);
            string formattedAddDay = addDay.ToString("yyyy-MM-dd");
            TB_EndDate.Text = formattedAddDay;

            if (!string.IsNullOrEmpty(Request.QueryString["gamecode"]))
            {
                gameCode = Request.QueryString["gamecode"];
                LoadGame(gameCode);
                BTN_Save.Text = "수정";
            }
            else
            {
                gameCode = null;
                BTN_Save.Text = "저장";
            }
        }

        protected void BTN_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(gameCode))
            {
                SaveGame();
            }
            else
            {
                UpdateGame();
            }

        }

        protected void SaveGame()
        {
            string strSQL = "INSERT INTO Game_List(";
            strSQL += "[GameCode], [GameName], [GameDate], [StadiumName], [GameHost]";
            strSQL += ", [StartRecruiting], [EndRecruiting], [HoleMaximum], [GameNote]";
            strSQL += ", [PostIp], [PostUser]";
            strSQL += ") VALUES (";
            strSQL += "dbo.fn_GameCode()";
            strSQL += ", '" + TB_GameName.Text + "'";
            strSQL += ", '" + TB_GameDate.Text + "'";
            strSQL += ", '" + TB_StadiumName.Text + "'";
            strSQL += ", '" + TB_GameHost.Text + "'";
            strSQL += ", '" + TB_StartDate.Text + "'";
            strSQL += ", '" + TB_EndDate.Text + "'";
            strSQL += ", '" + TB_HoleMaximum.Text + "'";
            strSQL += ", '" + TB_Note.Text + "'";
            strSQL += ", '" + ipaddr + "'";
            strSQL += ", '" + Global.uvm.UserID + "'";
            strSQL += ");";

            SaveLastStep(strSQL);
        }

        protected void UpdateGame()
        {
            string strSQL = "UPDATE Game_List SET";
            strSQL += " GameName = '" + TB_GameName.Text + "'";
            strSQL += ", GameDate = '" + TB_GameDate.Text + "'";
            strSQL += ", StadiumName = '" + TB_StadiumName.Text + "'";
            strSQL += ", GameHost = '" + TB_GameHost.Text + "'";
            strSQL += ", StartRecruiting = '" + TB_StartDate.Text + "'";
            strSQL += ", EndRecruiting = '" + TB_EndDate.Text + "'";
            strSQL += ", HoleMaximum = '" + TB_HoleMaximum.Text + "'";
            strSQL += ", GameNote = '" + TB_Note.Text + "'";
            strSQL += ", ModifyIp = '" + ipaddr + "'";
            strSQL += ", ModifyUser = '" + Global.uvm.UserID + "'";
            strSQL += " WHERE GameCode = '" + gameCode + "'";
            strSQL += ";";

            SaveLastStep(strSQL);
        }

        protected void SaveLastStep(string strSQL)
        {
            DB_Management dbWrite = new DB_Management();
            string writeResult = dbWrite.DB_Write(strSQL);

            if (writeResult.Equals("Success"))
            {
                Response.Redirect("/Sites/Admin/GameList.aspx");
            }
            else
            {
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += writeResult;
                strAlarm += "');</script>";
                Response.Write(strAlarm);
            }
        }

        protected void LoadGame(string gamecode)
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
        }

        protected string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
        }
    }
}