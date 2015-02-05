Public Partial Class saxoviewer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim message As String = ""
        Dim saxo_username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
        Dim saxo_pwd = ConfigurationManager.AppSettings("saxo_pwd").ToString()
        Dim api As ArticleDbViewerSaxoInterface.api = New ArticleDbViewerSaxoInterface.api()
        Dim viewuri As String = Session("viewuri")
        Dim bytes() As Byte = api.GetFile(viewuri, saxo_username, saxo_pwd, message)
        Dim saxoArticle As String = System.Text.Encoding.ASCII.GetString(bytes)
        Dim baseaddress As String = "<base href=""http://zzdev.mn1.dc.publicus.com"" target=""_blank"">"
        Dim mSaxo As String = saxoArticle.Replace("<head>", "<head>" & System.Environment.NewLine & baseaddress & System.Environment.NewLine)
        Response.Write(mSaxo)
    End Sub

End Class