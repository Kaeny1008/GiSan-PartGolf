<%@ Page Title="신규등록" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Player Register.aspx.cs" Inherits="GiSanParkGolf.Sites.Player_Register" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <p class="lead">신규 선수를 등록한다.</p>
    <h1>선수정보</h1>

    <asp:Label ID="label8" runat="server" Text ="ID"></asp:Label>
    <asp:TextBox ID="txtID" runat="server"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="Check" /><br />
    <asp:Label ID="label9" runat="server" Text ="암호"></asp:Label>
    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><br />
    <asp:Label ID="label10" runat="server" Text ="확인"></asp:Label>
    <asp:TextBox ID="txtReCheck" runat="server" TextMode="Password"></asp:TextBox><br /><br />
    <asp:Label ID="label1" runat="server" Text ="이름"></asp:Label>
    <asp:TextBox ID="txtName" runat="server"></asp:TextBox><br />
    <asp:Label ID="label2" runat="server" Text ="성별"></asp:Label>
    <asp:DropDownList ID="DropDownList1" runat="server">
        <asp:ListItem>선택</asp:ListItem>
        <asp:ListItem>남자</asp:ListItem>
        <asp:ListItem>여자</asp:ListItem>
    </asp:DropDownList><br />
    <asp:Label ID="label3" runat="server" Text ="생년월인"></asp:Label>
    <asp:TextBox ID="TextBox1" runat="server" TextMode="Date"></asp:TextBox><br />
    <asp:Label ID="label4" runat="server" Text="주소"></asp:Label>
    <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox><br />
    <asp:Label ID="label5" runat="server" Text="상세주소"></asp:Label>
    <asp:TextBox ID="txtAddress2" runat="server"></asp:TextBox><br />
    <asp:Label ID="label7" runat="server" Text="비고"></asp:Label>
    <asp:TextBox ID="txtMemo" runat="server" TextMode="MultiLine" Columns="50" Rows="5"></asp:TextBox><br />
    <asp:Label ID="label6" Width="400px" Height="20px" runat="server" ForeColor="#FF3300"></asp:Label><br />
    <asp:Button ID="btnRegister" 
        Width="150px" 
        Height="30px" 
        Font-Bold="true" 
        runat="server" 
        Text="선수등록" 
        OnClick="BTN_Register_Click">
    </asp:Button>
</asp:Content>
