using GiSanParkGolf.Class;
using System;
using System.Diagnostics;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameUserSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.uvm.UserClass.Equals(1))
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            // 검색 모드 결정
            searchProperty.SearchMode =
                    (!string.IsNullOrEmpty(Request.QueryString["SearchField"]) &&
                        !string.IsNullOrEmpty(Request.QueryString["SearchQuery"]));
            if (searchProperty.SearchMode)
            {
                searchProperty.SearchField = Request.QueryString["SearchField"];
                searchProperty.SearchQuery = Request.QueryString["SearchQuery"];
            }

            // 쿼리스트링에 따른 페이지 보여주기
            if (Request["Page"] != null)
            {
                // Page는 보여지는 쪽은 1, 2, 3, ... 코드단에서는 0, 1, 2, ...
                searchProperty.PageIndex = Convert.ToInt32(Request["Page"]) - 1;
            }
            else
            {
                searchProperty.PageIndex = 0; // 1페이지
            }

            // 쿠키를 사용한 리스트 페이지 번호 유지 적용: 
            // 100번째 페이지의 글 보고, 다시 리스트 왔을 때 100번째 페이지 표시
            if (Request.Cookies["GameUserSetting"] != null)
            {
                if (!String.IsNullOrEmpty(
                    Request.Cookies["GameUserSetting"]["PageNum"]))
                {
                    searchProperty.PageIndex = Convert.ToInt32(
                        Request.Cookies["GameUserSetting"]["PageNum"]);
                }
                else
                {
                    searchProperty.PageIndex = 0;
                }
            }

            //레코드 카운트 출력
            if (searchProperty.SearchMode == false)
            {
                // 테이블의 전체 레코드
                searchProperty.RecordCount =
                    Global.dbManager.GetGameCountAll();
            }
            else
            {
                // 해당하는 레코드 수
                searchProperty.RecordCount =
                    Global.dbManager.GetGameCountBySearch(searchProperty.SearchField, searchProperty.SearchQuery);
            }
            lblTotalRecord.Text = searchProperty.RecordCount.ToString();

            if (!Page.IsPostBack)
            {
                Load_GameList();
            }
        }

        protected void Load_GameList()
        {
            if (searchProperty.SearchMode == false) // 기본 리스트
            {
                GameList.DataSource =
                    Global.dbManager.GetGameALL(
                        searchProperty.PageIndex
                        );
            }
            else
            {
                GameList.DataSource =
                    Global.dbManager.GetGameSeachAll(
                        searchProperty.PageIndex,
                        searchProperty.SearchField,
                        searchProperty.SearchQuery
                        );
            }
            GameList.DataBind();
        }

        protected void LnkGame_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            //Debug.WriteLine("선택된 GameCode : " + btn.ToolTip.ToString());
            LoadGame(btn.ToolTip.ToString());
        }

        protected void LoadGame(string gameCode)
        {
            var gameinfo = new DB_Management().GetGameInformation(gameCode);

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