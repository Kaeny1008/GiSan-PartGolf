<%@ Page Title="대회" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameCreate.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function ValidateCheck(modalname, title, body, yes, yesbutton) {
            var isValid = false;
            isValid = Page_ClientValidate('NewGame');

            if (isValid) {
                ShowModal(modalname, title, body, yes, yesbutton);
            }
        }
        var showModalName; //모달이름
        var showTitle; //타이틀
        var showBody; //바디내용
        var onlyYes; // 예 버튼만 표시
        var showYesButton
        function ShowModal(modalname, title, body, yes, yesbutton) {
            showModalName = modalname;
            showTitle = title;
            showBody = body;
            onlyYes = yes;
            showYesButton = yesbutton;
            $(modalname).modal("show");
        }
        $(document).on('show.bs.modal', showModalName, function () {
            var modal = $(this);
            modal.find('.modal-title').text(showTitle);
            modal.find('.modal-body').text(showBody);
            if (onlyYes) {
                modal.find('.modal-footer #MainContent_BTN_Save').hide();
                modal.find('.modal-footer #MainContent_BTN_Update').hide();
                modal.find('.modal-footer #MainContent_BTN_Cancel').hide();
                modal.find('.modal-footer #BTN_No').text("확인");
                return;
            } else {
                modal.find('.modal-footer #BTN_No').text("아니오");
            }
            switch (showYesButton) {
                case 0:
                    modal.find('.modal-footer #MainContent_BTN_Save').show();
                    modal.find('.modal-footer #MainContent_BTN_Update').hide();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').hide();
                    break;
                case 1:
                    modal.find('.modal-footer #MainContent_BTN_Save').hide();
                    modal.find('.modal-footer #MainContent_BTN_Update').show();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').hide();
                    break;
                case 2:
                    modal.find('.modal-footer #MainContent_BTN_Save').hide();
                    modal.find('.modal-footer #MainContent_BTN_Update').hide();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').show();
                    break;
                default:
                    break;
            }
        })
        /*로딩을 완료하기 위해(안그러면 로딩이 안되어 모달 실행이 안된다.*/
        /*아래 두함수가 셋트로 움직여야 한다.*/
        var launch = false;
        function launchModal(modalname, title, body, yes, yesbutton) {
            showModalName = modalname;
            showTitle = title;
            showBody = body;
            onlyYes = yes;
            showYesButton = yesbutton;
            launch = true;
        }
        function pageLoad() {
            if (launch) {
                $(showModalName).modal("show");
            }
        }
    </script>
    <style>
        .input-group{
            text-align: center;
            width:100%;
        }
        .input-group-text{
            min-width: 30%;
            max-width: 30%;
            text-align: center;
        }
        .redfont{
            color:red;
        }
        .form-control{
            min-width: 70%;
            max-width: 70%;
        }
        .bc-white{
            background-color:white;
        }
    </style>


    <div class="center_container">
        <div style="width:40%">
            <div class="input-group mb-3">
                <span class="input-group-text redfont">대회명</span>
                <asp:TextBox ID="TB_GameName" runat="server" class="form-control"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text redfont">대회일자</span>
                <asp:TextBox ID="TB_GameDate" runat="server" class="form-control" TextMode="date"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text redfont">대회장소</span>
                <asp:TextBox ID="TB_StadiumName" runat="server" class="form-control"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text redfont">주최</span>
                <asp:TextBox ID="TB_GameHost" runat="server" class="form-control"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text redfont">홀당 최대인원</span>
                <asp:TextBox ID="TB_HoleMaximum" runat="server" class="form-control" TextMode="Number" Text="4"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text redfont">모집시작</span>
                <asp:TextBox ID="TB_StartDate" runat="server" class="form-control" TextMode="date"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text redfont">모집종료</span>
                <asp:TextBox ID="TB_EndDate" runat="server" class="form-control" TextMode="date"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">비고</span>
                <asp:TextBox ID="TB_Note" runat="server" class="form-control bc-white" Height="300px" TextMode="MultiLine"></asp:TextBox>
            </div>
            <br />
            <% if (string.IsNullOrEmpty(Request.QueryString["gamecode"]))
                {
            %>
                    <asp:button ID="BTN_ModalShow" type="button" runat="server" 
                        class="btn btn-outline-success btn-lg" 
                        style="width:150px; height:50px" 
                        Text="저장" 
                        ValidationGroup="NewGame"
                        OnClientClick="ValidateCheck('#MainModal', '저장', '저장 하시겠습니까?', false, 0);return false;" />
            <%
                }
                else
                {
            %>
                    <asp:button ID="Button1" type="button" runat="server" 
                        class="btn btn-outline-success btn-lg" 
                        style="width:150px; height:50px" 
                        Text="저장" 
                        ValidationGroup="NewGame"
                        OnClientClick="ValidateCheck('#MainModal', '수정', '저장 하시겠습니까?', false, 1);return false;" />
                    <asp:button ID="BTN_ModalShow2" type="button" runat="server" 
                        class="btn btn-outline-success btn-lg" 
                        style="width:150px; height:50px" 
                        Text="대회취소"
                        OnClientClick="ValidateCheck('#MainModal', '취소', '대회를 취소 하시겠습니까?', false, 2);return false;"  />
            <%
                }
            %>
        </div>
    </div>



    <!-- 유효성검사 -->
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
        runat="server"
        ErrorMessage="개최명은 필 수 입력 항목입니다." 
        ControlToValidate="TB_GameName"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame"/><br />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
        runat="server"
        ErrorMessage="대회장소는 필 수 입력 항목입니다." 
        ControlToValidate="TB_StadiumName"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame"/><br />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
        runat="server"
        ErrorMessage="주최는 필 수 입력 항목입니다." 
        ControlToValidate="TB_GameHost"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame"/><br />
    <asp:ValidationSummary ID="ValidationSummary_SignupForm"
        ShowMessageBox="true" 
        ShowSummary="false"
        HeaderText="다음 사항을 확인하여 주십시오."
        runat="server" ValidationGroup="NewGame" />

    <!-- Modal -->
    <div class="modal fade" id="MainModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    저장 하시겠습니까?
                </div>
                <div class="modal-footer">
                    <button id="BTN_No" type="button" class="btn btn-secondary" data-bs-dismiss="modal" onclick="location.href='/Sites/Admin/GameList.aspx'">아니오</button>
                    <asp:Button ID="BTN_Save" runat="server" OnClick="BTN_Save_Click" class="btn btn-primary" Text="예" />
                    <asp:Button ID="BTN_Update" runat="server" OnClick="BTN_Update_Click" class="btn btn-primary" Text="예" />
                    <asp:Button ID="BTN_Cancel" runat="server" OnClick="BTN_Cancel_Click" class="btn btn-primary" Text="예" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
