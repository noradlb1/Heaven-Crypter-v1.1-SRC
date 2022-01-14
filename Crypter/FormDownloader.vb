Public Class FormDownloader
    Private Sub TextBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.Click
        If TextBox1.Text = "Example: http://www.google.com" Then
            TextBox1.Clear()
        End If
    End Sub
    Private Sub FormDownloader_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBox2.SelectedIndex = 1
        ComboBox1.SelectedIndex = 18
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If String.IsNullOrEmpty(TextBox1.Text) Then
            MessageBox.Show("The selected URL is not available at the moment.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Try
                My.Computer.Network.Ping(TextBox1.Text)
                MessageBox.Show("The selected URL is available at the moment and works flawlessly.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("The selected URL is not available at the moment.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If TextBox1.Text = String.Empty Then
            Exit Sub
        End If
        If TextBox1.Text = "Example: http://www.google.com" Then
            Exit Sub
        End If
        Dim foL As String = String.Empty
        Dim NameOfFile As String = TextBox2.Text
        If ComboBox2.Text = "HD" Then
            foL = ComboBox1.Text
        Else
            foL = String.Empty
        End If
        Dim newArray() As String = {NameOfFile, TextBox1.Text, "N/A", ComboBox2.Text, foL, "Downloader"}
        Dim newLV As New ListViewItem(newArray)
        Form1.ListView2.Items.Add(newLV)
        Close()
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Close()
    End Sub
    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            ComboBox1.Enabled = False
        Else
            ComboBox1.Enabled = True
        End If
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text = "Example: http://www.google.com" Then
        Else
            TextBox2.Text = TextBox1.Text.Substring(TextBox1.Text.LastIndexOf("/") + 1)
        End If
    End Sub
End Class