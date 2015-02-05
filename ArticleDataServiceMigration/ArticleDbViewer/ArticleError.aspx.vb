
Partial Class ArticleError
    Inherits WebFrontEnd

    Protected Sub ArticleError_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _message As String = ""
        Dim dsMigrateError As System.Data.DataSet

        If (Request("article_uid") IsNot Nothing) Then
            Dim article_uid As String = Request("article_uid")
            dsMigrateError = BusinessRule.BusinessRule.GetMigrateArticleErrorInfo(article_uid)
            _message = BusinessRule.BusinessRule.GetErrorMessage(dsMigrateError)
        End If
        Response.Write(_message)
    End Sub
End Class
