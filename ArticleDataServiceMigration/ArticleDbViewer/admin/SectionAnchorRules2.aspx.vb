
Imports DataModels

Partial Class SectionAnchorRules2
    Inherits WebFrontEnd

    'Protected Sub FormView1_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertedEventArgs) Handles FormView1.ItemInserted
    '    GridView1.DataBind()
    'End Sub


    Protected Sub SectionAnchorRules2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim siteid As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.siteid))
            Dim publicationName As String = mytracksession.GetValue(MySession.tracksession.keycodes.publicationName)
            lblPub.Text = publicationName
            DsrcSectionMap.WhereParameters("siteid").DefaultValue = siteid
            GridView1.DataBind()
            Dim ans As String = BusinessRule.BusinessRule.PubMapStatus(siteid)
            chkMapcomplete.Checked = IIf(ans.Equals("Y"), True, False)
        End If
    End Sub


    Private Function GetColumnIndexByHeaderText(ByVal gv As GridView, ByVal columnText As String) As Integer
        Dim cell As TableCell
        For n = 0 To gv.HeaderRow.Cells.Count - 1
            cell = gv.HeaderRow.Cells(n)
            If (cell.Text.ToString.Equals(columnText)) Then
                Return n
            End If
        Next
        Return -1
    End Function


    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound

        If (e.Row.RowType = DataControlRowType.DataRow And chkProfileDetail.Checked) Then
            For Each colname As String In {"ProfileId", "ProfileId2"}
                Dim index As Integer = GetColumnIndexByHeaderText(GridView1, colname)
                Dim strId As String = e.Row.Cells(index).Text
                If (IsNumeric(strId) AndAlso Convert.ToInt32(strId) > 0) Then
                    Dim id As Integer = Convert.ToInt32(strId)
                    Dim p As List(Of profile) = BusinessRuleC.BusinessRule.GetProfile(id)
                    If p.Count > 0 Then
                        e.Row.Cells(index).Text = String.Format("{0}  ({1}) ", e.Row.Cells(index).Text, p.Item(0).fieldname)
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub chkProfileDetail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkProfileDetail.CheckedChanged
        Dim nop As String = ""
        Dim currentpage As Integer = GridView1.PageIndex
        Dim currentpagesize As Integer = GridView1.PageSize
        Dim idx As Integer = GridView1.Rows.Count
        Dim row As GridViewRow
        For Each row In GridView1.Rows
            For Each colname As String In {"ProfileId", "ProfileId2"}
                Dim index As Integer = GetColumnIndexByHeaderText(GridView1, colname)
                Dim strId As String = row.Cells(index).Text.Trim.Split(" ")(0)
                If (IsNumeric(strId) AndAlso Convert.ToInt32(strId) > 0) Then
                    Dim id As Integer = Convert.ToInt32(strId)
                    If (chkProfileDetail.Checked) Then
                        Dim p As List(Of profile) = BusinessRuleC.BusinessRule.GetProfile(id)
                        If p.Count > 0 Then row.Cells(index).Text = String.Format("{0}  ({1}) ", row.Cells(index).Text, p.Item(0).fieldname)
                    Else
                        row.Cells(index).Text = row.Cells(index).Text.Trim.Split(" ")(0)
                    End If
                End If
            Next
        Next
    End Sub

    Protected Sub chkMapcomplete_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkMapcomplete.CheckedChanged
        Dim ans As String = IIf(chkMapcomplete.Checked = True, "Y", "N")
        Dim siteid As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.siteid))
        Dim message As String = BusinessRule.BusinessRule.updatepubmap(siteid, ans)

    End Sub
End Class
