using Antlr.Runtime;
using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameUserSetting : System.Web.UI.Page
    {
        private string SearchField
        {
            get => ViewState["SearchField"]?.ToString();
            set => ViewState["SearchField"] = value;
        }

        private string SearchKeyword
        {
            get => ViewState["SearchKeyword"]?.ToString();
            set => ViewState["SearchKeyword"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.uvm.UserClass.Equals(1))
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            if (!IsPostBack)
            {
                SearchField = null;
                SearchKeyword = null;
                GameList.PageIndex = 0;
                LoadGameList();
            }
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            SearchField = search.SelectedField;
            SearchKeyword = search.Keyword;

            GameList.PageIndex = 0;
            LoadGameList();
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");

            GameList.PageIndex = 0;
            LoadGameList();
        }

        protected void Pager_PageChanged(object sender, int newPage)
        {
            GameList.PageIndex = newPage;
            LoadGameList();
        }

        private void LoadGameList()
        {
            // 검색 조건 준비
            string field = SearchField;
            string keyword = SearchKeyword;

            // SQL에서 조건 필터링 → 결과만 받아옴
            List<GameListModel> result = Global.dbManager.GetGames(field, keyword);

            // 바인딩
            GameList.DataSource = result;
            GameList.DataBind();

            lblTotalRecord.Text = result.Count.ToString();
            pager.CurrentPage = GameList.PageIndex;
            pager.TotalPages = GameList.PageCount;
        }

        protected void GameList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int no = (GameList.PageIndex * GameList.PageSize) + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = no.ToString(); // No 컬럼 처리
            }
        }

        protected void LnkGame_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            LoadGame(btn.ToolTip.ToString());
        }

        protected void LoadGame(string gameCode)
        {
            var gameinfo = Global.dbManager.GetGameInformation(gameCode);

            if (gameinfo == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '오류', '대회 정보를 찾을 수 없습니다.', true);", true);
                return;
            }

            bool isClosed = gameinfo.GameStatus == "대회종료" || gameinfo.GameStatus == "취소";
            if (isClosed)
            {
                string msg = gameinfo.GameStatus == "대회종료" ? "종료된 대회입니다." : "취소된 대회입니다.";
                ClientScript.RegisterStartupScript(this.GetType(), "key", $"launchModal('#MainModal', '확인', '{msg}', true);", true);

                TB_GameName.Text = TB_GameDate.Text = TB_StadiumName.Text = TB_GameHost.Text =
                TB_StartDate.Text = TB_EndDate.Text = TB_HoleMaximum.Text = TB_Note.Text =
                TB_User.Text = TB_GameStatus.Text = TB_GameCode.Text = string.Empty;

                BTN_EarlyClose.Disabled = BTN_PlayerCheck.Disabled = BTN_Setting.Disabled = true;
                return;
            }

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = Helper.ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = Helper.ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = Helper.ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
            TB_User.Text = gameinfo.ParticipantNumber.ToString();
            TB_GameStatus.Text = gameinfo.GameStatus;
            TB_GameCode.Text = gameinfo.GameCode;

            BTN_EarlyClose.Disabled = BTN_PlayerCheck.Disabled = BTN_Setting.Disabled = false;

            switch (gameinfo.GameStatus)
            {
                case "조기마감":
                    BTN_EarlyClose.Disabled = true;
                    break;
                case "코스배치 완료":
                    BTN_EarlyClose.Disabled = true;
                    BTN_PlayerCheck.Disabled = true;
                    break;
                case "대회종료":
                    BTN_EarlyClose.Disabled = true;
                    BTN_PlayerCheck.Disabled = true;
                    BTN_Setting.Disabled = true;
                    break;
            }
        }

        protected void DbWrite(string strSQL)
        {
            try
            {
                Global.dbManager.DB_Write(strSQL);
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '확인', '저장되었습니다.', true);", true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DB Write Error: " + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '오류', '데이터베이스에 오류가 발생했습니다. 관리자에게 문의하세요.', true);", true);
            }
        }

        protected void BTN_EarlyCloseYes_Click(object sender, EventArgs e)
        {
            string strSQL = "UPDATE Game_List SET GameStatus = 'EarlyClose'";
            strSQL += " WHERE GameCode = '" + TB_GameCode.Text + "';";

            DbWrite(strSQL);
        }


        protected void BTN_SettingYes_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("2")
;
        }
    }
}