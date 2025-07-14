<%@ Page Title="인원 코스 및 배치" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameUserSetting.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameUserSetting" %>

<%@ Register Src="~/Controls/PagingControl.ascx" TagPrefix="uc1" TagName="PagingControl" %>
<%@ Register Src="~/Controls/SearchControl.ascx" TagPrefix="uc1" TagName="SearchControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-5" style="background-color:aliceblue; border-top-left-radius:1rem; border-bottom-left-radius:1rem;">
            <div class="row">
                <div style="text-align:left;">
                    <h4 style="color:cornflowerblue">대회인원 코스 및 배치</h4>
                    <p>대회명을 클릭하여 설정하여 주십시오.</p>
                </div>
            </div>
            <div class="row mb-1">
                <div>
                <uc1:SearchControl runat="server" ID="SearchControl" />
                </div>
            </div>
            <div class="row">
                <asp:GridView ID="GameList"
                    runat="server" AutoGenerateColumns="False" DataKeyNames="GameCode"
                    CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                    ShowHeaderWhenEmpty="true" >
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
                                <asp:LinkButton ID="LnkGame" runat="server" CssClass="HyperLink" OnClick="LnkGame_Click" CommandName="select" 
                                    ToolTip='<%#Eval("GameCode")%>'>
                                    <%#Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25)%>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                            <ItemStyle Width="200px" />
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
                    </Columns>
                    <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
                </asp:GridView>
            </div>
            <div class="row">
                <div style="font-style: italic; text-align: right; font-size: 8pt;">
                    Total Record:
                    <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
                </div>
                <div class="center_container">
                    <uc1:PagingControl runat="server" ID="PagingControl" />
                </div>
            </div>
        </div>

        <style>
            .input-group{
                text-align: center;
                width:100%;
            }
            .input-group-text{
                min-width: 20%;
                max-width: 20%;
                text-align: center;
            }
            .form-control{
                min-width: 30%;
                max-width: 30%;
                text-align: left;
            }
            .note{
                min-width: 80%;
                max-width: 80%;
                height: 300px;
            }
        </style>
        <script type="text/javascript"> 
            var showModalName; //모달이름
            var showTitle; //타이틀
            var showBody; //바디내용
            var onlyYes; // 예 버튼만 표시
            var showYesButton
            function ShowModal(modalname, title, body, yes, yesbutton) {
                showModalName = modalname;
                showTitle = title;
                showBody = body;
                onlyYes = yes;
                showYesButton = yesbutton;
                $(modalname).modal("show");
            }
            $(document).on('show.bs.modal', showModalName, function () {
                var modal = $(this);
                modal.find('.modal-title').text(showTitle);
                modal.find('.modal-body').text(showBody);
                if (onlyYes) {
                    modal.find('.modal-footer #MainContent_BTN_EarlyCloseYes').hide();
                    modal.find('.modal-footer #MainContent_BTN_PlayerCheckYes').hide();
                    modal.find('.modal-footer #MainContent_BTN_SettingYes').hide();
                    modal.find('.modal-footer #BTN_No').text("확인");
                    return;
                } else {
                    modal.find('.modal-footer #BTN_No').text("아니오");
                }
                switch (showYesButton){
                    case 0:
                        modal.find('.modal-footer #MainContent_BTN_EarlyCloseYes').show();
                        modal.find('.modal-footer #MainContent_BTN_PlayerCheckYes').hide();
                        modal.find('.modal-footer #MainContent_BTN_SettingYes').hide();
                        break;
                    case 1:
                        modal.find('.modal-footer #MainContent_BTN_EarlyCloseYes').hide();
                        modal.find('.modal-footer #MainContent_BTN_PlayerCheckYes').show();
                        modal.find('.modal-footer #MainContent_BTN_SettingYes').hide();
                        break;
                    case 2:
                        modal.find('.modal-footer #MainContent_BTN_EarlyCloseYes').hide();
                        modal.find('.modal-footer #MainContent_BTN_PlayerCheckYes').hide();
                        modal.find('.modal-footer #MainContent_BTN_SettingYes').show();
                        break;
                    default:
                        break;
                }
            })
            /*로딩을 완료하기 위해(안그러면 로딩이 안되어 모달 실행이 안된다.*/
            /*아래 두함수가 셋트로 움직여야 한다.*/
            var launch = false;
            function launchModal(modalname, title, body, yes, yesbutton) {
                showModalName = modalname;
                showTitle = title;
                showBody = body;
                onlyYes = yes;
                showYesButton = yesbutton;
                launch = true;
            }
            function pageLoad() {
                if (launch) {
                    $(showModalName).modal("show");
                }
            }
            function winPopUPCenter(url, winName, pwidth, pheight, scrollYN, resizeYN) {
                var win = null;
                var winL = (screen.width - pwidth) / 2;
                var winT = (screen.height - pheight) / 2;
                var spec = 'toolbar=no,'; // 도구메뉴
                spec += 'status=no,'; // 상태바
                spec += 'location=yes,'; // 주소관련메뉴
                spec += 'height=' + pheight + ','; // 높이
                spec += 'width=' + pwidth + ','; // 너비
                spec += 'top=' + winT + ','; // 세로위치
                spec += 'left=' + winL + ','; // 가로위치
                spec += 'scrollbars=' + (scrollYN == undefined ? "no" : scrollYN) + ','; // 스크롤바 여부(기본)
                spec += 'resizable=' + (resizeYN == undefined ? "no" : resizeYN); // 창크기조정 여부

                var gameCode = document.getElementById('MainContent_TB_GameCode').value;
                url += '?GameCode=' + gameCode;
                console.log(gameCode);
                win = window.open(url, winName, spec);
                if (parseInt(navigator.appVersion) >= 4) {
                    win.window.focus();
                }
                $(showModalName).modal("hide");
            }
        </script>

        <!-- Modal 시작 -->
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
                        <asp:Button ID="BTN_EarlyCloseYes" runat="server" Text="예" OnClick="BTN_EarlyCloseYes_Click" CssClass="btn btn-primary"/>
                        <asp:Button ID="BTN_PlayerCheckYes" runat="server" Text="예" CssClass="btn btn-primary"
                            onClientclick="winPopUPCenter('GameUserList.aspx', 'User List', 750, 800);return false;" />
                        <asp:Button ID="BTN_SettingYes" runat="server" Text="예" OnClick="BTN_SettingYes_Click" CssClass="btn btn-primary"/>
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal 끝 -->

        <div class="col" style="background-color:lightskyblue; border-top-right-radius:1rem; border-bottom-right-radius:1rem">
            <div style="text-align:left;">
                <h4 style="color:white">선택된 대회정보입니다.</h4>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">현재상태</span>
                <asp:TextBox ID="TB_GameStatus" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                <span class="input-group-text">대회코드</span>
                <asp:TextBox ID="TB_GameCode" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회명</span>
                <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                <span class="input-group-text">대회일자</span>
                <asp:TextBox ID="TB_GameDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회장소</span>
                <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                <span class="input-group-text">주최</span>
                <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">홀당 최대인원</span>
                <asp:TextBox ID="TB_HoleMaximum" runat="server" CssClass="form-control" TextMode="Number" Text="" Enabled="false"></asp:TextBox>
                <span class="input-group-text" style="background-color:blueviolet;color:white">참가인원</span>
                <asp:TextBox ID="TB_User" runat="server" CssClass="form-control" TextMode="Number" Text="" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">모집시작</span>
                <asp:TextBox ID="TB_StartDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                <span class="input-group-text">모집종료</span>
                <asp:TextBox ID="TB_EndDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">비고</span>
                <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control note" TextMode="MultiLine" Enabled="false"></asp:TextBox>
            </div>
            <div class="row">
                <div class="center_container">
                    <div class="btn-group" role="group" aria-label="Basic mixed styles example">
                        <button runat="server" id="BTN_EarlyClose" type="button" class="btn btn-danger" disabled 
                            onclick="ShowModal('#MainModal', '조기마감 확인', '모집을 조기마감 하시겠습니까?', false, 0);return false;">
                            조기마감
                        </button>
                        <button runat="server" id="BTN_PlayerCheck" type="button" class="btn btn-warning" disabled
                            onclick="ShowModal('#MainModal', '참가자확인', '참가자 리스트를 확인 하시겠습니까?', false, 1);return false;">
                            참가자 확인
                        </button>
                        <button runat="server" id="BTN_Setting" type="button" class="btn btn-success" disabled
                            onclick="ShowModal('#MainModal', '코스배치 확인', '선수 코스배치를 하시겠습니까?', false, 2);return false;">
                            선수 코스배치
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
