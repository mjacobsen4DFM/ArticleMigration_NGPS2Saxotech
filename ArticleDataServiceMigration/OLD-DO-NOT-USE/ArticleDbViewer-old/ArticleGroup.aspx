<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ArticleGroup.aspx.vb" Inherits="ArticleDbViewer.ArticleGroup" Theme="Theme1" %>
<%@ Register TagPrefix="UserControl" TagName="BreadCrumb" Src="~/controls/BreadCrumbTrail.ascx" %> 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Article Group</title>
</head>
<body>
<%--    <UserControl:BreadCrumb runat="server" />
--%>    <form id="form1" runat="server">
    
    <div class="box" >
            <center>
            <table border="1" >
            <tr>
                <td class="blue">session id</td>
                <td><asp:Label id="lblsessionid" runat="server" ></asp:Label></td>
            </tr>
                
            <tr>
                <td class="blue">Publication</td>
                <td><asp:Label runat="server" ID="lblpub" ></asp:Label></td>        
            </tr>
            <tr>
            <td class="blue">Category</td>
            <td>
                <asp:DropDownList ID="DrpCategory" runat="server">
                </asp:DropDownList>
            </td>
            </tr>
            <tr>
            <td class="blue">Start Date
            </td>
            <td>
                <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
                <asp:regularexpressionvalidator ControlToValidate="txtStartDate" ErrorMessage="Enter a valid start date e.g (yyyy-mm-dd)" id="RegularExpressionValidator1" runat="server" ValidationExpression="(19|20|21)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01])"  ></asp:regularexpressionvalidator>

            </td>
            </tr>
            <tr>
            <td class="blue">End Date
            </td>
            <td>
                <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>
               <asp:regularexpressionvalidator ControlToValidate="txtEndDate" ErrorMessage="Enter a valid end date e.g (yyyy-mm-dd)" id="RegularExpressionValidator3" runat="server" ValidationExpression="(19|20|21)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01])"  ></asp:regularexpressionvalidator>
            </td>
            </tr>
            <tr>

            <td colspan="2">
                <asp:Button ID="btnSubmit" runat="server" Text="Articles" width="100px"/>
                <asp:Button ID="btnGalleries" runat="server" Text="Galleries" width="100px" />
                <asp:Button ID="btnChangeSite" runat="server" Text="Change Pubication" />
            </td>
            </tr>
            <tr>
            <td colspan="7">
            <asp:validationsummary id="ValSummary" runat="server" width="200px" height="30px"></asp:validationsummary>
            </td>
            </tr>
        </table>
            </center>
    </div>
    </form>
</body>
</html>
