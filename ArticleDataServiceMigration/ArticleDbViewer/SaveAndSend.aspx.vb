
Partial Class SaveAndSend
    Inherits WebFrontEnd




    Protected Sub SaveAndSend_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _message As String = String.Empty
        Dim article_uid As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("article_uid")), Request("article_uid"), ""))
        Dim username As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("username")), Request("username"), ""))
        Dim headline As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("headline")), Request("headline"), ""))

        Dim summary As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("summary")), Request("summary"), ""))
        Dim body As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("body")), Request("body"), ""))
        Dim byline As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("byline")), Request("byline"), ""))
        Dim _action As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("action")), Request("action"), ""))

        Dim profileid As String = Server.UrlDecode(IIf(Not String.IsNullOrEmpty(Request("profileid")), Request("profileid"), ""))
        If (IsNumeric(profileid)) Then
            BusinessRule.BusinessRule.SaveArticleProfile(article_uid, Convert.ToInt32(profileid))
        End If

        _message = BusinessRule.BusinessRule.SaveArticle(article_uid, headline, summary, body, byline)
        'Mark the Article as edited
        Dim bEdited As Boolean = IIf(String.IsNullOrEmpty(_message), True, False)
        If (bEdited) Then BusinessRule.BusinessRule.ArticleEdit(article_uid, username)
        If Not String.IsNullOrEmpty(_message) Then
            Response.Write(_message)
            Exit Sub
        End If
        If (_action.Equals("save", StringComparison.InvariantCultureIgnoreCase)) Then
            Response.Write(String.Format("Article: {0} saved", article_uid))
            Exit Sub
        End If


        Dim saxo_host As String = mytracksession.GetValue(MySession.tracksession.keycodes.owsTarget)
        Dim saxo_username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
        Dim saxo_pwd = ConfigurationManager.AppSettings("saxo_pwd").ToString()
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)
        Dim documents As String = Server.MapPath(ConfigurationManager.AppSettings("documents").ToString())
        Dim temp As String = Server.MapPath(ConfigurationManager.AppSettings("temp").ToString())
        Dim logFlag As String = mytracksession.GetValue(MySession.tracksession.keycodes.logging)
        Dim taxpubid As String = BusinessRule.BusinessRule.GetTaxPubid(siteid)
        Dim bOverriden As Boolean = DataAuthentication.DataAuthentication.ArticleState(article_uid)
        If (Not bOverriden) Then BusinessRule.BusinessRule.ArticleSelect(article_uid, username)

        If (bEdited) Then
            Try
                'Migrate Article
                Dim bResult As Boolean = MigrateArticle(username, taxpubid, siteid, destination_siteid, documents, temp, saxo_host, saxo_username, saxo_pwd, article_uid, logFlag)
                _message = IIf(CBool(bResult), String.Format("Sending Article ({0}) Succeeded", article_uid), String.Format("Sending Article ({0}) Failed", article_uid))
                Response.Write(_message)
                Exit Sub
            Catch ex As Exception
                Response.Write(String.Format("Exception: {0} stack: {1}", ex.Message, ex.StackTrace))
                'The error is a lie
            End Try
        Else
            Response.Write("Unkown Error")
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
            bResult = BusinessRule.BusinessRule.MigrateArticleManually(taxpubid, siteid, destination_siteid, documents, temp, saxo_host, saxo_username, saxo_pwd, article_uid, logFlag)
            'Checkin the article
            BusinessRule.BusinessRule.Checkin(article_uid, username, siteid)
            'Send the article

        Else
            'The article is locked, should not send
            bResult = False
        End If

        'Report
        Return bResult
    End Function


End Class
