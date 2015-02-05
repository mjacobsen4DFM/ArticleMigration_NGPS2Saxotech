<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SectionAnchorRules2.aspx.vb"
    Inherits="SectionAnchorRules2" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <div>
            <mycontrol:info ID="Info1" runat="server" />
            <div>
                <iframe src="Profile.aspx" frameborder="1" height="150" width="80%" scrolling="no">
                </iframe>
            </div>
<%--            <div style="visibility:hidden">
                <asp:FormView ID="FormView1" runat="server" DataKeyNames="id" DataSourceID="DsrcSectionMap"
                    DefaultMode="Insert">
                    <EditItemTemplate>
                        id:
                        <asp:Label ID="idLabel1" runat="server" Text='<%# Eval("id") %>' />
                        <br />
                        siteid:
                        <asp:TextBox ID="siteidTextBox" runat="server" Text='<%# Bind("siteid") %>' />
                        <br />
                        sectionAnchor:
                        <asp:TextBox ID="sectionAnchorTextBox" runat="server" Text='<%# Bind("sectionAnchor") %>' />
                        <br />
                        sectionName:
                        <asp:TextBox ID="sectionNameTextBox" runat="server" Text='<%# Bind("sectionName") %>' />
                        <br />
                        ProfileId:
                        <asp:TextBox ID="ProfileIdTextBox" runat="server" 
                            Text='<%# Bind("ProfileId") %>' />
                        <br />
                        ts:
                        <asp:TextBox ID="tsTextBox" runat="server" Text='<%# Bind("ts") %>' />
                        <br />
                        ProfileId2:
                        <asp:TextBox ID="ProfileId2TextBox" runat="server" 
                            Text='<%# Bind("ProfileId2") %>' />
                        <br />
                        <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                            Text="Update" />
                        &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False"
                            CommandName="Cancel" Text="Cancel" />
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        id:
                        <asp:TextBox ID="idTextBox" runat="server" Text='<%# Bind("id") %>' />
                        <br />
                        siteid:
                        <asp:TextBox ID="siteidTextBox" runat="server" Text='<%# Bind("siteid") %>' />
                        <br />
                        sectionAnchor:
                        <asp:TextBox ID="sectionAnchorTextBox" runat="server" 
                            Text='<%# Bind("sectionAnchor") %>' />
                        <br />
                        sectionName:
                        <asp:TextBox ID="sectionNameTextBox" runat="server" 
                            Text='<%# Bind("sectionName") %>' />
                        <br />
                        ProfileId:
                        <asp:TextBox ID="ProfileIdTextBox" runat="server" 
                            Text='<%# Bind("ProfileId") %>' />
                        <br />
                        ts:
                        <asp:TextBox ID="tsTextBox" runat="server" Text='<%# Bind("ts") %>' />
                        <br />
                        ProfileId2:
                        <asp:TextBox ID="ProfileId2TextBox" runat="server" 
                            Text='<%# Bind("ProfileId2") %>' />
                        <br />
                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                            CommandName="Insert" Text="Insert" />
                        &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" 
                            CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                    </InsertItemTemplate>
                    <ItemTemplate>
                        id:
                        <asp:Label ID="idLabel" runat="server" Text='<%# Eval("id") %>' />
                        <br />
                        siteid:
                        <asp:Label ID="siteidLabel" runat="server" Text='<%# Bind("siteid") %>' />
                        <br />
                        sectionAnchor:
                        <asp:Label ID="sectionAnchorLabel" runat="server" Text='<%# Bind("sectionAnchor") %>' />
                        <br />
                        sectionName:
                        <asp:Label ID="sectionNameLabel" runat="server" Text='<%# Bind("sectionName") %>' />
                        <br />
                        ProfileId:
                        <asp:Label ID="ProfileIdLabel" runat="server" Text='<%# Bind("ProfileId") %>' />
                        <br />
                        ts:
                        <asp:Label ID="tsLabel" runat="server" Text='<%# Bind("ts") %>' />
                        <br />
                        ProfileId2:
                        <asp:Label ID="ProfileId2Label" runat="server" 
                            Text='<%# Bind("ProfileId2") %>' />
                        <br />
                        <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit"
                            Text="Edit" />
                        &nbsp;<asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete"
                            Text="Delete" />
                        &nbsp;<asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" CommandName="New"
                            Text="New" />
                    </ItemTemplate>
                </asp:FormView>
            </div>
--%>            <div>
                <span>Publication</span>
                <asp:Label ID="lblPub" runat="server" Text=""></asp:Label>
            </div>
            <div>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id"
                    DataSourceID="DsrcSectionMap" AllowPaging="true" PageSize="10">
                    <Columns>
                        <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" EditImageUrl="~/images/edit.png"
                            DeleteImageUrl="~/images/delete.png" />

                        <asp:BoundField DataField="id" HeaderText="id" ReadOnly="True" SortExpression="id" />
                        <asp:BoundField DataField="siteid" HeaderText="siteid" SortExpression="siteid" />
                        <asp:BoundField DataField="sectionAnchor" HeaderText="sectionAnchor" SortExpression="sectionAnchor" />
                        <asp:BoundField DataField="sectionName" HeaderText="sectionName" SortExpression="sectionName" />
                        <asp:BoundField DataField="ProfileId" HeaderText="ProfileId" SortExpression="ProfileId" />
                        <asp:BoundField DataField="ProfileId2" HeaderText="ProfileId2" 
                            SortExpression="ProfileId2" />
                    </Columns>
                    <RowStyle BackColor="#DEDFDE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                    <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />
                </asp:GridView>
            </div>
            <div>
                Detail<asp:CheckBox ID="chkProfileDetail" runat="server" AutoPostBack="true" /> &nbsp;
                SectionMap Complete <asp:CheckBox ID="chkMapcomplete" runat="server" AutoPostBack="true" />
            </div>
        </div>
    </center>
    <asp:EntityDataSource ID="DsrcSectionMap" runat="server" ConnectionString="name=ArticleDataDbEntities"
        DefaultContainerName="ArticleDataDbEntities" EnableDelete="True"
        EnableInsert="True" EnableUpdate="True" EntitySetName="sectionAnchorMaps" EntityTypeFilter="sectionAnchorMap"
        Where="it.siteid = @siteid">
        <WhereParameters>
            <asp:Parameter Name="siteid" Type="Int32" />
        </WhereParameters>
    </asp:EntityDataSource>
    <br />
    </form>
</body>
</html>
