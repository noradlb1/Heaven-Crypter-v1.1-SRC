Public Class FormScanner

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            WebBrowser1.Document.GetElementById("l_ogin").InnerText = "Timtam"
            WebBrowser1.Document.GetElementById("passw").InnerText = "hjgbsd8798kj"
            WebBrowser1.Document.GetElementById("login").InvokeMember("submit")
            WebBrowser1.Navigate("http://85.31.101.189/check.php")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub
    Private Sub FormScanner_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        WebBrowser1.Dispose()
        WebBrowser1.Stop()
    End Sub

    Private Sub FormScanner_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class