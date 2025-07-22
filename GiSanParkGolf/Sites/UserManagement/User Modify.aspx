<%@ Page Title="정보수정" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="User Modify.aspx.cs" Inherits="GiSanParkGolf.Sites.UserManagement.User_Modify" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript" type="text/javascript">
        function functionx(evt) {
            if (evt.charCode > 31 && (evt.charCode < 48 || evt.charCode > 57)) {
                alert("숫자만 입력하여 주십시오.");
                return false;
            }
        }
        function MonthValidate(source, arguments) {
            var varage = arguments.Value;
            var varmonth = varage.substring(2, 4);
            console.log("현재 월 : " + varmonth);

            if (parseInt(varmonth) < 1 || parseInt(varmonth) > 12) {
                arguments.IsValid = false;
            } else {
                arguments.IsValid = true;
            }
        }

        function DayValidate(source, arguments) {
            var varage = arguments.Value;
            var vayday = varage.substring(4, 6);
            console.log("현재 일 : " + vayday);

            if (parseInt(vayday) < 1 || parseInt(vayday) > 31) {
                arguments.IsValid = false;
            } else {
                arguments.IsValid = true;
            }
        }    

        function ValidateCheck() {
            var isValid = false;
            isValid = Page_ClientValidate('NewUser');
            console.log("체크완료? : " + isValid);
            //if (isValid) {
            //    isValid = Page_ClientValidate('NewUser');
            //}

            if (isValid) {
                console.log("모달띄운다");
                ShowModal();
            }
        }

        function CloseModal() {
            $("#SaveModal").modal("hide");
        }
        function ShowModal() {
            console.log("모달띄운다2");
            $("#SaveModal").modal("show");
        }
    </script>
    <style>
        .input-group-text{
            width: 150px;
        }
    </style>

    <div class="center_container">
        <div>
            <div>
                <div class="alert alert-success d-flex align-items-center" role="alert">
                    <svg xmlns="http://www.w3.org/2000/svg" 
                        width="50" height="50" 
                        fill="currentColor" 
                        class="bi bi-check-circle-fill flex-shrink-0 me-2" 
                        viewBox="0 0 16 16" 
                        role="img" 
                        aria-label="Success:">
                        <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"/>
                    </svg>
                    <div>
                        <strong>회원가입</strong><br />
                        아래 정보를 입력하여 주십시오.
                    </div>
                </div>
    
                <asp:TextBox ID="TextBox2" runat="server" ValidationGroup="NewUser" Text="Ready" Enabled="false" Width="150px" BorderStyle="None" ForeColor="white" BackColor="white" /><br />
                <asp:Label ID="IDResult" runat="server" Text=""></asp:Label><br />
                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon1">ID</span>
                    <asp:TextBox ID="txtID" runat="server" class="form-control" ValidationGroup="IDCheck" ReadOnly="true"></asp:TextBox>
                    <asp:button class="btn btn-outline-secondary" runat="server" ID="Btn_IDCheck" ValidationGroup="IDCheck" Text="Check" Enabled="false"/>
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon2">암호</span>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" placeholder="수정시에만 입력 하세요." ValidationGroup="NewUser" ReadOnly="true"/>
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon3">암호확인</span>
                    <asp:TextBox ID="txtReCheck" runat="server" TextMode="Password" class="form-control" placeholder="수정시에만 입력 하세요." ValidationGroup="NewUser" ReadOnly="true"/>
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon4">이름</span>
                    <asp:TextBox ID="txtName" runat="server" class="form-control" placeholder="" ValidationGroup="NewUser"/>
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon5">주민번호</span>
                    <asp:TextBox ID="txtBirthDay" runat="server" Width="140px" onkeypress="return functionx(event)" class="form-control" MaxLength="6" ValidationGroup="NewUser"></asp:TextBox>
                    <span class="input-group-text" style="width:15px">-</span>
                    <asp:TextBox ID="txtGender" runat="server" Width="40px" onkeypress="return functionx(event)" class="form-control" MaxLength="1" ValidationGroup="NewUser"></asp:TextBox>
                    <span class="input-group-text" style="width:60px">******</span>
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon6">주소</span>
                    <asp:TextBox ID="txtAddress" runat="server" class="form-control" placeholder="" ValidationGroup="NewUser"/>
                    <asp:button class="btn btn-outline-secondary" runat="server" ID="Button1" Text="검색" OnClientClick="return readyAlarm(event)" />
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon7">상세주소</span>
                    <asp:TextBox ID="txtAddress2" runat="server" class="form-control" placeholder="" ValidationGroup="NewUser"/>
                </div>

                <div class="input-group mb-3">
                    <span class="input-group-text" id="basic-addon8">비고</span>
                    <asp:TextBox ID="txtMemo" runat="server" class="form-control" placeholder="" ValidationGroup="NewUser" TextMode="MultiLine" Height="200px"/>
                </div>

                <asp:Label ID="label6" runat="server" ForeColor="#FF3300"></asp:Label><br />

                <%--<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#SaveModal" Width="100%" Height="40px">
                    수정하기
                </button>--%>
                <asp:Button ID="BTN_Modify" 
                    Font-Bold="true" 
                    runat="server" 
                    Text="수정하기" 
                    class="btn btn-primary" 
                    ValidationGroup="NewUser"
                    width="100%"
                    height="40px"
                    OnClientClick="ValidateCheck();return false;" />
            </div>
        </div>

        <!-- Modal Popup -->
        <div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5" id="exampleModalLabel">저장확인</h1>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        수정내용을 저장 하시겠습니까?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                        <asp:Button ID="BTN_Save" 
                            runat="server" 
                            OnClick="BTN_Register_Click" 
                            class="btn btn-primary" 
                            Text="예" />
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal Popup -->
        

        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
            runat="server"
            ErrorMessage="암호는 필 수 입력 항목입니다." 
            ControlToValidate="txtPassword"
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" 
            runat="server" 
            ControlToValidate="txtPassword" 
            ValidationExpression=".*[!@#$%^&*/].*"
            ErrorMessage="비밀번호는 반드시 !@#$%^&*/. 문자 중 하나를 포함해야 합니다." 
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" 
            runat="server" 
            ControlToValidate="txtPassword" 
            ValidationExpression="[^\s]{4,12}"
            ErrorMessage="비밀번호는 반드시 4~12자리의 공백 없는 문자로 이루어져야 합니다." 
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:CompareValidator ID="CompareValidator1" 
            runat="server" 
            ControlToValidate="txtPassword" 
            ControlToCompare="txtReCheck" 
            ErrorMessage="비밀번호가 일치하지 않습니다." 
            Display="None" ForeColor="red" 
            ValidationGroup="NewUser"/><br />--%>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" 
            runat="server"
            ErrorMessage="이름은 필 수 입력 항목입니다." 
            ControlToValidate="txtName"
            Display="None"
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
            runat="server"
            ErrorMessage="생년월일은 필 수 입력 항목입니다." 
            ControlToValidate="txtBirthDay"
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" 
            runat="server" 
            ControlToValidate="txtBirthDay" 
            ValidationExpression="[0-9]{6}" 
            ErrorMessage="생년월일은 6자리로 입력하여 주십시오." 
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:CustomValidator id="CustomValidator3"
            ControlToValidate="txtBirthDay"
            ClientValidationFunction="MonthValidate"
            Display="None"
            ErrorMessage="주민등록번호 월 입력이 잘못되었습니다."
            ForeColor="red"
            runat="server" 
            ValidationGroup="NewUser"/>
        <asp:CustomValidator id="CustomValidator1"
            ControlToValidate="txtBirthDay"
            ClientValidationFunction="DayValidate"
            Display="None"
            ErrorMessage="주민등록번호 일 입력이 잘못되었습니다."
            ForeColor="red"
            runat="server" 
            ValidationGroup="NewUser"/>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
            runat="server"
            ErrorMessage="성별은 필 수 입력 항목입니다." 
            ControlToValidate="txtGender"
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:ValidationSummary ID="ValidationSummary_SignupForm"
            ShowMessageBox="true" 
            ShowSummary="false"
            HeaderText="다음 사항을 확인하여 주십시오."
            runat="server"
            ValidationGroup="NewUser"/>
    </div>
</asp:Content>
