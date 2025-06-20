<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GiSanParkGolf.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>로그인</h2>

    아이디 : <asp:TextBox ID="txtUserID" runat="server"></asp:TextBox><br />
    암호 : <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><br />
    <asp:Button ID="btnLogin" runat="server" Text="로그인" OnClick="btnLogin_Click" />
</asp:Content>
