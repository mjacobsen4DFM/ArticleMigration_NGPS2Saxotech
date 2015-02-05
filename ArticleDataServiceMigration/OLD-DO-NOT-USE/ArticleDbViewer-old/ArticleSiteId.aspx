<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ArticleSiteId.aspx.vb"
    Inherits="ArticleDbViewer.ArticleSiteId" Theme="Theme1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Article Publication</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <center>
        <table border="1">
            <tr>
                <td class="blue">session id</td>
                <td><asp:Label id="lblsessionid" runat="server" ></asp:Label></td>
            </tr>
            <tr>
                <td class="blue">
                    Publication
                </td>
                <td>
                    <asp:DropDownList ID="DrpSite" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnSubmit" runat="server" Text="Next" Width="100px" />
                </td>
            </tr>
        </table>
        </center>
    </div>
    </form>
</body>
</html>
