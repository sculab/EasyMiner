Public Class Config_Trim
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



    Private Sub NumericUpDown1_MouseHover(sender As Object, e As EventArgs) Handles NumericUpDown1.MouseHover
        TextBox1.Text = "Results are kept if their length ratio to the reference sequence's median exceeds a set value; otherwise, they're removed. To prevent over-filtering, especially when sequencing and reference sources differ (e.g., genomic data vs. transcriptomic references), set this value to 0."
    End Sub


    Private Sub CheckBox1_MouseHover(sender As Object, e As EventArgs) Handles CheckBox1.MouseHover
        TextBox1.Text = "Activate this option to trim using only the longest fragment matching the reference sequence. If deactivated, it combines all matched fragments. Avoid using this feature when sequencing and reference sources differ, like genomic data versus transcriptomes, to prevent over-trimming."
    End Sub


    Private Sub Label1_MouseHover(sender As Object, e As EventArgs) Handles Label1.MouseHover
        TextBox1.Text = "Results are kept if their length ratio to the reference sequence's median exceeds a set value; otherwise, they're removed. To prevent over-filtering, especially when sequencing and reference sources differ (e.g., genomic data vs. transcriptomic references), set this value to 0."

    End Sub
End Class