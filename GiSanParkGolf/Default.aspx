<%@ Page Title="기산 파크골프" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GiSanParkGolf.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /*한번이상 클릭*/
        .HyperLink:visited {
	        color:black; 
        }
    </style>
    <div class="row">
        <div class="col">
            <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/BBS/BoardView.aspx?bbsId=notice">
                <h5>[공지사항]</h5>
            </asp:LinkButton>
            <asp:GridView ID="ctlBoardList"
                runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                CssClass="table table-bordered table-hover table-condensed 
                    table-striped table-responsive">
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
            <input type="text" class="form-control" placeholder="Last name" aria-label="Last name">
        </div>
    </div>
</asp:Content>
