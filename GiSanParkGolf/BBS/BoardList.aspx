<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BoardList.aspx.cs" Inherits="GiSanParkGolf.BBS.BoardList" %>

<%@ Register 
    Src="~/BBS/Controls/BoardSearchFormSingleControl.ascx" 
    TagPrefix="uc1" TagName="BoardSearchFormSingleControl" %>
<%@ Register Src="~/BBS/Controls/AdvancedPagingSingleWithBootstrap.ascx" 
    TagPrefix="uc1" TagName="AdvancedPagingSingleWithBootstrap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .MainLabel {
            text-align: center;
            font-size: 30px;
        }
        /*클릭한적 없는*/
        .HyperLink:link {
	        color:blue; 
            text-decoration:none;
        }
        /*한번이상 클릭*/
        .HyperLink:visited {
	        color:black; 
            text-decoration:none;
        }
        /*마우스 오버*/
        .HyperLink:hover {
	        color:blue; 
            text-decoration:underline;
        }
        /*클릭순간*/
        .HyperLink:active {
	        color:blue; 
            text-decoration:none;
        }
    </style>

    <script language="javascript">

    </script>

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
                        table-striped table-responsive">
                    <Columns>
                        <asp:TemplateField HeaderText="번호"
                            HeaderStyle-Width="50px"
                            ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# RecordCount - (Container.DataItemIndex) - (PageIndex * 10) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="제 목"
                            ItemStyle-HorizontalAlign="Left"
                            HeaderStyle-Width="350px">
                            <ItemTemplate>
                                <%# Dul.BoardLibrary.FuncStep(Eval("Step")) %>
                                <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink"
                                    NavigateUrl='<%# "BoardView.aspx?bbsid=" + Request.QueryString["bbsId"] + "&Id=" + Eval("Id") %>'>
                                    <%# Dul.StringLibrary.CutStringUnicode(Eval("Title").ToString(), 30) %>
                                    <a style="font-size:13px"><%# Dul.BoardLibrary.EmptyCommentCount(Eval("CommentCount")) %></a>
                                    <%--모바일에 적용하면 될듯
                                    <br />
                                    <a style="font-size:13px"><%# Eval("Name") %>&nbsp;|조회&nbsp;<%# Eval("ReadCount") %></a>--%>
                                </asp:HyperLink>
                                <%--<%# Dul.BoardLibrary.FuncNew(Eval("PostDate"))%>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--첨부파일 다운로드는 게시판 최상위에서 표시 안해도 될듯하여 false함--%>
                        <asp:TemplateField HeaderText="첨부"
                            HeaderStyle-Width="70px"
                            ItemStyle-HorizontalAlign="Center" 
                            Visible="false">
                            <ItemTemplate>
                                <%# Dul.BoardLibrary.FuncFileDownSingle(
                                    Convert.ToInt32(Eval("Id")), 
                                    Eval("FileName").ToString(), 
                                    Eval("FileSize").ToString()) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="작성자"
                            HeaderStyle-Width="60px"
                            ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:TemplateField HeaderText="작성일"
                            ItemStyle-Width="90px"
                            ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# Dul.BoardLibrary.FuncShowTime(
                                    Eval("PostDate")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ReadCount" HeaderText="조회수"
                            ItemStyle-HorizontalAlign="Right"
                            HeaderStyle-Width="60px"></asp:BoundField>
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
                    if (!String.IsNullOrEmpty(global_asax.uvm.UserClass))
                    {
                        if (global_asax.uvm.UserClass.Equals("Administrator"))
                        {
            %>
                        <td style="text-align: right;">
                            <asp:Button ID="BTN_Write" runat="server" class="btn btn-primary" Text="글쓰기" OnClick="BTN_Write_Click" />
                        </td>
            <%
                        }
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
