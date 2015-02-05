Imports Microsoft.VisualBasic
Imports WcfServiceAuthLib
Imports DataTransferObjects


Public Class WebFrontEnd
    Inherits System.Web.UI.Page
    Public mytracksession As New MySession.tracksession

    Public Sub WebFrontEnd()

    End Sub


    Protected Function GetAuthenticatedUser() As Boolean
        Dim authenticated As Boolean = False
        If (mytracksession.GetValue(MySession.tracksession.keycodes.authenticated).Contains("YES")) Then
            authenticated = True
        Else
            Dim userauth As New WcfServiceAuthLib.UserAuthentication
            Dim userinfo As New DTOUser
            Dim errorList As New DTOErrorList
            Dim windowsid As String = System.Security.Principal.WindowsIdentity.GetCurrent().Name
            errorList = userauth.LoginWithWindowsLoginId("_connectionkey", windowsid, "", userinfo) ' check if they are allowed to use windows authentication
            If (errorList.List.Count = 0 And userinfo IsNot Nothing) Then
                mytracksession.SetValue(MySession.tracksession.keycodes.authenticated, "YES")
                mytracksession.SetValue(MySession.tracksession.keycodes.permission, userinfo.permission)
                mytracksession.SetValue(MySession.tracksession.keycodes.username, userinfo.WindowsLoginId)
                mytracksession.SetValue(MySession.tracksession.keycodes.windowsid, "Y")
                authenticated = True
            End If
        End If
        Return authenticated
    End Function

End Class
