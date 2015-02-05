<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MigrateFromQA0.aspx.cs" Inherits="MigrateFromQA0" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.0/jquery.min.js" type="text/javascript"></script>
    <script src="js/migrate.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <mycontrol:info ID="Info1" runat="server" />
    </div>
    <center>
        <div>
            <table>
                <tr>
                    <td>
                        Publication
                    </td>
                    <td>
                        <asp:DropDownList ID="DrpPub" runat="server" class="mypub">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                <td align="left" >Start Date</td>
                <td>
                    <asp:TextBox ID="TxtStart" runat="server" class="mystart" ></asp:TextBox></td>
                </tr>
                <tr>
                <td  align="left">End Date</td>
                <td>
                    <asp:TextBox ID="txtEnd" runat="server" class="myend" ></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2">
<%--                     <input id="migratepub" type="button" value="submit" style="width: 100px;" />
--%>

                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                            onclick="btnSubmit_Click" />
</td>
                </tr>
            </table>
        </div>
        <iframe id="mytarget2" src="" class="mytarget" runat="server" width="500px" height="500px"></iframe>
        <div id="mytarget"  style="width:500px height:500px;" class="mytarget">
        </div>

    </center>
    </form>
</body>
</html>
