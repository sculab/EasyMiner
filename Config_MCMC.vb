Imports System.IO
Imports System.Threading
Public Class Config_MCMC
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If File.Exists(TextBox2.Text) Then
            If CSng(TextBox4.Text) > 0 Then
                Dim th1 As New Thread(AddressOf do_mcmctree)
                th1.Start()
                Me.Hide()
            Else
                MsgBox("The variance of the mutation rate must be greater than 0.")
            End If
        Else
            MsgBox("Sequence file must be provided.")
        End If


    End Sub

    Public Sub do_mcmctree()
        Dim rgene_a As Single = CSng(TextBox3.Text) * CSng(TextBox3.Text) / CSng(TextBox4.Text)
        Dim rgene_b As Single = CSng(TextBox3.Text) / CSng(TextBox4.Text)
        If TextBox2.Text.ToLower.EndsWith(".phy") = False Then
            ConvertFastaToPhylip(TextBox2.Text, Path.Combine(form_main.TextBox1.Text, "MCMC_infile.phy"))
        End If
        Using sr As New StreamReader(Path.Combine(root_path, "analysis", "mcmc_config.ctl"))
            Using sw As New StreamWriter(Path.Combine(form_main.TextBox1.Text, "MCMC_config.txt"))
                Dim temp_config As String = sr.ReadToEnd
                temp_config = temp_config.Replace("{seed}", TextBox1.Text)
                temp_config = temp_config.Replace("{rootage}", NumericUpDown1.Value.ToString)
                temp_config = temp_config.Replace("{model}", ComboBox1.SelectedIndex.ToString)
                temp_config = temp_config.Replace("{alpha}", ComboBox2.SelectedIndex.ToString)
                temp_config = temp_config.Replace("{rgene_a}", rgene_a.ToString)
                temp_config = temp_config.Replace("{rgene_b}", rgene_b.ToString)
                temp_config = temp_config.Replace("{burnin}", NumericUpDown2.Value.ToString)
                temp_config = temp_config.Replace("{sqmpfreq}", NumericUpDown3.Value.ToString)
                temp_config = temp_config.Replace("{nsample}", NumericUpDown4.Value.ToString)
                temp_config = temp_config.Replace("{cleandata}", ComboBox3.SelectedIndex.ToString)
                sw.Write(temp_config)
            End Using
        End Using
        safe_delete(Path.Combine(form_main.TextBox1.Text, "FigTree.tre"))
        Dim SI_mcmctree As New ProcessStartInfo()
        SI_mcmctree.FileName = currentDirectory + "analysis\mcmctree.exe" ' 替换为实际的命令行程序路径
        SI_mcmctree.WorkingDirectory = form_main.TextBox1.Text ' 替换为实际的运行文件夹
        SI_mcmctree.CreateNoWindow = False
        SI_mcmctree.Arguments = "MCMC_config.txt"
        Dim process_filter As Process = Process.Start(SI_mcmctree)
        process_filter.WaitForExit()
        process_filter.Close()
        'safe_copy(Path.Combine(root_path, "temp", "mcmc_file.txt"), Path.Combine(form_main.TextBox1.Text, "MCMC_log.txt"))
        'safe_copy(Path.Combine(root_path, "temp", "mcmc_output.txt"), Path.Combine(form_main.TextBox1.Text, "MCMC_output.txt"))
        If File.Exists(Path.Combine(form_main.TextBox1.Text, "FigTree.tre")) Then
            Using sr As New StreamReader(Path.Combine(form_main.TextBox1.Text, "FigTree.tre"))
                Using sw As New StreamWriter(Path.Combine(form_main.TextBox1.Text, "MCMC_dated.tree"))
                    sw.WriteLine(sr.ReadLine())
                    sw.WriteLine(sr.ReadLine())
                    sw.WriteLine(sr.ReadLine())
                    Dim dated_tree As String = sr.ReadLine().Substring(11)
                    Dim temp_tree As String = ""
                    Dim temp_node_id As Integer = -1
                    Dim fossil_count As Integer = 0
                    For Each my_char As Char In dated_tree
                        temp_tree += my_char
                        If my_char = ")" Then
                            temp_node_id += 1
                            Dim temp_sv As String = time_view.Item(temp_node_id).Item(1).ToString
                            temp_tree += " [&support_value=" + temp_sv + "]"
                        End If
                    Next
                    temp_tree = temp_tree.Replace("] [&", ",")
                    sw.WriteLine("UTREE 1 = " + temp_tree)
                    sw.Write(sr.ReadToEnd)
                End Using

            End Using
            MsgBox("Analysis Complete! Please check the 'MCMC_*' in output folder.")
        End If
        safe_delete(Path.Combine(form_main.TextBox1.Text, "FigTree.tre"))
        If form_config_mcmc.CheckBox1.Checked Then
            safe_delete(Path.Combine(form_main.TextBox1.Text, "MCMC_config.txt"))
            safe_delete(Path.Combine(form_main.TextBox1.Text, "MCMC_seqfile.phy"))
            safe_delete(Path.Combine(form_main.TextBox1.Text, "MCMC_intree.tree"))
            safe_delete(Path.Combine(form_main.TextBox1.Text, "SeedUsed"))
        End If
    End Sub
    Private Sub Config_MCMC_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 4
        ComboBox2.SelectedIndex = 1
        ComboBox3.SelectedIndex = 1
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta File(*.fasta)|*.fas;*.fasta;*.fa|PHYLIP File|*.phy"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".fas"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            TextBox2.Text = opendialog.FileName
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
    End Sub
End Class