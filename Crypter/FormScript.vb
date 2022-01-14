Imports System.IO
Public Class FormScript

#Region "Batch"
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim sDLG As New SaveFileDialog With {.Filter = "Batch Files (.bat)|*.bat", .ShowHelp = True}
        If TextBox1.Text = String.Empty Then Return
        If sDLG.ShowDialog = vbOK Then
            File.WriteAllText(sDLG.FileName, TextBox1.Text)
            MessageBox.Show("Successfully compiled Batch Script." & vbNewLine & vbNewLine & "Saved As:" & vbNewLine & sDLG.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If TextBox1.Text = String.Empty Then Return
        Try
            Dim LocCompiler As String = Environment.GetFolderPath(Environment.SpecialFolder.Templates) & "\BatchScript.debug.bat"

            If File.Exists(LocCompiler) Then File.Delete(LocCompiler)
            File.WriteAllText(LocCompiler, TextBox1.Text)
            Process.Start(LocCompiler)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Compiling Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        TextBox1.Clear()
    End Sub
#End Region

#Region "Visual Basic Script"
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim sDLG As New SaveFileDialog With {.Filter = "VB Script Files (.vbs)|*.vbs", .ShowHelp = True}
        If TextBox2.Text = String.Empty Then Return
        If sDLG.ShowDialog = vbOK Then
            File.WriteAllText(sDLG.FileName, TextBox2.Text)
            MessageBox.Show("Successfully compiled VB Script." & vbNewLine & vbNewLine & "Saved As:" & vbNewLine & sDLG.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        If TextBox2.Text = String.Empty Then Return
        Try
            Dim LocCompiler As String = Environment.GetFolderPath(Environment.SpecialFolder.Templates) & "\VBScript.debug.vbs"

            If File.Exists(LocCompiler) Then File.Delete(LocCompiler)
            File.WriteAllText(LocCompiler, TextBox2.Text)
            Process.Start(LocCompiler)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Compiling Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        TextBox2.Clear()
    End Sub
#End Region

#Region "HTML"
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Dim sDLG As New SaveFileDialog With {.Filter = "HTML Files (.html)|*.html", .ShowHelp = True}
        If TextBox3.Text = String.Empty Then Return
        If sDLG.ShowDialog = vbOK Then
            File.WriteAllText(sDLG.FileName, TextBox3.Text)
            MessageBox.Show("Successfully compiled HTML Script." & vbNewLine & vbNewLine & "Saved As:" & vbNewLine & sDLG.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        If TextBox3.Text = String.Empty Then Return
        Try
            Dim LocCompiler As String = Environment.GetFolderPath(Environment.SpecialFolder.Templates) & "\HTMLScript.debug.html"

            If File.Exists(LocCompiler) Then File.Delete(LocCompiler)
            File.WriteAllText(LocCompiler, TextBox3.Text)
            Process.Start(LocCompiler)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Compiling Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        TextBox3.Clear()
    End Sub
#End Region

    Private Sub FormScript_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        On Error Resume Next
        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Templates) & "\BatchScript.debug.bat")
        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Templates) & "\VBScript.debug.vbs")
        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Templates) & "\HTMLScript.debug.html")
    End Sub
End Class