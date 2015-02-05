
Partial Class Process
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.BufferOutput = False
        Response.Write("Hello World<br>")
        System.Threading.Thread.Sleep(3000)
        Response.Write("Hello World")
        System.Threading.Thread.Sleep(3000)
    End Sub
End Class
