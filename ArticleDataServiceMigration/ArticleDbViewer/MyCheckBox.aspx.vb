
Partial Class MyCheckBox
    Inherits WebFrontEnd

    Protected Sub MyCheckBox_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then
            Exit Sub
        End If

        Dim asset_uid As String = Request("asset_uid")
        Dim checked As String = Request("checked")
        Dim article_uid As String = Request("article_uid")
        Dim username As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        BusinessRule.BusinessRule.Checkbox(article_uid, asset_uid, checked, siteid)
    End Sub

End Class
