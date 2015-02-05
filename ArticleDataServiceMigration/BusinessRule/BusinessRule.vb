Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Configuration
Imports UncAccess
Imports System

Public Class BusinessRule

    Private Enum asset
        article = 102
        image = 108
        freeform = 111
        pdf = 104
    End Enum


    Private Shared Function GetFileData(ByVal inputFilePath As String) As String
        If Not File.Exists(inputFilePath) Then Return ""
        Dim sr As New StreamReader(inputFilePath)
        Dim buffer As String = sr.ReadToEnd()
        sr.Close()
        Return buffer
    End Function

    Private Shared Sub Logging(ByVal article_uid As String, ByVal statement As String, ByVal logFlag As String)
        Dim message As String = ""
        If (logFlag.Equals("SUPERADMIN")) Then
            DataAuthentication.DataAuthentication.spExecute("LogStatement", message, "@article_uid", article_uid, "@statement", statement)
        End If
    End Sub


    Public Shared Function BusGetDataset(ByVal query As String) As DataSet
        Return DataAuthentication.DataAuthentication.GetDataSet(query)
    End Function

    Public Shared Function IsArticleLocked(ByVal article_uid As String, ByRef username As String) As Boolean
        Return (DataAuthentication.DataAuthentication.IsArticleLocked(article_uid, username))
    End Function

    Public Shared Function Checkout(ByVal article_uid As String, ByVal username As String) As String
        Dim errmsg As String = DataAuthentication.DataAuthentication.Checkout(article_uid, username)
        Return errmsg
    End Function

    Public Shared Function Checkin(ByVal article_uid As String, ByVal username As String, ByVal siteid As String) As String
        Dim errmsg As String = DataAuthentication.DataAuthentication.Checkin(article_uid, username, siteid)
        Return errmsg
    End Function

    Public Shared Function ArticleSelect(ByVal article_uid As String, ByVal username As String, Optional ByVal forceState As Boolean = False, Optional ByVal selectState As Boolean = False) As String
        Dim errmsg As String = DataAuthentication.DataAuthentication.ArticleSelect(article_uid, username, forceState, selectState)
        Return errmsg
    End Function

    Public Shared Function ArticleEdit(ByVal article_uid As String, ByVal username As String) As String
        Dim errmsg As String = DataAuthentication.DataAuthentication.ArticleEdit(article_uid, username)
        Return errmsg
    End Function

    Public Shared Function ArticleResent(ByVal article_uid As String, ByVal sent As Boolean) As String
        Dim errmsg As String = DataAuthentication.DataAuthentication.ArticleResent(article_uid, sent)
        Return errmsg
    End Function

    Public Shared Function IsSoftDelete(ByVal article_uid As String, ByVal asset_uid As String, ByVal siteid As String) As Boolean
        Return DataAuthentication.DataAuthentication.IsSoftDelete(article_uid, asset_uid, siteid)
    End Function

    Public Shared Sub DeleteFromSaxoArticle(ByVal article_uid As String)
        DataAuthentication.DataAuthentication.DeleteFromSaxoArticle(article_uid)
    End Sub


    Public Shared Sub Checkbox(ByVal article_uid As String, ByVal asset_uid As String, ByVal checked As String, ByVal siteid As String)
        DataAuthentication.DataAuthentication.Checkbox(article_uid, asset_uid, checked, siteid)
    End Sub

    Public Shared Function GetArticleViewURI(ByVal destination_siteid As String, ByVal article_uid As String) As String
        Return DataAuthentication.DataAuthentication.GetArticleViewURI(destination_siteid, article_uid)
    End Function

    Public Shared Function GetArticleStoryurl(ByVal destination_siteid As String, ByVal article_uid As String) As String
        Return DataAuthentication.DataAuthentication.GetArticleStoryurl(destination_siteid, article_uid)
    End Function


    Public Shared Function GetDestinationSiteid(ByVal siteid As String, ByVal column As DataAuthentication.DataAuthentication.Fields) As String
        Return DataAuthentication.DataAuthentication.GetDestinationSiteid(siteid, column)
    End Function

    Public Shared Function MyCategories(ByVal siteid As String) As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.MyCategories(siteid)
    End Function

    Public Shared Function MySites() As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.MySites()
    End Function

    Public Shared Function GetImagePaths(ByVal article_uid As String) As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.GetImagePaths(article_uid)
    End Function

    Public Shared Function GetArticle(ByVal article_uid As String) As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.GetArticle(article_uid)
    End Function

    'Added function to get selected articles (if the view is restricted by error/hold types, include that criteria)
    Public Shared Function GetSelectedArticles(ByVal siteid As String, ByVal errorholdtype As String) As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.GetSelectedArticles(siteid, errorholdtype)
    End Function

    Public Shared Function ArticleState(ByVal article_uid As String) As Boolean
        Return DataAuthentication.DataAuthentication.ArticleState(article_uid)
    End Function


    Public Shared Function AllArticlesSelected(ByVal query As String) As Boolean
        Return DataAuthentication.DataAuthentication.AllArticlesSelected(query)
    End Function

    'Deprecated "CheckMigrateArticleError" and "MigrateArticleErrorDetail" in favor of getting a dataset and pulling the details as needed -MSJ 20130322
    'Public Shared Function CheckMigrateArticleError(ByVal article_uid As String) As Boolean
    '    Return DataAuthentication.DataAuthentication.CheckMigrateArticleError(article_uid)
    'End Function

    'Public Shared Function MigrateArticleErrorDetail(ByVal article_uid As String) As String
    '    Return DataAuthentication.DataAuthentication.MigrateArticleErrorDetail(article_uid)
    'End Function

    'Added error status functions to simplify error reporting -MSJ 20130322
    Public Shared Function GetMigrationErrorTypes(Optional ByVal errorCategory As String = "") As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.GetMigrationErrorTypes(errorCategory)
    End Function

    Public Shared Function GetMigrateArticleErrorInfo(ByVal article_uid As String) As System.Data.DataSet
        Return DataAuthentication.DataAuthentication.GetMigrateArticleErrorInfo(article_uid)
    End Function

    'Get the error type -MSJ 20130322
    Public Shared Function GetErrorType(ByVal ds As System.Data.DataSet) As Integer
        Return DataAuthentication.DataAuthentication.GetErrorType(ds)
    End Function

    'Get the image to highlight the error -MSJ 20130322
    Public Shared Function GetErrorImage(ByVal ds As System.Data.DataSet) As String
        Return DataAuthentication.DataAuthentication.GetErrorImage(ds)
    End Function

    'Get the error description for the alt-text of errors -MSJ 20130322
    Public Shared Function GetErrorTypeDescription(ByVal ds As System.Data.DataSet) As String
        Return DataAuthentication.DataAuthentication.GetErrorTypeDescription(ds)
    End Function

    'Get the error message from MigrationErrors table -MSJ 20130322
    Public Shared Function GetErrorMessage(ByVal ds As System.Data.DataSet) As String
        Return DataAuthentication.DataAuthentication.GetErrorMessage(ds)
    End Function


    Public Shared Function SaveArticleProfile(ByVal article_uid As String, ByVal profileid As Integer) As String
        Return DataAuthentication.DataAuthentication.SaveArticleProfile(article_uid, profileid)
    End Function

    Public Shared Function SaveArticle(ByVal article_uid As String, ByVal heading As String, ByVal summary As String, ByVal body As String, ByVal byline As String) As String
        Return DataAuthentication.DataAuthentication.SaveArticle(article_uid, heading, summary, body, byline)
    End Function

    Public Shared Function DoesArticleExist(ByVal article_uid As String) As Boolean
        Return DataAuthentication.DataAuthentication.DoesArticleExist(article_uid)
    End Function

    Public Shared Function GetFreeForm(ByVal article_uid As String) As DataSet
        Return DataAuthentication.DataAuthentication.GetFreeForm(article_uid)
    End Function

    Public Shared Function DoesSaxoAssetExist(ByVal destination_siteid As String, ByVal asset_uid As String) As Boolean
        Return DataAuthentication.DataAuthentication.DoesSaxoAssetExist(destination_siteid, asset_uid)
    End Function

    Public Shared Function GetSaxotechHost(ByVal siteid As Integer) As String
        Dim url As String = ""
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(String.Format("select owstarget from saxo_pubMap where siteid = {0}", siteid))
        If ds.Tables(0).Rows.Count = 1 Then
            url = ds.Tables(0).Rows(0).Item(0).ToString()
        End If
        Return url
    End Function

    ' =========================================================================================================================================================================

    Private Shared Function sendpdf(ByVal article_uid As String, ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal destination_siteid As String, ByVal dr As DataRow, ByRef sb As StringBuilder, ByVal logFlag As String) As Boolean
        Dim message As String = "", status As Boolean = False
        Dim rc As New OWC.rest
        Try

            Dim asset_uid As String = dr("asset_uid").ToString()
            If DataAuthentication.DataAuthentication.DoesSaxoAssetExist(destination_siteid, asset_uid) Then
                Logging(article_uid, String.Format("Asset: {0} already exits in table: saxo_image for site: {1} ", asset_uid, destination_siteid), logFlag)
                Return True
            End If
            Dim binaryurl As String = dr("binaryurl").ToString()
            Dim data As Byte() = rc.getFile(New Uri(binaryurl), message)
            If message.Length > 0 Then
                Logging(article_uid, String.Format("Asset: {0}  ImageUrl: {1}  error message: {2} ", asset_uid, binaryurl, message), logFlag)
                sb.Append(String.Format("rc.getfile asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
                Return status
            End If

            Dim u As Uri = New Uri(binaryurl)
            Dim filename As String = Path.GetFileName(u.AbsolutePath)

            Dim _address As String = String.Format("{0}{1}/mediafiles/{2}", saxohost, destination_siteid, filename)
            Dim location As String = rc.postPdf(_address, username, pwd, data, message)
            If message.Length > 0 Then
                Logging(article_uid, String.Format("Asset: {0}  ImageUrl: {1} OWS Post: {2}  error message: {3} ", asset_uid, binaryurl, _address, message), logFlag)
                sb.Append(String.Format("rc.postImage asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
                Return status
            End If

            DataAuthentication.DataAuthentication.spExecute("LoadSaxoImage", message, "@asset_uid", asset_uid, "@destination_siteid", destination_siteid, "@url", location)
            If message.Length > 0 Then
                Logging(article_uid, String.Format("Stored Procedure: LoadSaxoImage Table: saxo_image asset_uid: {0} destination site: {1} url: {2} error message: {3}", asset_uid, destination_siteid, location, message), logFlag)
                sb.Append(String.Format("sp:LoadSaxoImage asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
            Else
                Logging(article_uid, String.Format("Stored Procedure: LoadSaxoImage Table: saxo_image asset_uid: {0} destination site: {1} url: {2} Success", asset_uid, destination_siteid, location), logFlag)
                status = True
            End If
        Catch ex As Exception
            sb.Append(String.Format("Exception {0} {1}", ex.Message, ex.StackTrace))
            status = False
        End Try
        Return status
    End Function




    Private Shared Function DoesFileExist(ByVal uncBasePath As String, ByVal user As String, ByVal pwd As String, ByVal dom As String, ByVal filenamepath As String) As Boolean
        Dim status As Boolean = False
        Using unc As New UNCAccessWithCredentials()
            If unc.NetUseWithCredentials(uncBasePath, user, dom, pwd) Then
                If File.Exists(filenamepath) Then
                    status = True
                End If
            End If
        End Using
        ' End using 
        Return status
    End Function


    Private Shared Function GetUncFile(ByVal imgUrl As String, ByRef message As String) As Byte()
        Dim data As Byte() = Nothing
        Dim fileExists As Boolean = False
        Dim ImageUrl As String = ConfigurationManager.AppSettings("ImageUrl")
        Try
            If imgUrl.StartsWith(ImageUrl) Then
                Dim uncBasePath As String = ConfigurationManager.AppSettings("uncBasePath")
                Dim image_unc As String = imgUrl.Replace(ImageUrl, String.Format("{0}\", uncBasePath)).Replace("/", "\")
                Dim dom As String = ConfigurationManager.AppSettings("uncDomain")
                Dim user As String = ConfigurationManager.AppSettings("uncUser")
                Dim pwd As String = ConfigurationManager.AppSettings("uncPwd")
                fileExists = DoesFileExist(uncBasePath, user, pwd, dom, image_unc)
                If (fileExists) Then
                    Dim fs As FileStream = File.OpenRead(image_unc)
                    data = New Byte(fs.Length - 1) {}
                    fs.Read(data, 0, data.Length)
                End If
            End If
        Catch ex As Exception
            message = ex.Message & ex.StackTrace
        End Try
        Return data
    End Function


    Private Shared Function sendImage(ByVal article_uid As String, ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal destination_siteid As String, ByVal dr As DataRow, ByRef sb As StringBuilder, ByVal logFlag As String) As Boolean
        Dim message As String = "", status As Boolean = False
        Dim rc As New OWC.rest
        Try

            Dim asset_uid As String = dr("asset_uid").ToString()
            If DataAuthentication.DataAuthentication.DoesSaxoAssetExist(destination_siteid, asset_uid) Then
                Logging(article_uid, String.Format("Asset: {0} already exits in table: saxo_image for site: {1} ", asset_uid, destination_siteid), logFlag)
                Return True
            End If

            Dim data As Byte() = Nothing
            Dim ImgUrl As String = dr("imagepath").ToString()
            If (ConfigurationManager.AppSettings("useUNC") IsNot Nothing) Then
                Dim useUNC As String = ConfigurationManager.AppSettings("useUNC")
                If (useUNC.Equals("Y")) Then
                    Dim ImageUrl As String = ConfigurationManager.AppSettings("ImageUrl")
                    If (ImgUrl.StartsWith(ImageUrl)) Then
                        data = GetUncFile(ImgUrl, message)
                        If (Data Is Nothing Or Data.Length <= 0 Or message.Length > 0) Then
                            Logging(article_uid, String.Format("Asset: {0}  ImageUrl: {1}  error message: {2} ", asset_uid, ImgUrl, message), logFlag)
                            sb.Append(String.Format("GetUncFile asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
                            Return status
                        End If
                    End If
                End If
            End If



            data = rc.getFile(New Uri(ImgUrl), message)
            If message.Length > 0 Then
                Logging(article_uid, String.Format("Asset: {0}  ImageUrl: {1}  error message: {2} ", asset_uid, ImgUrl, message), logFlag)
                sb.Append(String.Format("rc.getfile asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
                Return status
            End If

            Dim img_address As New Uri(String.Format("{0}/{1}/myaccount/tempstore/images", saxohost, destination_siteid))
            Dim location As String = rc.postImage(img_address, username, pwd, message, Data)
            If message.Length > 0 Then
                Logging(article_uid, String.Format("Asset: {0}  ImageUrl: {1} OWS Post: {2}  error message: {3} ", asset_uid, ImgUrl, img_address, message), logFlag)
                sb.Append(String.Format("rc.postImage asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
                Return status
            End If
            DataAuthentication.DataAuthentication.spExecute("LoadSaxoImage", message, "@asset_uid", asset_uid, "@destination_siteid", destination_siteid, "@url", location)
            If message.Length > 0 Then
                Logging(article_uid, String.Format("Stored Procedure: LoadSaxoImage Table: saxo_image asset_uid: {0} destination site: {1} url: {2} error message: {3}", asset_uid, destination_siteid, location, message), logFlag)
                sb.Append(String.Format("sp:LoadSaxoImage asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine))
            Else
                Logging(article_uid, String.Format("Stored Procedure: LoadSaxoImage Table: saxo_image asset_uid: {0} destination site: {1} url: {2} Success", asset_uid, destination_siteid, location), logFlag)
                status = True
            End If
        Catch ex As Exception
            sb.Append(String.Format("Exception {0} {1} ", ex.Message, ex.StackTrace))
            status = False
        End Try
        Return status
    End Function


    Public Shared Sub ClearLogStatements(ByVal article_uid As String)
        Dim message As String = ""
        DataAuthentication.DataAuthentication.spExecute("LogstatementsClear", message, "@article_uid", article_uid)
    End Sub




    Public Shared Function MigratePDF(ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal destination_siteid As String, ByVal article_uid As String, ByVal logFlag As String, ByRef found As Integer, ByRef sent As Integer) As Boolean
        Dim _message As String = "", _message2 As String = ""
        Dim status As Boolean = False
        Dim sb As New StringBuilder()
        Dim pdfFailures As Integer = 0
        Dim errorType As Integer

        Try
            sent = 0
            Dim sqlstatement As String = String.Format("select asset_uid, binaryurl from pdf where asset_uid in (select asset_uid from asset where asset_type = {0} and  article_uid = '{1}' )", CInt(asset.pdf), article_uid)
            Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            found = ds.Tables(0).Rows.Count
            Logging(article_uid, sqlstatement, logFlag)
            For Each dr As DataRow In ds.Tables(0).Rows
                status = sendpdf(article_uid, saxohost, username, pwd, destination_siteid, dr, sb, logFlag)
                If status = False Then
                    pdfFailures += 1
                Else
                    sent += 1
                End If
            Next
            If found = 0 Then status = True
            _message = sb.ToString()
        Catch ex As Exception
            status = False
            _message = String.Format("Exception {0} {1} ", ex.Message, ex.StackTrace)
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            errorType = IIf(IsNumeric(ex.InnerException.Message), ex.InnerException.Message, 0)
        End Try

        If ((pdfFailures > 0) Or (_message.Length > 0)) Then
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            DataAuthentication.DataAuthentication.spExecute("MigrationErrors", _message2, "@article_uid", article_uid, "@myMessage", _message, "@errorType", errorType)
            status = False
        Else
            status = True
        End If
        Return status

    End Function


    Public Shared Function MigrateImage(ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal destination_siteid As String, ByVal article_uid As String, ByVal logFlag As String, ByRef found As Integer, ByRef sent As Integer) As Boolean
        Dim _message As String = "", _message2 As String = ""
        Dim status As Boolean = False
        Dim sb As New StringBuilder()
        Dim ImageFailures As Integer = 0
        Dim errorType As Integer

        Try
            sent = 0
            DataAuthentication.DataAuthentication.spExecute("migrationClear", _message2, "@article_uid", article_uid)
            Logging(article_uid, "Clear migrationerror table", logFlag)
            Dim sqlstatement As String = String.Format("select asset_uid, imagepath from image where asset_uid in (select asset_uid from asset where asset_type = {1} and  article_uid = '{0}' except select asset_uid from image_tracker where article_uid = '{0}'  and soft_delete = 'Y'  )", article_uid, CInt(asset.image))
            Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            found = ds.Tables(0).Rows.Count

            Logging(article_uid, sqlstatement, logFlag)
            For Each dr As DataRow In ds.Tables(0).Rows
                status = sendImage(article_uid, saxohost, username, pwd, destination_siteid, dr, sb, logFlag)
                If status = False Then
                    ImageFailures += 1
                Else
                    sent += 1
                End If
            Next
            If found = 0 Then status = True
            _message = sb.ToString()
        Catch ex As Exception
            status = False
            _message = String.Format("Exception {0} {1}", ex.Message, ex.StackTrace)
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            errorType = IIf(IsNumeric(ex.InnerException.Message), ex.InnerException.Message, 0)
        End Try

        If ((ImageFailures > 0) Or (_message.Length > 0)) Then
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            DataAuthentication.DataAuthentication.spExecute("MigrationErrors", _message2, "@article_uid", article_uid, "@myMessage", _message, "@errorType", errorType)
            status = False
        Else
            status = True
        End If
        Return status
    End Function


    Private Shared Function cdata(ByVal src As String) As String
        Return String.Format("<![CDATA[{0}]]>", src)
    End Function


    'Private Shared Function GetCategoryMap(ByVal article_uid As String, ByVal key As String, ByVal logFlag As String) As String
    '    Dim sqlstatement As String = String.Format("select {0} from sectionAnchorMap s, article a  where a.article_uid = '{0}' category = '{1}'", key)
    '    Logging(article_uid, sqlstatement, logFlag)
    '    Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)

    '    Dim nrows As Integer = ds.Tables(0).Rows.Count
    '    If nrows = 0 Then
    '        Return System.Configuration.ConfigurationManager.AppSettings(key).ToString()
    '    Else
    '        Return ds.Tables(0).Rows(0)(0).ToString()
    '    End If
    'End Function




    Public Shared Function GetSaxoProfile(ByVal siteid As String, ByVal article_uid As String, ByVal anchor As String) As String
        Dim GeneralNewsProfile As String = "1030040"
        Return GetProfile(siteid, article_uid, anchor, "profileid", GeneralNewsProfile, "N")
    End Function

    Public Shared Function GetDs(ByVal sqlstatement As String) As DataSet
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        Return ds
    End Function

    Public Shared Function CreateCommand(ByVal querystring As String) As Integer
        Return DataAuthentication.DataAuthentication.CreateCommand(querystring)
    End Function

    Private Shared Function GetProfile(ByVal siteid As String, ByVal article_uid As String, ByVal anchor As String, ByVal key As String, ByVal defaultprofile As String, ByVal logFlag As String) As String
        Dim sqlstatement As String = String.Format("select {0} from sectionAnchorMap where sectionAnchor  = '{1}' and siteid = {2} ", key, anchor, siteid)
        Dim myprofile As String = IIf(key.Equals("profileid"), defaultprofile, "")
        Logging(article_uid, sqlstatement, logFlag)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        Dim nrows As Integer = ds.Tables(0).Rows.Count
        If (nrows = 1) AndAlso Not (IsDBNull(ds.Tables(0).Rows(0).Item(0))) Then
            myprofile = IIf(String.IsNullOrEmpty(ds.Tables(0).Rows(0)(0)), myprofile, ds.Tables(0).Rows(0)(0).ToString().Trim)
        End If
        Return myprofile
    End Function

    Private Shared Function GetProfileOverride(ByVal article_uid As String) As Integer
        Dim myprofile As String = "0"
        Dim sqlstatement As String = String.Format("select profileid from article_profileoverride where article_uid = '{0}'", article_uid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        Dim nrows As Integer = ds.Tables(0).Rows.Count
        If (nrows = 1) AndAlso Not (IsDBNull(ds.Tables(0).Rows(0).Item(0))) Then
            myprofile = IIf(String.IsNullOrEmpty(ds.Tables(0).Rows(0)(0)), myprofile, ds.Tables(0).Rows(0)(0).ToString().Trim)
        End If
        Return IIf(IsNumeric(myprofile), Convert.ToInt32(myprofile), 0)
    End Function


    Private Shared Function BuildGallery(ByVal siteid As String, ByVal profileurl As String, ByVal documents As String, ByVal temp As String, ByVal saxohost As String, ByVal destination_siteid As String, ByVal article_uid As String, ByVal category As String, ByVal anchor As String, ByVal startdate As String, ByVal sqlstatement As String, ByRef xmldata As String, ByRef _message As String, ByVal logFlag As String) As Boolean
        Dim GeneralNewsProfile As String = "29788"
        Dim galleryXML As String = "", gallerySaxo As String = ""
        Dim status As Boolean = False
        galleryXML = String.Format("{0}\{1}_gal.xml", temp, article_uid)
        gallerySaxo = String.Format("{0}\{1}_galsaxo.xml", temp, article_uid)

        Try

            '  Dim mappedcat As String = GetCategoryMap(article_uid, "saxo_category", category, logFlag)
            Logging(article_uid, sqlstatement, logFlag)

            Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            Dim count As Integer = ds.Tables(0).Rows.Count
            If count < 2 Then ' any images ?
                Logging(article_uid, String.Format("Number of Images {0} does not qualify for a Gallery", count), logFlag)
                status = True
                Return status
            End If

            Logging(article_uid, String.Format("Building gallery for {0} Images ", count), logFlag)

            Dim x As New XmlDocument()
            x.Load(String.Format("{0}\gallery.xml", documents))
            x.GetElementsByTagName("siteuri").Item(0).InnerText = String.Format("{0}/{1}", saxohost, destination_siteid)
            x.GetElementsByTagName("site").Item(0).InnerText = destination_siteid

            x.GetElementsByTagName("profileid").Item(0).InnerXml = profileurl & GetProfile(siteid, article_uid, anchor, "profileid", GeneralNewsProfile, logFlag)

            Dim profile2 As String = GetProfile(siteid, article_uid, anchor, "profileid2", "", logFlag)
            If String.IsNullOrEmpty(profile2) Then
                x.GetElementsByTagName("profileid2").Item(0).InnerXml = ""
            Else
                x.GetElementsByTagName("profileid2").Item(0).InnerXml = profileurl & profile2
            End If

            x.GetElementsByTagName("startdate").Item(0).InnerText = startdate
            Dim xn As XmlNode = x.GetElementsByTagName("images").Item(0)
            Dim itemnumber As Integer = 1

            For Each dr As DataRow In ds.Tables(0).Rows
                Dim location As String = dr(1).ToString()
                Dim caption As String = dr(2).ToString()
                Dim xk As XmlNode = x.CreateNode(XmlNodeType.Element, "image", "")
                xk.InnerXml = String.Format("<location>{0}</location><itemnumber>{1}</itemnumber><caption>{2}</caption>", cdata(location), itemnumber, cdata(caption))
                itemnumber += 1
                xn.AppendChild(xk)
            Next
            Dim xsltemplate As String = String.Format("{0}\gallery.xslt", documents)
            x.Save(galleryXML)
            Dim xslt As New XslCompiledTransform()
            xslt.Load(xsltemplate)
            xslt.Transform(galleryXML, gallerySaxo)
            Dim sr As New StreamReader(gallerySaxo)
            xmldata = sr.ReadToEnd()
            sr.Close()
            status = True

        Catch e As Exception
            _message = String.Format("{0} {1}", e.Message, e.StackTrace)
        Finally
            Dim tempFiles As String() = {galleryXML, gallerySaxo}
            For Each tempFile As String In tempFiles
                If tempFile.Length > 0 AndAlso File.Exists(tempFile) Then
                    File.Delete(tempFile)
                End If
            Next
        End Try
        Return status
    End Function





    Public Shared Function MigrateGallery(ByVal siteid As String, ByVal documents As String, ByVal temp As String, ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal destination_siteid As String, ByVal article_uid As String, ByVal logFlag As String, Optional ByVal overwrite As String = "Y") As Boolean
        Dim rc As New OWC.rest
        Dim ds1 As DataSet
        Dim _message As String = "", _message2 As String = "", xmldata As String = "", gallery_uid As String = "", sqlstatement = ""
        Dim status As Boolean = False
        Dim errorType As Integer

        Try
            If (overwrite.Equals("N")) Then
                sqlstatement = String.Format("select count(*) from saxo_gallery where article_uid = '{0}' and destination_siteid = '{1}'", article_uid, destination_siteid)
                ds1 = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
                If (Convert.ToInt32(ds1.Tables(0).Rows(0).Item(0)) = 1) Then
                    Return True
                End If
                ds1.Dispose()
            End If

            sqlstatement = String.Format("select gallery_uid from saxo_gallery where article_uid = '{0}' and destination_siteid = '{1}'", article_uid, destination_siteid)
            ds1 = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            Dim galleryUpdate As Boolean = False, galleryUpdateAddress As String = ""
            If (ds1.Tables(0).Rows.Count = 1) Then
                galleryUpdate = True
                galleryUpdateAddress = ds1.Tables(0).Rows(0).Item(0).ToString
            End If


            Dim GalleryAddress As New Uri(String.Format("{0}/{1}/galleries", saxohost, destination_siteid))

            DataAuthentication.DataAuthentication.spExecute("DeleteSaxoGallery", _message, "@article_uid", article_uid)
            Logging(article_uid, String.Format("Stored Procedure DeleteSaxoGallery delete article_uid: {0} from saxo_gallery table  ", article_uid), logFlag)
            sqlstatement = String.Format("select startdate,category,anchor from article where article_uid = '{0}'", article_uid)
            Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            Logging(article_uid, sqlstatement, logFlag)
            Dim idx As Integer = ds.Tables(0).Rows.Count
            If idx <> 1 Then
                _message = String.Format("Table article returned {0} rows", idx)
                Logging(article_uid, _message, logFlag)
                Throw New Exception(_message)
            End If
            Dim dr As DataRow = ds.Tables(0).Rows(0)
            Dim category As String = dr("category").ToString()
            Dim anchor As String = IIf(IsDBNull(dr("anchor")), "", dr("anchor").ToString())
            Dim strStartdate As String = DirectCast(dr("startdate"), DateTime).ToString("yyyy-MM-dd")

            sqlstatement = String.Format("select owstarget from saxo_pubmap where siteid = {0}", siteid)
            Logging(article_uid, sqlstatement, logFlag)
            Dim ds2 As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            If (ds2.Tables(0).Rows.Count <> 1) Then
                Throw New Exception(String.Format("Site code: {0} returned rows: {1} from saxo_pubmap "))
            End If
            Dim ProfileUrl As String = ds2.Tables(0).Rows(0).Item(0).ToString() & "/zz/profiles/"
            ds2.Dispose()

            sqlstatement = String.Format("Select sx.asset_uid, sx.url,im.caption, im.imagepath from saxo_image sx , image im where (im.asset_uid = sx.asset_uid) and sx.destination_siteid = '{0}' and im.asset_uid in ( select asset_uid from asset where asset_type = {2} and article_uid = '{1}' except select asset_uid from image_tracker where article_uid = '{1}'  and soft_delete = 'Y' )", destination_siteid, article_uid, CInt(asset.image))
            If Not (BuildGallery(siteid, ProfileUrl, documents, temp, saxohost, destination_siteid, article_uid, category, anchor, strStartdate, sqlstatement, xmldata, _message, logFlag)) Then Throw New Exception(String.Format("BuildGallery {0}", _message))

            If xmldata.Length > 0 Then
                Logging(article_uid, String.Format("Gallery: {0} ", xmldata), logFlag)
                If (galleryUpdate) Then
                    gallery_uid = rc.executeCMD("PUT", galleryUpdateAddress, username, pwd, xmldata, _message)
                Else
                    gallery_uid = rc.postStory(GalleryAddress, username, pwd, xmldata, _message)
                End If
                If _message.Length > 0 Then
                    Logging(article_uid, String.Format("OWS Post {0} for Gallery Data Failed message {1} ", GalleryAddress, _message), logFlag)
                    Throw New Exception(String.Format("rc.postStory {0} ", _message))
                End If
                DataAuthentication.DataAuthentication.spExecute("loadSaxoGallery", _message, "@article_uid", article_uid, "@destination_siteid", destination_siteid, "@gallery_uid", gallery_uid, "@similarity", 0)
                If _message.Length > 0 Then
                    Logging(article_uid, String.Format("OWS Post {0} for Gallery Success  , Failed Stored Procedure Save: loadSaxoGallery message {1} ", GalleryAddress, _message), logFlag)
                    Throw New Exception(String.Format("sp:loadSaxoGallery Article uid: {1} {2}", article_uid, _message))
                End If
            End If ' xmldata  
            Logging(article_uid, String.Format("OWS Post: {0} for Gallery Success, Saved gallery_uid: {1}  to Stored Procedure loadSaxoGallery ", GalleryAddress, gallery_uid), logFlag)
            status = True

        Catch ex As Exception
            status = False
            _message = String.Format("Exception {0}", ex.Message)
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            errorType = IIf(IsNumeric(ex.InnerException.Message), ex.InnerException.Message, 0)
        End Try
        If status = False Then
            If (_message.Length = 0) Then _message = "unknown Error "
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            DataAuthentication.DataAuthentication.spExecute("MigrationErrors", _message2, "@article_uid", article_uid, "@myMessage", _message, "@errorType", errorType)
        End If
        Return status
    End Function




    Private Shared Sub loadImages(ByVal article_uid As String, ByVal destination_siteid As String, ByRef x As XmlDocument, ByVal logFlag As String)
        Dim sqlstatement As String = String.Format("select si.url, i.caption from image i, saxo_image si where i.asset_uid = si.asset_uid and  si.destination_siteid ='{0}' and  i.asset_uid in (select asset_uid from asset where article_uid = '{1}' except select asset_uid from image_tracker where  article_uid = '{1}' and soft_delete ='Y')", destination_siteid, article_uid)
        Logging(article_uid, sqlstatement, logFlag)
        Dim dsImage As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If dsImage.Tables(0).Rows.Count = 1 Then
            x.GetElementsByTagName("image").Item(0).InnerText = dsImage.Tables(0).Rows(0)(0).ToString()
            x.GetElementsByTagName("imageCaption").Item(0).InnerText = If(Convert.IsDBNull(dsImage.Tables(0).Rows(0)(1)), "", dsImage.Tables(0).Rows(0)(1).ToString())
            Logging(article_uid, "Setting Main Image (only One Image For this Article)", logFlag)
            Return
        End If
        sqlstatement = String.Format("select gallery_uid from saxo_gallery where article_uid = '{0}' and destination_siteid ='{1}' ", article_uid, destination_siteid)
        Dim dsGallery As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        Logging(article_uid, sqlstatement, logFlag)
        Dim count As Integer = dsGallery.Tables(0).Rows.Count
        If count = 1 Then
            Dim galleryuid As String = dsGallery.Tables(0).Rows(0)(0).ToString()
            x.GetElementsByTagName("gallerypathuid").Item(0).InnerXml = galleryuid
            Dim uri As New Uri(galleryuid)
            x.GetElementsByTagName("galleryuid").Item(0).InnerXml = Path.GetFileName(uri.LocalPath)
            Logging(article_uid, String.Format("Contains Gallery Gallery_uid: {0}", galleryuid), logFlag)
        Else
            Logging(article_uid, String.Format("Gallery table saxo_gallery returned {0} rows", count), logFlag)
        End If
    End Sub


    Private Shared Function getViewUri(ByVal story_xml As String) As String
        Dim viewuri As String = ""
        Try
            Dim xdoc As New XmlDocument()
            xdoc.LoadXml(story_xml)
            Dim root As XmlElement = xdoc.DocumentElement
            If root.HasAttribute("viewuri") Then
                viewuri = root.GetAttribute("viewuri")
            End If
        Catch generatedExceptionName As Exception
        End Try
        Return viewuri
    End Function


    Public Shared Function htmlholdrules() As ArrayList
        Dim alist As New ArrayList
        Dim sqlstatement As String = "select htmltag from htmlholdrules"
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        For Each dr As DataRow In ds.Tables(0).Rows
            alist.Add(dr(0).ToString)
        Next
        Return alist
    End Function

    Public Shared Function gethtmlholdrulesID(ByVal htmltag As String) As String
        Dim sqlstatement As String = String.Format("select id from htmlholdrules where htmltag = '{0}'", htmltag)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        Return ds.Tables(0).Rows(0).Item(0).ToString()
    End Function

    Private Shared Function getMediaFiles(ByVal article_uid As String) As ArrayList
        'Add to asset_type for additional assets TBD 4.8.2013
        Dim ar As New ArrayList
        Dim sqlstatement As String = String.Format("select url from saxo_image where asset_uid in  (select asset_uid from asset where asset_type = {0} and  article_uid = '{1}' )", CInt(asset.pdf), article_uid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        For Each dr As DataRow In ds.Tables(0).Rows
            ar.Add(dr.Item(0).ToString)
        Next
        Return ar
    End Function


    Public Shared Function GetTaxPubid(ByVal siteid As String) As String
        Dim taxpubid As String = ""
        ' fix 8/6/2013 fix for southern California publication 513

        If (siteid.Equals("513")) Then

        End If


        Dim sqlstatement As String = String.Format("select taxonomyPubId from saxo_pubMap where siteid = {0} ", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If ds.Tables(0).Rows.Count = 1 Then
            taxpubid = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        End If
        Return taxpubid
    End Function

    Public Shared Function MigrateArticleManually(ByVal taxpubid As String, ByVal siteid As String, ByVal destination_siteid As String, ByVal documents As String, ByVal temp As String, ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal article_uid As String, ByVal logFlag As String) As Boolean
        Dim found As Integer = 0, sent = 0
        Dim _message As String = "", status As Boolean

        status = MigrateImage(saxohost, username, pwd, destination_siteid, article_uid, logFlag, found, sent)
        If (status = True) Then
            If (found > 1) Then status = (MigrateGallery(siteid, documents, temp, saxohost, username, pwd, destination_siteid, article_uid, logFlag))
        End If

        If (status = True) Then
            Dim holdrules As ArrayList = htmlholdrules()
            ' PDF asset migration on hold 4.8.2013
            ' MigratePDF(saxohost, username, pwd, destination_siteid, article_uid, "N", found, sent)
            Return MigrateArticle(taxpubid, siteid, destination_siteid, documents, temp, saxohost, username, pwd, article_uid, logFlag, holdrules)

        End If
        Return False
    End Function

    Public Shared Function distinctPubs() As DataSet
        Dim sqlstatement As String = String.Format("select siteid,pubname  from saxo_pubmap where sectionmapcomplete='Y'")
        Return DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
    End Function


    Public Shared Function updatepubmap(ByVal siteid As Integer, ByVal sectionmapcomplete As String) As String
        Return DataAuthentication.DataAuthentication.updatepubmap(siteid, sectionmapcomplete)
    End Function

    Public Shared Function PubMapStatus(ByVal siteid As Integer) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select sectionmapcomplete from saxo_pubmap where siteid = {0}", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        Return data
    End Function



    Public Shared Function PubStatusSaxo(ByVal siteid As String) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select count(*) from article a, saxo_article s where a.article_uid= s.article_uid and a.siteid = '{0}'", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        Return data
    End Function

    Public Shared Function PubStatus(ByVal siteid As String) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select count(*) from article where siteid = '{0}'", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        Return data
    End Function


    Public Shared Function PubEarliestStartSaxo(ByVal siteid As String) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select min(startdate) from article a, saxo_article s where a.article_uid= s.article_uid and a.siteid = '{0}'", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If ds.Tables(0).Rows.Count = 1 Then
            data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        End If
        Return data
    End Function


    Public Shared Function PubEarliestStart(ByVal siteid As String) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select min(startdate) from article where siteid = '{0}'", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If ds.Tables(0).Rows.Count = 1 Then
            data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        End If
        Return data
    End Function

    Public Shared Function PubLatestStart(ByVal siteid As String) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select max(startdate) from article where siteid = '{0}'", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If ds.Tables(0).Rows.Count = 1 Then
            data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        End If
        Return data
    End Function



    Public Shared Function PubLatestStartSaxo(ByVal siteid As String) As String
        Dim data As String = ""
        Dim sqlstatement As String = String.Format("select max(startdate) from article a, saxo_article s where a.article_uid= s.article_uid and a.siteid = '{0}'", siteid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If ds.Tables(0).Rows.Count = 1 Then
            data = IIf(IsDBNull(ds.Tables(0).Rows(0).Item(0)), "", ds.Tables(0).Rows(0).Item(0))
        End If
        Return data
    End Function


    Public Shared Function ArticleMigrated(ByVal article_uid As String) As Boolean
        Dim sqlstatement As String = String.Format("select storyurl from saxo_article where article_uid = '{0}'", article_uid)
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        If (ds.Tables(0).Rows.Count = 1) Then
            Console.WriteLine(String.Format("Article Exists: {0} skipping ", article_uid))
            Return True
        End If
        Return False
    End Function



    Public Shared Function MigrateArticle(ByVal taxpubid As String, ByVal siteid As String, ByVal destination_siteid As String, ByVal documents As String, ByVal temp As String, ByVal saxohost As String, ByVal username As String, ByVal pwd As String, ByVal article_uid As String, ByVal logFlag As String, ByVal holdrules As ArrayList, Optional ByVal bypassImgages As String = "N") As Boolean
        Dim here As Integer = 0
        Dim bDebug As Boolean = False
        Dim rc As New OWC.rest
        Dim status As Boolean = False
        Dim _message As String = "", _message2 As String = ""
        Dim articleXML As String = String.Format("{0}\{1}.xml", temp, article_uid)
        Dim articleSaxoXML As String = String.Format("{0}\{1}_saxo.xml", temp, article_uid)
        Dim GeneralNewsProfile As String = "1030040"
        Dim GeneralSportsProfile As String = "1032511"
        Dim errorType As Integer
        Dim bOverriden As Boolean = DataAuthentication.DataAuthentication.ArticleState(article_uid)
        Dim bArticleviewuri As Boolean = True ' 06.24.2013
        Dim storyurl As String = String.Empty

        Try
            bOverriden = IIf(ConfigurationManager.AppSettings("articleoverride").Equals("Y"), True, False) ' 5.13.2013 article override
            bArticleviewuri = IIf(ConfigurationManager.AppSettings("getarticleviewuri").Equals("Y"), True, False) ' 5.13.2013 article override
        Catch ex As Exception
        End Try

        Try
            DataAuthentication.DataAuthentication.spExecute("DeleteMigrationError", _message, "@article_uid", article_uid)
            Dim sqlstatement As String = String.Format("select count(*) from asset where article_uid = '{0}' and asset_type = {1}", article_uid, CInt(asset.freeform))
            Dim dsFree As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            If (Convert.ToInt32(dsFree.Tables(0).Rows(0).Item(0)) > 0 AndAlso Not bOverriden) Then
                Dim exRule As New Exception(gethtmlholdrulesID("FREEFORM"))
                Throw New Exception(String.Format(" Article violates holdrule: FREEFORM", article_uid), exRule)
            End If
            dsFree.Dispose()

            sqlstatement = String.Format("select * from article where article_uid = '{0}'", article_uid)
            Logging(article_uid, sqlstatement, logFlag)
            Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)



            Dim idx As Integer = ds.Tables(0).Rows.Count
            If idx <> 1 Then
                _message = String.Format("Table article article_uid: {0} returned: {1} rows", article_uid, idx)
                Logging(article_uid, _message, logFlag)
                Throw New Exception(_message)
            End If
            If (taxpubid.Trim.Length = 0) Then
                Throw New Exception(String.Format("Siteid: {0} [taxonomyPubId] table [saxo_pubMap] missing must assign to send to saxotech", siteid))
            End If


            Dim articleRow As DataRow = ds.Tables(0).Rows(0)
            Dim x As New XmlDocument()
            x.Load(String.Format("{0}\xmldocument.xml", documents))
            x.GetElementsByTagName("siteuri").Item(0).InnerText = String.Format("{0}/{1}", saxohost, destination_siteid)
            x.GetElementsByTagName("site").Item(0).InnerText = destination_siteid
            Dim startdate As DateTime = If(Convert.IsDBNull(articleRow("startdate")), DateTime.Now, DirectCast(articleRow("startdate"), DateTime))
            x.GetElementsByTagName("timestamp").Item(0).InnerText = String.Format("{0}T00:00:00Z", startdate.ToString("yyyy-MM-dd"))

            Dim enddate As DateTime = If(Convert.IsDBNull(articleRow("enddate")), DateTime.Now.AddYears(100), DirectCast(articleRow("enddate"), DateTime))
            x.GetElementsByTagName("startdate").Item(0).InnerXml = startdate.ToString("yyyyMMdd")
            x.GetElementsByTagName("enddate").Item(0).InnerXml = enddate.ToString("yyyyMMdd")

            sqlstatement = String.Format("select owstarget from saxo_pubmap where siteid = {0}", siteid)
            Logging(article_uid, sqlstatement, logFlag)
            Dim ds2 As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
            If (ds2.Tables(0).Rows.Count <> 1) Then
                Throw New Exception(String.Format("Site code: {0} returned rows: {1} from saxo_pubmap "))
            End If
            Dim ProfileUrl As String = ds2.Tables(0).Rows(0).Item(0).ToString() & "/zz/profiles/"
            ds2.Dispose()
            sqlstatement = String.Format("select count(*) from asset where asset_type = {1} and article_uid = '{0}' and asset_uid not in ( select asset_uid from image_tracker where article_uid = '{0}' and soft_delete = 'Y')", article_uid, CInt(asset.image))
            Logging(article_uid, sqlstatement, logFlag)
            ds2 = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)

            If (bypassImgages.Equals("N")) Then
                If Convert.ToInt32(ds2.Tables(0).Rows(0).Item(0)) > 0 Then loadImages(article_uid, destination_siteid, x, logFlag)
            End If
            Dim mysiteid As String = If(Convert.IsDBNull(articleRow("siteid")), "0", articleRow("siteid"))
            Dim displaygroups As String() = If(Convert.IsDBNull(articleRow("displaygroup")), "".Trim.Split(" "c), articleRow("displaygroup").Trim.Split(" "c))
            Dim displaygroupfound As Boolean = False
            Dim hashprofiles As New Hashtable
            Dim hashdpubids As New Hashtable
            Dim generalsports As Integer = 0
            Dim keys As String() = {"article_uid", "siteid", "origsite", "anchor", "seodescription", "relatedarticles", "heading", "summary", "body", "byline", "category"}



            For Each k As String In keys
                Dim value As String = If(Convert.IsDBNull(articleRow(k)), "", articleRow(k).ToString())
                If (bDebug) Then
                    File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", k, value, vbCrLf))
                End If
                Select Case k

                    Case "anchor"
                        ' Special fix for SoCal publication 513 to use socal lookup table 
                        ' =========================================================================================================================================================================
                        ' =========================================================================================================================================================================
                        If (mysiteid.Equals("513")) Then
                            ' Get a unique collection of profileids 
                            For Each displaygroup As String In displaygroups
                                If (displaygroup.Trim.Length > 0) Then
                                    Dim dsGroups As DataSet = DataAuthentication.DataAuthentication.GetDataSet(String.Format("select profileid from socal where displaygroupid = {0} and migrate = 'x'", displaygroup))
                                    If (dsGroups.Tables(0).Rows.Count > 0) Then
                                        Dim profileids As String() = dsGroups.Tables(0).Rows(0).Item(0).Split(","c)
                                        For Each p As String In profileids
                                            If Not hashprofiles.Contains(p) Then hashprofiles.Add(p, p)
                                        Next
                                    End If
                                End If
                            Next
                            Dim i As Integer = 0
                            ' added xml profileids to xmldocument for more then 2 profiles  
                            Dim xn As XmlNode = x.GetElementsByTagName("profileids").Item(0)
                            If (hashprofiles.Count.Equals(0)) Then
                                x.GetElementsByTagName("profileid").Item(0).InnerXml = GeneralSportsProfile
                            End If
                            ' added 8.28.2013 flag to not send profile for articles that may already exist on saxotech but need to be sent for date range for redirects 
                            Dim useprofile As String = "Y"
                            Try
                                useprofile = ConfigurationManager.AppSettings("useprofile").ToString()
                            Catch
                            End Try

                            If (hashprofiles.Count > 0 And useprofile.Equals("Y")) Then '
                                For Each profileid As String In hashprofiles.Keys
                                    i += 1
                                    If (i.Equals(1)) Then
                                        x.GetElementsByTagName("profileid").Item(0).InnerXml = String.Format("{0}{1}", ProfileUrl, profileid)
                                    Else
                                        Dim xk As XmlNode = x.CreateNode(XmlNodeType.Element, "profileid", "")
                                        xk.InnerText = String.Format("{0}{1}", ProfileUrl, profileid)
                                        xn.AppendChild(xk)
                                    End If
                                Next
                            End If
                            ' =========================================================================================================================================================================
                            ' =========================================================================================================================================================================
                        Else
                            Dim useprofile As String = "Y"
                            Try
                                useprofile = ConfigurationManager.AppSettings("useprofile").ToString()
                            Catch
                            End Try
                            ' added 8.28.2013 flag to not send profile for articles that may already exist on saxotech but need to be sent for date range for redirects 
                            If (useprofile.Equals("Y")) Then
                                x.GetElementsByTagName("anchor").Item(0).InnerXml = value
                                If String.IsNullOrEmpty(value) Then
                                    x.GetElementsByTagName("profileid").Item(0).InnerXml = GeneralNewsProfile
                                Else
                                    x.GetElementsByTagName("profileid").Item(0).InnerXml = ProfileUrl & GetProfile(siteid, articleRow("article_uid"), value, "profileid", GeneralNewsProfile, logFlag)
                                    Dim profile1 As Integer = GetProfileOverride(article_uid)
                                    If (profile1 > 0) Then
                                        x.GetElementsByTagName("profileid").Item(0).InnerXml = ProfileUrl & profile1.ToString()
                                    End If
                                    Dim profile2 As String = GetProfile(siteid, articleRow("article_uid"), value, "profileid2", "", logFlag)
                                    If String.IsNullOrEmpty(profile2) Then
                                        x.GetElementsByTagName("profileid2").Item(0).InnerXml = ""
                                    Else
                                        x.GetElementsByTagName("profileid2").Item(0).InnerXml = ProfileUrl & profile2
                                    End If
                                End If
                            End If
                        End If
                        Exit Select
                    Case "origsite"
                        If (IsNumeric(value)) Then
                            x.GetElementsByTagName("origsite").Item(0).InnerXml = GetTaxPubid(value)
                        End If
                    Case "siteid"
                        x.GetElementsByTagName("siteid").Item(0).InnerXml = taxpubid
                        If (bDebug) Then
                            here += 1
                            File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, taxpubid, vbCrLf))
                        End If
                        ' x.GetElementsByTagName("taxonomyword").Item(0).InnerXml = taxpubid
                        ' 7.26.2013  taxonomywords used to create taxonomy groups example: West Coast Lang properites pubtaxids see each others archives
                        value = String.Empty    'ConfigurationManager.AppSettings("taxonomywords").ToString().Trim
                        If (bDebug) Then
                            here += 1
                            File.AppendAllText("here.log", String.Format("here taxonomywords {0} this {1}{2}", here, value, vbCrLf))
                        End If
                        ' if taxonomywords is empty default to this publication
                        If value.Length = 0 Then value = taxpubid
                        ' 08.06.2013 special fix for southern california articles 
                        ' Special fix for SoCal publication 513 to use socal lookup table 
                        ' =========================================================================================================================================================================
                        If (mysiteid.Equals("513")) Then
                            Dim xn As XmlNode = x.GetElementsByTagName("taxonomywords").Item(0)
                            For Each displaygroup As String In displaygroups
                                If (displaygroup.Trim.Length > 0) Then
                                    Dim dsGroups As DataSet = DataAuthentication.DataAuthentication.GetDataSet(String.Format("select pubid  from socal where displaygroupid = {0}  and migrate = 'x'", displaygroup))
                                    For Each dr As DataRow In dsGroups.Tables(0).Rows
                                        Dim pubids As String() = If(Convert.IsDBNull(dr.Item(0)), " ".Trim.Split(","c), dr.Item(0).Split(","c))
                                        If (pubids.Count > 0) Then
                                            For Each pubid As String In pubids
                                                If Not (hashdpubids.Contains(pubid)) Then
                                                    hashdpubids.Add(pubid, pubid)
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                            If (hashdpubids.Count.Equals(0)) Then ' these will be forced into SPORTS profile
                                generalsports = 1
                                displaygroupfound = True
                                Dim taxonomywords As String() = value.Split(" "c)
                                For Each taxonomyword As String In taxonomywords
                                    Dim xk As XmlNode = x.CreateNode(XmlNodeType.Element, "taxonomyword", "")
                                    xk.InnerText = taxonomyword
                                    xn.AppendChild(xk)
                                Next
                            End If
                            If (hashdpubids.Count > 0) Then
                                displaygroupfound = True
                                Dim i As Integer = 0
                                For Each pubid As String In hashdpubids.Keys
                                    i += 1
                                    If (i.Equals(1)) Then x.GetElementsByTagName("siteid").Item(0).InnerXml = pubid
                                    Dim xk As XmlNode = x.CreateNode(XmlNodeType.Element, "taxonomyword", "")
                                    xk.InnerText = pubid
                                    xn.AppendChild(xk)
                                Next
                            End If
                            ' =========================================================================================================================================================================
                            ' =========================================================================================================================================================================
                        Else
                            Dim xn As XmlNode = x.GetElementsByTagName("taxonomywords").Item(0)
                            If (bDebug) Then
                                here += 1
                                File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, "xn", vbCrLf))
                            End If
                            Dim taxonomywords As String() = value.Split(" "c)
                            If (bDebug) Then
                                here += 1
                                File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, "taxonomywords", vbCrLf))
                            End If
                            For Each taxonomyword As String In taxonomywords
                                Dim xk As XmlNode = x.CreateNode(XmlNodeType.Element, "taxonomyword", "")
                                If (bDebug) Then
                                    here += 1
                                    File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, "xk", vbCrLf))
                                End If
                                xk.InnerText = taxonomyword
                                xn.AppendChild(xk)
                            Next
                        End If
                    Case "category"
                        x.GetElementsByTagName("category").Item(0).InnerXml = cdata(value)
                        Exit Select
                    Case "article_uid"
                        x.GetElementsByTagName("contentid").Item(0).InnerText = String.Format("ci_{0}", article_uid)
                        Exit Select
                    Case "keyword"
                        Dim xn As XmlNode = x.GetElementsByTagName("keywords").Item(0)
                        Dim options As RegexOptions = RegexOptions.None
                        Dim regex As New Regex("[ ]{2,}", options)
                        value = regex.Replace(value, " ")
                        value = value.Trim()
                        If value.Length > 0 Then
                            Dim keywords As String() = value.Split(" "c)
                            For Each kvalue As String In keywords
                                Dim xk As XmlNode = x.CreateNode(XmlNodeType.Element, "keyword", "")
                                xk.InnerText = kvalue
                                xn.AppendChild(xk)
                            Next
                        End If
                    Case "heading", "summary", "body", "byline"
                        If (bOverriden AndAlso k = "body") Then
                            value = cleanBody(value, holdrules)
                        Else
                            For Each h As String In holdrules
                                If (value.IndexOf("<" & h, StringComparison.InvariantCultureIgnoreCase) <> -1 AndAlso Not bOverriden) Then
                                    Dim exRule As New Exception(gethtmlholdrulesID(h))
                                    Throw New Exception(String.Format(" {0} contains holdrule: {1}", k, h), exRule)
                                End If
                            Next
                        End If
                        If (k.Equals("heading")) Then
                            value = StripTags(value)
                        End If

                        x.GetElementsByTagName(k).Item(0).InnerXml = cdata(value)
                    Case Else

                        x.GetElementsByTagName(k).Item(0).InnerXml = cdata(value)
                        Exit Select
                End Select
            Next

            ' 4.8.2013 add in support for media files PPB
            '    Dim arList As ArrayList = getMediaFiles(articleRow("article_uid"))
            '    Dim node As XmlNode = x.GetElementsByTagName("mediafiles").Item(0)
            '    For Each url As String In arList
            '        Dim elem As XmlElement = x.CreateElement("mediafile")
            '        elem.InnerText = url
            '        node.AppendChild(elem)
            '    Next

            If (bDebug) Then
                here += 1
                File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, "out", vbCrLf))
            End If



            x.Save(articleXML)
            Dim xslt As New XslCompiledTransform()
            xslt.Load(String.Format("{0}\article.xslt", documents))
            xslt.Transform(articleXML, articleSaxoXML)
            Dim article_xml As String = GetFileData(articleSaxoXML)

            If (bDebug) Then
                here += 1
                File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, "articlexml", vbCrLf))
            End If
            Dim sx As New XmlDocument()
            sx.LoadXml(article_xml)
            If (mysiteid.Equals("513") And Not displaygroupfound) Then
                Console.WriteLine("Article {0} Display Group not found {1} ", article_uid, displaygroups)
                Return False
            End If

            If (bDebug) Then
                here += 1
                File.AppendAllText("here.log", String.Format("here {0} this {1}{2}", here, "sw", vbCrLf))
            End If
            ' @siteid is the original cms numeric site id             @destination_siteid -  Saxotech 
            If _message.Length > 0 Then Throw New Exception(_message)
            Dim StoryAddress As String = String.Format("{0}{1}/stories", saxohost, destination_siteid)
            sqlstatement = String.Format("select storyurl from saxo_article where article_uid = '{0}' and destination_siteid = '{1}'", article_uid, destination_siteid)
            Logging(article_uid, sqlstatement, logFlag)
            storyurl = String.Empty

            If (bDebug) Then
                here += 1
                File.AppendAllText("here.log", String.Format("here {0} this story address {1}{2}", here, StoryAddress, vbCrLf))
            End If
            Dim ds3 As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)

            Dim startwatch As DateTime = DateTime.Now
            If (bDebug) Then
                here += 1
                File.AppendAllText("here.log", String.Format("here {0} this ds {1}{2}", here, ds3.Tables(0).Rows.Count, vbCrLf))
            End If


            If (ds3.Tables(0).Rows.Count = 1) Then
                Dim UpdateStoryUrl As String = ds3.Tables(0).Rows(0).Item(0).ToString()
                Dim uri As New Uri(UpdateStoryUrl)
                Dim StoryId As String = Path.GetFileName(uri.LocalPath)
                StoryAddress = String.Format("{0}/{1}", StoryAddress, StoryId)
                ' rc.executeDelete(StoryAddress, username, pwd, _message)
                ' DeleteFromSaxoArticle(article_uid)
                ' storyurl = rc.postStory(New Uri(StoryAddress), username, pwd, article_xml, _message)
                storyurl = rc.putStory(StoryAddress, username, pwd, article_xml, _message)
                Console.WriteLine(String.Format("Update Story {0}", article_uid))
            Else
                storyurl = rc.postStory(New Uri(StoryAddress), username, pwd, article_xml, _message)
                Console.WriteLine(String.Format("Create New Story {0}", article_uid))
            End If
            If (bDebug) Then
                here += 1
                File.AppendAllText("here.log", String.Format("here {0} this story url {1}{2}", here, storyurl, vbCrLf))
            End If
            If _message.Length > 0 Then
                _message = String.Format("Message: {0} OWS (POST,PUT) Story: {1} article_xml: {2}", _message, StoryAddress, article_xml)
                Logging(article_uid, _message, logFlag)
                Throw New Exception(_message)
            End If
            Dim viewuri As String = ""
            Dim story_xml As String = ""

            Dim t2 As TimeSpan = DateTime.Now.Subtract(startwatch)

            Console.WriteLine(String.Format("OWS elapsed time {0}", t2))


            If (bArticleviewuri) Then  ' 06.24.2013 get viewuri 
                Dim storyuri As New Uri(storyurl)
                startwatch = DateTime.Now
                Console.WriteLine(String.Format("Getfile {0}", storyuri))

                Dim bytestoryxml As Byte() = rc.getFile(storyuri, username, pwd, _message)
                If _message.Length > 0 Then
                    Logging(article_uid, String.Format("OWS get storyuri: {0}  msg: {1}  (story xml) ", storyuri, _message), logFlag)
                    Throw New Exception(_message)
                End If
                t2 = DateTime.Now.Subtract(startwatch)
                Console.WriteLine(String.Format("Getfile elapsed time {0}", t2))
                story_xml = Encoding.UTF8.GetString(bytestoryxml)
                viewuri = getViewUri(story_xml)
            End If

            startwatch = DateTime.Now

            DataAuthentication.DataAuthentication.spExecute("LoadSaxoArticle", _message, "@article_uid", article_uid, "@siteid", siteid, "@destination_siteid", destination_siteid, "@xmldata", article_xml, "@viewuri", viewuri, "@storyurl", storyurl)
            If _message.Length > 0 Then
                Logging(article_uid, String.Format("stored proecedure LoadSaxoArticle table: saxo_article msg: {0}", _message), logFlag)
                Throw New Exception(_message)
            End If
            Logging(article_uid, "stored proecedure LoadSaxoArticle table: saxo_article success", logFlag)
            t2 = DateTime.Now.Subtract(startwatch)
            Console.WriteLine(String.Format("Stored Procedure Save elapsed time {0}", t2))
            If (generalsports.Equals(1)) Then Console.WriteLine("Placed in: General SPORTS")
            status = True
        Catch ex As Exception
            status = False
            _message = String.Format("Article {0} Exception {1} stack {2} ", article_uid, ex.Message, ex.StackTrace)

            File.AppendAllText(String.Format("c:\temp\Migrate_{0}_Errors.log", siteid), String.Format("{4}{4}{6}{5}taxpubid: {0}, siteid: {1}, articleid: {2}, error: {3}{4}", taxpubid, siteid, article_uid, ex.ToString, vbCrLf, vbTab, Date.Now.ToString))
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            If ((ex.InnerException IsNot Nothing)) AndAlso (Not String.IsNullOrEmpty(ex.InnerException.Message)) Then
                errorType = IIf(IsNumeric(ex.InnerException.Message), ex.InnerException.Message, 0)
            Else
                errorType = 0
            End If
        Finally
            Dim tempFiles As String() = {articleXML, articleSaxoXML}
            For Each tempFile As String In tempFiles
                If tempFile.Length > 0 AndAlso File.Exists(tempFile) Then
                    File.Delete(tempFile)
                End If
            Next
        End Try
        If status = False Then
            If (_message.Length = 0) Then _message = "unknown Error "
            'Added "errorType" parameter to the MigrationErrors stored procedure, and underlying table -MSJ 20130322
            DataAuthentication.DataAuthentication.spExecute("MigrationErrors", _message2, "@article_uid", article_uid, "@myMessage", _message, "@errorType", errorType)
        End If

        If (bOverriden AndAlso status) Then
            'Mark as having been sent
            ArticleResent(article_uid, status)
        End If

        If (status) Then File.AppendAllText(String.Format("c:\temp\Migrate_{0}_Success.log", siteid), String.Format("{4}{4}{6}{5}taxpubid: {0}, siteid: {1}, articleid: {2}, storyurl: {3}{4}", taxpubid, siteid, article_uid, storyurl, vbCrLf, vbTab, Date.Now.ToString))
        Return status
    End Function


    Public Shared Function storyurltoViewuri(ByVal username As String, ByVal pwd As String, ByVal storyurl As String) As String
        Dim rc As New OWC.rest
        Dim viewuri As String = ""
        Try
            Dim _message As String = ""
            Dim storyuri As New Uri(storyurl)
            Dim bytestoryxml As Byte() = rc.getFile(storyuri, username, pwd, _message)
            If _message.Length > 0 Then Return ""
            Dim story_xml As String = Encoding.UTF8.GetString(bytestoryxml)
            viewuri = getViewUri(story_xml)
        Catch ex As Exception
        End Try
        Return viewuri
    End Function



    Public Shared Function StripTags(ByVal html As String) As String
        ' Remove HTML tags.
        Return Regex.Replace(html, "<.*?>", "")
    End Function

    Public Shared Function cleanBody(ByVal value As String, ByVal holdrules As ArrayList) As String
        Dim cleanValue As String = value
        Dim holdTag As String = String.Empty
        Dim endTag As String = String.Empty
        Dim startIndex As Integer = 0
        Dim holdTagIndex As Integer = 0
        Dim endTagIndex As Integer = 0
        Dim anotherHoldTagIndex As Integer = 0
        Dim anotherTagOrEndIndex As Integer = 0


        For Each H As String In holdrules
            'Find the H and its ender, strip the content
            'Possible endings: />, </H>
            'Possible issues: another H in the story, with endings which may confuse the parser
            'Look for another H, and make sure we don't get the wrong ender
            holdTag = "<" & H
            holdTagIndex = value.IndexOf(holdTag, StringComparison.InvariantCultureIgnoreCase)

            'Parse until the holdTag is not found
            Do Until holdTagIndex = -1
                'Make sure that the closer is not after the next opener
                anotherHoldTagIndex = value.IndexOf(holdTag, holdTagIndex + holdTag.Length, StringComparison.InvariantCultureIgnoreCase)
                If (anotherHoldTagIndex = -1) Then
                    anotherTagOrEndIndex = value.Length - holdTagIndex
                Else
                    anotherTagOrEndIndex = anotherHoldTagIndex - holdTagIndex
                End If

                Dim endTagFound As Boolean = False
                'Look for self-closer /> between opening tag and end of text OR next opening tag
                endTag = "/>"
                endTagIndex = value.IndexOf(endTag, holdTagIndex + holdTag.Length, anotherTagOrEndIndex - holdTag.Length, StringComparison.InvariantCultureIgnoreCase)

                'If there is a self closer, pull the text before the open and after the self-closer
                If (endTagIndex <> -1) Then
                    'Remove the holdTag's contents
                    endTagFound = True
                    value = value.Substring(startIndex, holdTagIndex) & value.Substring(endTagIndex + endTag.Length)
                Else
                    'Look for matching closer </H> between opening tag and end of text OR next opening tag
                    endTag = "</" & H & ">"
                    endTagIndex = value.IndexOf(endTag, holdTagIndex + holdTag.Length, anotherTagOrEndIndex - holdTag.Length, StringComparison.InvariantCultureIgnoreCase)

                    If (endTagIndex <> -1) Then
                        'Remove the holdTag's contents
                        endTagFound = True
                        value = value.Substring(startIndex, holdTagIndex) & value.Substring(endTagIndex + endTag.Length)
                    End If
                End If
                If (Not endTagFound) Then
                    Throw New Exception(String.Format(" Improperly terminated holdrule: {0}", holdTag))
                End If
                'See if there are more instances of this holdTag
                holdTagIndex = value.IndexOf(holdTag, StringComparison.InvariantCultureIgnoreCase)
            Loop
        Next
        Return value
    End Function
End Class
