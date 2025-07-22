<%@ Page Title="기산 파크골프" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GiSanParkGolf.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Bootstrap Icons CDN --%>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css">

    <%-- 전체 레이아웃 컨테이너 --%>
    <div class="container-fluid px-4 py-4">
        <div class="row g-4">

            <%-- 공지사항 섹션 --%>
            <div class="col-md-6">
                <div class="card custom-card border-start border-primary">
                    <%-- 카드 헤더 --%>
                    <div class="card-header bg-white">
                        <h5 class="card-title">
                            <i class="bi bi-megaphone-fill me-2 text-primary"></i>
                            <asp:LinkButton ID="LinkButton1" runat="server"
                                CssClass="NoneDeco link-hover text-primary fw-bold"
                                PostBackUrl="~/BBS/BoardView.aspx?bbsId=notice">
                                공지사항
                            </asp:LinkButton>
                        </h5>
                    </div>

                    <%-- 공지사항 목록 테이블 --%>
                    <div class="card-body">
                        <asp:GridView ID="NoticeList" runat="server" AutoGenerateColumns="False"
                            CssClass="table table-sm table-borderless" ShowHeaderWhenEmpty="true">
                            <HeaderStyle CssClass="text-center text-muted border-bottom" />
                            <RowStyle CssClass="text-center hover-row align-middle" />
                            <Columns>
                                <%-- 번호 열 --%>
                                <asp:TemplateField HeaderText="No.">
                                    <ItemTemplate><%#Eval("RowNumber")%></ItemTemplate>
                                </asp:TemplateField>

                                <%-- 제목 열 --%>
                                <asp:TemplateField HeaderText="제목">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkTitle" runat="server"
                                            CssClass="HyperLink link-hover fw-semibold text-dark"
                                            NavigateUrl='<%# "~/BBS/BoardView.aspx?bbsId=notice&Id=" + Eval("Id") %>'>
                                            <%# Dul.StringLibrary.CutStringUnicode(Eval("Title").ToString(), 30) %>
                                        </asp:HyperLink>
                                        <%-- 댓글 수 --%>
                                        <span class="text-muted small">
                                            <%# Dul.BoardLibrary.EmptyCommentCount(Eval("CommentCount")) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- 작성자 --%>
                                <asp:TemplateField HeaderText="작성자">
                                    <ItemTemplate><%#Eval("Name")%></ItemTemplate>
                                </asp:TemplateField>

                                <%-- 작성일 --%>
                                <asp:TemplateField HeaderText="작성일">
                                    <ItemTemplate><%# Dul.BoardLibrary.FuncShowTime(Eval("PostDate")) %></ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <%-- 대회목록 섹션 --%>
            <div class="col-md-6">
                <div class="card custom-card border-start border-success">
                    <%-- 카드 헤더 --%>
                    <div class="card-header bg-white">
                        <h5 class="card-title">
                            <i class="bi bi-calendar-event me-2 text-success"></i>
                            <asp:LinkButton ID="LinkButton2" runat="server"
                                CssClass="NoneDeco link-hover text-success fw-bold"
                                PostBackUrl="~/Sites/Player/JoinGame">
                                대회목록
                            </asp:LinkButton>
                        </h5>
                    </div>

                    <%-- 대회 목록 테이블 --%>
                    <div class="card-body">
                        <asp:GridView ID="GameList" runat="server" AutoGenerateColumns="False"
                            CssClass="table table-sm table-borderless" ShowHeaderWhenEmpty="true">
                            <HeaderStyle CssClass="text-center text-muted border-bottom" />
                            <RowStyle CssClass="text-center hover-row align-middle" />
                            <Columns>
                                <%-- 번호 --%>
                                <asp:TemplateField HeaderText="No.">
                                    <ItemTemplate><%#Eval("RowNumber")%></ItemTemplate>
                                </asp:TemplateField>

                                <%-- 대회명: 모집중일 때만 링크 출력 --%>
                                <asp:TemplateField HeaderText="대회명">
                                    <ItemTemplate>
                                        <%# Eval("GameStatus").ToString() == "모집중"
                                            ? "<a class='HyperLink link-hover fw-semibold text-dark' href='/Sites/Player/JoinGame.aspx?GameCode=" + Eval("GameCode") + "'>" +
                                                "<i class='bi bi-box-arrow-in-right me-1'></i>" +
                                                Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 30) +
                                            "</a>"
                                            : "<span class='text-muted'>" + Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 30) + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- 개최지 --%>
                                <asp:TemplateField HeaderText="개최지">
                                    <ItemTemplate><%#Eval("StadiumName")%></ItemTemplate>
                                </asp:TemplateField>

                                <%-- 상태 텍스트 --%>
                                <asp:TemplateField HeaderText="상태">
                                    <ItemTemplate>
                                        <%# Eval("GameStatus").ToString() == "모집중"
                                            ? "<span style='color:blue;'><i class='bi bi-check-circle-fill me-1'></i>모집중</span>"
                                            : "<span style='color:gray;'>" + Eval("GameStatus") + "</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>대회가 없습니다.</EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
