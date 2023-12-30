Public Class Config_Tree
    Public Event ConfirmClicked()
    Public Event CancelClicked()
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        RaiseEvent ConfirmClicked()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
        RaiseEvent CancelClicked()
    End Sub
End Class