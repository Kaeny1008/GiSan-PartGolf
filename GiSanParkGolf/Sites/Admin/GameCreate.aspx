<%@ Page Title="대회" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameCreate.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function ValidateCheck(modalname, title, body, yesbutton) {
            var isValid = false;
            isValid = Page_ClientValidate('NewGame');

            if (isValid) {
                ShowModal(modalname, title, body, yesbutton);
            }
        }
        var showModalName; //모달이름
        var showTitle; //타이틀
        var showBody; //바디내용
        var showYesButton

        function ShowModal(modalname, title, body, yesbutton) {
            showModalName = modalname;
            showTitle = title;
            showYesButton = yesbutton;
            $(modalname).modal("show");
        }

        $(document).on('show.bs.modal', showModalName, function () {
            var modal = $(this);
            modal.find('.modal-title').text(showTitle);
            modal.find('.modal-body').text(showBody);

            switch (showYesButton) {
                case 0:
                    modal.find('.modal-footer #MainContent_BTN_Save').show();
                    modal.find('.modal-footer #MainContent_BTN_Update').hide();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').hide();
                    modal.find('.modal-footer #BTN_Completed').hide();
                    break;
                case 1:
                    modal.find('.modal-footer #MainContent_BTN_Save').hide();
                    modal.find('.modal-footer #MainContent_BTN_Update').show();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').hide();
                    modal.find('.modal-footer #BTN_Completed').hide();
                    break;
                case 2:
                    modal.find('.modal-footer #MainContent_BTN_Save').hide();
                    modal.find('.modal-footer #MainContent_BTN_Update').hide();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').show();
                    modal.find('.modal-footer #BTN_Completed').hide();
                    break;
                case 4:
                    modal.find('.modal-footer #MainContent_BTN_Save').hide();
                    modal.find('.modal-footer #MainContent_BTN_Update').hide();
                    modal.find('.modal-footer #MainContent_BTN_Cancel').hide();
                    modal.find('.modal-footer #BTN_No').hide();
                    modal.find('.modal-footer #BTN_Completed').show();
                    break;
                default:
                    break;
            }
        })

        /*로딩을 완료하기 위해(안그러면 로딩이 안되어 모달 실행이 안된다.*/
        /*아래 두함수가 셋트로 움직여야 한다.*/
        var launch = false;
        function launchModal(modalname, title, body, yesbutton) {
            showModalName = modalname;
            showTitle = title;
            showBody = body;
            showYesButton = yesbutton;
            launch = true;
        }
        function pageLoad() {
            if (launch) {
                $(showModalName).modal("show");
            }
        }
    </script>
    <style>
        .panel-container {
            display: flex;
            flex-wrap: wrap;
            justify-content: center; /* 좌/우 패널 가운데 정렬 */
            gap: 1rem;
        }
    </style>


    <!-- 상단 카드: 페이지 설명 영역 -->
    <div class="mb-3 text-center">
        <h4 class="fw-bold mb-2" id="MainTitle" runat="server">신규 대회 개최</h4>
        <p class="text-muted" style="font-size: 0.95rem;">
            대회의 정보를 입력하고 모집 일정을 설정하세요.
        </p>
    </div>
    <div class="container mt-4">
        <div class="panel-container d-flex flex-wrap gap-2">
            <!-- 좌측 패널 -->
            <div class="custom-card left-panel" style="flex: 1; min-width: 320px; max-width: 400px">
                <h5 class="card-title">대회 정보</h5>
                <div class="input-group mb-2">
                    <span class="input-group-text">대회명</span>
                    <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">일자</span>
                    <asp:TextBox ID="TB_GameDate" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">시각</span>
                    <asp:TextBox ID="TB_GameTime" runat="server" TextMode="Time" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">경기장</span>
                    <asp:DropDownList ID="DDL_Stadium" runat="server" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">주최</span>
                    <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">홀당 최대인원</span>
                    <asp:TextBox ID="TB_HoleMaximum" runat="server" TextMode="Number" Text="4" CssClass="form-control" />
                </div>
                <div class="input-group mb-2 align-items-center">
                    <span class="input-group-text">방식</span>
                    <div class="form-control d-flex flex-row justify-content-around">
                        <asp:RadioButtonList ID="rblPlayMode" runat="server" CssClass="d-flex gap-3" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Text="스트로크" Value="Stroke" Selected="True" />
                            <asp:ListItem Text="매치" Value="Match" Enabled="False" />
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="form-text text-muted mb-2">
                    모든 대회는 팀별 분산 출발 방식으로 진행됩니다.<br />
                    각 팀은 서로 다른 홀에서 동시에 출발하여<br /> 
                    전체 코스를 순환하는 형태의 경기입니다.
                </div>
            </div>

            <!-- 우측 패널 -->
            <div class="custom-card right-panel" style="flex: 1; min-width: 320px; max-width: 400px">
                <h5 class="card-title">모집 일정 및 비고</h5>
                <div class="input-group mb-2">
                    <span class="input-group-text">시작일</span>
                    <asp:TextBox ID="TB_StartDate" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">시각</span>
                    <asp:TextBox ID="TB_StartTime" runat="server" TextMode="Time" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">종료일</span>
                    <asp:TextBox ID="TB_EndDate" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">시각</span>
                    <asp:TextBox ID="TB_EndTime" runat="server" TextMode="Time" CssClass="form-control" />
                </div>
                <div class="input-group mb-2">
                    <span class="input-group-text">비고</span>
                    <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control" TextMode="MultiLine" Height="240px" />
                </div>
            </div>

        </div>
    </div>

    <!-- 아래쪽 버튼 카드 -->
    <div class="text-center mt-4">
        <% if (string.IsNullOrEmpty(Request.QueryString["gamecode"])) { %>
            <asp:Button ID="ButtonSave" runat="server"
                CssClass="btn btn-outline-primary btn-lg"
                Style="width:160px;" Text="저장"
                ValidationGroup="NewGame"
                OnClientClick="ValidateCheck('#MainModal', '저장', '저장 하시겠습니까?', 0); return false;" />
        <% } else { %>
            <asp:Button ID="ButtonUpdate" runat="server"
                CssClass="btn btn-outline-primary btn-lg"
                Style="width:160px;" Text="수정"
                ValidationGroup="NewGame"
                OnClientClick="ValidateCheck('#MainModal', '수정', '수정 하시겠습니까?', 1); return false;" />

            <asp:Button ID="ButtonCancel" runat="server"
                CssClass="btn btn-outline-danger btn-lg ms-2"
                Style="width:160px;" Text="대회취소"
                OnClientClick="ValidateCheck('#MainModal', '취소', '대회를 취소 하시겠습니까?', 2); return false;" />
        <% } %>
    </div>


    <!-- 유효성검사 -->
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
        runat="server"
        ErrorMessage="개최명은 필 수 입력 항목입니다." 
        ControlToValidate="TB_GameName"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame"/>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator_Stadium" 
        runat="server"
        ErrorMessage="경기장은 필 수 선택 항목입니다." 
        ControlToValidate="DDL_Stadium"
        InitialValue="" 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame" />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
        runat="server"
        ErrorMessage="주최는 필 수 입력 항목입니다." 
        ControlToValidate="TB_GameHost"
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame"/>
    <asp:RequiredFieldValidator 
        ID="RequiredFieldValidator_PlayMode" 
        runat="server" 
        ControlToValidate="rblPlayMode"
        InitialValue=""
        ErrorMessage="경기 방식을 선택해 주세요!" 
        Display="None" 
        ForeColor="red" 
        ValidationGroup="NewGame" />
    <asp:ValidationSummary ID="ValidationSummary_SignupForm"
        ShowMessageBox="true" 
        ShowSummary="false"
        HeaderText="다음 사항을 확인하여 주십시오."
        runat="server" ValidationGroup="NewGame" />

    <!-- Modal -->
    <div class="modal fade" id="MainModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    저장 하시겠습니까?
                </div>
                <div class="modal-footer">
                    <button id="BTN_No" type="button" class="btn btn-secondary" data-bs-dismiss="modal" aria-label="Close" >아니오</button>
                    <asp:Button ID="BTN_Save" runat="server" OnClick="BTN_Save_Click" class="btn btn-primary" Text="예" />
                    <asp:Button ID="BTN_Update" runat="server" OnClick="BTN_Update_Click" class="btn btn-primary" Text="예" />
                    <asp:Button ID="BTN_Cancel" runat="server" OnClick="BTN_Cancel_Click" class="btn btn-primary" Text="예" />
                    <button id="BTN_Completed" type="button" class="btn btn-secondary" data-bs-dismiss="modal" onclick="location.href='/Sites/Admin/GameList.aspx'" >확인</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
