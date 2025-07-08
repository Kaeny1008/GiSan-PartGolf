<%@ Page Title="대회참가" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="JoinGame.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.JoinGame" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="MainDIV" runat="server">
        <h5>[참여가능 대회 목록]</h5>
        <h8>참가하려는 대회명을 선택하여 참가신청을 하십시오.</h8>
        <asp:GridView ID="GameList"
            runat="server" AutoGenerateColumns="False"
            CssClass="table table-bordered table-hover table-condensed table-striped table-responsive">
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
                    <HeaderStyle Width="50px" />
                    <ItemStyle Width="50px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_Name" runat="server" Text="대회명"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink" 
                            NavigateUrl=<%# "~/Sites/Player/JoinGame.aspx?GameCode=" + Eval("GameCode")%>>
                            <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>
                        </asp:HyperLink>
                    </ItemTemplate>
                    <HeaderStyle Width="200px" />
                    <ItemStyle Width="200px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_Writer" runat="server" Text="개최지"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("StadiumName")%>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" />
                    <ItemStyle Width="120px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_Writer" runat="server" Text="주최자"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("GameHost")%>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" />
                    <ItemStyle Width="120px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_WriteDate" runat="server" Text="대회일자"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("GameDate", "{0:yyyy-MM-dd}")%>
                    </ItemTemplate>
                    <HeaderStyle Width="90px" />
                    <ItemStyle Width="90px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_WriteDate" runat="server" Text="모집시작"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("StartRecruiting", "{0:yyyy-MM-dd}")%>
                    </ItemTemplate>
                    <HeaderStyle Width="90px" />
                    <ItemStyle Width="90px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_WriteDate" runat="server" Text="모집종료"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("EndRecruiting", "{0:yyyy-MM-dd}")%>
                    </ItemTemplate>
                    <HeaderStyle Width="90px" />
                    <ItemStyle Width="90px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_WriteDate" runat="server" Text="참가자"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        0
                    </ItemTemplate>
                    <HeaderStyle Width="50px" />
                    <ItemStyle Width="50px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_WriteDate" runat="server" Text="상태"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("GameStatus").ToString().Equals("모집중") ?
                            "<a style=\"color:blue;\">모집중</a>"
                            :
                            "<a>" + Eval("GameStatus") + "</a>"
                        %>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" />
                    <ItemStyle Width="50px" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div id="GameContent" runat="server">

    </div>
</asp:Content>
