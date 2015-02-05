
Partial Class checkout
    Inherits WebFrontEnd

    Protected Sub checkout_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then
            Exit Sub
        End If

        Dim username As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim article_uid As String = Request("article_uid").ToString
        Dim errmsg As String = BusinessRule.BusinessRule.Checkout(article_uid, username)
        Dim status As String = " Complete"
        If (errmsg.Length > 0) Then status = errmsg
        Dim message As String = String.Format("Article_uid: {0} Check Out: {1}", article_uid, status)
        Response.Write(message)
    End Sub
End Class
