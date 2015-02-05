<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ArticleEdit.aspx.vb" Inherits="ArticleEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            height: 61px;
        }
        .style2
        {
            width: 1015px;
        }
        .style3
        {
            height: 61px;
            width: 1015px;
        }
    </style>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" language="javascript">

        var myusername = '<% = GetUserName() %>';    // used to pass tracksession user to jquery postback field
        var myarticleuid = '<% = GetArticleuid() %>';  // 

//        function makeSafe() {
//            $('#mainform').style.visibility = 'hidden';
//            $('#TxtHeadline').val =  window.escape($('#TxtHeadline').val());
//            $('#TxtSummary').val = window.escape($('#TxtSummary').val());
//            $('#TxtBody').val = window.escape($('#TxtBody').val());
//            $('#txtbyline').val = window.escape($('#txtbyline').val());
//        }


        //jquery class=saveandsend used to assign multiple input tags to postback use id field to delineate type (save , send)
        $(function () {
            $(".saveandsend").click(function (e) {
                e.preventDefault();
                var myHeadline = window.escape($('#TxtHeadline').val());
                var mySummary = window.escape($('#TxtSummary').val());
                var myBody = window.escape($('#TxtBody').val());
                var myByLine = window.escape($('#txtbyline').val());
                var myprofileid = window.escape($('#TxtProfile').val());
                alert('profileid:' + myprofileid);
                id = $(this).attr('id');
                //                alert('headline: ' + myHeadline);
                //                alert('summary: ' + mySummary);
                //                alert('byline: ' + myByLine);
                //                alert('body: ' + myBody);
                $.post("SaveAndSend.aspx", { headline: myHeadline, summary: mySummary, body: myBody, byline: myByLine, username: myusername, article_uid: myarticleuid, action: id, profileid: myprofileid }, function (data) {
                    window.opener.location.reload();
                    window.close();
                    // alert("SaveAndSend Status: " + data);
                    // location.reload();
                }); // post
            }); // send
        });        // end function



    </script>
    <script src="js/Myadmin.js" type="text/javascript" language="javascript"></script>
</head>
<body style="height: 668px">
<%--     <form id="form1" runat="server" onsubmit="makeSafe();"> --%>
    <form id="form2" runat="server">
    <div id="mainform">
        <table>
            <asp:Panel ID="pnlForm" runat="server" Visible="true">
                <asp:PlaceHolder ID="PlaceHoldImages" runat="server">
                    <tr>
                        <th>
                            Images
                        </th>
                        <td align="center">
                            <asp:GridView ID="gvGalleries" runat="server" AutoGenerateColumns="false" BackColor="White"
                                BorderColor="#E7E7FF" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3" GridLines="Horizontal"
                                AllowPaging="true" PageSize="5">
                                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" HorizontalAlign="Left" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblmsg" runat="server" Text='<%#Container.DataItem%>' Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
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
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <th>
                        Profile
                    </th>
                    <td class="style2">
                        <asp:TextBox ID="TxtProfile" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th>
                        Headline
                    </th>
                    <td class="style2">
                        <asp:TextBox ID="TxtHeadline" runat="server" Height="57px" TextMode="MultiLine" Width="888px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th>
                        Summary
                    </th>
                    <td class="style2">
                        <asp:TextBox ID="TxtSummary" runat="server" Height="76px" TextMode="MultiLine" Width="888px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th class="style1">
                        FreeForm Source (not editable)
                    </th>
                    <td class="style3">
                        <asp:Label ID="lblFreeForm" runat="server" Width="890px" BorderWidth="1" BorderColor="Silver"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="style1">
                        FreeForm View
                    </th>
                    <td colspan="2">
                        <div id="viewform" runat="server" style="width: 888px; height: 200px; overflow: scroll">
                        </div>
                    </td>
                </tr>
                <tr>
                    <th class="style1">
                        Body
                    </th>
                    <td class="style3">
                        <asp:TextBox ID="TxtBody" runat="server" Height="499px" TextMode="MultiLine" Width="888px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th>
                        byline
                    </th>
                    <td class="style2">
                        <asp:TextBox ID="txtbyline" runat="server" Height="57px" TextMode="MultiLine" Width="888px"></asp:TextBox>
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <th>
                </th>
                <td align="center">
                    <asp:Label ID="lblmsg1" runat="server" Width="850px" Style="margin-left: 1px"></asp:Label>
                    <br />
                    <asp:Label ID="lblmsg2" runat="server" Width="850px" Style="margin-left: 1px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
<%--                    <input type="button" value="Close" onclick="javascript:parent.location.reload();javascript:window.close();"  style="width: 108px" />
--%>                    <input type="button" value="Close" onclick="javascript:window.opener.location.reload();javascript:window.close();"  style="width: 108px" />

                    <!-- removed postback button using jquery ajax methods on class saveandsend -->
<%--
                    <asp:Button ID="btnSave" runat="server" Text="Save" Width="108px" />
                    <asp:Button ID="btnMigrate" runat="server" Text="Save and Send" Width="108px" />
--%> 
                    <input class="saveandsend" id="save" name="save" type="button" value="Save"  style="width:108px" />
                    <input class="saveandsend" id="send" name="send" type="button" value="Save and Send"  style="width:108px" />
                </td>
            </tr>
        </table>
    </div>
    </form> 
</body>
</html>
