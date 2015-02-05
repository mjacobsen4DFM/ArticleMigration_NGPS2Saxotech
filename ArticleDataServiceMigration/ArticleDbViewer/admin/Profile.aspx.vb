
Imports BusinessRuleC
Imports DataModels
Imports System.Text


Partial Class admin_Profile
    Inherits WebFrontEnd


    Private Sub loadBaseSubjectProfile(ByVal myname As String)
        Dim myprofiles As List(Of profile) = BusinessRuleC.BusinessRule.GetProfiles(0, myname)
        myprofiles = BusinessRuleC.BusinessRule.GetProfiles(1, myprofiles(0).parentid)
        DrpLevel.Visible = True
        DrpLevel.Items.Clear()
        For Each myprofile As profile In myprofiles
            DrpLevel.Items.Add(New ListItem(myprofile.fieldname, myprofile.id))
        Next
        mytracksession.SetValue(MySession.tracksession.keycodes.profilerootid, myprofiles(0).parentid)
        mytracksession.SetValue(MySession.tracksession.keycodes.profileLevel, 1)
        mytracksession.SetValue(MySession.tracksession.keycodes.profileTrail, String.Format("{0}:{1}", myname, myprofiles(0).parentid))
        btnPrevious.Enabled = False
        BtnNext.Enabled = True
        lblprofileTrail.Text = String.Format("{0} &gt;&gt;", myname)
    End Sub



    Protected Sub admin_Profile_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pnlSelected.Visible = False
            Dim defaultvalue As String = "subject"
            mytracksession.SetValue(MySession.tracksession.keycodes.profileroot, defaultvalue)
            loadBaseSubjectProfile(defaultvalue)
        End If
    End Sub


    Private Function setProfileTrail(ByVal level As Integer, ByVal profileid As Integer) As List(Of profile)
        Return BusinessRuleC.BusinessRule.GetProfilesForId(level, profileid)
    End Function

    Private Function English(ByVal profileTrail As String) As String
        Dim EnglishTrail As String = ""
        For Each profile As String In profileTrail.Split(",")
            EnglishTrail &= String.Format("{0} &gt;&gt; ", profile.Split(":")(0))
        Next
        Return EnglishTrail
    End Function


    Protected Sub BtnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnNext.Click
        pnlSelected.Visible = False
        btnPrevious.Enabled = True
        Dim profileid As String = DrpLevel.SelectedValue
        Dim profileName As String = DrpLevel.SelectedItem.Text
        Dim selectedProfile As String = String.Format("{0}:{1}", profileName, profileid)
        Dim level As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.profileLevel))
        Dim profileTrail As String = mytracksession.GetValue(MySession.tracksession.keycodes.profileTrail).Trim
        Dim profileRecords() As String = profileTrail.Split(",")
        Dim size As Integer = profileRecords.GetUpperBound(0)
        profileTrail = IIf(profileTrail.Length > 0, String.Format("{0},{1}", profileTrail, selectedProfile), selectedProfile)
        lblprofileTrail.Text = English(profileTrail)
        mytracksession.SetValue(MySession.tracksession.keycodes.profileTrail, profileTrail)
        mytracksession.SetValue(MySession.tracksession.keycodes.profileSelected, selectedProfile)
        level += 1
        mytracksession.SetValue(MySession.tracksession.keycodes.profileLevel, level)

        Dim myprofiles As List(Of profile) = setProfileTrail(level, profileid)
        If (myprofiles.Count = 0) Then
            BtnNext.Enabled = False
            DrpLevel.Visible = False
            Exit Sub
        End If
        DrpLevel.Items.Clear()
        For Each myprofile As profile In myprofiles
            DrpLevel.Items.Add(New ListItem(myprofile.fieldname, myprofile.id))
        Next

    End Sub



    Protected Sub btnPrevious_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrevious.Click
        pnlSelected.Visible = False
        BtnNext.Enabled = True
        Dim level As Integer = Convert.ToInt32(mytracksession.GetValue(MySession.tracksession.keycodes.profileLevel))
        level -= 1
        Dim profileTrail As String = mytracksession.GetValue(MySession.tracksession.keycodes.profileTrail).Trim
        Dim profileRecords() As String = profileTrail.Split(",")
        Dim size As Integer = profileRecords.GetUpperBound(0)
        profileTrail = ""
        Dim selectedvalue As String = ""
        Dim profileid As Integer = 0
        For n = 0 To size
            If n < size Then
                profileTrail &= String.Format("{0}{1}", profileRecords(n), IIf(n <> size - 1, ",", ""))
                profileid = Convert.ToInt32(profileRecords(n).Split(":")(1))
            Else
                selectedvalue = profileRecords(n).Split(":")(1)
            End If
        Next

        mytracksession.SetValue(MySession.tracksession.keycodes.profileTrail, profileTrail)
        lblprofileTrail.Text = English(profileTrail)

        Dim myprofiles As List(Of profile) = setProfileTrail(level, profileid)
        If (myprofiles.Count = 0) Then
            btnPrevious.Enabled = False
            Exit Sub
        End If
        DrpLevel.Visible = True
        DrpLevel.Items.Clear()
        For Each myprofile As profile In myprofiles
            DrpLevel.Items.Add(New ListItem(myprofile.fieldname, myprofile.id))
        Next
        DrpLevel.SelectedValue = selectedvalue
        mytracksession.SetValue(MySession.tracksession.keycodes.profileLevel, level)
        If level = 1 Then btnPrevious.Enabled = False
    End Sub

    Protected Sub RadioButtonList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButtonList1.SelectedIndexChanged
        Dim value As String = RadioButtonList1.SelectedValue()
        mytracksession.SetValue(MySession.tracksession.keycodes.profileroot, value)
        loadBaseSubjectProfile(value)

    End Sub

    Protected Sub BtnSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnSelect.Click
        Dim profileid As String = DrpLevel.SelectedValue
        Dim profileName As String = DrpLevel.SelectedItem.Text
        Dim selectedProfile As String = String.Format("{0}:{1}", profileName, profileid)
        pnlSelected.Visible = True
        lblprofilename.Text = profileName
        lblprofileId.Text = profileid
    End Sub
End Class
