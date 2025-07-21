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
        private static String gameCode = null;
        private static string ipaddr = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.uvm.UserClass.Equals(1))
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            ipaddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["gamecode"]))
                {

                    gameCode = Request.QueryString["gamecode"];
                    LoadGame(gameCode);
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string formattedDate = now.ToString("yyyy-MM-dd");
                    TB_GameDate.Text = formattedDate;
                    TB_StartDate.Text = formattedDate;

                    DateTime addDay = now.AddDays(15);
                    string formattedAddDay = addDay.ToString("yyyy-MM-dd");
                    TB_EndDate.Text = formattedAddDay;

                    gameCode = null;
                }
            }
        }

        protected void BTN_Save_Click(object sender, EventArgs e)
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
            strSQL += ", '" + Global.uvm.UserId + "'";
            strSQL += ");";

            SaveLastStep(strSQL);
        }

        protected void BTN_Update_Click(object sender, EventArgs e)
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
            strSQL += ", ModifyUser = '" + Global.uvm.UserId + "'";
            strSQL += " WHERE GameCode = '" + gameCode + "'";
            strSQL += ";";

            SaveLastStep(strSQL);
        }

        protected void BTN_Cancel_Click(object sender, EventArgs e)
        {
            string strSQL = "UPDATE Game_List SET";
            strSQL += " GameStatus = 'Cancel'";
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
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '확인', '저장 되었습니다.', true);", true);
                //Response.Redirect("/Sites/Admin/GameList.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', 'Error', '" + writeResult + "', true);", true);
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