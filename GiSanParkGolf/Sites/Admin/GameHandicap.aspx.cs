using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameHandicap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.RequireAdmin(this); // 관리자 확인

            if (!IsPostBack)
                LoadHandicapData();  // 최초 로딩 시 호출
        }

        protected void btnRecalculateAll_Click(object sender, EventArgs e)
        {
            try
            {
                var userList = Global.dbManager.GetUserHandicaps(null, null);
                int updatedCount = 0;
                string admin = Helper.CurrentUser?.UserName;

                foreach (var user in userList)
                {
                    // 나이 계산 (핸디캡 계산에 필요)
                    int age = Global.dbManager.CalculateAge(user.UserNumber.ToString());
                    int newHandicap = CalculateHandicapByAge(age);

                    // 이전 상태 비교
                    bool isChanged = user.AgeHandicap != newHandicap || user.Source != "자동";
                    if (!isChanged) continue;

                    // DB 업데이트
                    Global.dbManager.UpdateHandicap(user.UserId, newHandicap, "자동", admin);
                    updatedCount++;

                    // 변경 로그 저장
                    var log = new HandicapChangeLog
                    {
                        UserId = user.UserId,
                        Age = age,
                        PrevHandicap = user.AgeHandicap,
                        NewHandicap = newHandicap,
                        PrevSource = user.Source,
                        NewSource = "자동",
                        Reason = "전체 자동 재계산",
                        ChangedBy = admin
                    };
                    Global.dbManager.InsertHandicapChangeLog(log);
                }

                lblModalMessage.Text = $"총 {updatedCount}명의 핸디캡이 자동으로 재산정되었습니다.";
            }
            catch (Exception ex)
            {
                lblModalMessage.Text = $"자동 계산 중 오류가 발생했습니다: {ex.Message}";
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", "showMessageModal();", true);
            LoadHandicapData();
        }

        private int CalculateHandicapByAge(int age)
        {
            if (age < 50) return 0;
            if (age < 60) return 2;
            if (age < 70) return 4;
            if (age < 80) return 6;
            return 8;
        }

        protected void gvHandicaps_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvHandicaps.EditIndex = e.NewEditIndex;
            LoadHandicapData();

            // ✅ 편집 모드 진입 후 드롭다운 값 기반으로 txtHandicap 제어
            ScriptManager.RegisterStartupScript(this, this.GetType(), "triggerToggle", @"
                setTimeout(function() {
                    var row = document.querySelectorAll('#MainContent_gvHandicaps tr')[ " + (e.NewEditIndex + 1) + @"];
                    if (row) {
                        var ddl = row.querySelector('select[id*=""ddlSource""]');
                        if (ddl && typeof toggleHandicap === 'function') {
                            toggleHandicap(ddl);
                        }
                    }
                }, 100);
            ", true);
        }

        protected void gvHandicaps_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvHandicaps.EditIndex = -1;
            LoadHandicapData();  // 다시 바인딩
        }

        protected void gvHandicaps_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                string modifiedBy = Helper.CurrentUser?.UserName;
                GridViewRow row = gvHandicaps.Rows[e.RowIndex];
                string userId = gvHandicaps.DataKeys[e.RowIndex].Value.ToString();

                // 1️⃣ 컨트롤 찾아오기
                TextBox txtHandicap = (TextBox)row.FindControl("txtHandicap");
                DropDownList ddlSource = (DropDownList)row.FindControl("ddlSource");

                string selectedSource = ddlSource?.SelectedValue ?? "";
                int age = int.Parse(row.Cells[3].Text.Trim());             // 나이 셀: Index 3
                int prevHandicap = Convert.ToInt32(gvHandicaps.DataKeys[e.RowIndex].Values["AgeHandicap"]);
                string prevSource = gvHandicaps.DataKeys[e.RowIndex].Values["Source"].ToString();

                // 2️⃣ 새 핸디캡 계산
                int finalHandicap = 0;
                if (selectedSource == "자동")
                {
                    finalHandicap = CalculateHandicapByAge(age);
                }
                else
                {
                    if (!int.TryParse(txtHandicap.Text.Trim(), out finalHandicap))
                        throw new FormatException("핸디캡 입력값이 숫자가 아닙니다.");
                }

                // 3️⃣ 핸디캡 업데이트
                Global.dbManager.UpdateHandicap(userId, finalHandicap, selectedSource, modifiedBy);

                // 4️⃣ 변경 이력 로그 남기기
                var log = new HandicapChangeLog
                {
                    UserId = userId,
                    Age = age,
                    PrevHandicap = prevHandicap,
                    NewHandicap = finalHandicap,
                    PrevSource = prevSource,
                    NewSource = selectedSource,
                    Reason = "수동 편집",
                    ChangedBy = modifiedBy
                };
                Global.dbManager.InsertHandicapChangeLog(log);

                // 5️⃣ 편집 종료 + 메시지 표시
                gvHandicaps.EditIndex = -1;
                LoadHandicapData();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "highlightRow",
                    $"highlightEditedRow({e.RowIndex});", true);

                lblModalMessage.Text = $"✅ 사용자 핸디캡이 성공적으로 저장되고 변경 이력이 기록되었습니다.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessMsg", "showMessageModal();", true);
            }
            catch (Exception ex)
            {
                lblModalMessage.Text = $"⚠️ 저장 중 오류 발생: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorMsg", "showMessageModal();", true);
            }
        }
        private void LoadHandicapData()
        {
            // 검색 조건 준비
            string field = ViewState["SearchField"] as string;
            string keyword = ViewState["SearchKeyword"] as string;

            // SQL 조건 기반 핸디캡 조회
            var result = Global.dbManager.GetUserHandicaps(field, keyword);

            // 바인딩
            gvHandicaps.DataSource = result;
            gvHandicaps.DataBind();

            // 페이징 연결
            pager.CurrentPage = gvHandicaps.PageIndex;
            pager.TotalPages = gvHandicaps.PageCount;

            // 안내 메시지
            if (result.Count == 0)
            {
                lblModalMessage.Text = "현재 조건에 맞는 핸디캡 데이터가 없습니다.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "noData", "showMessageModal();", true);
            }
        }

        protected void gvHandicaps_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHandicaps.PageIndex = e.NewPageIndex;
            LoadHandicapData();
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            ViewState["SearchField"] = search.SelectedField;
            ViewState["SearchKeyword"] = search.Keyword;
            ViewState["ReadyOnly"] = search.ReadyOnly;

            gvHandicaps.PageIndex = 0;
            LoadHandicapData();
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");
            ViewState.Remove("ReadyOnly");

            gvHandicaps.PageIndex = 0;
            LoadHandicapData();
        }

        protected void Pager_PageChanged(object sender, int newPage)
        {
            gvHandicaps.PageIndex = newPage;
            LoadHandicapData();
        }
    }
}

