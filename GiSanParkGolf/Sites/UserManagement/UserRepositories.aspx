<%@ Page Title="신규등록" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserRepositories.aspx.cs" Inherits="GiSanParkGolf.Sites.UserManagement.UserRepositories" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Class/StyleSheet.css?after" rel="stylesheet"/>

    <div class="Center_Container">
        <div class="Center_Container_Content">
            <h12>회원가입</h12><br /><br />

            <asp:Label ID="label8" runat="server" Text ="ID"></asp:Label><br />
            <asp:TextBox ID="txtID" runat="server" placeholder="아이디를 입력하십시오." ValidationGroup="IDCheck" Width="360px" />
            <asp:TextBox ID="TextBox2" runat="server" ValidationGroup="NewUser" Text="Ready" Enabled="false" Width="60px" />
            <asp:Button ID="Btn_IDCheck" runat="server" Text="Check" OnClick="Btn_IDCheck_Click" ValidationGroup="IDCheck" />
            <asp:Label ID="Label13" runat="server" Text=""></asp:Label><br />

            <asp:Label ID="label9" runat="server" Text ="암호"></asp:Label><br />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="암호를 입력하십시오." ValidationGroup="NewUser" Width="410px"></asp:TextBox><br />
            
            <asp:Label ID="label10" runat="server" Text ="확인"></asp:Label><br />
            <asp:TextBox ID="txtReCheck" runat="server" TextMode="Password" placeholder="암호를 재입력 하십시오." ValidationGroup="NewUser" Width="410px"></asp:TextBox>

            <%--<asp:Label ID="label12" Width="400px" Height="20px" runat="server" ForeColor="#FF3300"></asp:Label>--%><br /><br /><br />
            <asp:Label ID="label1" runat="server" Text ="이름"></asp:Label><br />

            <asp:TextBox ID="txtName" runat="server" ValidationGroup="NewUser" Width="410px"></asp:TextBox><br />
            <asp:Label ID="label2" runat="server" Text ="성별"></asp:Label><br />
            <asp:DropDownList ID="DropDownList1" runat="server" Width="410px">
                <asp:ListItem>선택</asp:ListItem>
                <asp:ListItem>남자</asp:ListItem>
                <asp:ListItem>여자</asp:ListItem>
            </asp:DropDownList><br />

            <asp:Label ID="label3" runat="server" Text ="생년월인"></asp:Label><br />
            <asp:TextBox ID="TextBox1" runat="server" TextMode="Date" Width="410px"></asp:TextBox><br />

            <asp:Label ID="label4" runat="server" Text="주소"></asp:Label><br />
            <asp:TextBox ID="txtAddress" runat="server" ValidationGroup="NewUser" Width="410px"></asp:TextBox><br />

            <asp:Label ID="label5" runat="server" Text="상세주소"></asp:Label><br />
            <asp:TextBox ID="txtAddress2" runat="server" Width="410px"></asp:TextBox><br />

            <asp:Label ID="label7" runat="server" Text="비고"></asp:Label><br />
            <asp:TextBox ID="txtMemo" runat="server" TextMode="MultiLine" Columns="50" Rows="8"></asp:TextBox><br />

            <asp:Label ID="label6" Width="400px" Height="20px" runat="server" ForeColor="#FF3300"></asp:Label><br /><br />
            <div>

            </div>
            <asp:Button ID="btnCancel" 
                Width="150px" 
                Height="30px" 
                Font-Bold="true" 
                runat="server" 
                Text="취소" 
                OnClick="BTN_Cancel_Click">
            </asp:Button>
            <asp:Button ID="btnRegister" 
                Width="150px" 
                Height="30px" 
                Font-Bold="true" 
                runat="server" 
                Text="회원가입" 
                OnClick="BTN_Register_Click"
                ValidationGroup="NewUser">
            </asp:Button>
        </div>
    </div>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
        ErrorMessage="아이디는 필 수 입력 항목입니다." ControlToValidate="txtID"
        Display="None" ForeColor="red" ValidationGroup="NewUser">
    </asp:RequiredFieldValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
        ControlToValidate="txtID" ValidationExpression="[0-9a-zA-Z]{4,15}" 
        ErrorMessage="4~15자리의 문자, 숫자로 이루어져야 합니다." Display="None" ForeColor="red" 
        ValidationGroup="IDCheck">
    </asp:RegularExpressionValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" 
        ControlToValidate="txtID" 
        ValidationExpression="[a-zA-Z].*" 
        ErrorMessage="문자로 시작해야합니다." Display="None" ForeColor="red" 
        ValidationGroup="IDCheck">
    </asp:RegularExpressionValidator>
    <%--아래 항목은 아이디 유효성 검사를 했는지 확인하기 위해 있음--%>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
        ControlToValidate="TextBox2"
        ValidationExpression="OK"
        ErrorMessage="아이디를 검사하지 않았습니다." Display="None" ForeColor="red" 
        ValidationGroup="NewUser">
    </asp:RegularExpressionValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
        ErrorMessage="암호는 필 수 입력 항목입니다." ControlToValidate="txtPassword"
        Display="None" ForeColor="red" ValidationGroup="NewUser">
    </asp:RequiredFieldValidator><br />
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
        ControlToValidate="txtPassword" 
        ValidationExpression=".*[!@#$%^&*/].*"
        ErrorMessage="비밀번호는 반드시 !@#$%^&*/. 문자 중 하나를 포함해야 합니다." Display="None" ForeColor="red" ValidationGroup="NewUser">
    </asp:RegularExpressionValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
        ControlToValidate="txtPassword" 
        ValidationExpression="[^\s]{4,12}"
        ErrorMessage="비밀번호는 반드시 4~12자리의 공백 없는 문자로 이루어져야 합니다." Display="None" ForeColor="red" ValidationGroup="NewUser">
    </asp:RegularExpressionValidator>
    <asp:CompareValidator ID="CompareValidator1" runat="server" 
        ControlToValidate="txtPassword" 
        ControlToCompare="txtReCheck" 
        ErrorMessage="비밀번호가 일치하지 않습니다." Display="None" ForeColor="red" ValidationGroup="NewUser">
    </asp:CompareValidator><br />

    <asp:ValidationSummary ID="ValidationSummary_SignupForm"
        ShowMessageBox="true" 
        ShowSummary="false"
        HeaderText="Please correct the following errors in the page."
        runat="server" ValidationGroup="NewUser" />
    <asp:ValidationSummary ID="ValidationSummary1"
        ShowMessageBox="true" 
        ShowSummary="false"
        HeaderText="Please correct the following errors in the page."
        runat="server" ValidationGroup="IDCheck" />
</asp:Content>
