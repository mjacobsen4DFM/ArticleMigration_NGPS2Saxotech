Imports System.Text
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Configuration
Imports System.IO
Imports NDesk.Options
Imports System.Web

Module Module1


    Private Function RemoveTag(ByVal xmlcontent As String, ByVal startTag As String, ByVal endTag As String) As String
        Dim modifiedcontent As String = xmlcontent
        Dim inpos As Integer = xmlcontent.IndexOf(startTag)
        Dim epos As Integer = xmlcontent.IndexOf(endTag)
        If (inpos > 0 And inpos < epos) Then
            modifiedcontent = String.Format("{0}{1}", xmlcontent.Substring(0, inpos - 2), xmlcontent.Substring(epos + endTag.Length))
        Else
            If (inpos > 0) Then
                modifiedcontent = String.Format("{0}{1}", xmlcontent.Substring(0, inpos - 2), xmlcontent.Substring(inpos + startTag.Length))
            End If
        End If
        Return modifiedcontent
    End Function

    Private Function CheckArgument(ByVal arg As List(Of String), ByVal name As String) As Boolean
        If arg.Count = 1 Then
            Return True
        End If
        Dim _msg As String = [String].Format("You must specify argument --{0}", name)
        If arg.Count > 1 Then
            _msg = [String].Format("You must specify only one argument with  --{0}", name)
        End If
        Console.WriteLine("{0}", _msg)
        Return False
    End Function




    Private Sub processday(ByVal siteid As String, ByVal theDay As String)
        Dim rc As New OWC.rest
        Dim db As New DbCommon.dbcommon
        Dim message As String = ""
        Try

            Dim myaddress As String = HttpUtility.HtmlDecode(ConfigurationManager.AppSettings("target"))
            Dim address As String = String.Format(myaddress, siteid, theDay, theDay)

            ' Dim address As String = String.Format("http://pbruni-development.mn1.dc.publicus.com/section/proReport?SiteId={0}&start_date={1}&end_date={2}", siteid, theDay, theDay)
            Console.WriteLine(address)
            Dim b() As Byte = rc.getFile(New Uri(address), message)
            If b Is Nothing Then
                Console.WriteLine("No Data message: ", message)
                Exit Sub
            End If
            Dim xmlcontent As String = Encoding.UTF8.GetString(b)
            xmlcontent = xmlcontent.Trim
            Dim x As New XmlDocument
            x.LoadXml(xmlcontent)
            Dim xmlitems As XmlNodeList = x.GetElementsByTagName("item")
            Console.WriteLine("Total items: {0}", xmlitems.Count)
            For Each xmlitem As XmlNode In xmlitems
                Dim guid As String = xmlitem.SelectSingleNode("saxo-guid").InnerText
                Dim ds As DataSet = db.getDs(String.Format("select count(*) from item where pubcode = '{0}' and guid  = '{1}'", siteid, guid))
                Dim count As Integer = Convert.ToInt32(ds.Tables(0).Rows(0).Item(0))
                If count.Equals(1) Then
                    Console.WriteLine("Site: {0} GUID: {1} already exists", siteid, guid)
                    Continue For
                End If
                Dim sharedContent As String = IIf(xmlitem.SelectSingleNode("storysite").InnerText.ToUpper().Trim.Equals("ZZ"), "Y", "N")
                Dim link As String = xmlitem.SelectSingleNode("guid").InnerText
                Dim title As String = xmlitem.SelectSingleNode("title").InnerText
                Dim pubdate As String = xmlitem.SelectSingleNode("pubDate").InnerText
                Dim dpubdate As String = String.Format("{0}-{1}-{2}", pubdate.Substring(4, 2), pubdate.Substring(6, 2), pubdate.Substring(0, 4))      ' Convert.ToDateTime(pubdate).ToString("MM-dd-yyyy")
                Dim maincategoryid As String = xmlitem.SelectSingleNode("mainprofile").InnerText
                If String.IsNullOrEmpty(maincategoryid) Then maincategoryid = "0"
                Dim maincategory As String = ""
                Dim ds2 As DataSet = db.getDs(String.Format("select fieldname from profile where id = {0}", maincategoryid))
                If (ds2.Tables(0).Rows.Count.Equals(1)) Then maincategory = ds2.Tables(0).Rows(0).Item(0)
                db.spExecute("loadItem", message, "@pubcode", siteid, "@guid", guid, "@categoryid", CInt(maincategoryid), "@categoryname", maincategory, "@pubdate", dpubdate, "@sourceURL", link, "@sharedcontent", sharedContent)
                Dim profiles() As String = xmlitem.SelectSingleNode("profiles").InnerText.Split(",")
                For Each profile As String In profiles
                    If (IsNumeric(profile)) Then
                        Dim profilename As String = ""
                        Dim ds3 As DataSet = db.getDs(String.Format("select fieldname from profile where id = {0}", profile))
                        If (ds3.Tables(0).Rows.Count.Equals(1)) Then profilename = ds3.Tables(0).Rows(0).Item(0)
                        db.spExecute("loadExtended", message, "@pubcode", siteid, "@guid", guid, "@categoryid", CInt(profile), "@categoryname", profilename)
                    End If
                Next
            Next
        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.Message)
        End Try

    End Sub

    ' For Testing mal formed XML follows
    'Dim tempfile As String = String.Format("{0}\rss.xml", ConfigurationManager.AppSettings("temp"))
    'Dim sw As New StreamWriter(tempfile, False)
    'sw.Write(xmlcontent)
    'sw.Close()
    ' Dim sr As New StreamReader(tempfile)
    ' Dim xmlcontent As String = sr.ReadToEnd()
    ' sr.Close()
    ' While (xmlcontent.Contains("<media:content"))
    '    xmlcontent = RemoveTag(xmlcontent, "<media:content", "</media:content>")
    ' End While




    Sub Main(ByVal args As String())
        Dim rc As New OWC.rest
        Dim db As New DbCommon.dbcommon
        Dim message As String = ""
        Dim extra As New List(Of String)()
        Dim siteCodes As New List(Of String)()
        Dim startDates As New List(Of String)()
        Dim endDates As New List(Of String)()
        Try

            Dim p As OptionSet = New OptionSet()
            p.Add("c|sitecode=", " (required) the source Site Code.", Sub(v As String) siteCodes.Add(v))
            p.Add("s|start=", " (required) the start Date YYYYMMDD.", Sub(v) startDates.Add(v))
            p.Add("e|end=", " (required) the End Date YYYYMMDD .", Sub(v) endDates.Add(v))

            Try
                extra = p.Parse(args)
            Catch e As OptionException
                Console.Write("rssReader: ")
                Console.WriteLine(e.Message)
                Console.WriteLine("You need -c LA -s 20130101  -e 20130201  command line arguments.")
                Return
            End Try
            Dim valid As Boolean = True
            Dim svalid As Boolean = True
            Dim evalid As Boolean = True
            If Not CheckArgument(siteCodes, "sitecode") Then
                valid = False
            End If
            If Not valid Then Exit Sub

            If Not CheckArgument(startDates, "start") Then
                svalid = False

            End If
            If Not CheckArgument(endDates, "end") Then
                evalid = False
            End If
            Dim siteid As String = siteCodes(0)
            Dim mystartdate As Date = Now.AddDays(-1)
            Dim myenddate As Date = Now.AddDays(-1)

            If (svalid And evalid) Then
                mystartdate = String.Format("{0}-{1}-{2}", startDates(0).Substring(4, 2), startDates(0).Substring(6, 2), startDates(0).Substring(0, 4))
                myenddate = String.Format("{0}-{1}-{2}", endDates(0).Substring(4, 2), endDates(0).Substring(6, 2), endDates(0).Substring(0, 4))

            End If
            While (mystartdate <= myenddate)
                processday(siteid, mystartdate.ToString("yyyyMMdd"))
                mystartdate = mystartdate.AddDays(1)
            End While



        Catch ex As Exception
            Console.WriteLine("Exception: {0}", ex.Message)
        End Try

    End Sub

End Module
