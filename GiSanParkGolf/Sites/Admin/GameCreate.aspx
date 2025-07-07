<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameCreate.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function ValidateCheck() {
            var isValid = false;
            isValid = Page_ClientValidate('NewGame');

            if (isValid) {
                ShowModal();
            }
        }
        function ShowModal() {
            $("#SaveModal").modal("show");
        }
    </script>
    <style>
        .input-group{
            text-align: center;
            width: 500px; /*이 값에 따라 Textbox가 변한다.*/
        }
        .input-group-text{
            width: 150px;
            text-align: center;
        }
        .redfont{
            color:red;
        }
        .form-control{
            background-color:aliceblue;
        }
        .bc-white{
            background-color:white;
        }
    </style>


    <div class="center_container">
        <div>
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
                <asp:TextBox ID="TB_HoleMaximum" runat="server" class="form-control"></asp:TextBox>
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
            <asp:button ID="BTN_Save" type="button" runat="server" 
                class="btn btn-outline-success btn-lg" 
                style="width:300px; height:50px" 
                Text="저장" 
                ValidationGroup="NewGame"
                OnClientClick="ValidateCheck();return false;" />
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
    <div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
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
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                    <asp:Button ID="Button2" 
                        runat="server" 
                        OnClick="BTN_Save_Click" 
                        class="btn btn-primary" 
                        Text="예" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
