<%@ Page Title="Handicap 설정" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameHandicap.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameHandicap" enableEventValidation="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h2>게임 핸디캡 관리</h2>

        <!-- 게임 선택 -->
        <div class="mb-3">
            <asp:DropDownList ID="ddlGame" runat="server"
                AutoPostBack="true" CssClass="form-select"
                OnSelectedIndexChanged="ddlGame_SelectedIndexChanged" />
        </div>

        <!-- 검색 -->
        <div class="input-group mb-3" style="max-width:400px;">
            <asp:TextBox ID="txtSearch" runat="server"
                         CssClass="form-control"
                         Placeholder="참가자 이름 검색..." />
            <button runat="server" id="btnSearch"
                    class="btn btn-outline-secondary"
                    onserverclick="btnSearch_Click">
                검색
            </button>
        </div>

        <!-- 전체 자동 산정 -->
        <asp:Button ID="btnRecalc" runat="server"
            CssClass="btn btn-secondary mb-3"
            Text="전체 핸디캡 자동 계산"
            OnClick="btnRecalc_Click" />

        <!-- 핸디캡 GridView -->
        <asp:GridView ID="gvHandicap" runat="server"
            CssClass="table table-striped"
            AutoGenerateColumns="False"
            DataKeyNames="UserId"
            AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gvHandicap_PageIndexChanging"
            OnRowEditing="gvHandicap_RowEditing"
            OnRowCancelingEdit="gvHandicap_RowCancelingEdit"
            OnRowUpdating="gvHandicap_RowUpdating">

            <Columns>
                <asp:BoundField DataField="UserName" HeaderText="이름" ReadOnly="true" />
                <asp:BoundField DataField="GameName" HeaderText="대회명" ReadOnly="true" />

                <asp:TemplateField HeaderText="핸디캡">
                    <ItemTemplate>
                        <%# Eval("Handicap") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtHc" runat="server"
                            Text='<%# Bind("Handicap") %>'
                            CssClass="form-control" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="방식">
                    <ItemTemplate>
                        <%# Eval("Source") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlSource" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Auto" Value="Auto" />
                            <asp:ListItem Text="Manual" Value="Manual" />
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:CommandField ShowEditButton="True" />
            </Columns>
        </asp:GridView>

        <asp:Label ID="lblMsg" runat="server" CssClass="mt-3" />
</asp:Content>
