<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TempHint.aspx.cs" Inherits="GiSanParkGolf.TempHint" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <!-- Button trigger modal -->
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#CloseGame" data-title="Test Title">
          Launch demo modal
        </button>

        <!-- Modal -->
        <div class="modal fade" id="CloseGame" tabindex="-1" aria-labelledby="EarlyCloseModal" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5" id="exampleModalLabel2">확인</h1>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        종료된 대회입니다.
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">확인</button>
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal window option script -->
        <script type="text/javascript">
            $('#CloseGame').on('show.bs.modal', function () {
                var titleTxt = "abcd123123"
                var modal = $(this)
                modal.find('.modal-title').text('Title : ' + titleTxt)
            })
        </script>
</asp:Content>