<%@ Page Title="내 대회" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyGame.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.MyGame" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        var gamecode;

        function ShowModal(gamecode2) {
            gamecode = gamecode2;
            $("#SaveModal").modal("show");
        }
        function GoGameCancel() {
            var gp = "MyGame.aspx?GameCancel=true&GameCode=" + gamecode;
            console.log(gp);
            location.href = gp;

            return false;
        }
    </script>

    <div id="MainContent" runat="server">
        <div class="center_container">
            <div style="width:100%">
                <div style="text-align:left;">
                    <h4 style="color:cornflowerblue">참여 대회 목록</h4>
                    <p>내 대회의 결과 및 참가여부 수정을 할 수 있습니다.</p>
                </div>
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
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="여부"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("GameStatus").ToString().Equals("모집중") ?
                                    "<button type=\"button\" class=\"btn btn-primary\"" +
                                    " style=\"--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .75rem;\"" +
                                    " runat=\"server\" OnClick=\"ShowModal('" + Eval("GameCode") + "');return false;\" id=\"Button2\">" +
                                    "취소" +
                                    "</button>"
                                    :
                                    "<a>완료</a>"
                                %>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    참가취소 하시겠습니까?
                    <br />
                    다시 참가하려면 경기등록을 다시 신청하여야 합니다.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                    <asp:Button ID="Button2" 
                        runat="server" 
                        OnClientClick="return GoGameCancel();"
                        class="btn btn-primary" 
                        Text="예" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
