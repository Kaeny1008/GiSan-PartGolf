<%@ Page Title="Handicap 설정" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameHandicap.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameHandicap"  %>

<%@ Register TagPrefix="uc" TagName="NewSearchControl" Src="~/Controls/NewSearchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="NewPagingControl" Src="~/Controls/NewPagingControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">   
    <script>
        function showMessageModal() {
            var myModal = new bootstrap.Modal(document.getElementById('msgModal'));
            myModal.show();
        }
        function toggleHandicap(dropdown) {
            const row = dropdown.closest('tr');
            const txt = row.querySelector('input[id*="txtHandicap"]');
            if (!txt) return;

            if (dropdown.value === "자동") {
                txt.setAttribute("disabled", "true");
                txt.value = "자동 계산됨";
                txt.setAttribute("title", "자동 산정 모드입니다");
                txt.classList.add("bg-light");
            } else {
                txt.removeAttribute("disabled");

                // ✨ title 유지 → 포커스 이벤트에서 제거
                txt.classList.remove("bg-light");
            }
        }
        function markHandicapAsManual(txtbox) {
            const row = txtbox.closest('tr');
            const ddl = row.querySelector('select[id*="ddlSource"]');
            if (ddl) {
                ddl.value = "수동";
                toggleHandicap(ddl);
            }
        }
        function hideAllOption(dropdown) {
            for (let i = 0; i < dropdown.options.length; i++) {
                if (dropdown.options[i].value === "") {
                    dropdown.options[i].style.display = "none";
                    break;
                }
            }
        }
        function highlightEditedRow(rowIndex) {
            const row = document.querySelectorAll('#<%= gvHandicaps.ClientID %> tr')[rowIndex + 1];  // +1 for header
            if (row) {
                row.querySelectorAll('td').forEach(td => td.classList.add('changed-cell'));
            }
        }
        function showConfirmRecalcModal() {
            const modal = new bootstrap.Modal(document.getElementById('confirmRecalcModal'));
            modal.show();
        }
        function clearHandicapTooltip(txtbox) {
            txtbox.value = "";
        }
    </script>

    <style>
        .grid-center th,
        .grid-center td {
            text-align: center;
            vertical-align: middle;
        }

        .gridview-pager tfoot td {
            text-align: center !important;
        }

        .gridview-pager td table {
            margin: 0 auto !important;
        }
        .changed-cell {
            background-color: #f9ecec !important;
            transition: background-color 0.5s ease-in-out;
        }
    </style>

    <div class="container mt-4">

        <h2 class="mb-4">🏌️‍♂️ 파크골프 핸디캡 관리</h2>

        <%-- 필터 영역 --%>
        <div class="row mb-4">
             <%--드롭다운 & 검색창--%> 
            <div class="col-md-10 d-flex flex-wrap gap-2">
                <uc:NewSearchControl ID="search" runat="server"
                    OnSearchRequested="Search_SearchRequested"
                    OnResetRequested="Search_ResetRequested" />
            </div>

             <%--오른쪽 버튼--%> 
            <div class="col-md-2 text-end">
                <a href="GameHandicapLog.aspx" class="btn btn-outline-dark">
                    <i class="bi bi-bar-chart-line"></i> 핸디캡 기록 보기
                </a>
            </div>
        </div>

         <%--일괄 자동 계산 버튼--%> 
        <asp:Button ID="btnRecalculateAllTrigger" runat="server"
            Text="전체 자동 계산"
            CssClass="btn btn-outline-danger mb-3"
            OnClientClick="showConfirmRecalcModal(); return false;" />

        <%-- 핸디캡 출력 테이블 --%>
        <asp:GridView ID="gvHandicaps" runat="server"
            AutoGenerateColumns="False"
            AllowPaging="true"
            PageSize="10"
            PagerSettings-Visible="false"
            CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
            ShowHeaderWhenEmpty="true"
            DataKeyNames="UserId,AgeHandicap,Source"
            OnRowEditing="gvHandicaps_RowEditing"
            OnRowUpdating="gvHandicaps_RowUpdating"
            OnRowCancelingEdit="gvHandicaps_RowCancelingEdit">

          <Columns>
            <asp:BoundField DataField="UserId"     HeaderText="ID"       ReadOnly="True" />
            <asp:BoundField DataField="UserName"   HeaderText="이름"     ReadOnly="True" />
            <asp:BoundField DataField="UserNumber" HeaderText="생년월일" ReadOnly="True" />
            <asp:BoundField DataField="Age"        HeaderText="나이"     ReadOnly="True" />

            <asp:TemplateField HeaderText="핸디캡">
              <ItemTemplate>
                <%# Eval("AgeHandicap") %>
                <asp:HiddenField ID="hdnPrevHandicap" Value='<%# Eval("AgeHandicap") %>' runat="server" />
              </ItemTemplate>
              <EditItemTemplate>
                <asp:TextBox ID="txtHandicap" runat="server"
                             CssClass="form-control"
                             Text='<%# Bind("AgeHandicap") %>'
                             oninput="markHandicapAsManual(this)"
                             onfocus="clearHandicapTooltip(this)"/>
              </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="산정 방식">
              <ItemTemplate>
                <%# Eval("Source") %>
                <asp:HiddenField ID="hdnPrevSource" Value='<%# Eval("Source") %>' runat="server" />
              </ItemTemplate>
              <EditItemTemplate>
                <asp:DropDownList ID="ddlSource" runat="server"
                    CssClass="form-select"
                    SelectedValue='<%# Bind("Source") %>'
                    onchange="toggleHandicap(this)"
                    onfocus="hideAllOption(this); toggleHandicap(this);">
                    <asp:ListItem Text="자동" Value="자동" />
                    <asp:ListItem Text="수동" Value="수동" />
                </asp:DropDownList>
              </EditItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="LastUpdated" HeaderText="최종 수정일"
                            DataFormatString="{0:yyyy-MM-dd}" ReadOnly="True" />
            <asp:BoundField DataField="LastUpdatedBy" HeaderText="수정자" ReadOnly="True">
              <HeaderStyle HorizontalAlign="Center" />
              <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>

            <asp:TemplateField>
              <ItemTemplate>
                <asp:Button ID="btnEdit" runat="server" Text="편집"
                    CommandName="Edit"
                    CssClass="btn btn-sm btn-outline-primary" />
              </ItemTemplate>
              <EditItemTemplate>
                <asp:Button ID="btnUpdate" runat="server" Text="저장"
                    CommandName="Update"
                    CssClass="btn btn-sm btn-success me-2" />

                <asp:Button ID="btnCancel" runat="server" Text="취소"
                    CommandName="Cancel"
                    CssClass="btn btn-sm btn-secondary" />
              </EditItemTemplate>
            </asp:TemplateField>
          </Columns>

          <EmptyDataTemplate>
            <div class="text-center text-muted p-4 fs-5">
              ⚠️ 현재 등록된 핸디캡 데이터가 없습니다.
            </div>
          </EmptyDataTemplate>
        </asp:GridView>

        <uc:NewPagingControl ID="pager" runat="server"
            OnPageChanged="Pager_PageChanged" />
    </div>

    <%-- ✅ 메시지 모달 --%>
    <div class="modal fade" id="msgModal" tabindex="-1" aria-labelledby="msgModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title fs-5" id="msgModalLabel">알림</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="닫기"></button>
          </div>
          <div class="modal-body">
            <asp:Label ID="lblModalMessage" runat="server"/>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">확인</button>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="confirmRecalcModal" tabindex="-1" aria-labelledby="confirmRecalcModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">

          <div class="modal-header bg-warning text-dark">
            <h5 class="modal-title fs-5" id="confirmRecalcModalLabel">전체 자동 계산 확인</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="닫기"></button>
          </div>

          <div class="modal-body text-center">
            <p class="mb-2">⚠️ 모든 사용자 핸디캡을 <strong>자동 계산</strong>하시겠습니까?</p>
            <p class="text-muted">이 작업은 되돌릴 수 없습니다.</p>
          </div>

          <div class="modal-footer justify-content-center">
            <button type="button" class="btn btn-secondary me-2" data-bs-dismiss="modal">취소</button>
            <asp:Button ID="btnConfirmRecalc" runat="server"
                Text="예, 계산 진행"
                CssClass="btn btn-danger"
                OnClick="btnRecalculateAll_Click" />
          </div>

        </div>
      </div>
    </div>
</asp:Content>