<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .rowc
        {
            color: White;
            background-color: #990000;
            font-weight: bold;
            text-align: center;
        }
        .viewarea
        {
            overflow: scroll;
            width: 800px;
            height: 800px;
        }
        .wrapper
        {
            left: 50px;
        }
    </style>
</head>
<script type="text/javascript">
    function showDate(w, h, target) {
        left = (screen.width - w) / 2;
        tp = (screen.height - h) / 2;
        winprops = 'height=' + h + ',width=' + w + ',top=' + tp + ',left=' + left + 'resizable=0,toolbar=0,menubar=0,scrollbars=0';
        try { windowHandler.close(); } catch (e) { }
        windowHandler = window.open('calendar.aspx?formname=' + target, '', winprops, true);
        windowHandler.focus();
    }
</script>
<body>
    <form name="frmCalendar" id="frmCalendar" runat="server">
    <div>
        <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label>
    </div>
    <div class="wrapper">
        <div>
            <table>
                <tr class="rowc">
                    <td valign="middle">
                        Start Date &nbsp;<input name="txtDate" id="txtDate" type="text" style="width: 75px;"
                            readonly runat="server" />
                    </td>
                    <td align="center" title="Date Selector">
                        <a href="javascript:showDate(200,300,'frmCalendar.txtDate');">
                            <img src="images/cal.jpg" alt="" style="width: 40x; height: 50px" />
                        </a>
                    </td>
                </tr>
                <tr class="rowc">
                    <td valign="middle">
                        End Date &nbsp;<input name="txtDate2" id="txtDate2" type="text" style="width: 75px;"
                            readonly runat="server" />
                    </td>
                    <td align="center" title="Date Selector">
                        <a href="javascript:showDate(200,300,'frmCalendar.txtDate2');">
                            <img src="images/cal.jpg" alt="" style="width: 40x; height: 50px" />
                        </a>
                    </td>
                </tr>
                <tr class="rowc">
                    <td>
                        Publication
                    </td>
                    <td>
                        <asp:DropDownList ID="DrpPub" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="viewarea" class="viewarea" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
