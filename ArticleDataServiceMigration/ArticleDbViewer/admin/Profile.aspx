<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Profile.aspx.vb" Inherits="admin_Profile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profiles</title>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <div>
            <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal"
                AutoPostBack="true">
                <asp:ListItem Text="Subject" Value="Subject" Selected></asp:ListItem>
                <asp:ListItem Text="Geography" Value="Geography"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div>
            <asp:Label ID="lblprofileTrail" runat="server" Text=""></asp:Label>
            <asp:DropDownList ID="DrpLevel" runat="server">
            </asp:DropDownList>
            <asp:Button ID="btnPrevious" runat="server" Text="Previous" Width="100px" />
            <asp:Button ID="BtnNext" runat="server" Text="Next" Width="100px" />
            <asp:Button ID="BtnSelect" runat="server" Text="Select" Width="100px" />
        </div>
        <asp:Panel ID="pnlSelected" runat="server">
            <div>
                <table border="1">
                    <tr>
                        <td>
                            Profile Name
                        </td>
                        <td>
                            <asp:Label ID="lblprofilename" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Profile Id
                        </td>
                        <td>

                            <asp:Label ID="lblprofileId" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </center>
    </form>
</body>
</html>
