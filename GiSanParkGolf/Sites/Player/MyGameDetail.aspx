<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyGameDetail.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.MyGameDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mb-3 text-center">
        <h4 class="fw-bold mb-2" id="H1" runat="server">대회 정보</h4>
        <p class="text-muted" style="font-size: 0.95rem;">
            선택한 대회의 상세 정보를 확인 할 수 있습니다.
        </p>
    </div>
    <div class="custom-card" style="max-width:520px; margin:32px auto;">
        <div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회명</span>
                <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회일자</span>
                <asp:TextBox ID="TB_GameDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회장소</span>
                <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회방식</span>
                <asp:TextBox ID="TB_PlayMode" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">주최</span>
                <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">홀당 최대인원</span>
                <asp:TextBox ID="TB_HoleMaximum" runat="server" CssClass="form-control" TextMode="Number" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">모집시작</span>
                <asp:TextBox ID="TB_StartDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">모집종료</span>
                <asp:TextBox ID="TB_EndDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">비고</span>
                <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control" Height="120px" TextMode="MultiLine" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">참가인원</span>
                <asp:TextBox ID="TB_User" runat="server" CssClass="form-control" TextMode="Number" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">취소일자</span>
                <asp:TextBox ID="TB_CancelDate" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">취소사유</span>
                <asp:TextBox ID="TB_CancelReason" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
        </div>
        <div class="text-end mt-3">
            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-danger"
                Text="참가취소" OnClientClick="showCustomModal('cancel'); return false;" OnClick="btnCancel_Click" />
            <asp:Button ID="btnRejoin" runat="server" CssClass="btn btn-danger"
                Text="재참가" OnClientClick="showCustomModal('rejoin'); return false;" OnClick="btnRejoin_Click" />
            <a href="/Sites/Player/MyGame.aspx" class="btn btn-outline-secondary">목록으로</a>
        </div>
    </div>

    <script language="javascript">
        function showCustomModal(actionType) {
            var modal = new bootstrap.Modal(document.getElementById('cancelModal'));
            var okBtn = document.getElementById('modalOkBtn');
            var modalTitle = document.querySelector('#cancelModal .modal-title');
            var modalBody = document.querySelector('#cancelModal .modal-body');

            if (actionType === 'cancel') {
                modalTitle.textContent = "참가취소 확인";
                modalBody.innerHTML = `
                    정말 참가를 취소하시겠습니까?
                    <div class="mt-2">
                      <label for="cancelReasonInput" class="form-label">취소 사유 <span style="color:red">*</span></label>
                      <textarea id="cancelReasonInput" class="form-control" rows="2" maxlength="200" placeholder="사유를 입력하세요."></textarea>
                      <div id="cancelReasonError" class="text-danger mt-1" style="display:none;">취소 사유를 입력하세요.</div>
                    </div>
                `;
                okBtn.onclick = doCancelPostback;
                okBtn.textContent = "예(취소)";
                okBtn.className = "btn btn-danger";
            } else if (actionType === 'rejoin') {
                modalTitle.textContent = "재참가 안내";
                modalBody.innerHTML = `
                    <div>
                        재참가를 진행하시겠습니까?<br>
                        <span class="text-danger">※ 관리자에 의해 강제 취소된 경우에는 재참가가 불가합니다.</span>
                    </div>
                `;
                okBtn.onclick = doRejoinPostback;
                okBtn.textContent = "예(재참가)";
                okBtn.className = "btn btn-success";
            }
            modal.show();
        }

        // 각각의 함수
        function doCancelPostback() {
            var reason = document.getElementById('cancelReasonInput').value || '';
            document.getElementById('<%= hiddenCancelReason.ClientID %>').value = reason;
            __doPostBack('<%= btnCancel.UniqueID %>', '');
        }
        function doRejoinPostback() {
            __doPostBack('<%= btnRejoin.UniqueID %>', '');
        }
    </script>

    <asp:HiddenField ID="hiddenCancelReason" runat="server" />
    <asp:HiddenField ID="hiddenCancelledBy" runat="server" />
    <asp:HiddenField ID="HiddenAssignmentStatus" runat="server" />

    <div class="modal fade" id="cancelModal" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">참가취소 확인</h5>
          </div>
          <div class="modal-body">
            정말 참가를 취소하시겠습니까?
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
            <button type="button" class="btn btn-primary" id="modalOkBtn">예</button>
          </div>
        </div>
      </div>
    </div>
</asp:Content>
