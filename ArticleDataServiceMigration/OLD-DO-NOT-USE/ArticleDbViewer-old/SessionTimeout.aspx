<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SessionTimeout.aspx.vb"  Inherits="ArticleDbViewer.SessionTimeout" Theme="Theme1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Session Timeout</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <center>
            <table border="1" width="80%">
                <tr align="left">
                    <td class="TimeOutHead">
                        It appears that your browser is rejecting cookies or session time out
                    </td>
                </tr>
                <tr align="left">
                    <td class="Normal">
                        Once you have verified that you browser can accept cookies you may <a href="ArticleSiteId.aspx" class=""> CLICK HERE </a> to select Site
                    </td>
                </tr>
            </table>
        </center>
    </div>
    </form>
</body>
</html>
