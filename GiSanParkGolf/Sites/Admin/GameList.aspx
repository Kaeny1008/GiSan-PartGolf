<%@ Page Title="대회개최" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameList.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameList" %>

<%@ Register Src="~/Controls/PagingControl.ascx" TagPrefix="uc1" TagName="PagingControl" %>
<%@ Register Src="~/Controls/SearchControl.ascx" TagPrefix="uc1" TagName="SearchControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function NewGameOpen(strPath) {
            <%--자바스크립트 새창열기 예제--%>
            /*window.open(strPath, "_blank", "scrollbars,resizable,width=950,height=800,left=0,top=0");*/
            location.href = strPath;
            window.open(strPath);
        }
    </script>

    <div style="text-align: right;">
        <asp:Button ID="BTN_NewGame" runat="server" Text="신규대회 개최" class="btn btn-outline-success btn-lg" 
            OnClientClick="NewGameOpen('/Sites/Admin/GameCreate.aspx');return false;" />
    </div>
    <hr />
    <uc1:SearchControl runat="server" ID="SearchControl" />
    <hr />
    <div style="font-style: italic; text-align: right; font-size: 8pt;">
        Total Record:
        <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
    </div>
    <div>
        <h12>(대회명을 클릭하여 수정 / 확인)</h12><br />
    </div>

    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="GameCode"
        CssClass="table table-bordered table-hover table-condensed table-striped table-responsive">
        <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
        <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
        <FooterStyle BackColor="#CCCCCC" />
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
            <asp:TemplateField SortExpression="ContentName">
                <HeaderTemplate>
                    <asp:Label ID="LB_Name" runat="server" Text="대회명"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink"
                        NavigateUrl='<%# "GameCreate.aspx?gamecode=" + Eval("GameCode") %>'>
                        <%#Eval("GameName")%>
                    </asp:HyperLink>
                </ItemTemplate>
                <HeaderStyle Width="30%" />
                <ItemStyle Width="30%" />
            </asp:TemplateField>
            <asp:TemplateField SortExpression="ContentDate">
                <HeaderTemplate>
                    <asp:Label ID="LB_Date" runat="server" Text="개최일자"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("GameDate","{0:yyyy-MM-dd}")%>
                </ItemTemplate>
                <HeaderStyle Width="20%" />
                <ItemStyle Width="20%" />
            </asp:TemplateField>
            <asp:TemplateField SortExpression="ContentPlace">
                <HeaderTemplate>
                    <asp:Label ID="LB_Place" runat="server" Text="개최지"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("StadiumName")%>
                </ItemTemplate>
                <HeaderStyle Width="30%" />
                <ItemStyle Width="30%" />
            </asp:TemplateField>
            <asp:TemplateField SortExpression="ContentPlace">
                <HeaderTemplate>
                    <asp:Label ID="LB_Place" runat="server" Text="참가인원(신청)"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    준비중
                </ItemTemplate>
                <HeaderStyle Width="10%" />
                <ItemStyle Width="10%" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
    </asp:GridView>

    <div class="center_container">
        <div>
            <uc1:PagingControl runat="server" ID="PagingControl" />
        </div>
    </div>
</asp:Content>
