Public Class Config_Basic
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        NumericUpDown6.Enabled = CheckBox1.Checked Xor False
        NumericUpDown7.Enabled = CheckBox1.Checked Xor False
        NumericUpDown5.Enabled = CheckBox1.Checked Xor True
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        NumericUpDown3.Enabled = CheckBox3.Checked Xor False
    End Sub

    ' 假设这是你要执行的操作
    Public Event ConfirmClicked()
    Public Event CancelClicked()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If CheckBox4.Checked Then
            no_window = "1"
        Else
            no_window = "0"
        End If
        k1 = NumericUpDown1.Value.ToString
        k2 = NumericUpDown5.Value.ToString
        Me.Hide()
        RaiseEvent ConfirmClicked()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
        RaiseEvent CancelClicked()
    End Sub

    Private Sub Config_Basic_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged

    End Sub
End Class