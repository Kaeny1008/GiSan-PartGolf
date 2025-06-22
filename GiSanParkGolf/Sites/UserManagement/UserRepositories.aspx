<%@ Page Title="신규등록" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserRepositories.aspx.cs" Inherits="GiSanParkGolf.Sites.UserManagement.UserRepositories" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Class/StyleSheet.css?after2" rel="stylesheet" type="text/css"/>
    <script language="javascript">
        //function GenderValidate(source, arguments) {
        //    if (arguments.Value == "선택") {
        //        arguments.IsValid = false;
        //    } else {
        //        arguments.IsValid = true;
        //    }
        //}
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
    </script>

    <div class="Center_Container">
        <div class="Center_Container_Content">
            <h12>정보를 입력하여 주십시오.</h12><br /><br />
            <asp:Label ID="label8" runat="server" Text ="ID"></asp:Label>
            <asp:TextBox ID="TextBox2" runat="server" ValidationGroup="NewUser" Text="Ready" Enabled="false" Width="60px" BorderStyle="None" ForeColor="white" BackColor="white" /><br />
            <asp:TextBox ID="txtID" runat="server" placeholder="아이디를 입력하십시오." ValidationGroup="IDCheck" Width="430px" /><br />
            <asp:Button ID="Btn_IDCheck" runat="server" Text="Check" OnClick="Btn_IDCheck_Click" ValidationGroup="IDCheck" />
            <asp:Label ID="Label13" runat="server" Text=""></asp:Label><br />

            <asp:Label ID="label9" runat="server" Text ="암호"></asp:Label><br />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="암호를 입력하십시오." ValidationGroup="NewUser" Width="430px"></asp:TextBox><br />

            <asp:Label ID="label10" runat="server" Text ="확인"></asp:Label><br />
            <asp:TextBox ID="txtReCheck" runat="server" TextMode="Password" placeholder="암호를 재입력 하십시오." ValidationGroup="NewUser" Width="430px"></asp:TextBox><br />
            <br /><br />

            <asp:Label ID="label1" runat="server" Text ="이름"></asp:Label><br />
            <asp:TextBox ID="txtName" runat="server" ValidationGroup="NewUser" Width="430px"></asp:TextBox><br />

            <asp:Label ID="label3" runat="server" Text ="주민등록 번호"></asp:Label><br />
            <asp:TextBox ID="txtBirthDay" runat="server" Width="150px" onkeypress="return functionx(event)" MaxLength="6"></asp:TextBox>
            <asp:Label ID="label11" runat="server" Text ="-"></asp:Label>
            <asp:TextBox ID="txtGender" runat="server" Width="30px" onkeypress="return functionx(event)" MaxLength="1"></asp:TextBox>
            <asp:Label ID="label12" runat="server" Text ="******"></asp:Label>
            <br />

            <asp:Label ID="label4" runat="server" Text="주소"></asp:Label><br />
            <asp:TextBox ID="txtAddress" runat="server" ValidationGroup="NewUser" Width="430px"></asp:TextBox><br />

            <asp:Label ID="label5" runat="server" Text="상세주소"></asp:Label><br />
            <asp:TextBox ID="txtAddress2" runat="server" Width="430px"></asp:TextBox><br />

            <asp:Label ID="label7" runat="server" Text="비고" ></asp:Label><br />
            <asp:TextBox ID="txtMemo" runat="server" TextMode="MultiLine" Columns="1" Rows="8" Width="430px"></asp:TextBox><br /><br />

            <asp:Label ID="label6" runat="server" ForeColor="#FF3300"></asp:Label><br />

            <asp:Button ID="btnRegister" 
                Width="430px" 
                Height="40px" 
                Font-Bold="true" 
                runat="server" 
                Text="회원가입" 
                OnClick="BTN_Register_Click"
                ValidationGroup="NewUser">
            </asp:Button>
        </div>

        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
            runat="server"
            ErrorMessage="아이디는 필 수 입력 항목입니다." 
            ControlToValidate="txtID"
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
            runat="server" 
            ControlToValidate="txtID" 
            ValidationExpression="[0-9a-zA-Z]{4,15}" 
            ErrorMessage="4~15자리의 문자, 숫자로 이루어져야 합니다." 
            Display="None" 
            ForeColor="red" 
            ValidationGroup="IDCheck"/><br />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" 
            runat="server" 
            ControlToValidate="txtID" 
            ValidationExpression="[a-zA-Z].*" 
            ErrorMessage="문자로 시작해야합니다." 
            Display="None" 
            ForeColor="red" 
            ValidationGroup="IDCheck"/><br />
        <%--아래 항목은 아이디 유효성 검사를 했는지 확인하기 위해 있음--%>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" 
            runat="server" 
            ControlToValidate="TextBox2"
            ValidationExpression="OK"
            ErrorMessage="아이디를 검사하지 않았습니다." 
            Display="None" 
            ForeColor="red" 
            ValidationGroup="NewUser"/><br />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
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

        <asp:ValidationSummary ID="ValidationSummary_SignupForm"
            ShowMessageBox="true" 
            ShowSummary="false"
            HeaderText="다음 사항을 확인하여 주십시오."
            runat="server" ValidationGroup="NewUser" />
        <asp:ValidationSummary ID="ValidationSummary1"
            ShowMessageBox="true" 
            ShowSummary="false"
            HeaderText="다음 사항을 확인하여 주십시오."
            runat="server" ValidationGroup="IDCheck" />
    </div>
</asp:Content>
