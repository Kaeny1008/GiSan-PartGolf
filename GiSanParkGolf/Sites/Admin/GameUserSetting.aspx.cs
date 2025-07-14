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
            if (gameinfo.GameStatus.Equals("대회종료"))
            {
                //string strAlarm = @"<script language='JavaScript'>window.alert('";
                //strAlarm += "종료된 대회입니다.";
                //strAlarm += "');</script>";
                //Response.Write(strAlarm);

                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '확인', '이미 종료된 대회입니다.', true);", true);

                TB_GameName.Text = string.Empty;
                TB_GameDate.Text = string.Empty;
                TB_StadiumName.Text = string.Empty;
                TB_GameHost.Text = string.Empty;
                TB_StartDate.Text = string.Empty;
                TB_EndDate.Text = string.Empty;
                TB_HoleMaximum.Text = string.Empty;
                TB_Note.Text = string.Empty;
                TB_User.Text = string.Empty;
                TB_GameStatus.Text = string.Empty;
                TB_GameCode.Text = string.Empty;    

                BTN_EarlyClose.Disabled = true;
                BTN_PlayerCheck.Disabled = true;
                BTN_Setting.Disabled = true;
            }
            else
            {
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

                BTN_EarlyClose.Disabled = false;
                BTN_PlayerCheck.Disabled = false;
                BTN_Setting.Disabled = false;

                if (gameinfo.GameStatus.Equals("조기마감"))
                {
                    BTN_EarlyClose.Disabled = true;
                }

                if (gameinfo.GameStatus.Equals("코스배치 완료"))
                {
                    BTN_EarlyClose.Disabled = true;
                    BTN_PlayerCheck.Disabled = true;
                }

                if (gameinfo.GameStatus.Equals("대회종료"))
                {
                    BTN_EarlyClose.Disabled = true;
                    BTN_PlayerCheck.Disabled = true;
                    BTN_Setting.Disabled = true;
                }
            }

        }

        protected void BTN_EarlyCloseYes_Click(object sender, EventArgs e)
        {
            string strSQL = "UPDATE Game_List SET GameStatus = 'EarlyClose'";
            strSQL += " WHERE GameCode = '" + TB_GameCode.Text + "';";

            string result =  Global.dbManager.DB_Write(strSQL);
            if (result.Equals("Success"))
            {
                BTN_EarlyClose.Disabled = true;
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '확인', '저장되었습니다.', true);", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', 'Error', '" + result + "', true);", true);
            }
;       }

        protected void BTN_PlayerCheckYes_Click(object sender, EventArgs e)
        {
            //Response.Write("<script>window.open('GameUserList.aspx'" + 
            //    ", 'popup'" + "" +
            //    ", 'scrollbars, resizable, width=450, height=450, left=0, top=0'" + 
            //    ")</script>");
            //Response.Write("<script>NewWin('GameUserList.aspx')</script>");

            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "NewWin('/Sites/Admin/GameUserList.aspx')", true);
            ClientScript.RegisterStartupScript(this.GetType(), "key", "NewWin('/Sites/Admin/GameUserList.aspx')", false);
        }

        protected void BTN_SettingYes_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("2")
;
        }
    }
}