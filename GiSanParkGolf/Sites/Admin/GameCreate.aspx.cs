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
            Helper.RequireAdmin(this); // 관리자 확인

            ipaddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!Page.IsPostBack)
            {
                LoadStadiumList();

                if (!string.IsNullOrEmpty(Request.QueryString["gamecode"]))
                {
                    MainTitle.InnerText = "대회 내용 수정";
                    gameCode = Request.QueryString["gamecode"];
                    LoadGame(gameCode);
                }
                else
                {
                    MainTitle.InnerText = "신규 대회 개최";
                    DateTime now = DateTime.Now;

                    // 대회일자 + 시간
                    TB_GameDate.Text = now.ToString("yyyy-MM-dd");
                    TB_GameTime.Text = "10:00";

                    // 모집시작일 + 시간
                    TB_StartDate.Text = now.ToString("yyyy-MM-dd");
                    TB_StartTime.Text = "09:00";

                    // 모집종료일 + 시간
                    DateTime endDate = now.AddDays(15);
                    TB_EndDate.Text = endDate.ToString("yyyy-MM-dd");
                    TB_EndTime.Text = "17:00";

                    gameCode = null;
                }
            }
        }

        private void LoadStadiumList()
        {
            var stadiumList = Global.dbManager.GetStadiums();  // 우리가 만든 경기장 목록 함수

            DDL_Stadium.DataSource = stadiumList;
            DDL_Stadium.DataTextField = "StadiumName";   // 사용자에게 보여질 이름
            DDL_Stadium.DataValueField = "StadiumCode";  // 실제 저장할 코드
            DDL_Stadium.DataBind();

            DDL_Stadium.Items.Insert(0, new ListItem("경기장을 선택하세요", ""));
        }


        protected void BTN_Save_Click(object sender, EventArgs e)
        {
            // 날짜 + 시간 조합
            DateTime gameDateTime = DateTime.Parse($"{TB_GameDate.Text} {TB_GameTime.Text}");
            DateTime recruitStartTime = DateTime.Parse($"{TB_StartDate.Text} {TB_StartTime.Text}");
            DateTime recruitEndTime = DateTime.Parse($"{TB_EndDate.Text} {TB_EndTime.Text}");
            string stadiumCode = DDL_Stadium.SelectedValue;
            string stadiumName = DDL_Stadium.SelectedItem.Text;
            string playMode = rblPlayMode.SelectedValue;

            string strSQL = "INSERT INTO Game_List(";
            strSQL += "GameCode, GameName, GameDate, StadiumCode ,StadiumName, GameHost";
            strSQL += ", StartRecruiting, EndRecruiting, HoleMaximum, GameNote";
            strSQL += ", PostIp, PostUser, PlayMode";
            strSQL += ") VALUES (";
            strSQL += "dbo.fn_GameCode()";
            strSQL += ", '" + TB_GameName.Text + "'";
            strSQL += ", '" + gameDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            strSQL += ", '" + stadiumCode + "'";
            strSQL += ", '" + stadiumName + "'";
            strSQL += ", '" + TB_GameHost.Text + "'";
            strSQL += ", '" + recruitStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            strSQL += ", '" + recruitEndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            strSQL += ", '" + TB_HoleMaximum.Text + "'";
            strSQL += ", '" + TB_Note.Text + "'";
            strSQL += ", '" + ipaddr + "'";
            strSQL += ", '" + Helper.CurrentUser?.UserId + "'";
            strSQL += ", '" + playMode + "'";
            strSQL += ");";

            SaveLastStep(strSQL);
        }

        protected void BTN_Update_Click(object sender, EventArgs e)
        {
            // 날짜 + 시간 조합
            DateTime gameDateTime = DateTime.Parse($"{TB_GameDate.Text} {TB_GameTime.Text}");
            DateTime recruitStartTime = DateTime.Parse($"{TB_StartDate.Text} {TB_StartTime.Text}");
            DateTime recruitEndTime = DateTime.Parse($"{TB_EndDate.Text} {TB_EndTime.Text}");
            string stadiumCode = DDL_Stadium.SelectedValue;
            string stadiumName = DDL_Stadium.SelectedItem.Text;
            string playMode = rblPlayMode.SelectedValue;

            string strSQL = "UPDATE Game_List SET";
            strSQL += " GameName = '" + TB_GameName.Text + "'";
            strSQL += ", GameDate = '" + gameDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            strSQL += ", StadiumCode = '" + stadiumCode + "'";
            strSQL += ", StadiumName = '" + stadiumName + "'";
            strSQL += ", GameHost = '" + TB_GameHost.Text + "'";
            strSQL += ", StartRecruiting = '" + recruitStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            strSQL += ", EndRecruiting = '" + recruitEndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            strSQL += ", HoleMaximum = '" + TB_HoleMaximum.Text + "'";
            strSQL += ", GameNote = '" + TB_Note.Text + "'";
            strSQL += ", ModifyIp = '" + ipaddr + "'";
            strSQL += ", ModifyUser = '" + Helper.CurrentUser?.UserId + "'";
            strSQL += ", PlayMode = '" + playMode + "'";
            strSQL += " WHERE GameCode = '" + gameCode + "';";

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
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '확인', '저장 되었습니다.', 4);", true);
                //Response.Redirect("/Sites/Admin/GameList.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', 'Error', '" + writeResult + "', -1);", true);
            }
        }

        protected void LoadGame(string gamecode)
        {
            var gameinfo = new DB_Management().GetGameInformation(gamecode);

            // 경기장 드롭다운 선택 바인딩
            if (!string.IsNullOrEmpty(gameinfo.StadiumCode))
            {
                DDL_Stadium.SelectedValue = gameinfo.StadiumCode;
            }

            // 대회 정보 바인딩
            TB_GameName.Text = gameinfo.GameName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;

            // 날짜와 시간 분리 바인딩
            TB_GameDate.Text = gameinfo.GameDate.ToString("yyyy-MM-dd");
            TB_GameTime.Text = gameinfo.GameDate.ToString("HH:mm");

            TB_StartDate.Text = gameinfo.StartRecruiting.ToString("yyyy-MM-dd");
            TB_StartTime.Text = gameinfo.StartRecruiting.ToString("HH:mm");

            TB_EndDate.Text = gameinfo.EndRecruiting.ToString("yyyy-MM-dd");
            TB_EndTime.Text = gameinfo.EndRecruiting.ToString("HH:mm");

            // 경기 방식 바인딩 (RadioButtonList 선택 설정)
            if (string.IsNullOrEmpty(gameinfo.PlayMode) || gameinfo.PlayMode == "미입력")
            {
                foreach (ListItem item in rblPlayMode.Items)
                    item.Selected = false;
            }
            else
            {
                var selectedItem = rblPlayMode.Items.FindByValue(gameinfo.PlayMode);
                if (selectedItem != null)
                    selectedItem.Selected = true;
            }
        }

        protected string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
        }
    }
}