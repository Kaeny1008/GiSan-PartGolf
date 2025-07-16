<%@ Page Title="핸디캡 변경 이력"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="GameHandicapLog.aspx.cs"
    Inherits="GiSanParkGolf.Sites.Admin.GameHandicapLog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .grid-center th,
        .grid-center td {
            text-align: center;
            vertical-align: middle;
        }

        .gridview-pager tfoot td {
            text-align: center !important;
        }

        .gridview-pager td table {
            margin: 0 auto !important;
        }
        .changed-cell {
            background-color: #f9ecec !important;
            transition: background-color 0.5s ease-in-out;
        }
    </style>
    <script type="text/javascript">
        function showModal() {
            const modalEl = document.getElementById('msgModal');
            if (modalEl) {
                const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                modal.show();
            }
        }
    </script>
    <div class="container mt-4">
        <h2 class="mb-3">🔍 핸디캡 변경 이력 조회</h2>

        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
            <div class="row mb-3">
                <div class="col-md-10 d-flex flex-wrap gap-2">
                    <%-- 사용자 ID 입력 + 조회 & 초기화 버튼 --%>
                    <div class="col-md-auto">
                        <div class="input-group">
                            <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control" />
                            <asp:Button ID="btnSearch" runat="server" Text="조회"
                                CssClass="btn btn-outline-secondary" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnReset" runat="server" Text="초기화"
                                CssClass="btn btn-primary" OnClick="btnReset_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:GridView ID="gvLog" runat="server"
            AutoGenerateColumns="False"
            CssClass="gridview-pager grid-center table table-bordered table-hover table-condensed table-striped table-responsive"
            AllowPaging="true"
            PageSize="10"
            OnPageIndexChanging="gvLog_PageIndexChanging"
            OnRowDataBound="gvLog_RowDataBound"
            PagerSettings-Mode="NumericFirstLast"  
            PagerSettings-PageButtonCount="10"      
            PagerSettings-PreviousPageText="◀"
            PagerSettings-NextPageText="▶"
            PagerSettings-FirstPageText="처음"
            PagerSettings-LastPageText="끝"
            PagerStyle-HorizontalAlign="Center"
            PagerStyle-CssClass="custom-pager">

            <Columns>
                <%--번호 컬럼--%> 
                <asp:TemplateField HeaderText="No">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 + (gvLog.PageSize * gvLog.PageIndex) %>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>

                <%--사용자 ID → ID 로 표시--%> 
                <asp:BoundField DataField="UserId" HeaderText="ID" />
                <asp:BoundField DataField="UserName" HeaderText="이름" />
                <asp:BoundField DataField="Age" HeaderText="나이" />
                <asp:BoundField DataField="PrevHandicap" HeaderText="이전 핸디캡" />
                <asp:BoundField DataField="NewHandicap" HeaderText="변경 핸디캡" />
                <asp:BoundField DataField="PrevSource" HeaderText="이전 방식" />
                <asp:BoundField DataField="NewSource" HeaderText="변경 방식" />
                <asp:BoundField DataField="ChangedBy" HeaderText="수정자" />
                <asp:BoundField DataField="ChangedAt" HeaderText="변경일시" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="Reason" HeaderText="변경 사유" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="modal fade" id="msgModal" tabindex="-1" aria-labelledby="msgModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title fs-5" id="msgModalLabel">알림</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="닫기"></button>
          </div>
          <div class="modal-body">
            <asp:Label ID="lblModalMessage" runat="server"/>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">확인</button>
          </div>
        </div>
      </div>
    </div>
</asp:Content>
