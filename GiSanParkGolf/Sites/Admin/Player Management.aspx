<%@ Page Title="선수정보(관리자)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Player Management.aspx.cs" 
    Inherits="GiSanParkGolf.Sites.Admin.Player_Management" EnableEventValidation="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <link href="/Content/Site.css" rel="stylesheet"/>
    <div>
        <div id="PlayList">
            <div>
                <asp:Label ID="LB_Search" runat="server" Text="이름" Width="60px" Height="17px"></asp:Label>
                <asp:TextBox ID="TB_Search" runat="server" placeholder="이름을 입력하여 검색"></asp:TextBox>
                <asp:Button ID="BTN_Search" runat="server" Text="검색" />
                <br />
            </div>           

            <asp:Label ID="LabelResult" runat="server" Text="선택정보" Visible="false"></asp:Label>
            <h12>(선수이름을 클릭하여 수정)</h12><br />
            <asp:GridView ID="GridView1" runat="server" BorderWidth="1" Width="100%" BorderStyle="Solid"
                AutoGenerateColumns="false" ShowHeader="true" CellPadding="1" 
                CellSpacing="0" DataKeyNames="UserId" AllowPaging="True" 
                AllowSorting="True"  PageSize="10"   PagerStyle-HorizontalAlign="Center" 
                onpageindexchanging="grdList_PageIndexChanging">
                <HeaderStyle Width="80px" Height="75px" HorizontalAlign="center" BackColor="LightSlateGray" ForeColor="white" />
                <RowStyle Height="34px" HorizontalAlign="Center"/>
                <AlternatingRowStyle  />                            
                    <Columns>                                                   
                        <asp:BoundField DataField="UserId" HeaderText="ID">
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:BoundField>    
                        <asp:TemplateField HeaderText="이름">
                            <ItemTemplate>
                                <asp:Button ID="PlayerButton" runat="server" 
                                    Text='<%# Eval("UserName") %>' 
                                    OnClick="MyButtonClick" CssClass="RowButton"/>
                            </ItemTemplate>
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="15%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:TemplateField>    
                        <asp:BoundField DataField="UserBirthOfDate" HeaderText="생년월일" DataFormatString="{0:d}">
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="30%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:BoundField> 
                        <asp:BoundField DataField="UserNote" HeaderText="비고">
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="45%" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="left"/>
                        </asp:BoundField> 
                    </Columns>
            </asp:GridView>
        </div>
    </div>
    <%--Text='<%# Bind("UserId") %>'--%> 
</asp:Content>
