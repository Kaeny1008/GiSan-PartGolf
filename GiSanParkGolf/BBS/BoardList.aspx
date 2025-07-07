<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BoardList.aspx.cs" Inherits="GiSanParkGolf.BBS.BoardList" %>

<%@ Register Src="~/BBS/Controls/BoardSearchFormSingleControl.ascx" 
    TagPrefix="uc1" TagName="BoardSearchFormSingleControl" %>
<%@ Register Src="~/BBS/Controls/AdvancedPagingSingleWithBootstrap.ascx" 
    TagPrefix="uc1" TagName="AdvancedPagingSingleWithBootstrap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .MainLabel {
            text-align: center;
            font-size: 30px;
        }
        /*한번이상 클릭*/
        .HyperLink:visited {
	        color:black; 
        }
    </style>

    <div style="text-align: center;"><asp:Label ID="LBMainTitle" runat="server" Class="MainLabel">게시판</asp:Label></div>
    <span style="color: #ff0000">게시판입니다.</span>
    <hr />
    <table style="width: 100%; margin-left: auto; margin-right: auto;">
        <tr>
            <td>
                <div style="font-style: italic; text-align: right; font-size: 8pt;">
                    Total Record:
                    <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
                </div>
                <asp:GridView ID="ctlBoardList"
                    runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    CssClass="table table-bordered table-hover table-condensed 
                        table-striped table-responsive ">
                    <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
                    <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_No" runat="server" Text="No."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# RecordCount - (Container.DataItemIndex) - (PageIndex * 10) %>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Name" runat="server" Text="제목"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%--<%# Dul.BoardLibrary.FuncStep(Eval("Step")) %>--%>
                                <%# Dul.BoardLibrary.FuncNew(Eval("PostDate"))%>
                                <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink"
                                    NavigateUrl='<%# "BoardView.aspx?bbsId=" + Request.QueryString["bbsId"] + "&Id=" + Eval("Id") %>'>
                                    <%# Dul.StringLibrary.CutStringUnicode(Eval("Title").ToString(), 30) %>
                                    <a style="font-size:13px"><%# Dul.BoardLibrary.EmptyCommentCount(Eval("CommentCount")) %></a>
                                    <%--모바일에 적용하면 될듯
                                    <br />
                                    <a style="font-size:13px"><%# Eval("Name") %>&nbsp;|조회&nbsp;<%# Eval("ReadCount") %></a>--%>
                                </asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle Width="350px" />
                            <ItemStyle Width="350px" HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <%--첨부파일 다운로드는 게시판 최상위에서 표시 안해도 될듯하여 false함--%>
                        <asp:TemplateField Visible="false">
                            <HeaderTemplate>
                                <asp:Label ID="LB_Add" runat="server" Text="첨부"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# Dul.BoardLibrary.FuncFileDownSingle(
                                    Convert.ToInt32(Eval("Id")), 
                                    Eval("FileName").ToString(), 
                                    Eval("FileSize").ToString()) %>
                            </ItemTemplate>
                            <HeaderStyle Width="70px" />
                            <ItemStyle Width="70px" />
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
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_ReadCount" runat="server" Text="조회수"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("ReadCount","{0:0}")%>
                            </ItemTemplate>
                            <HeaderStyle Width="60px" />
                            <ItemStyle Width="60px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td style="text-align: center;">
                <uc1:AdvancedPagingSingleWithBootstrap runat="server"
                    ID="AdvancedPagingSingleWithBootstrap" />
            </td>
        </tr>
        <tr>
            <% 
                //공지사항일때는 Administrator만 글쓰기 가능
                if (Request.QueryString["bbsId"].Equals("notice"))
                {
                    if (Page.User.Identity.IsAuthenticated)
                    {
                        System.Diagnostics.Debug.WriteLine("게시판 : 로그인 되어 있다.");
                        if (global_asax.uvm.UserClass.Equals(1))
                        {
                            System.Diagnostics.Debug.WriteLine("현재 유저등급 OK");
            %>
                            <td style="text-align: right;">
                                <asp:Button ID="BTN_Write" runat="server" class="btn btn-primary" Text="글쓰기" OnClick="BTN_Write_Click" />
                            </td>
            <%
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("게시판 : 로그인 되어 있지 않다.");
                    }
                }
                else
                {
            %>
                    <td style="text-align: right;">
                        <asp:Button ID="Button1" runat="server" class="btn btn-primary" Text="글쓰기" OnClick="BTN_Write_Click" />
                    </td>
            <%
                } 
            %>
        </tr>
    </table>

    
 
    <uc1:BoardSearchFormSingleControl runat="server"
        ID="BoardSearchFormSingleControl" />
</asp:Content>
