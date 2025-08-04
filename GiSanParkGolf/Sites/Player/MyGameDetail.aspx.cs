using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Player
{
    public partial class MyGameDetail : System.Web.UI.Page
    {
        private string GameCode
        {
            get => Request.QueryString["gamecode"];
            set => Request.QueryString["gamecode"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadGame();
            }
        }

        protected void LoadGame()
        {
            var gameinfo = (new DB_Management()).GetMyGameInformation(GameCode, Helper.CurrentUser?.UserId);

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = Helper.ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = Helper.ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = Helper.ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
            TB_User.Text = gameinfo.ParticipantNumber.ToString();
            TB_PlayMode.Text = gameinfo.PlayModeToText;

            TB_CancelDate.Text = gameinfo.CancelDate.HasValue ? gameinfo.CancelDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
            TB_CancelReason.Text = gameinfo.CancelReason ?? "";
            hiddenCancelledBy.Value = gameinfo.CancelledBy ?? "";
            HiddenAssignmentStatus.Value = gameinfo.AssignmentStatus;

            // 버튼 기본값
            btnCancel.Visible = false;
            btnRejoin.Visible = false;

            if (gameinfo.IsCancelled == 0) // 참가중
            {
                btnCancel.Visible = true;
            }
            else if (gameinfo.IsCancelled == 1) // 참가취소 상태
            {
                btnRejoin.Visible = true;
            }
            // else: 참가 기록 없음(null) → 신청 버튼 등 필요시 추가
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string cancelReason = hiddenCancelReason.Value;

            if (string.IsNullOrEmpty(cancelReason))
            {
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    "showResultModal('취소 사유를 반드시 입력하세요.', '참가취소');", true
                );
                return;
            }

            string dbWrite = Global.dbManager.MyGameCancel(GameCode, Helper.CurrentUser?.UserId, cancelReason);

            if (dbWrite == "Success")
            {
                string message = "참가가 성공적으로 취소되었습니다.";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    $"showResultModal('{message}', '참가취소');", true
                );
                LoadGame();
            }
            else
            {
                string message = "참가취소 중 오류가 발생했습니다.";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    $"showResultModal('{message}', '참가취소');", true
                );
            }
        }

        protected void btnRejoin_Click(object sender, EventArgs e)
        {
            if (hiddenCancelledBy.Value == "Admin")
            {
                string message = "이 게임은 관리자에 의해 취소되어 재참가가 불가합니다.";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    $"showResultModal('{message}', '재참가');", true
                );
                return; // 관리자에 의해 취소된 게임은 재참가 불가
            }

            if (HiddenAssignmentStatus.Value == "Cancelled")
            {
                string message = "관리자가 취소처리하여 재참가가 불가능합니다.";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    $"showResultModal('{message}', '재참가');", true
                );
                return; // 사용자요청 -> 관라자에 의해 취소 승인된 게임은 재참가 불가
            }

            string dbWrite = Global.dbManager.MyGameRejoin(GameCode, Helper.CurrentUser?.UserId);

            if (dbWrite == "Success")
            {
                string message = "재참가가 성공적으로 저장되었습니다.";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    $"showResultModal('{message}', '재참가');", true
                );
                LoadGame();
            }
            else
            {
                string message = "재참가 중 오류가 발생했습니다.";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(), "ShowResultModal",
                    $"showResultModal('{message}', '재참가');", true
                );
            }
        }
    }
}