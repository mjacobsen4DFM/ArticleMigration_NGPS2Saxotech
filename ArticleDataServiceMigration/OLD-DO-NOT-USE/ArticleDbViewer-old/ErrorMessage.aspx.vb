Public Partial Class ErrorMessage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("row") = 0
        lblmsg.Text = Request("msg") & Session("stacktrace").ToString
    End Sub

End Class