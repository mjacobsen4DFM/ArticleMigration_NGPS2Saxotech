Imports DbCommon
Imports System.Xml
Imports System.IO
Imports System.Data

Namespace MySession

    Public Class tracksession
        Public Enum keycodes
            siteid = 1
            publicationName = 2
            destination_siteid = 3
            row = 4
            article_uid = 5
            query = 6
            category = 7
        End Enum
        Public Enum cookiecodes
            SID = 1
        End Enum

        Private cTrackClient As String = "TrackClient"
        Private mydbcommon As New DbCommon.dbcommon


        Public Function CreateSessionId() As String
            Dim mysid As String = ""
            Dim value As String = ""
            Dim key As cookiecodes = cookiecodes.SID
            If Not (HttpContext.Current.Request.Cookies(cTrackClient) Is Nothing) Then
                If Not (HttpContext.Current.Request.Cookies(cTrackClient)(key.ToString) Is Nothing) Then
                    value = System.Convert.ToString(HttpContext.Current.Request.Cookies(cTrackClient)(key.ToString)).Trim
                End If
            End If
            If value.Length = 0 Then
                mysid = System.Guid.NewGuid().ToString()        ' reset SID
                SetCookie(cookiecodes.SID, mysid)               ' Generate a new Session Id 
                value = mysid
            End If
            Return value
        End Function

        Private Sub SetCookie(ByVal key As cookiecodes, ByVal value As String)
            Dim userCookie As System.Web.HttpCookie
            userCookie = HttpContext.Current.Request.Cookies(cTrackClient)
            If userCookie Is Nothing Then
                userCookie = New System.Web.HttpCookie(cTrackClient)
            End If
            userCookie.Values(key.ToString) = value
            userCookie.Expires = DateTime.Now.AddHours(8)
            HttpContext.Current.Response.Cookies.Add(userCookie)
        End Sub

        Public Function GetCookie(ByVal key As cookiecodes) As String
            Dim value As String = ""
            If Not (HttpContext.Current.Request.Cookies(cTrackClient) Is Nothing) Then
                If Not (HttpContext.Current.Request.Cookies(cTrackClient)(key.ToString) Is Nothing) Then
                    value = System.Convert.ToString(HttpContext.Current.Request.Cookies(cTrackClient)(key.ToString)).Trim
                End If
            End If
            If (value.Length = 0) Then
                HttpContext.Current.Response.Redirect("~/SessionTimeout.aspx")
            End If
            Return value
        End Function



        Private Function GetDocument(ByVal sid As String) As String
            Dim ds As DataSet
            Dim value As String = "<xmldocument></xmldocument>"
            ds = mydbcommon.getDs("TrackSessionSelect", sid)
            If ds.Tables(0).Rows.Count = 1 Then value = ds.Tables(0).Rows(0).Item(0).ToString()
            Return value
        End Function


        Public Function GetValue(ByVal key As keycodes) As String
            Dim value As String = ""
            Dim xmldoc As XmlDocument = New XmlDocument()
            Dim sid As String = GetCookie(cookiecodes.SID)
            Dim keyname As String = key.ToString
            Dim sXML As String = GetDocument(sid)
            xmldoc.LoadXml(sXML)
            Dim xmlnode As XmlNode = xmldoc.SelectSingleNode("//" & keyname)
            If xmlnode IsNot Nothing Then value = xmlnode.InnerText
            Return value
        End Function

        Public Sub SetValue(ByVal key As keycodes, ByVal value As String)
            Dim intRows As Integer, errmsg As String = ""
            Dim sXML As String
            Dim xmldoc As XmlDocument = New XmlDocument()
            Dim hParams As New Dictionary(Of String, Object)
            Dim keyname As String = key.ToString

            Dim sid As String = GetCookie(cookiecodes.SID)
            sXML = GetDocument(sid)
            xmldoc.LoadXml(sXML)
            Dim xmlnode As XmlNode = xmldoc.SelectSingleNode("//" & keyname)
            If xmlnode Is Nothing Then
                Dim xmle As XmlElement = xmldoc.CreateElement(keyname)
                xmle.InnerText = value
                xmldoc.DocumentElement.PrependChild(xmle)
            Else
                xmlnode.InnerText = value
            End If
            sXML = xmldoc.OuterXml
            intRows = mydbcommon.spExecute2("TrackSessionInsert", errmsg, "@sid", sid, "@xmldocument", sXML)
            If intRows < 1 Then
                Throw New Exception("TrackSession sid: " & sid & " Insert returned " & intRows & " rows")
            End If
        End Sub


    End Class
End Namespace

