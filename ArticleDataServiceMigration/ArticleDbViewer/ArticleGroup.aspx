<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ArticleGroup.aspx.vb" Inherits="ArticleGroup"
    Theme="Theme1" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Article Group</title>
    <script type="text/javascript">
        function showDate(w, h, clientid) {
            left = (screen.width - w) / 2;
            tp = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + tp + ',left=' + left + 'resizable=0,toolbar=0,menubar=0,scrollbars=0';
            try { windowHandler.close(); } catch (e) { }
            windowHandler = window.open('calendar.aspx?ctlid=' + clientid, '', winprops, true);
            windowHandler.focus();
        }
    </script>
</head>
<body>
    <form id="frmMain" runat="server">
    <div class="box">
        <center>
            <mycontrol:info ID="Info1" runat="server" />
            <div>
                <asp:GridView ID="GridView1" runat="server">
                </asp:GridView>
                <asp:Label ID="lblsession" runat="server" Text="" ForeColor="White"></asp:Label>
            </div>
            <table border="1">
                <tr>
                    <th>
                        Publication
                    </th>
                    <td>
                        <asp:DropDownList ID="DrpSite" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <th>
                        Category
                    </th>
                    <td>
                        <asp:DropDownList ID="DrpCategory" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <th>
                        Articles
                    </th>
                    <td>
                        <asp:RadioButtonList ID="RadioList" runat="server" AutoPostBack="false" RepeatDirection="Horizontal">
                            <asp:ListItem Text="All" Value="A" />
                            <asp:ListItem Text="Held" Value="H" Selected="True" />
                            <asp:ListItem Text="Sent" Value="S" />
                            <asp:ListItem Text="Images" Value="I" />
                            <asp:ListItem Text="Gallery Editor" Value="G" />
                        </asp:RadioButtonList>
                        Explicit Logging<asp:CheckBox ID="chkLogging" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>
                        Hold Conditions
                    </th>
                    <td>
                        <asp:PlaceHolder ID="phHolds" runat="server" />
                        <input id="chkErrors" type="checkbox" name="chkErrors" />Errors
                    </td>
                </tr>
                <tr>
                    <th>
                        Start Date
                    </th>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
                        <a href="javascript:showDate(200,300,'<%=txtStartDate.ClientID %>');">
                            <img src="images/cal.jpg" style="width: 20px; height: 20px" alt="" />
                        </a>
                    </td>
                </tr>
                <tr>
                    <th>
                        End Date
                    </th>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>
                        <a href="javascript:showDate(200,300,'<%=txtEndDate.ClientID %>');">
                            <img src="images/cal.jpg" style="width: 20px; height: 20px" alt="" />
                        </a>
                    </td>
                </tr>
                <tr>
                    <th>
                        CID
                    </th>
                    <td>
                        <asp:TextBox ID="txtArticle" runat="server" Style="margin-left: 0px" Width="175px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" Width="100px" />
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td>
                        <asp:RegularExpressionValidator ControlToValidate="txtStartDate" ErrorMessage="Enter a valid start date e.g (yyyy-mm-dd)"
                            ID="RegularExpressionValidator1" runat="server" ValidationExpression="(19|20|21)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01])"></asp:RegularExpressionValidator>
                        <asp:RegularExpressionValidator ControlToValidate="txtEndDate" ErrorMessage="Enter a valid end date e.g (yyyy-mm-dd)"
                            ID="RegularExpressionValidator3" runat="server" ValidationExpression="(19|20|21)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01])"></asp:RegularExpressionValidator>
                    </td>
                </tr>
            </table>
        </center>
        <asp:Label ID="lblTest" runat="server" />
    </div>
    </form>
</body>
</html>
