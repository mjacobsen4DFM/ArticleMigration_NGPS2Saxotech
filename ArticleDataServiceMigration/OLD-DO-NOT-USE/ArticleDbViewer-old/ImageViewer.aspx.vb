Imports System.IO
Imports System.Text
Imports System.Net
Imports System.Collections.Generic

Partial Public Class ImageViewer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim ds As DataSet = common.CheckCacheQueryId(Session.SessionID, Session("query"))
            Dim groupsize As Integer = 10
            Session("startrow") = 0
            Session("groupsize") = groupsize
            Session("endrow") = Session("startrow") + (Session("groupsize") - 1)
            articlePage.InnerHtml = loadRows()
        End If

    End Sub

    Private Function GetFileData(ByVal inputFilePath As String) As String
        If Not File.Exists(inputFilePath) Then Return ""
        Dim sr As New StreamReader(inputFilePath)
        Dim buffer As String = sr.ReadToEnd()
        sr.Close()
        Return buffer
    End Function



    Private Function loadRows() As String
        Dim articleDic As New Dictionary(Of String, String)
        Dim strBuild As New StringBuilder()
        Dim strBuildGallery As New StringBuilder()
        Dim htmlData As String = GetFileData(String.Format("{0}/objects/galleryviewer.pbo", Server.MapPath("templates")))
        Dim startrow As Integer = Convert.ToInt32(Session("startrow"))
        Dim endrow As Integer = Session("endrow")
        Dim ds As DataSet = common.CheckCacheQueryId(Session.SessionID, Session("query"))

        Dim dt As DataTable = ds.Tables(0)
        Dim totalrows As Integer = dt.Rows.Count
        Session("total") = totalrows
        If (endrow >= totalrows) Then endrow = totalrows - 1
        For row = startrow To endrow
            articleDic.Clear()
            Dim querynames() As String = {"article_uid", "startdate", "category", "heading"}
            For Each tag In querynames
                articleDic.Add(tag, dt.Rows(row).Item(tag).ToString)
            Next

            Dim sb As New StringBuilder
            Dim dsImages As System.Data.DataSet = common.GetImagePaths(articleDic("article_uid"))
            Dim dr As DataRow
            For Each dr In dsImages.Tables(0).Rows
                Dim asset_uid As String = dr("asset_uid").ToString
                Dim checked As String = ""
                If (common.softDelete(asset_uid, "")) Then checked = "checked"
                Dim checkbox As String = String.Format("<input type=""checkbox"" name=""deleteimage"" value=""{0}""  {1} />", asset_uid, checked)
                Dim anchor As String = String.Format("<img src=""{0}""  title=""{1}"" alt=""{2}"" width=""200"" />", dr("imagePath").ToString, dr("caption").ToString, asset_uid)
                sb.Append(String.Format("<table border=""1"" align=""left""><tr><td title=""{0}"">", asset_uid))
                sb.Append(anchor)
                sb.Append("</td></tr><tr><td title=""size"" align=""center"" >")
                sb.Append(checkbox)
                sb.Append("</td></tr></table>")
            Next
            Dim gallery As String = sb.ToString()
            articleDic.Add("gallery", gallery)
            articleDic.Add("row", row)

            Dim galleryItem As String = ""
            If (htmlData.Contains("<%GalleryItemsCollection%>")) Then
                galleryItem = GetFileData(String.Format("{0}/objects/galleryItem.pbo", Server.MapPath("templates")))
                Dim tagnames() As String = {"row", "article_uid", "startdate", "category", "heading", "gallery"}

                For Each tag In tagnames
                    Dim value As String = articleDic(tag)
                    galleryItem = galleryItem.Replace(String.Format("<%{0}%>", tag), value)
                Next
            End If
            If (galleryItem.Length() > 0) Then strBuildGallery.Append(galleryItem)
        Next
        htmlData = htmlData.Replace("<%GalleryItemsCollection%>", strBuildGallery.ToString())
        Return htmlData
    End Function

















    Private Sub checkrow()
        Dim startrow As Integer = Session("startrow")
        Dim total As Integer = Session("total")
        If startrow > total - 1 Then startrow = total - 1
        If startrow < 0 Then startrow = 0
        Session("startrow") = startrow
        If (Session("endrow") > Session("total") - 1) Then
            Session("endrow") = Session("total") - 1
            If (Session("endrow") < 0) Then Session("endrow") = 0
        End If

    End Sub


    Protected Sub btnNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNext.Click

    End Sub


    Protected Sub btnPrevious_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrevious.Click

    End Sub
End Class