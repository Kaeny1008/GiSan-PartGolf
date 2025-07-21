<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GamePlayerList.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GamePlayerList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>참가자 확인</title>
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/Content/bootstrap.min.css") %>    
        <%: Scripts.Render("~/Scripts/bootstrap.bundle.min.js") %>
    </asp:PlaceHolder>
    <style type="text/css" media="print">
        body * {
            visibility: hidden;
        }

        #printArea, #printArea * {
            visibility: visible;
        }

        #printArea {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="BTN_ToExcel" runat="server" Text="Excel 저장" OnClick="BTN_ToExcel_Click" CssClass="btn btn-primary" />
            <asp:Button ID="BTN_Print" runat="server"
                Text="프린트 하기"
                CssClass="btn btn-outline-dark"
                OnClientClick="window.print(); return false;" />
            <hr />
            <div id="printArea">
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
                                <%#Eval("UserNumber")%>
                            </ItemTemplate>
                            <HeaderStyle Width="30%" />
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
