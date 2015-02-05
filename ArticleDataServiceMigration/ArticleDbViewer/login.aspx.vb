
Imports WcfServiceAuthLib
Imports DataTransferObjects

Partial Class login
    Inherits WebFrontEnd


    Protected Sub Login1_Authenticate(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.AuthenticateEventArgs) Handles Login1.Authenticate
        Dim userauth As New WcfServiceAuthLib.UserAuthentication
        Dim userinfo As New DTOUser
        Dim errorList As New DTOErrorList

        errorList = userauth.LoginWithUserName("_connectionkey", Login1.UserName, Login1.Password, userinfo)
        If (errorList.List.Count > 0 Or userinfo Is Nothing) Then
            lblmsg.Text = "Login or password incorrect.<br>Please try again."
            e.Authenticated = False
            Return
        End If
        Dim sessionid As String = mytracksession.CreateSessionId()
        mytracksession.SetValue(MySession.tracksession.keycodes.authenticated, "YES")
        mytracksession.SetValue(MySession.tracksession.keycodes.permission, userinfo.permission)
        mytracksession.SetValue(MySession.tracksession.keycodes.username, userinfo.UserName)
        mytracksession.SetValue(MySession.tracksession.keycodes.windowsid, "N")
        e.Authenticated = True
        Response.Redirect("ArticleGroup.aspx")


    End Sub
End Class
