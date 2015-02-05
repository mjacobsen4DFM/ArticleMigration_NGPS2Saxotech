<%@ Page Language="VB" AutoEventWireup="false" CodeFile="articleview2.aspx.vb" Inherits="articleview2" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script src="js/Myadmin.js" type="text/javascript">

        var theForm = document.forms['form1'];
        if (!theForm) {
            theForm = document.form1;
        }

        function __doPostBack(eventTarget, eventArgument) {
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.__EVENTTARGET.value = eventTarget;
                theForm.__EVENTARGUMENT.value = eventArgument;
                theForm.submit();
            }
        }

    </script>
    <script language="javascript" type="text/javascript">


            function showSelection(ans) {
            try {
                var w = 440;
                var h = 580;
                var winl = (screen.width - w) / 2;
                var wint = (screen.height - h) / 2;
                winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + ',resizable=1,toolbar=0,menubar=0,scrollbars=0';
                try { windowHandler.close(); } catch (e) { }
                windowHandler = window.open('SelectAll.aspx?selected=' + ans, '', winprops, true);
                windowHandler.focus();
            } catch (e) {
                alert('showSelection' + e);
            }
        }


        $(function () {
            $("#SelectAll").click(function (e) {
                ans = "N";
                if ($('#SelectAll').prop('checked')) {
                    ans = "Y";
                }
                showSelection(ans);
            }); // SelectAll
        });     // end function



        $(document).ready(function () {
            $(".checkout").click(function (e) {
                e.preventDefault();
                id = $(this).attr('id');
                // alert("check out: " + id );
                $.post("Checkout.aspx", { article_uid: id }, function (data) {
                    // alert("Check Out Status: " + data);
                    location.reload();
                }); // post
            }); // checkout
        }); // document ready

        $(document).ready(function () {
            $(".checkin").click(function (e) {
                e.preventDefault();
                id = $(this).attr('id');
                $.post("Checkin.aspx", { article_uid: id }, function (data) {
                    // alert("Check In Status: " + data);
                    location.reload();
                }); // post
            }); // checkout
        }); // document ready

        $(document).ready(function () {
            $(".ArticleSelect").click(function (e) {
                e.preventDefault();
                id = $(this).attr('id');
                $.post("ArticleSelect.aspx", { article_uid: id }, function (data) {
                    // alert("Check In Status: " + data);
                    location.reload();
                }); // post
            }); // checkout
        }); // document ready

        $(document).ready(function () {
            $(".migrate").click(function (e) {
                e.preventDefault();
                id = $(this).attr('id');
                bEdited = 'False';
                $.post("migrate.aspx", { article_uid: id }, function (data) {
                    // alert("Migrate Status: " + data);
                    location.reload();
                }); // post
            }); // checkout
        }); // document ready
    

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <mycontrol:info ID="Info1" runat="server" />
        <div style="float: right; margin-top: 5px;">
            <%--<span>Total Rows:
                <%= gvArticles.Rows.Count%></span> --%><span>Page:</span>
            <asp:DropDownList ID="DropDownPage" runat="server" AutoPostBack="true">
            </asp:DropDownList>
        </div>
        <br />
        <asp:Panel ID="pnlGrid" runat="server" Width="1024">
            <asp:Panel ID="Panel1" runat="server" Width="1024"  HorizontalAlign="Left" Wrap="false">
                <input id="SelectAll" type="checkbox" runat="server" />Select All Articles, from all pages, for migration
<%--                <asp:ImageButton ID="btnSelectAll" ImageUrl="images/checkbox-open.png" runat="server"
                    Height="18" ImageAlign="Left" />
                <asp:Label ID="lblSelectAll" runat="server" align="left" Text="Select All Articles, from all pages, for migration" />
--%>            </asp:Panel>
            <asp:GridView ID="gvArticles" runat="server" AutoGenerateColumns="False" BackColor="White"
                BorderColor="#E7E7FF" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3" GridLines="Horizontal"
                AllowPaging="True" PageSize="25" Wrap="false" Width="1024">
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" HorizontalAlign="Left" />
                <Columns>
                    <asp:TemplateField HeaderText="Select">
                        <ItemTemplate>
                            <a href="#" id='<%# eval("article_uid")%>' class="ArticleSelect">
                                <asp:Panel ID="pnlChecked" runat="server" Visible="false">
                                    <img src="images/checkbox-checked.png" height="18" border="0" alt="Select Article for migration" />
                                </asp:Panel>
                                <asp:Panel ID="pnlOpen" runat="server" Visible="false">
                                    <img src="images/checkbox-open.png" height="18" border="0" alt="Eliminate Article from migration" />
                                </asp:Panel>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Check In/Out">
                        <ItemTemplate>
                            <asp:Panel ID="pnlCheckOut" runat="server">
                                <a href="#" id='<%# eval("article_uid")%>' class="checkout">
                                    <img src="images/checkout.png" height="38" border="0" alt="Checkout" /></a>
                            </asp:Panel>
                            <asp:Panel ID="pnlCheckIn" runat="server">
                                <a href="#" id='<%# eval("article_uid")%>' class="checkin">
                                    <img src="images/checkin.png" height="38" border="0" alt="Checkin" /></a></a>
                            </asp:Panel>
                            <asp:Panel ID="pnlwho" runat="server">
                                <asp:Label ID="lblUsername" runat="server" Text=""></asp:Label>
                            </asp:Panel>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150px" />
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" ForeColor="snow" Width="150px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="article_uid" HeaderText="CID" ReadOnly="True" InsertVisible="False" />
                    <asp:BoundField DataField="startdate" HeaderText="Start Date" ReadOnly="True" />
                    <asp:BoundField DataField="category" HeaderText="Category" ReadOnly="True" />
                    <asp:BoundField DataField="heading" HeaderText="Headline" ReadOnly="True" />
                    <asp:BoundField DataField="imagecount" HeaderText="Images" ReadOnly="True" />
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                <AlternatingRowStyle BackColor="#F7F7F7" />
            </asp:GridView>
            <i>Viewing Page
                <%=gvArticles.PageIndex + 1 %>
                of
                <%= gvArticles.PageCount%>
            </i>
        </asp:Panel>
    </center>
    </form>
</body>
</html>
