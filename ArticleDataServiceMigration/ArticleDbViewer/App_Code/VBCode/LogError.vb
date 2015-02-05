Imports Microsoft.VisualBasic
Imports System.Diagnostics
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration

Public Class LogError
    Private oMessage As String
    Private oException As Exception
    Private oHTTPSessionState As HttpSessionState
    Private oHTTPRequest As HttpRequest
    Private mstrErrorAsString As String
    Private strSid As String

    Private Function GetStringFromArray(ByVal poArray As String()) As String
        Dim i As Integer
        Dim oSB As New StringBuilder()
        Dim strKey As String

        For i = poArray.GetLowerBound(0) To poArray.GetUpperBound(0)
            strKey = CType(poArray.GetValue(i), String)
            oSB.Append(strKey & " - " & oHTTPRequest.Form.Item(strKey) & vbCrLf)
        Next i
        Return oSB.ToString
    End Function

    Private Function GetErrorAsString() As String
        Dim oTempException As Exception
        Dim intExceptionLevel As Integer
        Dim oSB As New StringBuilder()
        Dim strFormData As String
        oSB.Append("-----------------" & System.DateTime.Now.ToString & "-----------------" & vbCrLf)
        oSB.Append("SID:" & strSid & vbCrLf)
        oTempException = oException
        While Not (oTempException Is Nothing)
            'is this the 1st, 2nd, etc exception in the hierarchy
            intExceptionLevel += 1
            oSB.Append(intExceptionLevel & ": Error Description:" & oTempException.Message & vbCrLf)

            oSB.Append(intExceptionLevel & ": Source:" & Replace(oTempException.Source, vbCrLf, vbCrLf & intExceptionLevel & ": ") & vbCrLf)

            oSB.Append(intExceptionLevel & ": Stack Trace:" & Replace(oTempException.StackTrace, vbCrLf, vbCrLf & intExceptionLevel & ": ") & vbCrLf)

            oSB.Append(intExceptionLevel & ": Target Site:" & Replace(oTempException.TargetSite.ToString, vbCrLf, vbCrLf & intExceptionLevel & ": ") & vbCrLf)

            'get the next exception to log
            oTempException = oTempException.InnerException

        End While
        strFormData = vbTab & Me.GetStringFromArray(oHTTPRequest.Form.AllKeys).Replace(vbCrLf, vbCrLf & vbTab)
        If strFormData.Length > 1 Then '1 because if the form is empty it will just contain the tab prefixed to the line.
            oSB.Append("Form Data:" & vbCrLf)
            'remove the last tab so it doesn't screw up formatting on the line after it.
            oSB.Append(strFormData.Substring(0, strFormData.Length - 1))
        Else
            oSB.Append("Form Data: No Form Data Found")
        End If
        Return oSB.ToString()

    End Function


    Public Sub New(ByVal poMessage As String, ByVal poHTTPSessionState As HttpSessionState, ByVal poHTTPRequest As HttpRequest, ByVal SID As String)
        oMessage = poMessage
        oHTTPSessionState = poHTTPSessionState
        oHTTPRequest = poHTTPRequest
        strSid = SID
    End Sub

    Public Sub New(ByVal poException As Exception, ByVal poHTTPSessionState As HttpSessionState, ByVal poHTTPRequest As HttpRequest, ByVal SID As String)
        oException = poException
        oHTTPSessionState = poHTTPSessionState
        oHTTPRequest = poHTTPRequest
        strSid = SID
    End Sub

    ReadOnly Property ErrorAsString() As String
        Get
            'if this text hasn't been set yet, then fill it in.
            If mstrErrorAsString Is Nothing Then mstrErrorAsString = Me.GetErrorAsString
            Return mstrErrorAsString
        End Get

    End Property

    Private Sub LogToEventLog(ByVal strEventLogName As String)

        Dim oEventLog As New Diagnostics.EventLog()

        ' You may have specific a custom event log, so make sure it exists first
        If (Not EventLog.SourceExists(strEventLogName)) Then
            EventLog.CreateEventSource(strEventLogName, strEventLogName)
        End If
        oEventLog.Source = strEventLogName
        oEventLog.WriteEntry(Me.ErrorAsString, EventLogEntryType.Error)

    End Sub

    Private Sub LogToFile(ByVal strFileName As String)
        Dim oSRLogFile As New StreamWriter(strFileName, True)

        Try
            oSRLogFile.Write(Me.ErrorAsString)
            oSRLogFile.Close()
        Catch ex As Exception
            'hopefully you also have the log to event log selected and that may have worked. If not, you will probably
            'not know the error information since the attempt to log to file failed. 
            'It may have failed for some reason writing to a file (this could be because of an exclusive lock on the file
            'or security permissions, aspnet_wp may not have permissions to the file, etc etc. 

            'one option here may be shell off an email or net send, depending on your system requirements.
            'Throw ex
        End Try


    End Sub

    Private Function GetConn() As SqlConnection
        Dim strConn As String = ConfigurationManager.ConnectionStrings("connstr").ToString()
        Return New SqlConnection(strConn)
    End Function

    Private Function checkObjectvalue(ByVal o As Object) As String
        If o Is Nothing Then
            Return ""
        Else
            Return o.ToString
        End If
    End Function

    Private Sub LogToDB()
        Dim oTempException As Exception
        Dim intExceptionLevel As Integer
        Dim cmd As New SqlCommand("CreateErrorEntry")
        Dim strFormData As String
        Dim conn As SqlClient.SqlConnection = Nothing


        conn = GetConn()
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        Dim sp As SqlParameter

        sp = cmd.Parameters.Add("@strSID", SqlDbType.VarChar, 40)
        sp.Value = strSid

        sp = cmd.Parameters.Add("@strRequestMethod", SqlDbType.VarChar, 5)
        sp.Value = oHTTPRequest.ServerVariables("REQUEST_METHOD")

        sp = cmd.Parameters.Add("@intServerPort", SqlDbType.Int)
        sp.Value = oHTTPRequest.ServerVariables("SERVER_PORT")

        sp = cmd.Parameters.Add("@strHTTPS", SqlDbType.VarChar, 3)
        sp.Value = oHTTPRequest.ServerVariables("HTTPS")

        sp = cmd.Parameters.Add("@strLocalAddr", SqlDbType.VarChar, 15)
        sp.Value = oHTTPRequest.ServerVariables("LOCAL_ADDR")

        sp = cmd.Parameters.Add("@strHostAddress", SqlDbType.VarChar, 15)
        sp.Value = oHTTPRequest.ServerVariables("REMOTE_ADDR")

        sp = cmd.Parameters.Add("@strUserAgent", SqlDbType.VarChar, 255)
        sp.Value = oHTTPRequest.ServerVariables("HTTP_USER_AGENT")

        sp = cmd.Parameters.Add("@strURL", SqlDbType.VarChar, 400)
        sp.Value = oHTTPRequest.ServerVariables("URL")

        sp = cmd.Parameters.Add("@strCustomerRefID", SqlDbType.VarChar, 40)
        If oHTTPSessionState IsNot Nothing Then
            sp.Value = oHTTPSessionState.SessionID
        Else
            sp.Value = " "
        End If

        sp = cmd.Parameters.Add("@strFormData", SqlDbType.VarChar, 2000)
        'this field is 2000 chars long. The form data may be longer, so to avoid an error, return only a portion of it.
        'if you require a longer field, modify the database and the 2000 above and below and make sure if using sql server
        'that your total record length does not go over sql server's 8k limit unless you change the FormData field to a a text data type.
        strFormData = Me.GetStringFromArray(oHTTPRequest.Form.AllKeys)

        If strFormData.Length > 2000 Then
            sp.Value = strFormData.Substring(0, 2000)
        Else
            sp.Value = strFormData
        End If

        sp = cmd.Parameters.Add("@strAllHTTP", SqlDbType.VarChar, 2000)
        sp.Value = Replace(oHTTPRequest.ServerVariables("ALL_HTTP"), vbLf, vbCrLf)

        sp = cmd.Parameters.Add("@dteInsertDate", SqlDbType.DateTime)
        sp.Value = System.DateTime.Now

        sp = cmd.Parameters.Add("@blnIsCookieLess", SqlDbType.Bit)
        If oHTTPSessionState IsNot Nothing Then
            sp.Value = oHTTPSessionState.IsCookieless
        Else
            sp.Value = False
        End If

        sp = cmd.Parameters.Add("@blnIsNewSession", SqlDbType.Bit)
        If oHTTPSessionState IsNot Nothing Then
            sp.Value = oHTTPSessionState.IsNewSession
        Else
            sp.Value = False
        End If

        Dim intID As Integer
        Try
            cmd.Connection.Open()
            intID = CInt(cmd.ExecuteScalar())
        Catch ex As Exception
            'Throw ex
        End Try

        Dim strMachine As String
        Dim ctext As HttpContext = HttpContext.Current
        strMachine = ctext.Server.MachineName()

        'Each exception can have an inner exception, providing more details as to the source of the error.
        'loop through these and log to the database. Each one will be related to it's parent exception by
        'use of a integer "ExceptionLevel" flag. Level 1 is the top level, 2 is a child to 1, 3 is a child to 2, etc etc
        oTempException = oException
        While Not (oTempException Is Nothing)
            'is this the 1st, 2nd, etc exception in the hierarchy
            intExceptionLevel += 1
            cmd.CommandText = "LogException"
            cmd.Parameters.Clear()

            sp = cmd.Parameters.Add("@intSessionErrorID", SqlDbType.Int)
            sp.Value = intID

            sp = cmd.Parameters.Add("@intExceptionLevel", SqlDbType.Int)
            sp.Value = intExceptionLevel

            sp = cmd.Parameters.Add("@strMessage", SqlDbType.VarChar, 1000)
            sp.Value = oTempException.Message

            sp = cmd.Parameters.Add("@strSource", SqlDbType.VarChar, 200)
            sp.Value = checkObjectvalue(oTempException.Source)

            sp = cmd.Parameters.Add("@strStackTrace", SqlDbType.VarChar, 4000)
            sp.Value = checkObjectvalue(oTempException.StackTrace)

            sp = cmd.Parameters.Add("@strTargetSite", SqlDbType.VarChar, 100)
            If oTempException.TargetSite Is Nothing Then
                sp.Value = checkObjectvalue(oTempException.TargetSite)
            Else
                sp.Value = ""
            End If

            sp = cmd.Parameters.Add("@strMachine", SqlDbType.VarChar, 50)
            sp.Value = strMachine

            Try
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                'log to event log that the db logging failed.
                'Throw ex
            End Try

            'get the next exception to log
            oTempException = oTempException.InnerException
        End While

        cmd.Connection.Close()

    End Sub



    Public Sub LogError()
        'read in the XML config settings and see what logging options should be used.
        Dim strEventLog As String = ConfigurationManager.AppSettings.Item("ErrorLoggingEventLogType")
        Dim strLogFile As String = System.Web.HttpContext.Current.Server.MapPath("temp") & ConfigurationManager.AppSettings.Item("ErrorLoggingLogFile")

        'This error handling may seem strange, however nothing exists in the catch blocks because an error may fail
        'being logged to one location. If thats the case, we don't want to stop trying the other locations by exiting 
        'this sub. If say the event log is full, we still want to log to a file or database without bombing out.
        Try
            If ConfigurationManager.AppSettings("ErrorLoggingLogToEventLog").ToUpper = "TRUE" And strEventLog <> "" Then LogToEventLog(strEventLog)
        Catch
            ' always continue
        End Try

        Try
            If ConfigurationManager.AppSettings.Item("ErrorLoggingLogToFile").ToUpper = "TRUE" Then LogToFile(strLogFile)
        Catch

        End Try

        Try
            If ConfigurationManager.AppSettings.Item("ErrorLoggingLogToDB").ToUpper = "TRUE" Then LogToDB()
        Catch

        End Try


    End Sub



End Class
