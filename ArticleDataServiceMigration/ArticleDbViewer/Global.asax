<%@ Application Language="VB" %>

<script runat="server">

  
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Dim UnHandledException As Exception = Nothing
        Dim context As HttpContext = HttpContext.Current
		
        Try
            If context Is Nothing Then Exit Sub
            For Each ex As Exception In context.AllErrors()
                UnHandledException = ex
                Exit For
            Next
            If UnHandledException Is Nothing Then
                UnHandledException = Server.GetLastError().InnerException
                If UnHandledException Is Nothing Then Exit Sub
            End If
            Dim mytracksession As New MySession.tracksession
            Dim cLogError As New LogError(UnHandledException, context.Session, Request, mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID))
            cLogError.LogError()
        Catch
        Finally
            Server.ClearError()
            Response.Redirect("~/ApplicationError.aspx")
        End Try

        ' Code that runs when an unhandled error occurs
    End Sub

    
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub
       
</script>