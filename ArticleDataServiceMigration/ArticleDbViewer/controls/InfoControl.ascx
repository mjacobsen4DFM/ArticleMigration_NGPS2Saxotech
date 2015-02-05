<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InfoControl.ascx.vb"
    Inherits="InfoControl" %>
<div style="background-color: #4A3C8C; color: White; width:100%;height:35px; font-size:30px; font-family:Times New Roman;" >
    Article Data Services
    <div style="float:right;"> 
            <img src="images/power.jpg" height="35px"  alt="" />
    </div>
</div>
<div>
    <table cellspacing="0" cellpadding="3" style="border-style: solid; border-width: 1px; border-color: Black; margin-top:5px;margin-bottom:5px;">
        <tr align="left">
            <asp:Panel ID="PnlPub" runat="server">
                <td style="background-color: #4A3C8C; color: White">
                    Publication
                </td>
                <td>
                    <asp:Label ID="lblPublication" runat="server" Text=""></asp:Label>
                </td>
            </asp:Panel>
            <asp:Panel ID="PnlCat" runat="server">
                <td style="background-color: #4A3C8C; color: White">
                    Category
                </td>
                <td>
                    <asp:Label ID="lblCategory" runat="server" Text=""></asp:Label>
                </td>
            </asp:Panel>
            <td style="background-color: #4A3C8C; color: White">
                Login
            </td>
            <td>
                <asp:Label ID="lbluser" runat="server" Text=""></asp:Label>
            </td>
            <asp:Panel ID="Pnlgroup" runat="server">
                <td style="background-color: White;">
                    <a id="A1" href="../articlegroup.aspx" runat="server">Home</a>
                </td>
            </asp:Panel>
            <asp:Panel ID="pnlAdmin" runat="server">
                <td style="background-color: White;">
                    <a id="A2" href="../admin/ArticleFlagRules.aspx" runat="server">Html Rules</a>
                </td>
                <td style="background-color: White;">
                    <a id="A3" href="../admin/SectionAnchorRules2.aspx" runat="server">SectionAnchor Rules</a>
                </td>
                <td style="background-color: White;">
                    <a id="A6" href="../admin/LoadSectionAnchorMap.aspx" runat="server">Load New SectionAnchor Map</a>
                </td>
                <td style="background-color: White;">
                    <a id="A7" href="../admin/MigratefromQA0.aspx" runat="server">MigrateFromQA0</a>
                </td>

                <td style="background-color: White;">
                    <a id="A4" href="../admin/Exception.aspx" runat="server">Exceptions</a>
                </td>
                <td style="background-color: White;">
                    <a id="A5" href="../admin/PubStatus.aspx" runat="server">Pub Status</a>
                </td>
            </asp:Panel>

            <asp:Panel ID="PnlLogout" runat="server">
                <td style="background-color: White;">
                    <a id="logoutlink" href="../logout.aspx" runat="server">Logout</a>
                </td>
            </asp:Panel>

        </tr>
    </table>
</div>
