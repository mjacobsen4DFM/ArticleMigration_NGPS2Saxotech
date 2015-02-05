
Partial Class logout
    Inherits WebFrontEnd

    Protected Sub logout_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        mytracksession.closeSessionId()
        Response.Redirect("login.aspx")
    End Sub
End Class
