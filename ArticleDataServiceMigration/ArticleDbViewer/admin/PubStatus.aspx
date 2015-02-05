<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PubStatus.aspx.vb" Inherits="admin_PubStatus"
    ValidateRequest="false" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <center>
    <div>
                <mycontrol:info ID="Info1" runat="server" />
    </div>

         <table>
         <tr>
         <td>
         Publication
         </td>
         <td>
              <asp:DropDownList ID="DrpPub" runat="server" ></asp:DropDownList>
         </td>
         <td colsapn="2">
             <asp:Button ID="btnsubmit" runat="server" Text="Submit" />
         </td>
         </tr>
         </table>
        
        <div id="mydata" runat="server">
        </div>
    </center>
    </form>
</body>
</html>
