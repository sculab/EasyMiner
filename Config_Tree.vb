Public Class Config_Tree
    Public Event ConfirmClicked()
    Public Event CancelClicked()
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DataGridView1.EndEdit()
        DataGridView1.Refresh()
        Me.Hide()
        RaiseEvent ConfirmClicked()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
        RaiseEvent CancelClicked()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        DataGridView1.Enabled = CheckBox1.Checked
    End Sub
End Class