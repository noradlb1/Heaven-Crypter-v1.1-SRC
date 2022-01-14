Public Class FormBinder
    Dim NameOfFile As String = String.Empty
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim o As New OpenFileDialog With {.Filter = "All Files (.*)|*", .ShowHelp = True}
        If o.ShowDialog = vbOK Then
            TextBox1.Text = o.FileName
            NameOfFile = o.SafeFileName
        End If
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Close()
    End Sub
    Private Sub FormBinder_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBox2.SelectedIndex = 1
        ComboBox1.SelectedIndex = 18
    End Sub
    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            ComboBox1.Enabled = False
        Else
            ComboBox1.Enabled = True
        End If
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim foL As String = String.Empty
        If ComboBox2.Text = "HD" Then
            foL = ComboBox1.Text
        Else
            foL = "N/A"
        End If
        Dim I As New IO.FileInfo(TextBox1.Text)
        Dim newArray() As String = {NameOfFile, TextBox1.Text, Form1.Format(I.Length), ComboBox2.Text, foL, "Binder"}
        Dim newLV As New ListViewItem(newArray)
        Form1.ListView2.Items.Add(newLV)
        Close()
    End Sub
End Class