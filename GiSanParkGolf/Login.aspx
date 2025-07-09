<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GiSanParkGolf.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="center_container">
        <div>
            <br />
            <br />
            <br />
            <br />
            <br />
            <div class="alert alert-primary d-flex align-items-center" role="alert">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-exclamation-triangle-fill flex-shrink-0 me-2" viewBox="0 0 16 16" role="img" aria-label="Warning:">
                    <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
                </svg>
                <div>
                    <strong>로그인</strong><br />
                    로그인 후 이용 할 수 있습니다.
                </div>
            </div>
            <br />
            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon1" style="width:100px">ID</span>
                <asp:TextBox ID="txtUserID" runat="server" class="form-control" placeholder="" />
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text" id="basic-addon2" style="width:100px">비밀번호</span>
                <asp:TextBox ID="txtPassword" runat="server" class="form-control" placeholder="" TextMode="Password" />
            </div>
            <br />
            <br />
            <asp:Button ID="btnLogin" runat="server" Text="로그인" class="btn btn-primary" OnClick="BtnLogin_Click" ValidationGroup="UserLogin" />
            <asp:Button ID="BtnRegister" runat="server" Text="회원가입" class="btn btn-secondary" OnClick="BtnRegister_Click" />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                runat="server"
                ErrorMessage="ID를 입력하여 주십시오." 
                ControlToValidate="txtUserID"
                Display="None" 
                ForeColor="red" 
                ValidationGroup="UserLogin"/><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                runat="server"
                ErrorMessage="비밀번호를 입력하여 주십시오." 
                ControlToValidate="txtPassword"
                Display="None" 
                ForeColor="red" 
                ValidationGroup="UserLogin"/><br />
            <asp:ValidationSummary ID="ValidationSummary_SignupForm"
                ShowMessageBox="true" 
                ShowSummary="false"
                HeaderText="다음 사항을 확인하여 주십시오."
                runat="server" ValidationGroup="UserLogin" />
        </div>
    </div>
</asp:Content>