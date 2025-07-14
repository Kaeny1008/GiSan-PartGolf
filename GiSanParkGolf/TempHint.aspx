<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TempHint.aspx.cs" Inherits="GiSanParkGolf.TempHint" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <!-- Button trigger modal -->
        <button runat="server" id="BTN_PlayerCheck" type="button" class="btn btn-warning" 
            onclick="ShowModal('#MainModal', '테스트 타이틀', '여기는 입력내용 입니다.', true);return false;">
            참가자 확인
        </button>

        <!-- Modal -->
        <div class="modal fade" id="MainModal" tabindex="-1" aria-labelledby="MainModal" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        이미 종료된 대회입니다.
                    </div>
                    <div class="modal-footer">
                        <button id="BTN_No" type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                        <asp:Button ID="BTN_EarlyCloseYes" 
                            runat="server" 
                            class="btn btn-primary" 
                            Text="예" 
                            OnClick="BTN_EarlyCloseYes_Click" />
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal window option script -->
        <script type="text/javascript">
            var showmodalname;
            var showtitle;
            var showbody;
            var onlyyes;
            function ShowModal(modalname, title, body, yes) {
                showmodalname = modalname;
                showtitle = title;
                showbody = body;
                onlyyes = yes;
                $(modalname).modal("show");
            }
            $('#MainModal').on('show.bs.modal', function () {
                var modal = $(this);
                modal.find('.modal-title').text(showtitle);
                modal.find('.modal-body').text(showbody);

                if (onlyyes) {
                    modal.find('.modal-footer #BTN_No').hide("true");
                }
                
            })
        </script>
</asp:Content>