<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GalleryView.aspx.vb" Inherits="GalleryView"  %>
<%@ Register TagPrefix="mycontrol" TagName="info" src="~/controls/InfoControl.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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

      $(document).ready(function () {
          $(".mycheckbox").change(function () {
              id = $(this).attr('id');

              ary = id.split('_');
              ischecked = '';
              // if ($('.mycheckbox:checkbox:checked')) {
              if ($(this).prop('checked')) {
                  ischecked = 'Y';
              }
              else {
                  //checked to unchecked
                  ischecked = 'N';
              }
              $.post("MyCheckbox.aspx", { article_uid: ary[0], asset_uid: ary[1], checked: ischecked }, function (data) {
                  location.reload();
              }); // post

          });
      });

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

  </script>


</head>

<body>
    <form id="form1" runat="server">
    <center>
        <mycontrol:info ID="Info1" runat="server" />
        <div>
            <asp:GridView ID="gvGalleries" runat="server" AutoGenerateColumns="false" BackColor="White"
                BorderColor="#E7E7FF" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3" GridLines="Horizontal"
                AllowPaging="true" PageSize="100">
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" HorizontalAlign="Left" />
                <Columns>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Panel ID="pnlCheckOut" runat="server">
                                 <a href="#" id='<%# eval("article_uid")%>' class="checkout">Check Out</a>
                            </asp:Panel>
                            <asp:Panel ID="pnlCheckIn" runat="server">
                                 <a href="#"id='<%# eval("article_uid")%>' class="checkin">Check In</a>
                            </asp:Panel>
                            <asp:Panel ID="pnlwho" runat="server">
                                <asp:Label ID="lblUsername" runat="server" Text=""></asp:Label>
                            </asp:Panel>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150px" />
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" ForeColor="snow" Width="150px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="article_uid" HeaderText="article_uid" ReadOnly="True"
                        InsertVisible="False" />
                    <asp:BoundField DataField="heading" HeaderText="heading" ReadOnly="True" />
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
            <i>Viewing Page
                <%=gvGalleries.PageIndex + 1 %>
                of
                <%= gvGalleries.PageCount%>
            </i>
        </div>
    </center>
    </form>
</body>
</html>
