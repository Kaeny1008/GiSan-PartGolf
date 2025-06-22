<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GiSanParkGolf.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Class/StyleSheet.css?after2" rel="stylesheet"/>
    
    <div class="Center_Container">
        <div class="Center_Container_Content">
            <br />
            <br />
            <br />
            <br />
            <br />
            <h2>로그인</h2>
            <asp:Label runat="server" CssClass="NormalLabel">ID</asp:Label><br />
            <asp:TextBox ID="txtUserID" runat="server" placeholder="사용자 ID 입력"></asp:TextBox><br />
            <asp:Label runat="server" CssClass="NormalLabel">암호</asp:Label><br />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="비밀번호 입력"></asp:TextBox><br />
            <br />
            <asp:Button ID="btnLogin" runat="server" Text="로그인" OnClick="BtnLogin_Click" CssClass="NormalButton" />
            <asp:Button ID="BtnRegister" runat="server" Text="회원가입" OnClick="BtnRegister_Click" CssClass="NormalButton" />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
        </div>
    </div>
</asp:Content>
