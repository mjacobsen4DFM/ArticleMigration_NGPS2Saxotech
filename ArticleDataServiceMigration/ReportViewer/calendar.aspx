<%@ Page Language="VB" AutoEventWireup="false" CodeFile="calendar.aspx.vb" Inherits="calendar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal id="Literal1" runat="server"></asp:Literal>
        <asp:Calendar id="Calendar1" runat="server" 
                     OnSelectionChanged="Calendar1_SelectionChanged" 
                     OnDayRender="Calendar1_dayrender" 
                     ShowTitle="true" DayNameFormat="FirstTwoLetters" 
                     SelectionMode="Day" BackColor="#ffffff" 
                     FirstDayOfWeek="Sunday" BorderColor="#000000" 
                     ForeColor="#00000" Height="60" Width="120">
        <TitleStyle backcolor="Navy" forecolor="White" />
        <NextPrevStyle backcolor="Navy" forecolor="White" />
        <OtherMonthDayStyle forecolor="Silver" />
    </asp:Calendar>
    </div>
    </form>
</body>
</html>
