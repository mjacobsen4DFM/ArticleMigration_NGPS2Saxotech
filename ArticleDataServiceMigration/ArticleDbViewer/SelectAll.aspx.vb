Imports System.Data
Imports System.IO

Partial Class SelectAll
    Inherits WebFrontEnd


    Private Sub ToggleSelectArticles(ByVal bSelected As Boolean)
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        Dim username As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim article_uid As String = String.Empty
        Dim errmsg As String = String.Empty
        Dim bForced As Boolean = True
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
        Response.BufferOutput = False
        Dim total As Integer = ds.Tables(0).Rows.Count
        Dim idx As Integer = 0
        Dim mydiv As String = "<div id=""myArea"" class=""Mybox"" />"
        Response.Write(mydiv)

        For Each row As DataRow In ds.Tables(0).Rows

            article_uid = row.Item("article_uid").ToString
            errmsg = BusinessRule.BusinessRule.ArticleSelect(article_uid, username, bForced, bSelected)
            If (Not String.IsNullOrEmpty(errmsg)) Then
                errmsg = String.Empty
            End If
            idx += 1
            If ((idx Mod 10) = 0) Then
                Response.Write(String.Format("<script>myArea.innerText = "" Number of rows to Updated: {0} of {1} ""</script> ", idx, total))
            End If
        Next
    End Sub


    Protected Sub SelectAll_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        myclose.Disabled = True
        Response.BufferOutput = False
        Dim ans As String = Request("selected")
        mytracksession.SetValue(MySession.tracksession.keycodes.myselected, ans)
        Dim bSelected As Boolean = IIf(ans.Equals("Y"), True, False)
        ToggleSelectArticles(bSelected)
        myclose.Disabled = False
    End Sub
End Class
