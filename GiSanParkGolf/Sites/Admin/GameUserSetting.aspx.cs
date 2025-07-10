using GiSanParkGolf.Class;
using System;
using System.Diagnostics;
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
            Debug.WriteLine("선택된 GameCode : " + btn.ToolTip.ToString());
            LoadGame(btn.ToolTip.ToString());
        }

        protected void LoadGame(string gameCode)
        {
            var gameinfo = (new DB_Management()).GetGameInformation(gameCode);
            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = Helper.ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = Helper.ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = Helper.ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
            TB_User.Text = gameinfo.ParticipantNumber.ToString();

            BTN_1.Disabled = false;
            BTN_2.Disabled = false;
            BTN_3.Disabled = false;
        }
    }
}