<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ArticleView.aspx.vb" Inherits="ArticleView" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="js/Myadmin.js" type="text/javascript"> 
        var theForm = document.forms['form1'];
        if (!theForm) {
            theForm = document.form1;
        }
        function __doPostBack(eventTarget, eventArgument) {
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
            theForm.__EVENTTARGET.value = eventTarget;
            theForm.__EVENTARGUMENT.value = eventArgument;
            theForm.submit();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <mycontrol:info ID="Info1" runat="server" />
        <div style="float:right;margin-top:5px;">
            <span>Total Rows: <%= gvArticles.Rows.Count%></span>
            <span>Page:</span>
            <asp:DropDownList ID="DropDownPage" runat="server" AutoPostBack="true">
            </asp:DropDownList>
        </div>
        <div>
            <asp:GridView ID="gvArticles" runat="server" AutoGenerateColumns="false" BackColor="White"
                BorderColor="#E7E7FF" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3" GridLines="Horizontal"
                AllowPaging="true" PageSize="25">
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" HorizontalAlign="Left" />
                <Columns>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Panel ID="pnlCheckOut" runat="server">
                                <a href="#" onclick="javascript:HttpActionPost('CheckOut.aspx?article_uid=<%# eval("article_uid")%>','')">
                                    Check Out</a>
                            </asp:Panel>
                            <asp:Panel ID="pnlCheckIn" runat="server">
                                <a href="#" onclick="javascript:HttpActionPost('CheckIn.aspx?article_uid=<%# eval("article_uid")%>','' )">
                                    Check In</a>
                            </asp:Panel>
                            <asp:Panel ID="pnlwho" runat="server">
                                <asp:Label ID="lblUsername" runat="server" Text=""></asp:Label>
                            </asp:Panel>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150px" />
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" ForeColor="snow" Width="150px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="article_uid" HeaderText="article_uid" ReadOnly="True"
                        InsertVisible="False" />
                    <asp:BoundField DataField="startdate" HeaderText="startdate" ReadOnly="True" />
                    <asp:BoundField DataField="category" HeaderText="category" ReadOnly="True" />
                    <asp:BoundField DataField="heading" HeaderText="heading" ReadOnly="True" />
                    <asp:BoundField DataField="imagecount" HeaderText="imagecount" ReadOnly="True" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                <AlternatingRowStyle BackColor="#F7F7F7" />
            </asp:GridView>
            <i>Viewing Page
                <%=gvArticles.PageIndex + 1 %>
                of
                <%= gvArticles.PageCount%>
            </i>
        </div>
    </center>
    </form>
</body>
</html>
