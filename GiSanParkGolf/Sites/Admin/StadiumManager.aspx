<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StadiumManager.aspx.cs"
    Inherits="GiSanParkGolf.Sites.Admin.StadiumManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">

         <%--경기장 목록--%> 
        <div class="custom-card mb-4">
            <div class="d-flex justify-content-between align-items-center">
                <h5 class="card-title">경기장 목록</h5>
                <asp:Button ID="BTN_ShowStadiumForm" runat="server" Text="신규 등록" CssClass="btn btn-outline-primary btn-sm" OnClick="BTN_ShowStadiumForm_Click" />
            </div>
            <asp:GridView ID="GV_StadiumList" runat="server"
                CssClass="table table-bordered table-hover table-striped table-responsive"
                AutoGenerateColumns="False" OnSelectedIndexChanged="GV_StadiumList_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="StadiumCode" HeaderText="코드" />
                    <asp:BoundField DataField="StadiumName" HeaderText="경기장 이름" />
                    <asp:CommandField ShowSelectButton="True" SelectText="선택" ButtonType="Button" />
                </Columns>
            </asp:GridView>
        </div>

         <%--① 경기장 등록 폼--%> 
        <asp:Panel ID="Panel_StadiumForm" runat="server" Visible="false">
            <div class="custom-card mb-4">
                <h5 class="card-title">신규 경기장 등록</h5>
                <div class="border rounded p-3 bg-light">
                    <div class="input-group mb-2">
                        <span class="input-group-text">경기장 코드</span>
                        <asp:TextBox ID="TB_StadiumCode" runat="server" CssClass="form-control" />
                    </div>
                    <div class="input-group mb-2">
                        <span class="input-group-text">경기장 이름</span>
                        <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="input-group mb-3">
                        <span class="input-group-text">사용 여부</span>
                        <asp:DropDownList ID="DDL_StadiumActive" runat="server" CssClass="form-select">
                            <asp:ListItem Text="사용함" Value="True" />
                            <asp:ListItem Text="사용 안 함" Value="False" />
                        </asp:DropDownList>
                    </div>
                    <div class="input-group mb-3">
                        <span class="input-group-text">비고</span>
                        <asp:TextBox ID="TB_StadiumNote" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="text-end mt-2">
                        <asp:Button ID="BTN_InsertStadium" runat="server" Text="등록" CssClass="btn btn-primary btn-sm" />
                    </div>
                </div>
            </div>
        </asp:Panel>

         <%--② 코스 등록 폼--%> 
        <asp:Panel ID="Panel_CourseForm" runat="server" Visible="false">
            <div class="custom-card mb-4">
                <h5 class="card-title">코스 등록</h5>
                <div class="border rounded p-3 bg-light">
                    <div class="input-group mb-2">
                        <span class="input-group-text">코스명</span>
                        <asp:TextBox ID="TB_CourseName" runat="server" CssClass="form-control" placeholder="예: A코스" />
                    </div>
                    <div class="input-group mb-2">
                        <span class="input-group-text">최대 홀 수</span>
                        <asp:TextBox ID="TB_MaxHoleCount" runat="server" CssClass="form-control" TextMode="Number" />
                    </div>
                    <div class="input-group mb-3">
                        <span class="input-group-text">사용 여부</span>
                        <asp:DropDownList ID="DDL_CourseActive" runat="server" CssClass="form-select">
                            <asp:ListItem Text="사용함" Value="True" />
                            <asp:ListItem Text="사용 안 함" Value="False" />
                        </asp:DropDownList>
                    </div>
                    <div class="text-end mt-2">
                        <asp:Button ID="BTN_InsertCourse" runat="server" Text="코스 등록" CssClass="btn btn-success btn-sm" />
                    </div>
                </div>
            </div>
        </asp:Panel>

         <%--③ 홀 상세 정보 입력--%> 
        <asp:Panel ID="Panel_HoleForm" runat="server" Visible="false">
            <div class="custom-card mb-4">
                <h5 class="card-title">홀 정보 입력</h5>
                <p class="text-muted">자동 생성된 홀에 대해 거리와 Par 정보를 입력하세요.</p>

                <asp:GridView ID="GV_HoleDetail" runat="server"
                    CssClass="table table-bordered table-hover table-striped"
                    AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="HoleName" HeaderText="홀명" />
                        <asp:TemplateField HeaderText="거리(m)">
                            <ItemTemplate>
                                <asp:TextBox ID="TB_Distance" runat="server" Text='<%# Eval("Distance") %>' CssClass="form-control form-control-sm" TextMode="Number" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Par">
                            <ItemTemplate>
                                <asp:TextBox ID="TB_Par" runat="server" Text='<%# Eval("Par") %>' CssClass="form-control form-control-sm" TextMode="Number" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <div class="text-end mt-2">
                    <asp:Button ID="BTN_SaveHoleDetail" runat="server" Text="홀 정보 저장" CssClass="btn btn-warning btn-sm" />
                </div>
            </div>
        </asp:Panel>

    </div>
</asp:Content>
