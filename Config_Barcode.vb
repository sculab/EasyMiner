Imports System.IO
Imports System.Threading
Public Class Config_barcode

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If TextBox1.Text <> "" And TextBox2.Text <> "" And TextBox3.Text <> "" Then
            Dim th1 As New Thread(AddressOf run_barcode)
            th1.Start()
            Me.Hide()
        Else
            MsgBox("Please ensure all information is complete!")
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Hide()
    End Sub
    Public Sub run_barcode()
        PB_value = 33
        Directory.CreateDirectory(Path.Combine(currentDirectory, "temp", "temp_refs"))
        clean_fasta_file(TextBox2.Text, Path.Combine(currentDirectory, "temp", "temp_refs", "barcode.fasta"))
        clean_fasta_file(TextBox3.Text, Path.Combine(currentDirectory, "temp", "temp_refs", "barcode_refs.fasta"))
        If CheckBox1.Checked = False Then
            Dim SI_split_barcode As New ProcessStartInfo()
            SI_split_barcode.FileName = Path.Combine(currentDirectory, "analysis", "split_barcode.exe")
            SI_split_barcode.WorkingDirectory = Path.Combine(currentDirectory, "temp")
            SI_split_barcode.CreateNoWindow = False
            SI_split_barcode.Arguments = "-i " + """" + TextBox1.Text + """" + " -r " + """" + ".\temp_refs\barcode.fasta" + """" + " -o " + """" + form_main.TextBox1.Text + """" + " -p " + current_thread.ToString + " -w " + (NumericUpDown2.Value + 11).ToString
            Dim process_split_barcode As Process = Process.Start(SI_split_barcode)
            process_split_barcode.WaitForExit()
            process_split_barcode.Close()
        End If
        PB_value = 66
        Dim SI_build_barcode As New ProcessStartInfo()
        SI_build_barcode.FileName = Path.Combine(currentDirectory, "analysis", "build_barcode.exe")
        SI_build_barcode.WorkingDirectory = Path.Combine(currentDirectory, "temp")
        SI_build_barcode.CreateNoWindow = False
        SI_build_barcode.Arguments = "-i " + """" + Path.Combine(form_main.TextBox1.Text, "clean_data") + """" + " -r " + """" + ".\temp_refs\barcode_refs.fasta" + """" + " -o " + """" + form_main.TextBox1.Text + """" + " -p " + current_thread.ToString + " -m 0 -l " + NumericUpDown1.Value.ToString
        Dim process_build_barcode As Process = Process.Start(SI_build_barcode)
        process_build_barcode.WaitForExit()
        process_build_barcode.Close()
        PB_value = 100
        Dim result As DialogResult = MessageBox.Show("Analysis has been completed. Would you like to view the results file?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        ' 根据用户的选择执行相应的操作
        If result = DialogResult.Yes Then
            Process.Start("explorer.exe", """" + form_main.TextBox1.Text + """")
        End If
        PB_value = 0
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim opendialog As New FolderBrowserDialog
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            TextBox1.Text = opendialog.SelectedPath
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta File(*.fasta)|*.fas;*.fasta;*.fa|GenBank File|*.gb"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".fa"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            TextBox2.Text = opendialog.FileName
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta File(*.fasta)|*.fas;*.fasta;*.fa|GenBank File|*.gb"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".fa"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            TextBox3.Text = opendialog.FileName
        End If
    End Sub
End Class