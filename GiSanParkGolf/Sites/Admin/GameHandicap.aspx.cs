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
            if (!IsPostBack)
                LoadHandicapData();  // 최초 로딩 시 호출
        }

        protected void btnRecalculateAll_Click(object sender, EventArgs e)
        {
            try
            {
                var userList = Global.dbManager.GetUserHandicaps();
                int updatedCount = 0;

                foreach (var user in userList)
                {
                    int prev = user.AgeHandicap;
                    string prevSource = user.Source;

                    int newValue = CalculateHandicapByAge(user.Age);
                    string newSource = "자동";

                    // 핸디캡 업데이트
                    Global.dbManager.UpdateHandicap(user.UserId, newValue, newSource, Global.uvm.UserName);
                    updatedCount++;

                    // 변경 로그 기록
                    var log = new HandicapChangeLog
                    {
                        UserId = user.UserId,
                        Age = user.Age,
                        PrevHandicap = prev,
                        NewHandicap = newValue,
                        PrevSource = prevSource,
                        NewSource = newSource,
                        Reason = "전체 자동 재계산",
                        ChangedBy = Global.uvm.UserName
                    };
                    Global.dbManager.InsertHandicapChangeLog(log);
                }

                lblModalMessage.Text = $"✅ 총 {updatedCount}명의 핸디캡이 자동으로 재산정되고 기록되었습니다.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", "showMessageModal();", true);
            }
            catch (Exception ex)
            {
                lblModalMessage.Text = $"⚠️ 자동 계산 중 오류가 발생했습니다: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showMsg", "showMessageModal();", true);
            }

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
                string modifiedBy = Global.uvm.UserName;
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
            var all = Global.dbManager.GetUserHandicaps();

            // 🔍 검색 조건 적용
            string field = ViewState["SearchField"] as string;
            string keyword = ViewState["SearchKeyword"] as string;
            bool readyOnly = ViewState["ReadyOnly"] != null && (bool)ViewState["ReadyOnly"];

            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(field))
            {
                string lower = keyword.ToLower();
                switch (field)
                {
                    case "UserId":
                        all = all.FindAll(u => u.UserId.ToLower().Contains(lower));
                        break;
                    case "UserName":
                        all = all.FindAll(u => u.UserName.ToLower().Contains(lower));
                        break;
                }
            }

            if (readyOnly)
            {
                all = all.FindAll(u => u.Source == "승인대기"); // 필요시 값 수정
            }

            gvHandicaps.DataSource = all;
            gvHandicaps.DataBind();

            // 페이징 연결
            pager.CurrentPage = gvHandicaps.PageIndex;
            pager.TotalPages = gvHandicaps.PageCount;

            // 안내 메시지
            if (all.Count == 0)
            {
                lblModalMessage.Text = "⚠️ 현재 조건에 맞는 핸디캡 데이터가 없습니다.";
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

