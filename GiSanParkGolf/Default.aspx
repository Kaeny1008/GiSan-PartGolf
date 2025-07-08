<%@ Page Title="기산 파크골프" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GiSanParkGolf.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /*한번이상 클릭*/
        .HyperLink:visited {
	        color:blue; 
        }
        .NoneDeco {
          color: Black;
          text-decoration: none;
        }
    </style>
    <div class="row">
        <div class="col">
            <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/BBS/BoardView.aspx?bbsId=notice" CssClass="NoneDeco">
                <h5>[공지사항]</h5>
            </asp:LinkButton>
            <asp:GridView ID="NoticeList"
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
                            <asp:Label ID="LB_Name" runat="server" Text="제목"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink"
                                NavigateUrl='<%# "~/BBS/BoardView.aspx?bbsId=notice&Id=" + Eval("Id") %>'>
                                <%# Dul.StringLibrary.CutStringUnicode(Eval("Title").ToString(), 30) %>
                                <a style="font-size:13px"><%# Dul.BoardLibrary.EmptyCommentCount(Eval("CommentCount")) %></a>
                            </asp:HyperLink>
                        </ItemTemplate>
                        <HeaderStyle Width="350px" />
                        <ItemStyle Width="350px" HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_Writer" runat="server" Text="작성자"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("Name")%>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" />
                        <ItemStyle Width="60px" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_WriteDate" runat="server" Text="작성일"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Dul.BoardLibrary.FuncShowTime(Eval("PostDate")) %>
                        </ItemTemplate>
                        <HeaderStyle Width="90px" />
                        <ItemStyle Width="90px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div class="col">
            <asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/" CssClass="NoneDeco">
                <h5>[대회]</h5>
            </asp:LinkButton>
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
                                <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 30) %>
                            </asp:HyperLink>
                        </ItemTemplate>
                        <HeaderStyle Width="290px" />
                        <ItemStyle Width="290px" />
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
                            <asp:Label ID="LB_WriteDate" runat="server" Text="상태"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("GameStatus").ToString().Equals("모집중") ?
                                "<a style=\"color:blue;\">모집중</a>"
                                :
                                "<a>" + Eval("GameStatus") + "</a>"
                            %>
                        </ItemTemplate>
                        <HeaderStyle Width="90px" />
                        <ItemStyle Width="90px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
