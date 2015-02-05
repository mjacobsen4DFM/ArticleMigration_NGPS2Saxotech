Imports System.Net
Imports System.Collections.Generic
Imports System.IO
Imports System.Text.RegularExpressions

Partial Public Class ArticleViewer
    Inherits System.Web.UI.Page

    Private mytracksession As New MySession.tracksession


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            mytracksession.SetValue(MySession.tracksession.keycodes.row, "0")
            setRecord(0)
        End If
    End Sub





    Private Function GetFileData(ByVal inputFilePath As String) As String
        Dim sr As New StreamReader(inputFilePath)
        Dim buffer As String = sr.ReadToEnd()
        sr.Close()
        Return buffer
    End Function

    Private Function CheckSaxotechImage(ByVal article_uid As String) As String
        Dim sb As New StringBuilder
        Dim ds2 As DataSet, dr As DataRow, url As String = ""
        Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)  ' target publication id
        Dim query As String = String.Format("select asset_uid from image where asset_uid in (select asset_uid from asset where asset_type = 108 and  article_uid = '{0}' )", article_uid)
        Dim ds As DataSet = common.DataMine.GetDs(query)
        Dim idx As Integer = 0
        For Each dr In ds.Tables(0).Rows
            idx += 1
            Dim asset_uid = dr(0).ToString()
            query = String.Format("select url from saxo_image where asset_uid = '{0}' and destination_siteid = '{1}'", asset_uid, destination_siteid)
            ds2 = common.DataMine.GetDs(query)
            If (ds2.Tables(0).Rows.Count > 0) Then url = ds2.Tables(0).Rows(0).Item(0).ToString()
            Dim htmlData As String = GetFileData(String.Format("{0}/objects/saxoimageitem.pbo", Server.MapPath("templates")))
            htmlData = htmlData.Replace("<%row%>", idx.ToString).Replace("<%asset_uid%>", asset_uid).Replace("<%url%>", url)
            sb.Append(htmlData)
        Next
        query = String.Format("select gallery_uid from saxo_gallery where article_uid = '{0}' and destination_siteid ='{1}' ", article_uid, destination_siteid)
        ds = common.DataMine.GetDs(query)
        If (ds.Tables(0).Rows.Count > 0) Then
            For Each dr In ds.Tables(0).Rows
                idx += 1
                Dim htmlData As String = GetFileData(String.Format("{0}/objects/saxoimageitem.pbo", Server.MapPath("templates")))
                htmlData = htmlData.Replace("<%row%>", idx.ToString).Replace("<%asset_uid%>", "Gallery").Replace("<%url%>", dr(0).ToString())
                sb.Append(htmlData)
            Next
        End If
        query = String.Format("select xmldata,viewuri from saxo_article where article_uid = '{0}' and destination_siteid ='{1}' ", article_uid, destination_siteid)
        ds = common.DataMine.GetDs(query)
        If (ds.Tables(0).Rows.Count > 0) Then
            For Each dr In ds.Tables(0).Rows
                idx += 1
                Dim htmlData As String = GetFileData(String.Format("{0}/objects/saxoimageitem.pbo", Server.MapPath("templates")))
                Dim xmllen As Integer = dr(0).ToString.Length
                Dim artinfo As String = String.Format("Saxotech XML Length: {0}", xmllen)
                htmlData = htmlData.Replace("<%row%>", idx.ToString).Replace("<%asset_uid%>", artinfo).Replace("<%url%>", dr(1).ToString())
                sb.Append(htmlData)
            Next
        Else

        End If


        Dim iLen As Integer = sb.ToString().Length
        Dim rowHead As String = GetFileData(String.Format("{0}/objects/saxoimageitemHead.pbo", Server.MapPath("templates")))
        Dim strTable As String = String.Format("{0}{1}", rowHead, sb.ToString())
        Return IIf(iLen > 0, strTable, "")
    End Function




    Private Sub setRecord(ByVal row As Integer)
        Dim tags() As String = {"row", "article_uid", "startdate", "category", "imagecount", "heading", "story", "gallery", "script"}

        Dim htmlData As String = GetFileData(String.Format("{0}/objects/articleviewer.pbo", Server.MapPath("templates"))) ' default template
        Dim articleDic As New Dictionary(Of String, String)
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
        Dim dt As DataTable = ds.Tables(0)
        Dim total As Integer = dt.Rows.Count
        If total = 0 Then
            articlePage.InnerHtml = " No Rows Returned "
            Exit Sub
        End If

        IIf(row >= total - 1, btnNext.Enabled = False, btnNext.Enabled = True)
        If (row >= total - 1) Then row = total - 1
        If row < 0 Then row = 0
        If (row = 0) Then
            btnPrevious.Attributes.Add("onClick", "this.disabled=true;")
        End If
        mytracksession.SetValue(MySession.tracksession.keycodes.row, row.ToString)

        Dim querynames() As String = {"article_uid", "startdate", "category", "imagecount", "heading", "story"}
        Dim numImages As Integer = 0
        For Each tag In querynames
            articleDic.Add(tag, dt.Rows(row).Item(tag).ToString)
            If tag.Equals("imagecount") AndAlso IsNumeric(dt.Rows(row).Item(tag).ToString) Then
                numImages = Convert.ToInt32(dt.Rows(row).Item(tag).ToString)
            End If
        Next
        articleDic.Add("row", String.Format("{0}/{1}", row + 1, total))


        Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)
        Dim viewuri As String = common.GetArticleViewURI(destination_siteid, articleDic("article_uid"))
        viewuri = viewuri.Replace("zzedit", "zzdev")
        If (viewuri.Length > 0) Then
            Dim message As String = ""
            articleDic.Add("viewuri", viewuri)
            htmlData = GetFileData(String.Format("{0}/objects/saxoviewer.pbo", Server.MapPath("templates")))
            Dim tagsList() As String = {"row", "article_uid", "startdate", "category", "imagecount", "viewuri"}
            For Each tag In tagsList
                Dim value As String = articleDic(tag)
                htmlData = htmlData.Replace(String.Format("<%{0}%>", tag), value)
            Next
            articlePage.InnerHtml = htmlData
            btnPush.Visible = False
            Exit Sub
        End If
        btnPush.Visible = True
        Dim gallery As String = ""
        If (numImages > 0) Then
            Dim sb As New StringBuilder
            Dim dsImages As System.Data.DataSet = common.GetImagePaths(articleDic("article_uid"))
            Dim dr As DataRow
            sb.Append(String.Format("<table border=""1"" align=""left""><tr>"))
            For Each dr In dsImages.Tables(0).Rows
                Dim anchor As String = String.Format("<td><img src=""{0}""  title=""{1}"" alt=""{2}"" width=""200"" /></td>", dr("imagePath").ToString, dr("caption").ToString, dr("asset_uid").ToString)
                sb.Append(anchor)
            Next
            sb.Append("</tr></table>")
            gallery = sb.ToString()
        End If
        articleDic.Add("gallery", gallery)



        articleDic.Add("script", "")
        Dim bcontainScripting As Boolean = False
        For Each tag In tags
            Dim value As String = articleDic(tag)
            Dim stripped As String = ""
            If (value.Contains("<script")) Then
                bcontainScripting = True
                value = removeScripting(value)
            End If
            If (bcontainScripting And tag.Equals("script")) Then
                value = "Removed Scripting from view"
            End If
            htmlData = htmlData.Replace(String.Format("<%{0}%>", tag), value)
        Next
        mytracksession.SetValue(MySession.tracksession.keycodes.article_uid, articleDic("article_uid"))
        Dim imagecount As Integer = 0
        Dim saxoImageCollection As String = ""
        If IsNumeric(articleDic("imagecount")) Then imagecount = Convert.ToInt32(articleDic("imagecount"))
        If (imagecount > 0) Then saxoImageCollection = CheckSaxotechImage(articleDic("article_uid"))
        htmlData = htmlData.Replace("<%saxoimagecollection%>", saxoImageCollection)
        articlePage.InnerHtml = htmlData
    End Sub


    Private Function removeScripting(ByVal value As String) As String
        Return Regex.Replace(value, "<script.*?</script>", "", RegexOptions.Singleline Or RegexOptions.IgnoreCase)
    End Function

    Protected Sub btnNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNext.Click
        Dim row As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.row))
        row += 1
        setRecord(row)
    End Sub

    Protected Sub btnPrevious_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrevious.Click
        Dim row As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.row))
        row -= 1
        setRecord(row)
    End Sub

    Protected Sub btnPush_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPush.Click
        Dim image_message As String = ""
        Dim gallery_message As String = ""
        Dim article_message As String = ""

        Dim saxo_host As String = ConfigurationManager.AppSettings("saxo_host").ToString()
        Dim saxo_username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
        Dim saxo_pwd = ConfigurationManager.AppSettings("saxo_pwd").ToString()
        Dim article_uid As String = mytracksession.GetValue(MySession.tracksession.keycodes.article_uid)
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)
        Dim documents As String = ConfigurationManager.AppSettings("documents").ToString()
        Dim temp As String = ConfigurationManager.AppSettings("temp").ToString()

        Dim api As ArticleDbViewerSaxoInterface.api = New ArticleDbViewerSaxoInterface.api()
        api.MigrateImage(saxo_host, saxo_username, saxo_pwd, destination_siteid, article_uid, image_message)
        api.MigrateGallery(documents, temp, saxo_host, saxo_username, saxo_pwd, destination_siteid, article_uid, gallery_message)
        api.MigrateArticle(siteid, destination_siteid, documents, temp, saxo_host, saxo_username, saxo_pwd, article_uid, article_message)
        Dim row As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.row))
        setRecord(row)

    End Sub

    Protected Sub btnGroup_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGroup.Click
        Response.Redirect("ArticleGroup.aspx")
    End Sub
End Class