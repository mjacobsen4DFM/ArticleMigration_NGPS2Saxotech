<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SectionAnchorRuleEntity.aspx.vb"
    Inherits="admin_SectionAnchorRuleEntity" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
        <asp:GridView ID="gridSectionAnchor" runat="server" AutoGenerateColumns="False" ShowFooter="True"
            CssClass="grid" OnRowCommand="gridSectionAnchor_RowCommand" DataKeyNames="id"
            CellPadding="4" ForeColor="#333333" GridLines="None" OnRowCancelingEdit="gridSectionAnchor_RowCancelingEdit"
            OnRowEditing="gridSectionAnchor_RowEditing" OnRowUpdating="gridSectionAnchor_RowUpdating"
            OnRowDataBound="gridSectionAnchor_RowDataBound" OnRowDeleting="gridSectionAnchor_RowDeleting">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" Text="" CommandName="Edit" ToolTip="Edit"
                            CommandArgument=''><img src="../images/edit.png" width="30px" /></asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CommandName="Delete"
                            ToolTip="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");'
                            CommandArgument=''><img src="../images/delete.png"  width="30px" /></asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkInsert" runat="server" Text="" ValidationGroup="editGrp" CommandName="Update"
                            ToolTip="Save" CommandArgument=''><img src="../Images/add.png" width="30px" /></asp:LinkButton>
                        <asp:LinkButton ID="lnkCancel" runat="server" Text="" CommandName="Cancel" ToolTip="Cancel"
                            CommandArgument=''><img src="../Images/cancel.png"  width="30px" /></asp:LinkButton>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:LinkButton ID="lnkInsert" runat="server" Text="" ValidationGroup="newGrp" CommandName="InsertNew"
                            ToolTip="Add New Entry" CommandArgument=''><img src="../Images/add.png" width="30px" /></asp:LinkButton>
                        <asp:LinkButton ID="lnkCancel" runat="server" Text="" CommandName="CancelNew" ToolTip="Cancel"
                            CommandArgument=''><img src="../images/cancel.png" width="30px" /></asp:LinkButton>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Section Anchor">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtSectionAnchor" runat="server" Text='<%# Bind("sectionanchor") %>'
                            CssClass="" MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valSectionAnchor" runat="server" ControlToValidate="txtSectionAnchor"
                            Display="Dynamic" ErrorMessage="Section anchor is required." ForeColor="Red"
                            SetFocusOnError="True" ValidationGroup="editGrp">*</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblSectionAnchor" runat="server" Text='<%# Bind("sectionanchor") %>'></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtSectionAnchorNew" runat="server" CssClass="" MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valSectionAnchorNew" runat="server" ControlToValidate="txtSectionAnchorNew"
                            Display="Dynamic" ErrorMessage="Section anchor is required." ForeColor="Red"
                            SetFocusOnError="True" ValidationGroup="newGrp">*</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Section Name">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtSectionName" runat="server" Text='<%# Bind("sectionName") %>'
                            CssClass="" MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valSectionName" runat="server" ControlToValidate="txtSectionName"
                            Display="Dynamic" ErrorMessage="Section Name is required." ForeColor="Red" SetFocusOnError="True"
                            ValidationGroup="editGrp">*</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblSectionName" runat="server" Text='<%# Bind("sectionName") %>'></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtSectionNameNew" runat="server" CssClass="" MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valSectionNameNew" runat="server" ControlToValidate="txtSectionNameNew"
                            Display="Dynamic" ErrorMessage="Section Name is required." ForeColor="Red" SetFocusOnError="True"
                            ValidationGroup="newGrp">*</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Taxonomy Code">
                    <EditItemTemplate>
                        <asp:TextBox ID="txttaxonony" runat="server" Text='<%# Bind("taxonomyCode") %>' CssClass=""
                            MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valtaxonony" runat="server" ControlToValidate="txttaxonony"
                            Display="Dynamic" ErrorMessage="taxonony is required." ForeColor="Red" SetFocusOnError="True"
                            ValidationGroup="editGrp">*</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="valtaxononyreg" runat="server" ErrorMessage="Invalid taxonomy"
                            ValidationGroup="editGrp" SetFocusOnError="true" Display="Dynamic" ControlToValidate="txttaxonomy"
                            ForeColor="Red">*</asp:RegularExpressionValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbltaxonony" runat="server" Text='<%# Bind("taxonomyCode") %>'></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txttaxononyNew" runat="server" CssClass="" MaxLength="30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valtaxononyNew" runat="server" ControlToValidate="txttaxononyNew"
                            Display="Dynamic" ErrorMessage="taxonony is required." ForeColor="Red" SetFocusOnError="True"
                            ValidationGroup="newGrp">*</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="valtaxononyNewReq" runat="server" ErrorMessage="Invalid taxonony"
                            ValidationGroup="newGrp" SetFocusOnError="true" Display="Dynamic" ControlToValidate="txttaxononyNew"
                            ForeColor="Red">*</asp:RegularExpressionValidator>
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Left" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
