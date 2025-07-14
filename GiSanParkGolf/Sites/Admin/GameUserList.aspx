<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameUserList.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameUserList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>참가자 확인</title>
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/Content/bootstrap.min.css") %>    
        <%: Scripts.Render("~/Scripts/bootstrap.bundle.min.js") %>
    </asp:PlaceHolder>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GameList"
                runat="server" AutoGenerateColumns="False" DataKeyNames="UserId"
                CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                ShowHeaderWhenEmpty="true" >
                <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
                <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_No" runat="server" Text="No."></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("RowNumber")%>
                        </ItemTemplate>
                        <HeaderStyle Width="10%" />
                        <ItemStyle Width="10%" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_Name" runat="server" Text="ID"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("UserId")%>
                        </ItemTemplate>
                        <HeaderStyle Width="30%" />
                        <ItemStyle Width="30%" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_Name" runat="server" Text="성명"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("UserName")%>
                        </ItemTemplate>
                        <HeaderStyle Width="30%" />
                        <ItemStyle Width="30%" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_WriteDate" runat="server" Text="생년월일"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("UserNumber", "{0:yyyy-MM-dd}")%>
                        </ItemTemplate>
                        <HeaderStyle Width="30%" />
                        <ItemStyle Width="30%" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
