Imports System.IO
Imports System.Text
'Imports SmugMugModel
Imports System.Net
Imports System.Collections.Generic


Partial Public Class ListGalleries
    Inherits System.Web.UI.Page


    'Private Function Login() As SmugMugModel.MyUser
    '    Dim mySite As SmugMugModel.Site = New SmugMugModel.Site()
    '    SmugMugModel.Site.Proxy = WebRequest.DefaultWebProxy
    '    Dim userName As String = ConfigurationManager.AppSettings("username")
    '    Dim password As String = ConfigurationManager.AppSettings("password")
    '    Return mySite.Login(userName, password)
    'End Function


    'Private Function CheckCache(ByVal loginid As String) As SmugMugModel.MyUser
    '    Dim myuser As SmugMugModel.MyUser
    '    myuser = CType(HttpContext.Current.Cache(loginid), SmugMugModel.MyUser)
    '    If myuser Is Nothing Then
    '        myuser = Login()
    '        Dim ts As TimeSpan   '  put in cache 2 hour expiration
    '        ts = New TimeSpan(2, 0, 0)
    '        HttpContext.Current.Cache.Insert(loginid, myuser, Nothing, DateTime.MaxValue, ts)
    '    End If
    '    Return myuser
    'End Function



    'Private Function GetFileData(ByVal inputFilePath As String) As String
    '    If Not File.Exists(inputFilePath) Then Return ""
    '    Dim sr As New StreamReader(inputFilePath)
    '    Dim buffer As String = sr.ReadToEnd()
    '    sr.Close()
    '    Return buffer
    'End Function



    'Private Function loadSmugRows() As String
    '    Dim articleDic As New Dictionary(Of String, String)
    '    Dim strBuild As New StringBuilder()
    '    Dim strBuildGallery As New StringBuilder()
    '    Dim htmlData As String = GetFileData(String.Format("{0}/objects/galleryviewer.pbo", Server.MapPath("templates")))
    '    Dim startrow As Integer = Convert.ToInt32(Session("startrow"))
    '    Dim endrow As Integer = Session("endrow")
    '    Dim ds As DataSet = common.CheckCacheQueryId(Session.SessionID, Session("query"))

    '    Dim dt As DataTable = ds.Tables(0)
    '    For row = startrow To endrow
    '        articleDic.Clear()
    '        Dim querynames() As String = {"article_uid", "startdate", "category", "heading"}
    '        For Each tag In querynames
    '            articleDic.Add(tag, dt.Rows(row).Item(tag).ToString)
    '        Next


    '        'Dim albumid As String = "", albumkey As String = "", smugAnchor As String = ""
    '        'If (common.SmugLookUp(articleDic("article_uid"), albumid, albumkey)) Then
    '        '    Dim myuser As SmugMugModel.MyUser = CheckCache("login")
    '        '    Dim Imagelist As List(Of SmugMugModel.Image) = myuser.GetImages(False, "", True, 0, False, "", albumid, albumkey)
    '        '    Dim sb As New StringBuilder
    '        '    For Each im In Imagelist
    '        '        Dim anchor As String = String.Format("<img src=""{1}""  title=""{2}"" alt=""{3}"" />", im.MediumURL, im.TinyURL, im.Caption, im.id)
    '        '        Dim checked As String = ""
    '        '        If (common.SmugLookUpTracker(articleDic("article_uid"), im.id)) Then checked = "checked"
    '        '        Dim checkbox As String = String.Format("{6}<input type=""checkbox"" name=""deleteimage"" value=""{0}{1}{2}{3}{4}""  {5} />", articleDic("article_uid"), Chr(254), im.id, Chr(254), im.FileName, checked, im.Size)
    '        '        sb.Append(String.Format("<table border=""1"" align=""left""><tr><td title=""{0}"">", im.FileName))
    '        '        sb.Append(anchor)
    '        '        sb.Append("</td></tr><tr><td title=""size"" align=""center"" >")
    '        '        sb.Append(checkbox)
    '        '        sb.Append("</td></tr></table>")

    '        '    Next
    '        '    smugAnchor = sb.ToString()
    '        'End If
    '        'articleDic.Add("gallery", smugAnchor)
    '        'articleDic.Add("row", row)

    '        Dim galleryItem As String = ""
    '        If (htmlData.Contains("<%GalleryItemsCollection%>")) Then
    '            galleryItem = GetFileData(String.Format("{0}/objects/galleryItem.pbo", Server.MapPath("templates")))
    '            Dim tagnames() As String = {"row", "article_uid", "startdate", "category", "heading", "gallery"}

    '            For Each tag In tagnames
    '                Dim value As String = articleDic(tag)
    '                galleryItem = galleryItem.Replace(String.Format("<%{0}%>", tag), value)
    '            Next
    '        End If

    '        If (galleryItem.Length() > 0) Then strBuildGallery.Append(galleryItem)
    '    Next
    '    htmlData = htmlData.Replace("<%GalleryItemsCollection%>", strBuildGallery.ToString())
    '    Return htmlData
    'End Function




    'Private Sub deleteSmugImage(ByVal article_uid As String, ByVal image_uid As String)
    '    Dim errmsg As String = ""
    '    Dim myuser As SmugMugModel.MyUser = CheckCache("login")
    '    Dim query As String = String.Format("Select albumid, albumkey from smug_gallery where article_uid ='{0}'", article_uid)
    '    Dim ds As DataSet = common.DataMine.GetDs(query)
    '    If (ds.Tables(0).Rows.Count = 1) Then
    '        Dim albumid As String = ds.Tables(0).Rows(0).Item("albumid").ToString
    '        Dim albumkey As String = ds.Tables(0).Rows(0).Item("albumkey").ToString
    '        Dim Imagelist As List(Of SmugMugModel.Image) = myuser.GetImages(False, "", True, 0, False, "", albumid, albumkey)
    '        For Each im In Imagelist
    '            If (im.id = CLng(image_uid)) Then
    '                myuser.DeleteImage(im.id)
    '                common.DataMine.spExecute("UpdateSmugGalleryTrackerTS", errmsg, "@article_uid", article_uid, "@image_uid", image_uid)
    '            End If
    '        Next
    '    End If
    'End Sub



    'Private Sub SaveDeleteImageGroup(ByVal deleteimages As String, ByVal bDelete As Boolean)
    '    Dim errmsg As String = ""
    '    Dim img As String

    '    ' clear out all rows for this group that have not been deleted
    '    Dim startrow As Integer = Convert.ToInt32(Session("startrow"))
    '    Dim endrow As Integer = Convert.ToInt32(Session("endrow"))
    '    Dim ds As DataSet = common.CheckCacheQueryId(Session.SessionID, Session("query"))

    '    Dim dt As DataTable = ds.Tables(0)
    '    For row = startrow To endrow
    '        Dim article_uid As String = dt.Rows(row).Item(0).ToString
    '        common.DataMine.spExecute("DeleteSmugGalleryTracker", errmsg, "@article_uid", article_uid) ' clear out all articles within this group
    '    Next

    '    If (deleteimages IsNot Nothing AndAlso deleteimages.Length > 0) Then
    '        Dim imagegroup() As String = deleteimages.Split(",")
    '        For Each img In imagegroup
    '            Dim imgrow() As String = img.Split(Chr(254))
    '            Dim article_uid As String = imgrow(0)
    '            Dim image_uid As String = imgrow(1)
    '            Dim imagename As String = imgrow(2)

    '            common.DataMine.spExecute("UpdateSmugGalleryTracker", errmsg, "@article_uid", article_uid, "@image_uid", image_uid, "@imagename", imagename)
    '            If (bDelete) Then deleteSmugImage(article_uid, image_uid)
    '        Next
    '    End If
    'End Sub



    'Private Sub checkrow()
    '    Dim startrow As Integer = Session("startrow")
    '    Dim total As Integer = Session("total")
    '    If startrow > total - 1 Then startrow = total - 1
    '    If startrow < 0 Then startrow = 0
    '    Session("startrow") = startrow
    '    If (Session("endrow") > Session("total") - 1) Then
    '        Session("endrow") = Session("total") - 1
    '        If (Session("endrow") < 0) Then Session("endrow") = 0
    '    End If

    'End Sub




    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    '    If Not IsPostBack Then
    '        Dim ds As DataSet = common.CheckCacheQueryId(Session.SessionID, Session("query"))
    '        Dim groupsize As Integer = 10
    '        Session("total") = ds.Tables(0).Rows.Count
    '        Session("startrow") = 0
    '        Session("groupsize") = groupsize
    '        Session("endrow") = Session("startrow") + (Session("groupsize") - 1)
    '        checkrow()
    '        articlePage.InnerHtml = loadSmugRows()

    '    End If
    'End Sub







    'Protected Sub btnPrevious_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrevious.Click
    '    Session("startrow") = Session("startrow") - Session("groupsize")
    '    If (CInt(Session("startrow") <= 0)) Then Session("startrow") = 0
    '    Session("endrow") = Session("startrow") + (Session("groupsize") - 1)
    '    checkrow()
    '    articlePage.InnerHtml = loadSmugRows()
    'End Sub

    'Protected Sub btnNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNext.Click
    '    Dim deleteimages As String = Request("deleteimage")
    '    SaveDeleteImageGroup(deleteimages, False)
    '    Session("startrow") = Session("startrow") + Session("groupsize")
    '    Session("endrow") = Session("startrow") + (Session("groupsize") - 1)
    '    checkrow()
    '    articlePage.InnerHtml = loadSmugRows()
    'End Sub
End Class