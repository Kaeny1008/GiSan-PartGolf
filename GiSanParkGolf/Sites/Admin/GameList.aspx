<%@ Page Title="대회개최" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameList.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameList" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function NewGameOpen(strPath) {
            <%--자바스크립트 새창열기 예제--%>
            /*window.open(strPath, "_blank", "scrollbars,resizable,width=950,height=800,left=0,top=0");*/
            location.href = strPath;
        }
    </script>

    <div style="text-align: right;">
        <asp:Button ID="BTN_NewGame" runat="server" Text="신규대회 개최" class="btn btn-outline-success btn-lg" 
            OnClientClick="NewGameOpen('/Sites/Admin/GameCreate.aspx');return false;" />
    </div>
    <hr />
    <uc:NewSearchControl ID="search" runat="server"
        OnSearchRequested="Search_SearchRequested"
        OnResetRequested="Search_ResetRequested" />
    <hr />
    <div style="text-align: right; font-style: italic; font-size: 8pt;">
        Total Record:
        <asp:Literal ID="lblTotalRecord" runat="server" />
    </div>
    <div>
        <h12>(대회명을 클릭하여 수정 / 확인)</h12><br />
    </div>

    <asp:GridView ID="GridView1" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
        ShowHeaderWhenEmpty="true"
        OnRowDataBound="GridView1_RowDataBound">
        <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
        <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
        <FooterStyle BackColor="#CCCCCC" />
        <Columns>
            <%-- No 컬럼은 RowDataBound에서 처리 예정 --%>
            <asp:TemplateField HeaderText="No">
                <ItemTemplate />
                <ItemStyle HorizontalAlign="Center" Width="5%" />
                <HeaderStyle Width="5%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="대회명">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkTitle" runat="server"
                        NavigateUrl='<%# "GameCreate.aspx?gamecode=" + Eval("GameCode") %>'>
                        <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 45) %>
                    </asp:HyperLink>
                </ItemTemplate>
                <HeaderStyle Width="35%" />
                <ItemStyle Width="35%" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="LB_Date" runat="server" Text="개최일자"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("GameDate","{0:yyyy-MM-dd}")%>
                </ItemTemplate>
                <HeaderStyle Width="10%" />
                <ItemStyle Width="10%" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="LB_Place" runat="server" Text="개최지"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("StadiumName")%>
                </ItemTemplate>
                <HeaderStyle Width="10%" />
                <ItemStyle Width="10%" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="LB_Place" runat="server" Text="주최"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("GameHost")%>
                </ItemTemplate>
                <HeaderStyle Width="15%" />
                <ItemStyle Width="15%" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="LB_Place" runat="server" Text="참가인원"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("ParticipantNumber")%>
                </ItemTemplate>
                <HeaderStyle Width="10%" />
                <ItemStyle Width="10%" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="LB_Place" runat="server" Text="상태"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <%#Eval("GameStatus")%>
                </ItemTemplate>
                <HeaderStyle Width="10%" />
                <ItemStyle Width="10%" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
        <EmptyDataTemplate>대회가 없습니다.</EmptyDataTemplate>
    </asp:GridView>

    <div class="center_container">
        <div>
            <uc:NewPagingControl ID="pager" runat="server"
                OnPageChanged="Pager_PageChanged" />
        </div>
    </div>
</asp:Content>
