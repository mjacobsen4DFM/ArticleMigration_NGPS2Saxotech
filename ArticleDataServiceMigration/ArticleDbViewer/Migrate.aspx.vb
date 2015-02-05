
Partial Class Migrate
    Inherits WebFrontEnd

    Protected Sub Migrate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then
            Response.Write("Not Authorized")
            Exit Sub
        End If

        Dim bOverride As Boolean = False
        Dim bResult As Boolean = False
        Dim uri As String
        Dim article_uid As String = Request("article_uid")
        Dim edited As String = Request("edited")
        Dim batch As String = Request("batch")
        Dim bEdited As Boolean = CBool(IIf(Not String.IsNullOrEmpty(edited), edited, "false"))
        Dim bBatch As Boolean = CBool(IIf(Not String.IsNullOrEmpty(batch), batch, "false"))
        Dim username As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim saxo_host As String = mytracksession.GetValue(MySession.tracksession.keycodes.owsTarget)
        Dim saxo_username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
        Dim saxo_pwd = ConfigurationManager.AppSettings("saxo_pwd").ToString()
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)
        Dim documents As String = Server.MapPath(ConfigurationManager.AppSettings("documents").ToString())
        Dim temp As String = Server.MapPath(ConfigurationManager.AppSettings("temp").ToString())
        Dim logFlag As String = mytracksession.GetValue(MySession.tracksession.keycodes.logging)
        Dim taxpubid As String = BusinessRule.BusinessRule.GetTaxPubid(siteid)
        Dim referrer As String = Request.UrlReferrer.AbsolutePath.ToString

        If (String.IsNullOrEmpty(article_uid) AndAlso Not bBatch) Then
            Response.Write("False")
        Else
            ' Dim dsMigrateError As System.Data.DataSet = BusinessRule.BusinessRule.GetMigrateArticleErrorInfo(article_uid)
            'Trusting the user...
            'If (dsMigrateError.Tables(0).Rows.Count <> 0 AndAlso Not bEdited) Then
            '    Dim errorDescription As String = BusinessRule.BusinessRule.GetErrorTypeDescription(dsMigrateError)
            '    bOverride = MsgBox(String.Format("Article violates {0}{1}Continue?", errorDescription, System.Environment.NewLine), MsgBoxStyle.YesNo, "Override Error?")
            'End If

            'Select the article, if not already selected (for tracking)
            Dim bOverriden As Boolean = DataAuthentication.DataAuthentication.ArticleState(article_uid)
            If (Not bOverriden) Then BusinessRule.BusinessRule.ArticleSelect(article_uid, username)

            'Migrate Article
            bResult = MigrateArticle(username, taxpubid, siteid, destination_siteid, documents, temp, saxo_host, saxo_username, saxo_pwd, article_uid, logFlag)
            Response.Write(String.Format("Article_uid={0} migrated={1}", article_uid, bResult.ToString))
            'uri = String.Format("{0}?article_uid={1}&response={2}", referrer, article_uid, bResult.ToString)
            'Response.Redirect(uri)
            Exit Sub
        End If
    End Sub

    Private Function MigrateArticle(ByVal username As String, ByVal taxpubid As String, ByVal siteid As String, ByVal destination_siteid As String, ByVal documents As String, ByVal temp As String, ByVal saxo_host As String, ByVal saxo_username As String, ByVal saxo_pwd As String, ByVal article_uid As String, ByVal logFlag As String) As Boolean
        Dim bResult As Boolean = False

        'Determine if the article is checked out; if it is, then ensure it is checked out to the current user
        'If both, then mark it as sendable
        Dim whoLocked As String = String.Empty
        Dim bSendable As Boolean = False
        If (BusinessRule.BusinessRule.IsArticleLocked(article_uid, whoLocked)) Then
            If (username.Equals(whoLocked)) Then
                bSendable = True
            Else
                bSendable = False 'someone else checked out the article during processing
            End If
        End If

        If (bSendable) Then
            'Checkin the article
            BusinessRule.BusinessRule.Checkin(article_uid, username, siteid)
            'Send the article
            bResult = BusinessRule.BusinessRule.MigrateArticleManually(taxpubid, siteid, destination_siteid, documents, temp, saxo_host, saxo_username, saxo_pwd, article_uid, logFlag)
        Else
            'The article is locked, should not send
            bResult = False
        End If

        'Report
        Return bResult
    End Function
End Class
