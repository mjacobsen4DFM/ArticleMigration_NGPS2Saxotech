<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="css/StyleSheet.css" />
    <script src="js/Myscript.js" type="text/javascript"></script>
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
    <form id="aspnetForm" runat="server">
    <div style="position: relative; left: 50px;">
        <table>
            <tr>
                <th>
                    Site Code
                </th>
                <td>
                    <asp:DropDownList ID="DrpSite" runat="server">
                    </asp:DropDownList>
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
                <td colspan="2" align="center">
                    <input id="processButton" name="processButton" onclick="process(); " size="16" style="width: 100px;" type="button" value="Start" />
                </td>
            </tr>
        </table>
    </div>
    <div style="position: relative; left: 50px; width: auto;">
        <iframe id="view" class="view" frameborder="1" name="view" width="600px" height="400px" 
        scrolling="auto" style="display:block;">Sorry your browser doesn't support iframes</iframe>


<%--        <textarea id="txtlog" name="txtlog" rows="25" cols="100" runat="server" style="background:black;
            color: Yellow;" class="boxsizingBorder"></textarea>
--%>    </div>
    </form>
</body>
</html>
