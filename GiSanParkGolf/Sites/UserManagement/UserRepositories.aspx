<%@ Page Title="회원가입" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserRepositories.aspx.cs" Inherits="GiSanParkGolf.Sites.UserManagement.UserRepositories" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        //function GenderValidate(source, arguments) {
        //    if (arguments.Value == "선택") {
        //        arguments.IsValid = false;
        //    } else {
        //        arguments.IsValid = true;
        //    }
        //}
        function reCheckID(evt) {
            <%--document.getElementById('<%= IDResult.ClientID %>').innerText = '';--%>
            document.getElementById('<%= TextBox2.ClientID %>').value = 'Ready'; // Change the textbox value here 
        }

        function readyAlarm(evt) {
            alert("기능 준비중입니다.");
            return false;
        }

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

        //function ValidateCheck() {
        //    var isValid = false;
        //    isValid = Page_ClientValidate('NewUser');
        //    //if (isValid) {
        //    //    isValid = Page_ClientValidate('NewUser');
        //    //}

        //    if (isValid) {
        //        ShowModal();
        //    }
        //}

        function ValidateCheck() {
            var isValid = Page_ClientValidate('NewUser');

            if (isValid) {
                // 유효성 통과 → 가입 확인 모달 또는 가입 처리
                ShowModal();
            } else {
                // 유효성 실패 → 안내 모달 띄움
                var modal = new bootstrap.Modal(document.getElementById('ValidationModal'));
                modal.show();
            }
        }

        function CloseModal() {
            $("#SaveModal").modal("hide");
        }
        function ShowModal() {
            $("#SaveModal").modal("show");
        }
    </script>
    <style>
        .input-group-text{
            width: 150px;
        }
    </style>

    <!-- 상단 카드: 페이지 설명 영역 -->
    <div class="mb-3 text-center">
        <h4 class="fw-bold mb-2" id="MainTitle" runat="server">회원가입</h4>
        <p class="text-muted" style="font-size: 0.95rem;">
            아래 정보를 입력하여 주십시오.
        </p>
    </div>

    <div class="center_container mt-4">
        <div class="custom-card">
            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon1">ID</span>
                <asp:TextBox ID="txtID" runat="server" class="form-control" ValidationGroup="IDCheck" ReadOnly="false" onkeypress="reCheckID()"></asp:TextBox>
                <asp:button class="btn btn-outline-secondary" runat="server" ID="Btn_IDCheck" OnClick="Btn_IDCheck_Click" ValidationGroup="IDCheck" Text="Check" Enabled="true" AutoPostBack="false"/>
            </div>

            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon2">암호</span>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" placeholder="" ValidationGroup="NewUser"/>
            </div>

            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon3">암호확인</span>
                <asp:TextBox ID="txtReCheck" runat="server" TextMode="Password" class="form-control" placeholder="" ValidationGroup="NewUser"/>
            </div>

            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon4">이름</span>
                <asp:TextBox ID="txtName" runat="server" class="form-control" placeholder="" ValidationGroup="NewUser"/>
            </div>

            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon5">주민번호</span>
                <asp:TextBox ID="txtBirthDay" runat="server" Width="150px" onkeypress="return functionx(event)" class="form-control" MaxLength="6" ValidationGroup="NewUser"></asp:TextBox>
                <span class="input-group-text" style="width:15px">-</span>
                <asp:TextBox ID="txtGender" runat="server" Width="30px" onkeypress="return functionx(event)" class="form-control" MaxLength="1" ValidationGroup="NewUser"></asp:TextBox>
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
        </div>
    </div>

    <div class="text-center mt-4">
        <asp:Button ID="BTN_Modify" 
            Font-Bold="true" 
            runat="server" 
            Text="가입하기" 
            class="btn btn-primary" 
            ValidationGroup="NewUser"
            height="40px"
            OnClientClick="ValidateCheck();return false;" />
    </div>

    <asp:TextBox ID="TextBox2" runat="server" ValidationGroup="NewUser" Text="Ready" CssClass="visually-hidden" />

    <!-- Modal -->
    <div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered"> <%--modal-dialog-centered 를 옆에 넣으면 화면 중앙에 나타난다.--%>
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">저장확인</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    회원가입 하시겠습니까?<br />
                    관리자 승인 완료 후 이용 가능합니다.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                    <asp:Button ID="Button2" 
                        runat="server" 
                        OnClick="BTN_Register_Click" 
                        class="btn btn-primary" 
                        Text="예" />
                </div>
            </div>
        </div>
    </div>
    <!-- 유효성 검사 결과를 표시할 모달 -->
    <div class="modal fade" id="ValidationModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">

          <div class="modal-header">
            <h5 class="modal-title text-danger fw-bold">⚠ 입력 확인 필요</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="닫기"></button>
          </div>

          <div class="modal-body">
            <asp:ValidationSummary ID="ValidationSummary_SignupForm"
                runat="server"
                ValidationGroup="NewUser"
                ShowMessageBox="false"
                ShowSummary="true"
                HeaderText="<span class='fw-semibold'>다음 사항을 확인해주세요:</span>"
                CssClass="text-danger small" />
            
            <asp:ValidationSummary ID="ValidationSummary_IDCheck"
                runat="server"
                ValidationGroup="IDCheck"
                ShowMessageBox="false"
                ShowSummary="true"
                HeaderText="<span class='fw-semibold'>아이디 유효성 검사 확인:</span>"
                CssClass="text-danger small mt-2" />
          </div>

          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">확인</button>
          </div>

        </div>
      </div>
    </div>
    <!-- 아이디 중복 검사 결과 모달 -->
    <div class="modal fade" id="IDCheckResultModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">

          <div class="modal-header">
            <h5 class="modal-title fw-bold">아이디 검사 결과</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="닫기"></button>
          </div>

          <div class="modal-body">
            <asp:Label ID="Label_IDCheckResult" runat="server" CssClass="text-danger" />
          </div>

          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">확인</button>
          </div>

        </div>
      </div>
    </div>


    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" 
        runat="server"
        ErrorMessage="아이디는 필 수 입력 항목입니다." 
        ControlToValidate="txtID"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="IDCheck"/>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
        runat="server" 
        ControlToValidate="txtID" 
        ValidationExpression="[0-9a-zA-Z]{4,15}" 
        ErrorMessage="4~15자리의 문자, 숫자로 이루어져야 합니다." 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="IDCheck"/>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" 
        runat="server" 
        ControlToValidate="txtID" 
        ValidationExpression="[a-zA-Z].*" 
        ErrorMessage="문자로 시작해야합니다." 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="IDCheck"/>
    <%--아래 항목은 아이디 유효성 검사를 했는지 확인하기 위해 있음--%>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" 
        runat="server" 
        ControlToValidate="TextBox2"
        ValidationExpression="OK"
        ErrorMessage="아이디를 검사하지 않았습니다." 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
        runat="server"
        ErrorMessage="아이디는 필 수 입력 항목입니다." 
        ControlToValidate="txtID"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
        runat="server"
        ErrorMessage="암호는 필 수 입력 항목입니다." 
        ControlToValidate="txtPassword"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" 
        runat="server" 
        ControlToValidate="txtPassword" 
        ValidationExpression=".*[!@#$%^&*/].*"
        ErrorMessage="비밀번호는 반드시 !@#$%^&*/. 문자 중 하나를 포함해야 합니다." 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" 
        runat="server" 
        ControlToValidate="txtPassword" 
        ValidationExpression="[^\s]{4,12}"
        ErrorMessage="비밀번호는 반드시 4~12자리의 공백 없는 문자로 이루어져야 합니다." 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:CompareValidator ID="CompareValidator1" 
        runat="server" 
        ControlToValidate="txtPassword" 
        ControlToCompare="txtReCheck" 
        ErrorMessage="비밀번호가 일치하지 않습니다." 
        Display="None" ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" 
        runat="server"
        ErrorMessage="이름은 필 수 입력 항목입니다." 
        ControlToValidate="txtName"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
        runat="server"
        ErrorMessage="생년월일은 필 수 입력 항목입니다." 
        ControlToValidate="txtBirthDay"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" 
        runat="server" 
        ControlToValidate="txtBirthDay" 
        ValidationExpression="[0-9]{6}" 
        ErrorMessage="생년월일은 6자리로 입력하여 주십시오." 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
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
    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" 
        runat="server"
        ErrorMessage="성별은 필 수 입력 항목입니다." 
        ControlToValidate="txtGender"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewUser"/>
</asp:Content>
