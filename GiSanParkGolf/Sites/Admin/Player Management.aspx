<%@ Page Title="선수정보 관리" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Player Management.aspx.cs" 
    Inherits="GiSanParkGolf.Sites.Admin.Player_Management" EnableEventValidation="false" %>

<%@ Register 
    Src="~/Controls/PagingControl.ascx" 
    TagPrefix="uc1" TagName="PagingControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /*클릭한적 없는*/
        .HyperLink:link {
	        color:blue; 
            text-decoration:none;
        }
        /*한번이상 클릭*/
        .HyperLink:visited {
	        color:blue; 
            text-decoration:none;
        }
        /*마우스 오버*/
        .HyperLink:hover {
	        color:blue; 
            text-decoration:underline;
        }
        /*클릭순간*/
        .HyperLink:active {
	        color:blue; 
            text-decoration:none;
        }
    </style>
     <link href="/Class/StyleSheet.css?after" rel="stylesheet"/>

    <div>
        <div id="PlayList">
            <div>
                <asp:CheckBox ID="CheckBox1" runat="server" Text="승인대기만 보기" OnCheckedChanged="CheckBox_CheckedChanged" AutoPostBack="true" />
                <a>&emsp;&emsp;&emsp;&emsp;</a>
                <asp:TextBox ID="TB_Search" runat="server" placeholder="이름을 입력하여 검색"></asp:TextBox>
                <asp:Button ID="BTN_Search" runat="server" Text="검색" OnClick="BTN_Search_Click" />
            </div>      
            
            <hr />
            <div style="font-style: italic; text-align: right; font-size: 8pt;">
                Total Record:
                <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
            </div>
            <asp:Label ID="LabelResult" runat="server" Text="선택정보" Visible="false"></asp:Label>
            <h12>(선수이름을 클릭하여 수정)</h12><br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="UserId"
                CssClass="table table-bordered table-hover table-condensed table-striped table-responsive">
                <HeaderStyle HorizontalAlign="center" />
                <RowStyle HorizontalAlign="Center"/>
                <AlternatingRowStyle  />                            
                    <Columns>       
                        <asp:BoundField DataField="UserWClass" HeaderText="상태">
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:BoundField>    
                        <asp:BoundField DataField="UserId" HeaderText="ID">
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:BoundField>    
                        <asp:TemplateField HeaderText="이름">
                            <ItemTemplate>
                                <%--<asp:Button ID="PlayerButton" runat="server" 
                                    Text='<%# Eval("UserName") %>' 
                                    OnClick="MyButtonClick" CssClass="RowButton"/>--%>

                                <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink"
                                    NavigateUrl='<%# "~/Sites/Admin/Player Information.aspx?UserId=" + Eval("UserId") %>'>
                                    <%# Dul.StringLibrary.CutStringUnicode(Eval("UserName").ToString(), 30) %>
                                </asp:HyperLink>


                            </ItemTemplate>
                            <HeaderStyle Width="15%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="15%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:TemplateField>    
                        <asp:BoundField DataField="UserNumber" HeaderText="생년월일" DataFormatString="{0:d}">
                            <HeaderStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="10%" BorderStyle="Solid" BorderWidth="1px"/>
                        </asp:BoundField> 
                        <asp:BoundField DataField="UserNote" HeaderText="비고">
                            <HeaderStyle Width="55%" BorderStyle="Solid" BorderWidth="1px"/>
                            <ItemStyle Width="55%" BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="left"/>
                        </asp:BoundField> 
                    </Columns>
            </asp:GridView>
            <div>
            <div style="text-align: center;">
                <uc1:PagingControl runat="server"
                    ID="PagingControl" />
            </div>
            </div>
        </div>
    </div>
    <%--Text='<%# Bind("UserId") %>'--%> 
</asp:Content>
