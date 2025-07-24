<%@ Page Title="대회개최" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameList.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameList" %>
<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function NewGameOpen(strPath) {
            location.href = strPath;
        }
    </script>

    <!-- 상단 카드: 페이지 설명 영역 -->
    <div class="mb-3 text-center">
        <h4 class="fw-bold mb-2" id="MainTitle" runat="server">대회 개최</h4>
        <p class="text-muted" style="font-size: 0.95rem;">
            대회를 개최하거나 수정, 경기 취소 등등을 설정할 수 있습니다.<br />
            ※ 대회명을 클릭하면 수정 / 상세 확인 가능합니다
        </p>
    </div>

    <div class="container mt-4">
        <div class="custom-card">
            <!-- 신규 대회 개최 버튼 -->
            <div class="text-end mb-3">
                <asp:Button ID="BTN_NewGame" runat="server"
                    Text="신규 대회 개최"
                    CssClass="btn btn-outline-success btn-xs"
                    OnClientClick="NewGameOpen('/Sites/Admin/GameCreate.aspx'); return false;" />
            </div>

            <!-- 검색 컨트롤 -->
            <uc:NewSearchControl ID="search" runat="server"
                OnSearchRequested="Search_SearchRequested"
                OnResetRequested="Search_ResetRequested" />

            <!-- 총 레코드 표시 -->
            <div class="text-end mt-2" style="font-size: 0.75rem; font-style: italic;">
                전체 대회 수: <asp:Literal ID="lblTotalRecord" runat="server" />
            </div>

            <!-- 대회 목록 그리드 -->
            <asp:GridView ID="GridView1" runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-hover table-responsive"
                ShowHeaderWhenEmpty="true"
                OnRowDataBound="GridView1_RowDataBound">

                <HeaderStyle CssClass="table-light" />
                <RowStyle CssClass="hover-row" />

                <Columns>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate />
                        <ItemStyle Width="5%" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="대회명">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkTitle" runat="server"
                                CssClass="link-hover"
                                NavigateUrl='<%# "GameCreate.aspx?gamecode=" + Eval("GameCode") %>'>
                                <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 45) %>
                            </asp:HyperLink>
                        </ItemTemplate>
                        <ItemStyle Width="30%" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="개최일시">
                        <ItemTemplate>
                            <%# Eval("GameDate", "{0:yyyy-MM-dd HH:mm}") %>
                        </ItemTemplate>
                        <ItemStyle Width="13%" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="장소">
                        <ItemTemplate>
                            <%# Eval("StadiumName") %>
                        </ItemTemplate>
                        <ItemStyle Width="12%" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="주최">
                        <ItemTemplate>
                            <%# Eval("GameHost") %>
                        </ItemTemplate>
                        <ItemStyle Width="15%" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="참가인원">
                        <ItemTemplate>
                            <%# Eval("ParticipantNumber") %>
                        </ItemTemplate>
                        <ItemStyle Width="10%" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="상태">
                        <ItemTemplate>
                            <%# Eval("GameStatus") %>
                        </ItemTemplate>
                        <ItemStyle Width="10%" />
                    </asp:TemplateField>
                </Columns>

                <EmptyDataTemplate>
                    <div class="text-center text-muted py-3">등록된 대회가 없습니다.</div>
                </EmptyDataTemplate>
            </asp:GridView>

            <!-- 페이징 컨트롤 -->
            <div class="center_container mt-3">
                <uc:NewPagingControl ID="pager" runat="server"
                    OnPageChanged="Pager_PageChanged" />
            </div>
        </div>
    </div>
</asp:Content>
