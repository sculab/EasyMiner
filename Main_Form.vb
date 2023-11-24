Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Text
Imports System.Globalization
Imports System.Reflection.Emit

Public Class Main_Form

    Public Sub initialize_data()
        Dim Column_Select As New DataGridViewCheckBoxColumn
        Column_Select.HeaderText = "Select"
        DataGridView1.Columns.Insert(0, Column_Select)
        DataGridView1.AllowUserToAddRows = False

        Dim ref_table As New System.Data.DataTable
        ref_table.TableName = "Refs Table"
        Dim Column_ID As New System.Data.DataColumn("ID", System.Type.GetType("System.Int32"))
        Dim Column_Name As New System.Data.DataColumn("Name")
        Dim Column_Filter As New System.Data.DataColumn("Ref. Count")
        Dim Column_RF As New System.Data.DataColumn("Ref. Length")
        Dim Column_Reads As New System.Data.DataColumn("Filter Depth")
        Dim Column_Assemble As New System.Data.DataColumn("Assemble State")
        Dim Column_Align As New System.Data.DataColumn("Ass. Length")
        Dim Column_State As New System.Data.DataColumn("Ass. Depth")
        ref_table.Columns.Add(Column_ID)
        ref_table.Columns.Add(Column_Name)
        ref_table.Columns.Add(Column_Filter)
        ref_table.Columns.Add(Column_RF)
        ref_table.Columns.Add(Column_Reads)
        ref_table.Columns.Add(Column_Assemble)
        ref_table.Columns.Add(Column_Align)
        ref_table.Columns.Add(Column_State)
        mydata_Dataset.Tables.Add(ref_table)

        refsView = mydata_Dataset.Tables("Refs Table").DefaultView
        refsView.AllowNew = False
        refsView.AllowDelete = False
        refsView.AllowEdit = False

        Dim Column_Select1 As New DataGridViewCheckBoxColumn
        Column_Select1.HeaderText = "Select"
        DataGridView2.Columns.Insert(0, Column_Select1)
        DataGridView2.AllowUserToAddRows = False

        Dim data_table As New System.Data.DataTable
        data_table.TableName = "Data Table"
        Dim Column_ID1 As New System.Data.DataColumn("ID", System.Type.GetType("System.Int32"))
        Dim Column_1 As New System.Data.DataColumn("Data 1")
        Dim Column_2 As New System.Data.DataColumn("Data 2")
        data_table.Columns.Add(Column_ID1)
        data_table.Columns.Add(Column_1)
        data_table.Columns.Add(Column_2)

        mydata_Dataset.Tables.Add(data_table)

        seqsView = mydata_Dataset.Tables("Data Table").DefaultView
        seqsView.AllowNew = False
        seqsView.AllowDelete = False
        seqsView.AllowEdit = False
    End Sub
    Public Sub do_filter(ByVal refresh As Boolean, Optional no_window As Boolean = False)

        Dim SI_filter As New ProcessStartInfo()
        Dim filePath As String = out_dir + "\ref_reads_count_dict.txt"
        If refresh Then
            If File.Exists(filePath) Then
                ref_filter_result(filePath)
            Else
                MsgBox("Run failed, you should do filter first!")
                Exit Sub
            End If
            SI_filter.FileName = currentDirectory + "analysis\win_refilter.exe" ' 替换为实际的命令行程序路径
            SI_filter.WorkingDirectory = currentDirectory + "temp\" ' 替换为实际的运行文件夹路径
            SI_filter.CreateNoWindow = no_window
            SI_filter.Arguments = "-r " + """" + ref_dir + """"
            SI_filter.Arguments += " -q1" + q1 + " -q2" + q2
            SI_filter.Arguments += " -o " + """" + out_dir + """"
            SI_filter.Arguments += " -kf " + k1
            SI_filter.Arguments += " -s " + form_config_basic.NumericUpDown2.Value.ToString
            SI_filter.Arguments += " -gr " + form_config_basic.CheckBox2.Checked.ToString
            SI_filter.Arguments += " -lkd kmer_dict_k" + form_config_basic.NumericUpDown1.Value.ToString + ".dict"
            SI_filter.Arguments += " -rl " + reads_length.ToString
            SI_filter.Arguments += " -max_depth " + form_config_basic.NumericUpDown4.Value.ToString
            SI_filter.Arguments += " -max_size " + form_config_basic.NumericUpDown9.Value.ToString
            Dim process_filter As Process = Process.Start(SI_filter)
            process_filter.WaitForExit()
            process_filter.Close()
        Else
            If form_config_basic.CheckBox3.Checked And refs_type = "353" Then
                Dim result As DialogResult = MessageBox.Show("Should all reads be used?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    form_config_basic.CheckBox3.Checked = False
                End If
            End If
            SI_filter.FileName = currentDirectory + "analysis\win_filter.exe" ' 替换为实际的命令行程序路径
            SI_filter.WorkingDirectory = currentDirectory + "temp\" ' 替换为实际的运行文件夹路径
            SI_filter.CreateNoWindow = no_window
            SI_filter.Arguments = "-r " + """" + ref_dir + """"
            SI_filter.Arguments += " -q1" + q1 + " -q2" + q2
            SI_filter.Arguments += " -o " + """" + out_dir + """"
            SI_filter.Arguments += " -kf " + form_config_basic.NumericUpDown1.Value.ToString
            SI_filter.Arguments += " -s " + form_config_basic.NumericUpDown2.Value.ToString
            SI_filter.Arguments += " -gr " + form_config_basic.CheckBox2.Checked.ToString
            SI_filter.Arguments += " -lkd kmer_dict_k" + form_config_basic.NumericUpDown1.Value.ToString + ".dict"
            If form_config_basic.CheckBox3.Checked Then
                SI_filter.Arguments += " -m_reads " + form_config_basic.NumericUpDown3.Value.ToString
            Else
                SI_filter.Arguments += " -m_reads 1000000000"
            End If

            Dim process_filter As Process = Process.Start(SI_filter)
            process_filter.WaitForExit()
            process_filter.Close()


            If File.Exists(filePath) Then
                ref_filter_result(filePath)
            Else

                MsgBox("Could not find the filter result, please check option and try again!")
            End If
        End If

    End Sub
    Public Sub ref_filter_result(ByVal filePath As String)
        Try
            Dim count_dict As New Dictionary(Of String, Integer)

            ' 读取文件内容并将内容存入字典
            Using sr As New StreamReader(filePath)
                While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()
                    Dim parts As String() = line.Split(","c)

                    If parts.Length >= 2 Then
                        Dim key As String = parts(0)
                        Dim value As Integer

                        If Integer.TryParse(parts(1), value) Then
                            If count_dict.ContainsKey(key) Then
                                count_dict(key) = value
                            Else
                                count_dict.Add(key, value)
                            End If

                        End If
                    End If
                End While
            End Using
            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    If count_dict.ContainsKey(DataGridView1.Rows(i - 1).Cells(2).Value.ToString) Then
                        If reads_length = 0 Then
                            Dim sr As New StreamReader(out_dir + "\filtered\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + data_type)
                            If sr.ReadLine() Is Nothing = False Then
                                reads_length = sr.ReadLine().Length
                            End If
                            sr.Close()
                        End If
                        DataGridView1.Rows(i - 1).Cells(5).Value = CInt(count_dict(DataGridView1.Rows(i - 1).Cells(2).Value.ToString) / CInt(DataGridView1.Rows(i - 1).Cells(4).Value) * reads_length)
                    Else
                        DataGridView1.Rows(i - 1).Cells(5).Value = 0
                    End If
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub
    Public Sub do_assemble(Optional no_window As Boolean = False, Optional warning_message As Boolean = True)

        Dim SI_assembler As New ProcessStartInfo()
        SI_assembler.FileName = currentDirectory + "analysis\win_assembler.exe" ' 替换为实际的命令行程序路径
        SI_assembler.WorkingDirectory = currentDirectory + "temp\" ' 替换为实际的运行文件夹路径
        SI_assembler.CreateNoWindow = no_window
        SI_assembler.Arguments = "-r " + """" + ref_dir + """"
        SI_assembler.Arguments += " -q1" + q1 + " -q2" + q2
        SI_assembler.Arguments += " -o " + """" + out_dir + """"
        SI_assembler.Arguments += " -kf " + k1
        SI_assembler.Arguments += " -s " + form_config_basic.NumericUpDown2.Value.ToString
        SI_assembler.Arguments += " -gr " + form_config_basic.CheckBox2.Checked.ToString
        SI_assembler.Arguments += " -lkd kmer_dict_k" + form_config_basic.NumericUpDown1.Value.ToString + ".dict"
        SI_assembler.Arguments += " -gr " + form_config_basic.CheckBox2.Checked.ToString
        If form_config_basic.CheckBox1.Checked Then
            SI_assembler.Arguments += " -ka 0"
        Else
            SI_assembler.Arguments += " -ka " + k2
        End If
        SI_assembler.Arguments += " -k_min " + form_config_basic.NumericUpDown6.Value.ToString
        SI_assembler.Arguments += " -k_max " + form_config_basic.NumericUpDown7.Value.ToString
        SI_assembler.Arguments += " -limit_count " + form_config_basic.NumericUpDown8.Value.ToString
        SI_assembler.Arguments += " -p " + max_thread.ToString
        Dim process_filter As Process = Process.Start(SI_assembler)
        process_filter.WaitForExit()
        process_filter.Close()

        Dim filePath As String = out_dir + "\result_dict.txt"
        If File.Exists(filePath) Then
            ref_assemble_result(filePath)
        Else
            MsgBox("Could not find assemble result, please check option and try again!")
        End If


    End Sub

    Public Sub ref_assemble_result(ByVal filePath As String)
        Try
            Dim result_dict As New Dictionary(Of String, String)

            ' 读取文件内容并将内容存入字典
            Using sr As New StreamReader(filePath)
                While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()
                    Dim parts As String() = line.Split(","c)

                    If parts.Length >= 3 Then
                        Dim key As String = parts(0)
                        If result_dict.ContainsKey(key) Then
                            result_dict(key) = parts(1) + "," + parts(2)
                        Else
                            result_dict.Add(key, parts(1) + "," + parts(2))
                        End If
                    End If
                End While
            End Using
            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    If result_dict.ContainsKey(DataGridView1.Rows(i - 1).Cells(2).Value.ToString) Then
                        DataGridView1.Rows(i - 1).Cells(6).Value = result_dict(DataGridView1.Rows(i - 1).Cells(2).Value.ToString).Split(","c)(0)
                        If DataGridView1.Rows(i - 1).Cells(6).Value <> "success" Then
                            'DataGridView1.Rows(i - 1).Cells(6).Value = "failed"
                            DataGridView1.Rows(i - 1).Cells(7).Value = 0
                            DataGridView1.Rows(i - 1).Cells(8).Value = "failed"
                        End If
                        If File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                            Try

                                If reads_length = 0 Then
                                    Dim sr_r As New StreamReader(out_dir + "\filtered\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + data_type)
                                    If sr_r.ReadLine() Is Nothing = False Then
                                        reads_length = sr_r.ReadLine().Length
                                    End If
                                    sr_r.Close()
                                End If

                                Dim sr As New StreamReader(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                                sr.ReadLine()
                                DataGridView1.Rows(i - 1).Cells(7).Value = sr.ReadLine().Length
                                sr.Close()
                                DataGridView1.Rows(i - 1).Cells(8).Value = (CInt(result_dict(DataGridView1.Rows(i - 1).Cells(2).Value.ToString).Split(","c)(1)) * reads_length / CInt(DataGridView1.Rows(i - 1).Cells(7).Value)).ToString("F0")

                            Catch ex As Exception
                                MsgBox(ex.ToString)
                                File.Delete(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                                DataGridView1.Rows(i - 1).Cells(6).Value = "failed"
                                DataGridView1.Rows(i - 1).Cells(7).Value = 0
                                DataGridView1.Rows(i - 1).Cells(8).Value = "failed"
                            End Try

                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub
    Private Sub Main_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        currentDirectory = Application.StartupPath
        initialize_data()
        TextBox1.Text = currentDirectory + "results"
        NumericUpDown10.Maximum = System.Environment.ProcessorCount
        AddHandler form_config_basic.ConfirmClicked, AddressOf ConfirmClickedHandler
        AddHandler form_config_basic.CancelClicked, AddressOf SubCancel
    End Sub
    Private Sub SubCancel()
    End Sub
    Private Sub Main_Form_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        settings("language") = language
        settings("database_url") = database_url
        SaveSettings(root_path + "analysis\" + "setting.ini", settings)
        End
    End Sub

    Public Sub refresh_DataGridView1()
        DataGridView1.DataSource = refsView
        refsView.AllowNew = False
        refsView.AllowEdit = True

        DataGridView1.Columns(1).ReadOnly = True
        DataGridView1.Columns(2).ReadOnly = True
        DataGridView1.Columns(3).ReadOnly = True
        DataGridView1.Columns(4).ReadOnly = True
        DataGridView1.Columns(5).ReadOnly = True
        DataGridView1.Columns(6).ReadOnly = True
        DataGridView1.Columns(7).ReadOnly = True
        DataGridView1.Columns(8).ReadOnly = True

        DataGridView1.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(5).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(6).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(7).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(8).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(0).Width = 50
        DataGridView1.Columns(1).Width = 50
        DataGridView1.Columns(2).Width = 160
        DataGridView1.Columns(3).Width = 80
        DataGridView1.Columns(4).Width = 100
        DataGridView1.Columns(5).Width = 100
        DataGridView1.Columns(6).Width = 160
        DataGridView1.Columns(7).Width = 100
        DataGridView1.Columns(8).Width = 100
        DataGridView1.RefreshEdit()
        GC.Collect()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Select Case timer_id
            Case 0
                ProgressBar1.Value = PB_value
            Case 1
                ProgressBar1.Value = PB_value
                If ProgressBar1.Value = 100 Then
                    PB_value = 0
                    timer_id = 0
                End If

            Case 2
                Timer1.Enabled = False
                refresh_DataGridView1()

                data_loaded = True
                timer_id = 0
                Timer1.Enabled = True

            Case 3
                Timer1.Enabled = False
                DataGridView2.DataSource = seqsView
                seqsView.AllowNew = False
                seqsView.AllowEdit = True

                DataGridView2.Columns(1).ReadOnly = True
                DataGridView2.Columns(2).ReadOnly = True
                DataGridView2.Columns(3).ReadOnly = True

                DataGridView2.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
                DataGridView2.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
                DataGridView2.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable
                DataGridView2.Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable
                DataGridView2.Columns(0).Width = 50
                DataGridView2.Columns(1).Width = 50
                DataGridView2.Columns(2).Width = 400
                DataGridView2.Columns(3).Width = 400
                Timer1.Enabled = True
                DataGridView2.RefreshEdit()
                GC.Collect()
                timer_id = 0
                data_loaded = True
            Case 4
                If PB_value = -1 Then
                    PB_value = 0
                    timer_id = 0
                Else
                    ProgressBar1.Value = PB_value
                End If
            Case 5
                form_config_split.Show()
                timer_id = 0
            Case 6
                Timer1.Enabled = False
                Dim opendialog As New SaveFileDialog
                opendialog.Filter = "FastQ File (*.fq)|*.fq;*.FQ"
                opendialog.FileName = ""
                opendialog.DefaultExt = ".fq"
                opendialog.CheckFileExists = False
                opendialog.CheckPathExists = True
                Dim resultdialog As DialogResult = opendialog.ShowDialog()
                If resultdialog = DialogResult.OK Then
                    For i As Integer = 1 To seqsView.Count
                        If File.Exists(currentDirectory + "\results\Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".1.fq") And File.Exists(currentDirectory + "\results\Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".2.fq") Then

                            My.Computer.FileSystem.MoveFile(currentDirectory + "\results\Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".1.fq", opendialog.FileName.Substring(0, opendialog.FileName.Length - 3) + "_" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".1.fq", True)
                            My.Computer.FileSystem.MoveFile(currentDirectory + "\results\Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".2.fq", opendialog.FileName.Substring(0, opendialog.FileName.Length - 3) + "_" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".2.fq", True)

                        End If
                    Next
                End If
                Timer1.Enabled = True
                timer_id = 0
            Case 7
                form_config_plasty.Show()
                timer_id = 0
            Case 8 '批量细胞器拼接
                timer_id = 0
                Timer1.Enabled = False
                DataGridView1.EndEdit()
                Dim th1 As New Thread(AddressOf batch_assemble)
                If cpg_down_mode = 4 Then
                    th1.Start("cp")
                ElseIf cpg_down_mode = 5 Then
                    th1.Start("mito_plant")
                ElseIf cpg_down_mode = 9 Then
                    th1.Start("mito")
                End If

                Timer1.Enabled = True
            Case 9
                Timer1.Enabled = False
                refresh_DataGridView1()
                Timer1.Enabled = True
                data_loaded = True
                form_config_plasty.Show()
                timer_id = 0
        End Select
    End Sub
    Public Function check_batch_folder()
        Dim is_ready As Boolean = True
        For batch_i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                If My.Computer.FileSystem.DirectoryExists((TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")) = False Then
                    is_ready = False
                    Exit For
                End If
            End If
        Next
        Return is_ready
    End Function

    Public Sub batch_assemble(ByVal database_type As String)
        If File.Exists(TextBox1.Text + "\kmer_dict_k16.dict") Then
            File.Delete(TextBox1.Text + "\kmer_dict_k16.dict")
        End If
        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")
                DeleteDir(out_dir + "\NOVOPlasty")
                My.Computer.FileSystem.CreateDirectory(out_dir + "\NOVOPlasty")
                If database_type = "mito_plant" Then
                    If File.Exists(out_dir + "\Organelle\Gennome_cp.fasta") = False Then
                        File.AppendAllText(TextBox1.Text + "\log.txt", "The chloroplast genome was not found in the " + folder_name & Environment.NewLine)
                        Continue For
                    End If
                End If
                q1 = " " + """" + DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                q2 = " " + """" + DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                Dim SI_filter As New ProcessStartInfo()
                Dim count_file As String = out_dir + "\NOVOPlasty\ref_reads_count_dict.txt"
                SI_filter.FileName = currentDirectory + "analysis\win_filter.exe" ' 替换为实际的命令行程序路径
                SI_filter.WorkingDirectory = currentDirectory + "temp\" ' 替换为实际的运行文件夹路径
                SI_filter.CreateNoWindow = False
                SI_filter.Arguments = "-r " + """" + ref_dir + """"
                SI_filter.Arguments += " -q1" + q1 + " -q2" + q2
                SI_filter.Arguments += " -o " + """" + out_dir + "\NOVOPlasty" + """"
                SI_filter.Arguments += " -kf 16"
                SI_filter.Arguments += " -s " + form_config_basic.NumericUpDown2.Value.ToString
                SI_filter.Arguments += " -gr " + form_config_basic.CheckBox2.Checked.ToString
                SI_filter.Arguments += " -lkd ..\..\kmer_dict_k16.dict"
                If form_config_basic.CheckBox3.Checked Then
                    SI_filter.Arguments += " -m_reads " + form_config_basic.NumericUpDown3.Value.ToString
                Else
                    SI_filter.Arguments += " -m_reads 1000000000"
                End If
                SI_filter.Arguments += " -m 1"
                Dim process_filter As Process = Process.Start(SI_filter)
                process_filter.WaitForExit()
                process_filter.Close()
                If File.Exists(count_file) Then
                    Dim best_ref As String = ""
                    Dim max_value As Integer = 0
                    Using sr As New StreamReader(count_file)
                        While Not sr.EndOfStream
                            Dim line As String = sr.ReadLine()
                            Dim parts As String() = line.Split(","c)

                            If parts.Length >= 2 Then
                                If max_value < CInt(parts(1)) Then
                                    max_value = CInt(parts(1))
                                    best_ref = parts(0)
                                End If
                            End If
                        End While
                    End Using
                    If best_ref <> "" Then
                        Dim best_gb As String = best_ref.Split("#")(1).Replace(".fasta", "")
                        File.Copy(get_genome_data(database_type, "gb", best_gb).Result, out_dir + "\NOVOPlasty\ref_gb.gb", True)

                        File.Copy(ref_dir + best_ref + ".fasta", out_dir + "\NOVOPlasty\" + best_ref + ".fasta", True)
                        File.Move(out_dir + "\NOVOPlasty\filtered\all_1.fq", out_dir + "\NOVOPlasty\Project1.1.fq", True)
                        File.Move(out_dir + "\NOVOPlasty\filtered\all_2.fq", out_dir + "\NOVOPlasty\Project1.2.fq", True)
                        Dim sw1 As New StreamWriter(out_dir + "\NOVOPlasty\batch_file.txt")
                        sw1.WriteLine("Project1")
                        sw1.WriteLine(best_ref + ".fasta")
                        sw1.WriteLine("Project1.1.fq")
                        sw1.WriteLine("Project1.2.fq")
                        sw1.Close()

                        Dim sr As New StreamReader(currentDirectory + "\analysis\NOVO_config.txt")
                        Dim config_text As String = sr.ReadToEnd
                        Dim sw As New StreamWriter(out_dir + "\NOVOPlasty\NOVO_config.txt")
                        config_text = config_text.Replace("$batch_file$", "batch:batch_file.txt")
                        config_text = config_text.Replace("$type$", form_config_plasty.ComboBox1.Text)
                        config_text = config_text.Replace("$range$", form_config_plasty.TextBox1.Text)
                        config_text = config_text.Replace("$k-mer$", form_config_plasty.NumericUpDown1.Value.ToString)
                        config_text = config_text.Replace("$mem$", form_config_plasty.NumericUpDown2.Value.ToString)
                        config_text = config_text.Replace("$read_length$", form_config_plasty.NumericUpDown4.Value.ToString)
                        If form_config_plasty.NumericUpDown3.Value = 0 Then
                            config_text = config_text.Replace("$insert_size$", "")
                        Else
                            config_text = config_text.Replace("$insert_size$", form_config_plasty.NumericUpDown3.Value.ToString)
                        End If
                        config_text = config_text.Replace("$ref$", best_ref + ".fasta")
                        config_text = config_text.Replace("$chlo$", form_config_plasty.TextBox3.Text)
                        config_text = config_text.Replace("$out$", ".\")
                        sw.Write(config_text)
                        sw.Close()
                        sr.Close()

                        Dim SI_build_plasty As New ProcessStartInfo()
                        SI_build_plasty.FileName = currentDirectory + "analysis\NOVOPlasty4.3.4.exe" ' 替换为实际的命令行程序路径
                        SI_build_plasty.WorkingDirectory = out_dir + "\NOVOPlasty" ' 替换为实际的运行文件夹路径
                        SI_build_plasty.CreateNoWindow = False
                        SI_build_plasty.Arguments = "-c NOVO_config.txt"
                        Dim process_build_plasty As Process = Process.Start(SI_build_plasty)
                        process_build_plasty.WaitForExit()
                        process_build_plasty.Close()
                        If DebugToolStripMenuItem.Checked = False Then
                            If File.Exists(out_dir + "\NOVOPlasty\Project1.1.fq") Then
                                File.Delete(out_dir + "\NOVOPlasty\Project1.1.fq")
                            End If
                            If File.Exists(out_dir + "\NOVOPlasty\Project1.2.fq") Then
                                File.Delete(out_dir + "\NOVOPlasty\Project1.2.fq")
                            End If
                        End If
                        Dim assemble_file As String = ""
                        If File.Exists(out_dir + "\NOVOPlasty\Circularized_assembly_1_Project1.fasta") Then
                            assemble_file = out_dir + "\NOVOPlasty\Circularized_assembly_1_Project1.fasta"
                        End If
                        If File.Exists(out_dir + "\NOVOPlasty\Option_1_Project1.fasta") Then
                            Dim SI_check_option As New ProcessStartInfo()
                            If database_type = "mito_plant" Then
                                SI_check_option.FileName = currentDirectory + "analysis\check_option_blast.exe"
                            Else
                                SI_check_option.FileName = currentDirectory + "analysis\check_option_mafft.exe"
                            End If
                            SI_check_option.WorkingDirectory = out_dir + "\NOVOPlasty\"
                            SI_check_option.CreateNoWindow = False
                            SI_check_option.Arguments = "-i " + """" + out_dir + "\NOVOPlasty" + """" + " -r " + """" + best_ref + ".fasta" + """" + " -o " + "best.fasta"
                            Dim process_check_option As Process = New Process()
                            process_check_option.StartInfo = SI_check_option
                            process_check_option.Start()
                            process_check_option.WaitForExit()
                            process_check_option.Close()
                            If File.Exists(out_dir + "\NOVOPlasty\best.fasta") Then
                                assemble_file = out_dir + "\NOVOPlasty\best.fasta"
                            End If
                        End If
                        If File.Exists(assemble_file) Then
                            If database_type = "mito_plant" Then
                                Dim lines As List(Of String) = File.ReadAllLines(assemble_file).ToList()
                                If lines.Count > 0 Then
                                    lines(0) = ">" + folder_name
                                End If
                                File.WriteAllLines(out_dir + "\Organelle\" + folder_name + ".fasta", lines)
                                Continue For
                            End If
                            build_ann(out_dir + "\NOVOPlasty\" + best_ref + ".fasta", assemble_file, out_dir + "\NOVOPlasty\ref_gb.gb", out_dir + "\NOVOPlasty\output", currentDirectory + "temp\")
                            My.Computer.FileSystem.CreateDirectory(out_dir + "\Organelle\")
                            If File.Exists(out_dir + "\Organelle\warning.txt") Then
                                File.Delete(out_dir + "\Organelle\warning.txt")
                            End If
                            If File.Exists(out_dir + "\NOVOPlasty\output.fasta") Then
                                Dim lines As List(Of String) = File.ReadAllLines(out_dir + "\NOVOPlasty\output.fasta").ToList()
                                If lines.Count > 0 Then
                                    lines(0) = ">" + folder_name
                                End If
                                File.WriteAllLines(out_dir + "\Organelle\" + folder_name + ".fasta", lines)

                                Dim formattedDate As String = Date.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper()
                                lines = File.ReadAllLines(out_dir + "\NOVOPlasty\output.gb").ToList()
                                If lines.Count > 12 Then
                                    For i As Integer = 0 To 6
                                        lines(i) = lines(i).Replace("XX000000", folder_name).Replace("UNK 01-JAN-1980", formattedDate)
                                    Next
                                End If
                                File.WriteAllLines(out_dir + "\Organelle\" + folder_name + ".gb", lines)
                            Else
                                Dim lines As List(Of String) = File.ReadAllLines(assemble_file).ToList()
                                If lines.Count > 0 Then
                                    lines(0) = ">" + folder_name
                                End If
                                File.WriteAllLines(out_dir + "\Organelle\" + folder_name + ".fasta", lines)
                                File.AppendAllText(TextBox1.Text + "\log.txt", "The organelle genome of the " + folder_name + " lacks annotation." & Environment.NewLine)

                            End If
                            If File.Exists(out_dir + "\NOVOPlasty\output_log.txt") Then
                                File.Copy(out_dir + "\NOVOPlasty\output_log.txt", out_dir + "\Organelle\" + folder_name + "_warning.txt", True)
                            End If
                        Else
                            File.AppendAllText(TextBox1.Text + "\log.txt", "The organelle genome of the " + folder_name + " is not circularized." & Environment.NewLine)
                        End If
                    End If
                End If
            End If
        Next
        PB_value = 0
        MsgBox("Analysis completed!")
    End Sub
    Private Sub 测序文件ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 测序文件ToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "FastQ File(*.fq;*.fq.gz)|*.fq;*.fastq;*.FQ;*.fq.gz;*.gz|Fasta File(*.fasta)|*.fas;*.fasta;*.fa"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".fq"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            If opendialog.FileName.ToLower.EndsWith(".gz") Or opendialog.FileName.ToLower.EndsWith(".fq") Or opendialog.FileName.ToLower.EndsWith(".fastq") Then
                data_type = ".fq"
            Else
                data_type = ".fasta"
            End If
            If opendialog.FileNames.Length = 1 Then
                'mydata_Dataset.Tables("Data Table").Clear()
                data_loaded = False
                Dim newrow(2) As String
                seqsView.AllowNew = True
                seqsView.AddNew()
                newrow(0) = seqsView.Count
                newrow(1) = opendialog.FileNames(0)
                newrow(2) = opendialog.FileNames(0)
                seqsView.Item(seqsView.Count - 1).Row.ItemArray = newrow

                timer_id = 3
            Else
                Dim result As DialogResult = MessageBox.Show("Are these paired sequencing files?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                ' 根据用户的选择执行相应的操作
                If result = DialogResult.Yes Then
                    If opendialog.FileNames.Length Mod 2 = 0 Then
                        data_loaded = False
                        Dim sortedFileNames As String() = opendialog.FileNames.OrderBy(Function(path) path).ToArray()
                        For i As Integer = 1 To opendialog.FileNames.Length / 2
                            Dim newrow(2) As String
                            seqsView.AllowNew = True
                            seqsView.AddNew()
                            newrow(0) = seqsView.Count
                            newrow(1) = sortedFileNames((i - 1) * 2)
                            newrow(2) = sortedFileNames((i - 1) * 2 + 1)
                            seqsView.Item(seqsView.Count - 1).Row.ItemArray = newrow
                        Next
                        timer_id = 3
                    Else
                        MsgBox("The files are not in pairs (the number of files cannot be divided by 2 evenly).")
                    End If
                Else
                    data_loaded = False
                    Dim sortedFileNames As String() = opendialog.FileNames.OrderBy(Function(path) path).ToArray()
                    For i As Integer = 1 To opendialog.FileNames.Length
                        Dim newrow(2) As String
                        seqsView.AllowNew = True
                        seqsView.AddNew()
                        newrow(0) = seqsView.Count
                        newrow(1) = sortedFileNames(i - 1)
                        newrow(2) = sortedFileNames(i - 1)
                        seqsView.Item(seqsView.Count - 1).Row.ItemArray = newrow
                    Next
                    timer_id = 3
                End If

            End If
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete
        If data_loaded = False And refsView.Count > 0 Then
            For i As Integer = 1 To refsView.Count
                DataGridView1.Rows(i - 1).Cells(0).Value = True
            Next
        End If
    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick

    End Sub

    Private Sub DataGridView2_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView2.DataBindingComplete
        If data_loaded = False And seqsView.Count > 0 Then
            For i As Integer = 1 To seqsView.Count
                DataGridView2.Rows(i - 1).Cells(0).Value = True
            Next
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim opendialog As New FolderBrowserDialog
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            If opendialog.SelectedPath.EndsWith(":\") Then
                MsgBox("Please do not save to the root directory!")
            Else
                If Directory.GetFileSystemEntries(opendialog.SelectedPath).Length > 0 Then
                    Dim result As DialogResult = MessageBox.Show("The folder is not empty, its contents may be deleted. Are you sure to use this folder?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    ' 根据用户的选择执行相应的操作
                    If result = DialogResult.Yes Then

                        TextBox1.Text = opendialog.SelectedPath

                    End If
                Else
                    TextBox1.Text = opendialog.SelectedPath
                End If
            End If
        End If
    End Sub


    Private Sub 拼接ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 拼接ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            MenuClicked = "assemble"
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            form_config_basic.GroupBox2.Enabled = False
            form_config_basic.GroupBox3.Enabled = False
            form_config_basic.GroupBox4.Enabled = True
            form_config_basic.Show()

        Else
            MsgBox("Please select an output folder!")
        End If

    End Sub
    Private Sub menu_assemble()
        Dim refs_count As Integer = 0
        Dim seqs_count As Integer = 0
        Dim has_assemble As Boolean = False
        reads_length = 0
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        out_dir = TextBox1.Text.Replace("\", "/")
        q1 = ""
        q2 = ""
        k1 = form_config_basic.NumericUpDown1.Value.ToString
        k2 = form_config_basic.NumericUpDown5.Value.ToString
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)


        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                refs_count += 1
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
                If File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                    has_assemble = True
                End If
            End If
        Next
        If has_assemble Then
            Dim result As DialogResult = MessageBox.Show("Reassemble the successfully processed entries?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            ' 根据用户的选择执行相应的操作
            If result = DialogResult.Yes Then
                For i As Integer = 1 To refsView.Count
                    If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                        If File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                            File.Delete(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                        End If
                    End If
                Next
            End If
        End If

        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                seqs_count += 1
                q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                Else
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                End If
            End If
        Next

        If seqs_count >= 1 And refs_count >= 1 Then
            Dim th1 As New Thread(AddressOf do_assemble)
            th1.Start()
        Else
            MsgBox("Please select at least one reference and one sequencing data!")
        End If
    End Sub


    Private Sub 迭代ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 迭代ToolStripMenuItem.Click

    End Sub
    Public Sub do_iteration(ByVal times As Integer)
        For x As Integer = 1 To times
            PB_value = x / times * 100
            DataGridView1.EndEdit()
            Dim refs_count As Integer = 0
            Dim seqs_count As Integer = 0
            reads_length = 0
            ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
            out_dir = (TextBox1.Text + "/iteration").Replace("\", "/")

            My.Computer.FileSystem.CreateDirectory(out_dir)
            q1 = ""
            q2 = ""
            k1 = form_config_basic.NumericUpDown1.Value.ToString
            k2 = form_config_basic.NumericUpDown5.Value.ToString
            DeleteDir(ref_dir)
            My.Computer.FileSystem.CreateDirectory(ref_dir)

            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    If File.Exists(TextBox1.Text + "\iteration\contigs_all\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                        refs_count += 1
                        safe_copy(TextBox1.Text + "\iteration\contigs_all\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
                    End If
                End If
            Next
            DeleteDir(out_dir)
            For i As Integer = 1 To seqsView.Count
                If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    seqs_count += 1
                    q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                    If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                        q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                    Else
                        q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                    End If
                End If
            Next
            If seqs_count >= 1 And refs_count >= 1 Then
                do_filer_assemble(True)
            End If
        Next
        PB_value = -1
    End Sub
    Public Sub do_filer_assemble(Optional no_window As Boolean = False)
        do_filter(False, no_window)
        do_filter(True, no_window)
        do_assemble(no_window)
    End Sub

    Private Sub 进一步过滤ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 进一步过滤ToolStripMenuItem.Click
        MenuClicked = "refilter"

        form_config_basic.GroupBox2.Enabled = False
        form_config_basic.GroupBox3.Enabled = True
        form_config_basic.GroupBox4.Enabled = False
        form_config_basic.Show()
    End Sub
    Private Sub menu_refilter()
        DataGridView1.EndEdit()
        Dim refs_count As Integer = 0
        Dim seqs_count As Integer = 0
        reads_length = 0
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        out_dir = TextBox1.Text.Replace("\", "/")
        q1 = ""
        q2 = ""
        k1 = form_config_basic.NumericUpDown1.Value.ToString
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)

        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                refs_count += 1
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
            End If
        Next

        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                seqs_count += 1
                q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                Else
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                End If
            End If
        Next

        If seqs_count >= 1 And refs_count >= 1 Then
            Dim th1 As New Thread(AddressOf do_filter)
            th1.Start(True)
        Else
            MsgBox("Please select at least one reference and one sequencing data!")
        End If
    End Sub
    Private Sub 从头过滤ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 从头过滤ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            MenuClicked = "filter"
            form_config_basic.GroupBox2.Enabled = True
            form_config_basic.GroupBox3.Enabled = False
            form_config_basic.GroupBox4.Enabled = False
            form_config_basic.NumericUpDown1.Value = 21
            form_config_basic.Show()

        Else
            MsgBox("Please select an output folder!")
        End If

    End Sub
    Private Sub menu_filter()
        If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 Then
            Dim result As DialogResult = MessageBox.Show("Clear the output directory? If you are optimizing for previous results, please select 'NO'!", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                DeleteDir(TextBox1.Text)
                My.Computer.FileSystem.CreateDirectory(TextBox1.Text)
            End If
        End If

        Dim refs_count As Integer = 0
        Dim seqs_count As Integer = 0
        reads_length = 0
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        out_dir = TextBox1.Text.Replace("\", "/")
        q1 = ""
        q2 = ""
        k1 = form_config_basic.NumericUpDown1.Value.ToString
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)

        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                refs_count += 1
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
            End If
        Next

        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                seqs_count += 1
                q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                Else
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                End If
            End If
        Next

        If seqs_count >= 1 And refs_count >= 1 Then
            Dim th1 As New Thread(AddressOf do_filter)
            th1.Start(False)
        Else
            MsgBox("Please select at least one reference and one sequencing data!")
        End If
    End Sub
    Public Function check_data_count()
        Dim seqs_count As Integer = 0
        Dim refs_count As Integer = 0
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                refs_count += 1
            End If
        Next

        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                seqs_count += 1
            End If
        Next

        Return {refs_count, seqs_count}
    End Function


    Private Sub 载入参考序列ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 载入参考序列ToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta File(*.fasta)|*.fas;*.fasta;*.fa|GenBank File|*.gb"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".fas"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            current_file = opendialog.FileName
            mydata_Dataset.Tables("Refs Table").Clear()
            DataGridView1.DataSource = Nothing
            data_loaded = False
            If UBound(opendialog.FileNames) >= 1 Then
                If opendialog.FileName.ToLower.EndsWith(".gb") Then
                    Dim sw As New StreamWriter(root_path + "temp\temp.gb")
                    For Each file_name As String In opendialog.FileNames
                        Dim sr As New StreamReader(file_name)
                        sw.Write(sr.ReadToEnd)
                        sr.Close()
                    Next
                    sw.Close()
                    current_file = root_path + "temp\temp.gb"
                    form_config_split.Show()
                    refs_type = "gb"
                Else
                    DeleteDir(root_path + "temp\org_seq")
                    My.Computer.FileSystem.CreateDirectory(root_path + "temp\org_seq")
                    For Each FileName As String In opendialog.FileNames
                        safe_copy(FileName, root_path + "temp\org_seq\" + System.IO.Path.GetFileNameWithoutExtension(FileName).Replace(" ", "_").Replace(".", "_").Replace("-", "_") + ".fasta")
                    Next
                    refs_type = "fasta"
                    refresh_file()
                    timer_id = 2
                End If

            Else

                current_file = opendialog.FileName
                Dim result As DialogResult = MessageBox.Show("Importing as gene list? If importing as file list, select 'No'", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    If opendialog.FileName.ToLower.EndsWith(".gb") Then
                        form_config_split.Show()
                        refs_type = "gb"
                    Else
                        DeleteDir(root_path + "temp\org_seq")
                        My.Computer.FileSystem.CreateDirectory(root_path + "temp\org_seq")
                        safe_copy(current_file, root_path + "temp\org_seq\" + System.IO.Path.GetFileNameWithoutExtension(current_file).Replace(" ", "_").Replace(".", "_").Replace("-", "_") + ".fasta", True)
                        refs_type = "fasta"
                        refresh_file()
                        timer_id = 2
                    End If
                Else
                    DeleteDir(root_path + "temp\org_seq")
                    My.Computer.FileSystem.CreateDirectory(root_path + "temp\org_seq")

                    Dim SI_split_file As New ProcessStartInfo()
                    SI_split_file.FileName = currentDirectory + "analysis\split_file.exe" ' 替换为实际的命令行程序路径
                    SI_split_file.WorkingDirectory = currentDirectory + "analysis\" ' 替换为实际的运行文件夹路径
                    SI_split_file.CreateNoWindow = True

                    SI_split_file.Arguments = "-i " + """" + current_file + """"
                    SI_split_file.Arguments += " -o " + """" + root_path + "temp\org_seq" + """"
                    If opendialog.FileName.ToLower.EndsWith(".gb") Then
                        SI_split_file.Arguments += " -f genbank"
                        refs_type = "gb"
                    Else
                        SI_split_file.Arguments += " -f fasta"
                        refs_type = "fasta"
                    End If
                    Dim process_split_file As Process = Process.Start(SI_split_file)
                    process_split_file.WaitForExit()
                    process_split_file.Close()
                    refresh_file()
                    timer_id = 2
                End If

            End If

        End If
    End Sub

    Private Sub 导出ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 导出ToolStripMenuItem.Click
        Dim opendialog As New SaveFileDialog
        opendialog.Filter = "CSV File (*.csv)|*.csv;*.CSV"
        opendialog.FileName = ""
        opendialog.DefaultExt = ".csv"
        opendialog.CheckFileExists = False
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            If opendialog.FileName.ToLower.EndsWith(".csv") Then
                Dim dw As New StreamWriter(opendialog.FileName, False)
                Dim state_line As String = "ID,Name"
                For j As Integer = 3 To DataGridView1.ColumnCount - 1
                    state_line += "," + DataGridView1.Columns(j).HeaderText
                Next
                dw.WriteLine(state_line)
                For i As Integer = 1 To refsView.Count
                    state_line = i.ToString
                    For j As Integer = 2 To DataGridView1.ColumnCount - 1
                        state_line += "," + refsView.Item(i - 1).Item(j - 1)
                    Next
                    dw.WriteLine(state_line)
                Next
                dw.Close()
            End If
        End If
    End Sub

    Private Sub 全自动ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 全自动ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            form_config_basic.GroupBox2.Enabled = True
            form_config_basic.GroupBox3.Enabled = True
            form_config_basic.GroupBox4.Enabled = True
            form_config_basic.NumericUpDown1.Value = 21
            MenuClicked = "auto_assemble"
            form_config_basic.Show()

        Else
            MsgBox("Please select an output folder!")
        End If

    End Sub
    Private Sub menu_auto_assemble()
        If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 Then
            Dim result As DialogResult = MessageBox.Show("Clear the output directory?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                DeleteDir(TextBox1.Text)
                My.Computer.FileSystem.CreateDirectory(TextBox1.Text)
            End If
        End If
        Dim refs_count As Integer = 0
        Dim seqs_count As Integer = 0
        reads_length = 0
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        out_dir = TextBox1.Text.Replace("\", "/")
        q1 = ""
        q2 = ""
        k1 = form_config_basic.NumericUpDown1.Value.ToString
        k2 = form_config_basic.NumericUpDown5.Value.ToString
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)


        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                refs_count += 1
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
            End If
        Next
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                If File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                    File.Delete(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                End If
            End If
        Next

        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                seqs_count += 1
                q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                Else
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                End If
            End If
        Next

        If seqs_count >= 1 And refs_count >= 1 Then
            Dim th1 As New Thread(AddressOf do_filer_assemble)
            th1.Start()
        Else
            MsgBox("Please select at least one reference and one sequencing data!")
        End If
    End Sub
    Private Sub 导出参考序列ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 导出参考序列ToolStripMenuItem.Click
        Dim opendialog As New FolderBrowserDialog
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", opendialog.SelectedPath + "\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
                End If
            Next
        End If
    End Sub

    Private Sub 构建质体基因组ToolStripMenuItem_Click(sender As Object, e As EventArgs)


    End Sub

    Private Sub 导出测序文件ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 导出测序文件ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            Dim opendialog As New FolderBrowserDialog
            Dim resultdialog As DialogResult = opendialog.ShowDialog()
            If resultdialog = DialogResult.OK Then
                If opendialog.SelectedPath.EndsWith(":\") Then
                    MsgBox("Please do not save to the root directory!")
                Else

                    Dim my_input As String = InputBox("Number of Reads to SKIP (Million)", "SKIP", 0)
                    Dim reads_skip, reads_keep As Integer
                    If Not Integer.TryParse(my_input, reads_skip) Then
                        Exit Sub
                    End If
                    my_input = InputBox("Number of Reads to KEEP (Million)", "KEEP", 10)
                    If Not Integer.TryParse(my_input, reads_keep) Then
                        Exit Sub
                    End If
                    Dim th1 As New Threading.Thread(AddressOf export_seq)
                    th1.Start({reads_skip, reads_keep, opendialog.SelectedPath})
                End If

            End If

        Else
            MsgBox("Please select an output folder!")
        End If

    End Sub
    Public Sub export_seq(ByVal para() As String)
        For i As Integer = 1 To seqsView.Count
            If form_main.DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(i - 1).Cells(3).Value.ToString))

                Dim SI_build_fq As New ProcessStartInfo()
                SI_build_fq.FileName = currentDirectory + "analysis\build_fq.exe" ' 替换为实际的命令行程序路径
                SI_build_fq.WorkingDirectory = currentDirectory + "analysis\" ' 替换为实际的运行文件夹路径
                SI_build_fq.CreateNoWindow = False
                SI_build_fq.Arguments = "-i1 " + """" + form_main.DataGridView2.Rows(i - 1).Cells(2).Value.ToString + """"
                SI_build_fq.Arguments += " -i2 " + """" + form_main.DataGridView2.Rows(i - 1).Cells(3).Value.ToString + """"
                SI_build_fq.Arguments += " -o " + """" + para(2) + """"
                SI_build_fq.Arguments += " -o1 " + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + "_" + folder_name + ".1"
                SI_build_fq.Arguments += " -o2 " + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + "_" + folder_name + ".2"
                SI_build_fq.Arguments += " -skip " + para(0).ToString
                SI_build_fq.Arguments += " -m_reads " + para(1).ToString
                Dim process_build_fq As Process = Process.Start(SI_build_fq)
                process_build_fq.WaitForExit()
                process_build_fq.Close()
            End If
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text <> "" Then
            Process.Start("explorer.exe", TextBox1.Text)
        End If
    End Sub

    Private Sub 过滤ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 过滤ToolStripMenuItem.Click

    End Sub


    Public Sub do_align()
        Directory.CreateDirectory(TextBox1.Text + "\aligned\")
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        Dim count As Integer = 0
        Dim parallelOptions As New ParallelOptions()
        parallelOptions.MaxDegreeOfParallelism = max_thread
        Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                 count += 1
                                                                 PB_value = count / refsView.Count * 100
                                                                 If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                     'If File.Exists(TextBox1.Text + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                                                                     Dim in_path As String = ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                                                                     Dim out_path As String = TextBox1.Text + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"

                                                                     Dim SI_mafft As New ProcessStartInfo()
                                                                     SI_mafft.FileName = currentDirectory + "analysis\mafft-win\mafft.bat" ' 替换为实际的命令行程序路径
                                                                     SI_mafft.WorkingDirectory = currentDirectory + "analysis\mafft-win\" ' 替换为实际的运行文件夹路径
                                                                     SI_mafft.CreateNoWindow = True
                                                                     SI_mafft.UseShellExecute = False ' 必须为False以重定向输出和错误
                                                                     SI_mafft.RedirectStandardOutput = True
                                                                     SI_mafft.RedirectStandardError = True
                                                                     SI_mafft.Arguments = "--auto --inputorder " + """" + in_path + """" + ">" + """" + out_path + """"
                                                                     Dim process_mafft As Process = New Process()
                                                                     process_mafft.StartInfo = SI_mafft
                                                                     AddHandler process_mafft.OutputDataReceived, Sub(sender, e)
                                                                                                                      If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                          '处理输出数据
                                                                                                                          Console.WriteLine(e.Data)
                                                                                                                      End If
                                                                                                                  End Sub
                                                                     AddHandler process_mafft.ErrorDataReceived, Sub(sender, e)
                                                                                                                     If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                         '处理错误数据
                                                                                                                         Console.WriteLine("ERROR: " + e.Data)
                                                                                                                     End If
                                                                                                                 End Sub
                                                                     process_mafft.Start()
                                                                     process_mafft.BeginOutputReadLine() '开始异步读取输出
                                                                     process_mafft.BeginErrorReadLine() '开始异步读取错误
                                                                     process_mafft.WaitForExit()
                                                                     process_mafft.Close()
                                                                 End If
                                                             End Sub)
        PB_value = -1
        'MsgBox("Analysis completed!")
        Dim result As DialogResult = MessageBox.Show("Analysis has been completed. Would you like to view the results file?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        ' 根据用户的选择执行相应的操作
        If result = DialogResult.Yes Then
            Process.Start("explorer.exe", """" + out_dir.Replace("/", "\") + "\aligned" + """")
        End If
    End Sub

    Private Sub 清空数据ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 清空数据ToolStripMenuItem.Click
        mydata_Dataset.Tables("Data Table").Clear()
        DataGridView2.DataSource = Nothing
    End Sub

    Private Sub 迭代ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 迭代ToolStripMenuItem1.Click
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            form_config_basic.GroupBox2.Enabled = True
            form_config_basic.GroupBox3.Enabled = True
            form_config_basic.GroupBox4.Enabled = True
            MenuClicked = "iteration"
            form_config_basic.Show()


        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Private Sub menu_iteration()
        Dim refs_count As Integer = 0
        Dim seqs_count As Integer = 0
        reads_length = 0
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        out_dir = (TextBox1.Text + "/iteration").Replace("\", "/")
        DeleteDir(out_dir)
        My.Computer.FileSystem.CreateDirectory(out_dir)
        q1 = ""
        q2 = ""
        k1 = form_config_basic.NumericUpDown1.Value.ToString
        k2 = form_config_basic.NumericUpDown5.Value.ToString
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)

        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                If File.Exists(TextBox1.Text + "\contigs_all\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                    refs_count += 1
                    safe_copy(TextBox1.Text + "\contigs_all\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
                End If
            End If
        Next

        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                seqs_count += 1
                q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                Else
                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                End If
            End If
        Next
        If seqs_count >= 1 And refs_count >= 1 Then
            Dim th1 As New Thread(AddressOf do_filer_assemble)
            th1.Start()
        Else
            MsgBox("Please select at least one reference and one sequencing data!")
        End If
    End Sub
    Private Sub 重新拼接ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            Dim refs_count As Integer = 0
            Dim seqs_count As Integer = 0
            reads_length = 0
            ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
            out_dir = (TextBox1.Text + "/iteration").Replace("\", "/")
            DeleteDir(out_dir + "/results")
            q1 = ""
            q2 = ""
            k1 = form_config_basic.NumericUpDown1.Value.ToString
            k2 = form_config_basic.NumericUpDown5.Value.ToString
            DeleteDir(ref_dir)
            My.Computer.FileSystem.CreateDirectory(ref_dir)

            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    If File.Exists(TextBox1.Text + "\contigs_all\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                        refs_count += 1
                        safe_copy(TextBox1.Text + "\contigs_all\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
                    End If
                End If
            Next

            For i As Integer = 1 To seqsView.Count
                If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    seqs_count += 1
                    q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                    If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
                        q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                    Else
                        q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                    End If
                End If
            Next
            If seqs_count >= 1 And refs_count >= 1 Then
                Dim th1 As New Thread(AddressOf do_assemble)
                th1.Start()
            Else
                MsgBox("Please select at least one reference and one sequencing data!")
            End If
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub



    Private Sub 多次迭代ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If TextBox1.Text <> "" Then
            Dim my_input As String = InputBox("Please enter the number of iterations:", "Iterations", 1)
            Dim iterations_times As Integer
            If Not Integer.TryParse(my_input, iterations_times) Then
                Exit Sub
            End If
            timer_id = 4
            PB_value = 0
            Dim th1 As New Threading.Thread(AddressOf do_iteration)
            th1.Start(CInt(iterations_times))
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Public Sub do_mafft_align(ByVal in_path As String, ByVal out_path As String, Optional method As String = "--auto ")
        Dim SI_mafft As New ProcessStartInfo()
        SI_mafft.FileName = currentDirectory + "analysis\mafft-win\mafft.bat"
        SI_mafft.WorkingDirectory = currentDirectory + "analysis\mafft-win\"
        SI_mafft.CreateNoWindow = True
        SI_mafft.UseShellExecute = False
        SI_mafft.RedirectStandardOutput = True
        SI_mafft.RedirectStandardError = True
        SI_mafft.Arguments = method + "--inputorder " + """" + in_path + """" + ">" + """" + out_path + """"
        Dim process_mafft As Process = New Process()
        process_mafft.StartInfo = SI_mafft
        AddHandler process_mafft.OutputDataReceived, Sub(sender, e)
                                                         If Not String.IsNullOrEmpty(e.Data) Then
                                                             '处理输出数据
                                                             Console.WriteLine(e.Data)
                                                         End If
                                                     End Sub
        AddHandler process_mafft.ErrorDataReceived, Sub(sender, e)
                                                        If Not String.IsNullOrEmpty(e.Data) Then
                                                            '处理错误数据
                                                            Console.WriteLine("ERROR: " + e.Data)
                                                        End If
                                                    End Sub
        process_mafft.Start()
        process_mafft.BeginOutputReadLine()
        process_mafft.BeginErrorReadLine()
        process_mafft.WaitForExit()
        process_mafft.Close()
    End Sub


    Public Sub do_cut()
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
            End If
        Next
        If My.Computer.FileSystem.DirectoryExists(out_dir + "\results") Then
            If Directory.GetFileSystemEntries(out_dir + "\results").Length > 0 Then
                For i As Integer = 1 To refsView.Count
                    If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" And File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                        MergeFiles(ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                    End If
                Next
            End If
        End If
        timer_id = 4
        PB_value = 0
        Dim th1 As New Thread(AddressOf run_cut)
        th1.Start()

    End Sub

    Public Sub run_cut(ByVal pross_id As Integer)
        Directory.CreateDirectory(out_dir + "\aligned\")
        Directory.CreateDirectory(out_dir + "\trimed\")
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        Dim count As Integer = 0
        Dim parallelOptions As New ParallelOptions()
        parallelOptions.MaxDegreeOfParallelism = max_thread
        Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                 count += 1
                                                                 PB_value = count / refsView.Count * 100
                                                                 If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                     If File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                                                                         Dim in_path As String = ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                                                                         Dim out_path As String = out_dir + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"

                                                                         Dim SI_mafft As New ProcessStartInfo()
                                                                         SI_mafft.FileName = currentDirectory + "analysis\mafft-win\mafft.bat" ' 替换为实际的命令行程序路径
                                                                         SI_mafft.WorkingDirectory = currentDirectory + "analysis\mafft-win\" ' 替换为实际的运行文件夹路径
                                                                         SI_mafft.CreateNoWindow = True
                                                                         SI_mafft.UseShellExecute = False ' 必须为False以重定向输出和错误
                                                                         SI_mafft.RedirectStandardOutput = True
                                                                         SI_mafft.RedirectStandardError = True
                                                                         SI_mafft.Arguments = "--auto --inputorder " + """" + in_path + """" + ">" + """" + out_path + """"
                                                                         Dim process_mafft As Process = New Process()
                                                                         process_mafft.StartInfo = SI_mafft
                                                                         AddHandler process_mafft.OutputDataReceived, Sub(sender, e)
                                                                                                                          If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                              '处理输出数据
                                                                                                                              Console.WriteLine(e.Data)
                                                                                                                          End If
                                                                                                                      End Sub
                                                                         AddHandler process_mafft.ErrorDataReceived, Sub(sender, e)
                                                                                                                         If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                             '处理错误数据
                                                                                                                             Console.WriteLine("ERROR: " + e.Data)
                                                                                                                         End If
                                                                                                                     End Sub
                                                                         process_mafft.Start()
                                                                         process_mafft.BeginOutputReadLine() '开始异步读取输出
                                                                         process_mafft.BeginErrorReadLine() '开始异步读取错误
                                                                         process_mafft.WaitForExit()
                                                                         process_mafft.Close()


                                                                         Dim SI_trimed As New ProcessStartInfo()
                                                                         SI_trimed.FileName = currentDirectory + "analysis\trimal.exe" ' 替换为实际的命令行程序路径
                                                                         SI_trimed.WorkingDirectory = currentDirectory + "analysis\" ' 替换为实际的运行文件夹路径
                                                                         SI_trimed.CreateNoWindow = True
                                                                         SI_trimed.Arguments = "-in " + """" + out_dir + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                         SI_trimed.Arguments += " -out " + """" + out_dir + "\trimed\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                         SI_trimed.Arguments += " -automated1"
                                                                         Dim process_trimed As Process = Process.Start(SI_trimed)
                                                                         process_trimed.WaitForExit()
                                                                         process_trimed.Close()

                                                                     End If
                                                                 End If
                                                             End Sub)
        PB_value = -1
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub


    'Private Sub 过滤拼接切齐ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 过滤拼接切齐ToolStripMenuItem.Click
    '    If TextBox1.Text <> "" Then
    '        DataGridView1.EndEdit()
    '        If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 Then
    '            Dim result As DialogResult = MessageBox.Show("Clear the output directory?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
    '            If result = DialogResult.Yes Then
    '                DeleteDir(TextBox1.Text)
    '                My.Computer.FileSystem.CreateDirectory(TextBox1.Text)
    '            End If
    '        End If
    '        Dim refs_count As Integer = 0
    '        Dim seqs_count As Integer = 0
    '        reads_length = 0
    '        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
    '        out_dir = TextBox1.Text.Replace("\", "/")
    '        q1 = ""
    '        q2 = ""
    '        k1 = NumericUpDown1.Value.ToString
    '        k2 = NumericUpDown5.Value.ToString
    '        DeleteDir(ref_dir)
    '        My.Computer.FileSystem.CreateDirectory(ref_dir)


    '        For i As Integer = 1 To refsView.Count
    '            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
    '                refs_count += 1
    '                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
    '            End If
    '        Next
    '        For i As Integer = 1 To refsView.Count
    '            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
    '                If File.Exists(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
    '                    File.Delete(out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 1 To seqsView.Count
    '            If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
    '                seqs_count += 1
    '                q1 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
    '                If DataGridView2.Rows(i - 1).Cells(3).FormattedValue.ToString = "" Then
    '                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
    '                Else
    '                    q2 += " " + """" + DataGridView2.Rows(i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
    '                End If
    '            End If
    '        Next

    '        If seqs_count >= 1 And refs_count >= 1 Then
    '            Dim th1 As New Thread(AddressOf do_filer_assemble_trim)
    '            th1.Start()
    '        Else
    '            MsgBox("Please select at least one reference and one sequencing data!")
    '        End If
    '    Else
    '        MsgBox("Please select an output folder!")
    '    End If
    'End Sub
    Public Sub do_filer_assemble_trim(Optional no_window As Boolean = False)
        do_filter(False, no_window)
        do_filter(True, no_window)
        do_assemble(no_window)
        do_cut()
    End Sub

    Private Sub 刷新数据ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 刷新数据ToolStripMenuItem.Click
        out_dir = TextBox1.Text
        Dim filePath As String = TextBox1.Text + "\ref_reads_count_dict.txt"
        If File.Exists(filePath) Then
            ref_filter_result(filePath)
        End If
        filePath = TextBox1.Text + "\result_dict.txt"
        If File.Exists(filePath) Then
            ref_assemble_result(filePath)
        End If
    End Sub

    Private Sub 过滤拼接ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 过滤拼接ToolStripMenuItem.Click
        If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 And DebugToolStripMenuItem.Checked = False Then
            Dim result As DialogResult = MessageBox.Show("The result folder is not empty. Are you sure to proceed?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then
                Exit Sub
            End If
        End If
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            MenuClicked = "batch_auto_assemble"
            form_config_basic.GroupBox2.Enabled = True
            form_config_basic.GroupBox3.Enabled = True
            form_config_basic.GroupBox4.Enabled = True
            form_config_basic.NumericUpDown1.Value = 21
            form_config_basic.Show()

        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Private Sub menu_batch_auto_assemble()
        Dim refs_count As Integer = 0
        reads_length = 0
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        k1 = form_config_basic.NumericUpDown1.Value.ToString
        k2 = form_config_basic.NumericUpDown5.Value.ToString
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)

        timer_id = 4
        PB_value = 0
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                refs_count += 1
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
            End If
        Next
        If refs_count >= 1 Then
            Dim th1 As New Thread(AddressOf batch_filer_assemble)
            th1.Start()
        Else
            MsgBox("Please select at least one reference and one sequencing data!")
        End If
    End Sub
    Public Sub batch_filer_assemble()
        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")
                My.Computer.FileSystem.CreateDirectory(out_dir)

                q1 = " " + """" + DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString.Replace("\", "/") + """"
                q2 = " " + """" + DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString.Replace("\", "/") + """"
                do_filer_assemble(True)
                If DebugToolStripMenuItem.Checked = False Then
                    'If Directory.Exists(out_dir + "\filtered") Then
                    '    Directory.Delete(out_dir + "\filtered", True)
                    'End If
                    If File.Exists(out_dir + "\kmer_dict_k" + k1.ToString + ".dict") Then
                        File.Delete(out_dir + "\kmer_dict_k" + k1.ToString + ".dict")
                    End If
                    If Directory.Exists(out_dir + "\large_files") Then
                        Directory.Delete(out_dir + "\large_files", True)
                    End If
                End If
            End If
        Next
        PB_value = -1
        MsgBox("Analysis completed!")
    End Sub
    Public Function make_out_name(ByVal fq_1 As String, ByVal fq_2 As String)
        Dim out_name As String = ""
        For i As Integer = 1 To fq_1.Length
            If fq_1.Substring(i - 1, 1) = fq_2.Substring(i - 1, 1) Then
                out_name += fq_1.Substring(i - 1, 1)
            End If
        Next
        out_name = out_name.Replace(".fasta", "").Replace(".fastq", "").Replace(".fq", "").Replace(".gz", "").Replace(".fas", "").Replace("  ", " ").Replace(".", "_").Replace(" ", "_").Replace("-", "_")
        Do While out_name.EndsWith("_")
            out_name = out_name.Substring(0, out_name.Length - 1)
        Loop
        Return out_name
    End Function
    'Private Sub 过滤拼接切齐ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 过滤拼接切齐ToolStripMenuItem1.Click
    '    If TextBox1.Text <> "" Then
    '        DataGridView1.EndEdit()
    '        Dim refs_count As Integer = 0
    '        timer_id = 4
    '        PB_value = 0
    '        For i As Integer = 1 To refsView.Count
    '            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
    '                refs_count += 1
    '            End If
    '        Next
    '        If refs_count >= 1 Then
    '            timer_id = 4
    '            PB_value = 0
    '            Dim th1 As New Thread(AddressOf batch_trim)
    '            th1.Start()
    '        Else
    '            MsgBox("Please select at least one reference and one sequencing data!")
    '        End If
    '    Else
    '        MsgBox("Please select an output folder!")
    '    End If
    'End Sub
    Public Sub pre_align()
        Dim tmp_aligns As String = (currentDirectory + "temp\temp_aligns\").Replace("\", "/")
        DeleteDir(tmp_aligns)
        My.Computer.FileSystem.CreateDirectory(tmp_aligns)
        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
        DeleteDir(ref_dir)
        My.Computer.FileSystem.CreateDirectory(ref_dir)
        '拷贝到临时文件夹
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
            End If
        Next

        Dim count As Integer = 0
        Dim parallelOptions As New ParallelOptions()
        parallelOptions.MaxDegreeOfParallelism = max_thread
        '生成排序的临时文件
        Parallel.For(0, refsView.Count, parallelOptions, Sub(i)
                                                             count += 1
                                                             PB_value = count / refsView.Count * 100
                                                             If DataGridView1.Rows(i).Cells(0).FormattedValue.ToString = "True" Then

                                                                 Dim in_path As String = ref_dir + DataGridView1.Rows(i).Cells(2).Value.ToString + ".fasta"
                                                                 Dim out_path As String = tmp_aligns + DataGridView1.Rows(i).Cells(2).Value.ToString + ".fasta"

                                                                 Dim SI_mafft As New ProcessStartInfo()
                                                                 SI_mafft.FileName = currentDirectory + "analysis\mafft-win\mafft.bat"
                                                                 SI_mafft.WorkingDirectory = currentDirectory + "analysis\mafft-win\"
                                                                 SI_mafft.CreateNoWindow = True
                                                                 SI_mafft.UseShellExecute = False
                                                                 SI_mafft.RedirectStandardOutput = True
                                                                 SI_mafft.RedirectStandardError = True
                                                                 SI_mafft.Arguments = "--auto --inputorder " + """" + in_path + """" + ">" + """" + out_path + """"

                                                                 Dim process_mafft As Process = New Process()
                                                                 process_mafft.StartInfo = SI_mafft
                                                                 AddHandler process_mafft.OutputDataReceived, Sub(sender, e)
                                                                                                                  If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                      '处理输出数据
                                                                                                                      Console.WriteLine(e.Data)
                                                                                                                  End If
                                                                                                              End Sub
                                                                 AddHandler process_mafft.ErrorDataReceived, Sub(sender, e)
                                                                                                                 If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                     '处理错误数据
                                                                                                                     Console.WriteLine("ERROR: " + e.Data)
                                                                                                                 End If
                                                                                                             End Sub
                                                                 process_mafft.Start()
                                                                 process_mafft.BeginOutputReadLine()
                                                                 process_mafft.BeginErrorReadLine()
                                                                 process_mafft.WaitForExit()
                                                                 process_mafft.Close()
                                                             End If
                                                         End Sub)
    End Sub
    Public Sub batch_trim()
        pre_align()
        Dim tmp_aligns As String = (currentDirectory + "temp\temp_aligns\").Replace("\", "/")
        Dim count As Integer = 0
        PB_value = 0
        Dim parallelOptions As New ParallelOptions()
        parallelOptions.MaxDegreeOfParallelism = max_thread
        Parallel.For(1, seqsView.Count + 1, parallelOptions, Sub(batch_i)
                                                                 count += 1
                                                                 PB_value = count / seqsView.Count * 100
                                                                 If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                     Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                                                                     Dim temp_out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")
                                                                     My.Computer.FileSystem.CreateDirectory(temp_out_dir + "\aligned\")
                                                                     For i As Integer = 1 To refsView.Count
                                                                         If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                             Dim add_path As String = temp_out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                                                                             If File.Exists(add_path) Then
                                                                                 Dim in_path As String = tmp_aligns + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                                                                                 Dim out_path As String = temp_out_dir + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"

                                                                                 Dim SI_mafft As New ProcessStartInfo()
                                                                                 SI_mafft.FileName = currentDirectory + "analysis\mafft-win\mafft.bat" ' 替换为实际的命令行程序路径
                                                                                 SI_mafft.WorkingDirectory = currentDirectory + "analysis\mafft-win\" ' 替换为实际的运行文件夹路径
                                                                                 SI_mafft.CreateNoWindow = True
                                                                                 SI_mafft.UseShellExecute = False ' 必须为False以重定向输出和错误
                                                                                 SI_mafft.RedirectStandardOutput = True
                                                                                 SI_mafft.RedirectStandardError = True
                                                                                 SI_mafft.Arguments = "--add " + """" + add_path + """" + " " + """" + in_path + """" + ">" + """" + out_path + """"
                                                                                 Dim process_mafft As Process = New Process()
                                                                                 process_mafft.StartInfo = SI_mafft
                                                                                 AddHandler process_mafft.OutputDataReceived, Sub(sender, e)
                                                                                                                                  If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                                      '处理输出数据
                                                                                                                                      Console.WriteLine(e.Data)
                                                                                                                                  End If
                                                                                                                              End Sub
                                                                                 AddHandler process_mafft.ErrorDataReceived, Sub(sender, e)
                                                                                                                                 If Not String.IsNullOrEmpty(e.Data) Then
                                                                                                                                     '处理错误数据
                                                                                                                                     Console.WriteLine("ERROR: " + e.Data)
                                                                                                                                 End If
                                                                                                                             End Sub
                                                                                 process_mafft.Start()
                                                                                 process_mafft.BeginOutputReadLine() '开始异步读取输出
                                                                                 process_mafft.BeginErrorReadLine() '开始异步读取错误
                                                                                 process_mafft.WaitForExit()
                                                                                 process_mafft.Close()


                                                                                 Dim SI_trimed As New ProcessStartInfo()
                                                                                 SI_trimed.FileName = currentDirectory + "analysis\build_trimed.exe" ' 替换为实际的命令行程序路径
                                                                                 SI_trimed.WorkingDirectory = currentDirectory + "analysis\" ' 替换为实际的运行文件夹路径
                                                                                 SI_trimed.CreateNoWindow = True
                                                                                 SI_trimed.Arguments = "-o " + """" + temp_out_dir + """"
                                                                                 SI_trimed.Arguments += " -i " + """" + temp_out_dir + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                                 Dim process_trimed As Process = Process.Start(SI_trimed)
                                                                                 process_trimed.WaitForExit()
                                                                                 process_trimed.Close()
                                                                             End If
                                                                         End If
                                                                     Next
                                                                 End If
                                                             End Sub)
        PB_value = -1
        MsgBox("Analysis completed!")
    End Sub

    'Private Sub 切齐ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 切齐ToolStripMenuItem.Click
    '    If TextBox1.Text <> "" Then
    '        Dim refs_count As Integer = 0
    '        ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
    '        out_dir = TextBox1.Text.Replace("\", "/")
    '        DeleteDir(ref_dir)
    '        My.Computer.FileSystem.CreateDirectory(ref_dir)

    '        For i As Integer = 1 To refsView.Count
    '            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
    '                refs_count += 1
    '            End If
    '        Next
    '        If refs_count >= 1 Then
    '            do_cut()
    '        Else
    '            MsgBox("Please select at least one reference!")
    '        End If
    '    Else
    '        MsgBox("Please select an output folder!")
    '    End If
    'End Sub

    Private Sub 合并结果ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 合并结果ToolStripMenuItem.Click
        timer_id = 4
        PB_value = 0
        Dim th1 As New Thread(AddressOf do_combine)
        th1.Start(False)
    End Sub

    Public Sub do_combine(ByVal do_align As Boolean)

        Dim combine_res_dir As String = TextBox1.Text + "\combined_results\"
        Dim combine_trimed_dir As String = TextBox1.Text + "\combined_trimed\"
        My.Computer.FileSystem.CreateDirectory(combine_res_dir)

        If do_align Then
            My.Computer.FileSystem.CreateDirectory(combine_res_dir + "\aligned\")
            My.Computer.FileSystem.CreateDirectory(combine_trimed_dir)

        End If

        Dim count As Integer = 0
        Dim parallelOptions As New ParallelOptions()
        parallelOptions.MaxDegreeOfParallelism = max_thread
        Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                 count += 1
                                                                 PB_value = count / refsView.Count * 100
                                                                 If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                     Dim sw_res As New StreamWriter(combine_res_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", False, utf8WithoutBom)

                                                                     For batch_i As Integer = 1 To seqsView.Count
                                                                         If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                             Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                                                                             folder_name = folder_name.Replace("-", "_").Replace(":", "_")

                                                                             Dim temp_out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")
                                                                             Dim result_path As String = temp_out_dir + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                                                                             Dim trimed_path As String = temp_out_dir + "\trimed\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                                                                             Dim sr_line As String = ""
                                                                             If File.Exists(result_path) Then
                                                                                 Dim sr_res As New StreamReader(result_path)
                                                                                 sr_line = sr_res.ReadLine
                                                                                 sr_line = sr_res.ReadLine
                                                                                 If sr_line IsNot Nothing Then
                                                                                     If sr_line <> "" Then
                                                                                         sw_res.WriteLine(">" + batch_i.ToString + "_" + folder_name + "-" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString)
                                                                                         sw_res.WriteLine(sr_line)
                                                                                     End If

                                                                                 End If
                                                                                 sr_res.Close()
                                                                             End If

                                                                         End If
                                                                     Next
                                                                     sw_res.Close()
                                                                     If do_align Then
                                                                         do_mafft_align(combine_res_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", combine_res_dir + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")

                                                                         Dim SI_trimed As New ProcessStartInfo()
                                                                         SI_trimed.FileName = currentDirectory + "analysis\trimal.exe" ' 替换为实际的命令行程序路径
                                                                         SI_trimed.WorkingDirectory = currentDirectory + "analysis\" ' 替换为实际的运行文件夹路径
                                                                         SI_trimed.CreateNoWindow = True
                                                                         SI_trimed.Arguments = "-in " + """" + combine_res_dir + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                         SI_trimed.Arguments += " -out " + """" + combine_trimed_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                         SI_trimed.Arguments += " -automated1"
                                                                         Dim process_trimed As Process = Process.Start(SI_trimed)
                                                                         process_trimed.WaitForExit()
                                                                         process_trimed.Close()
                                                                     End If
                                                                 End If

                                                             End Sub)
        PB_value = -1
        MsgBox("Analysis completed!")
    End Sub


    Private Sub NumericUpDown10_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown10.ValueChanged
        max_thread = NumericUpDown10.Value
    End Sub


    Private Sub 合并比对ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 合并比对ToolStripMenuItem.Click
        timer_id = 4
        PB_value = 0
        Dim th1 As New Thread(AddressOf do_combine)
        th1.Start(True)
    End Sub

    Private Sub NumericUpDown10_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDown10.TextChanged
        max_thread = NumericUpDown10.Value
    End Sub

    Private Sub DebugToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DebugToolStripMenuItem.Click
        DebugToolStripMenuItem.Checked = DebugToolStripMenuItem.Checked Xor True

    End Sub



    Private Sub 叶绿体基因组ToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub 手动提取ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            'If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 Then
            '    Dim result As DialogResult = MessageBox.Show("Clear the output directory?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            '    If result = DialogResult.Yes Then
            '        DeleteDir(TextBox1.Text)
            '        My.Computer.FileSystem.CreateDirectory(TextBox1.Text)
            '    End If
            'End If

            Dim refs_count As Integer = 0
            Dim seqs_count As Integer = 0

            'DeleteDir(currentDirectory + "temp\seeds")
            'My.Computer.FileSystem.CreateDirectory(currentDirectory + "temp\seeds")

            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    refs_count += 1
                    If refs_count = 1 Then
                        safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", currentDirectory + "temp\seed.fasta", True)
                    Else
                        MsgBox("In this mode, you can only choose one reference sequence as a seed file.")
                        Exit Sub
                    End If
                End If
            Next
            Dim sw As New StreamWriter(currentDirectory + "temp\batch_file.txt")
            For i As Integer = 1 To seqsView.Count
                If DataGridView2.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    seqs_count += 1
                    sw.WriteLine("Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString)
                    sw.WriteLine("seed.fasta")
                    sw.WriteLine("Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".1.fq")
                    sw.WriteLine("Project" + DataGridView2.Rows(i - 1).Cells(1).FormattedValue.ToString + ".2.fq")
                End If
            Next
            sw.Close()
            If seqs_count >= 1 And refs_count >= 1 Then
                form_config_plasty.ComboBox1.Enabled = True
                form_config_plasty.TextBox3.Text = ""
                form_config_plasty.TextBox2.Text = ""
                cpg_assemble_mode = -1
                form_config_plasty.Show()
            Else
                MsgBox("Please select one reference as seed and at least one sequencing data!")
            End If
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub

    Private Sub ToolStripMenuItem3_Click_1(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        If check_batch_folder() = False Then
            MsgBox("To assemble the plant mitochondrial genome, it is necessary to already possess the chloroplast genome first.")
            Exit Sub
        End If
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        cpg_down_mode = 5
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = False
        form_config_cp.Show()
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 And DebugToolStripMenuItem.Checked = False Then
            Dim result As DialogResult = MessageBox.Show("The result folder is not empty. Are you sure to proceed?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then
                Exit Sub
            End If
        End If
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        cpg_down_mode = 4
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = False
        form_config_cp.Show()
    End Sub

    Private Sub 全选ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 全选ToolStripMenuItem1.Click
        For i As Integer = 1 To refsView.Count
            DataGridView1.Rows(i - 1).Cells(0).Value = True
        Next
        DataGridView1.RefreshEdit()
    End Sub

    Private Sub 清空ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 清空ToolStripMenuItem1.Click
        For i As Integer = 1 To refsView.Count
            DataGridView1.Rows(i - 1).Cells(0).Value = False
        Next
        DataGridView1.RefreshEdit()
    End Sub

    Private Sub 反选ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 反选ToolStripMenuItem.Click
        Dim sel_count As Integer = 0
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(0).Value = True Then
                DataGridView1.Rows(i - 1).Cells(0).Value = False
            Else
                DataGridView1.Rows(i - 1).Cells(0).Value = True
                sel_count += 1
            End If
        Next
        DataGridView1.RefreshEdit()
    End Sub

    Private Sub 失败的项ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 失败的项ToolStripMenuItem.Click
        Dim sel_count As Integer = 0
        For i As Integer = 1 To refsView.Count
            If DataGridView1.Rows(i - 1).Cells(6).Value = "failed" Then
                DataGridView1.Rows(i - 1).Cells(0).Value = True
                sel_count += 1

            End If
        Next
        MsgBox(sel_count.ToString + " were selected!")
        DataGridView1.RefreshEdit()
    End Sub

    Private Sub 序列比对ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 序列比对ToolStripMenuItem1.Click
        If TextBox1.Text <> "" Then
            Dim refs_count As Integer = 0
            ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
            out_dir = TextBox1.Text
            DeleteDir(ref_dir)
            My.Computer.FileSystem.CreateDirectory(ref_dir)

            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    refs_count += 1
                    safe_copy(currentDirectory + "temp\org_seq\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", True)
                End If
            Next
            If refs_count >= 1 Then
                If My.Computer.FileSystem.DirectoryExists(TextBox1.Text + "\results") Then
                    If Directory.GetFileSystemEntries(TextBox1.Text + "\results").Length > 0 Then
                        For i As Integer = 1 To refsView.Count
                            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" And File.Exists(TextBox1.Text + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                                MergeFiles(ref_dir + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta", TextBox1.Text + "\results\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                            End If
                        Next
                    End If
                End If
                timer_id = 4
                PB_value = 0
                Dim th1 As New Thread(AddressOf do_align)
                th1.Start()
            Else
                MsgBox("Please select at least one reference!")
            End If
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub

    Private Sub 切齐比对ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 切齐比对ToolStripMenuItem1.Click
        If TextBox1.Text <> "" Then
            Dim refs_count As Integer = 0
            ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
            out_dir = TextBox1.Text
            DeleteDir(ref_dir)
            My.Computer.FileSystem.CreateDirectory(ref_dir)

            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    refs_count += 1
                End If
            Next
            If refs_count >= 1 Then
                timer_id = 4
                PB_value = 0
                Dim th1 As New Thread(AddressOf do_trim)
                Dim result As DialogResult = MessageBox.Show("Trim the entire alignment? If you only want to trim terminal, please select 'No'.?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                ' 根据用户的选择执行相应的操作
                If result = DialogResult.Yes Then
                    th1.Start(False)
                Else
                    th1.Start(True)
                End If

            Else
                MsgBox("Please select at least one reference!")
            End If
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub

    Public Sub do_trim(ByVal terminalonly As Boolean)
        Directory.CreateDirectory(TextBox1.Text + "\trimed\")
        Dim count As Integer = 0
        Dim parallelOptions As New ParallelOptions()
        parallelOptions.MaxDegreeOfParallelism = max_thread
        Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                 count += 1
                                                                 PB_value = count / refsView.Count * 100
                                                                 If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                     If File.Exists(TextBox1.Text + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta") Then
                                                                         Dim SI_trimed As New ProcessStartInfo()
                                                                         SI_trimed.FileName = currentDirectory + "analysis\trimal.exe" ' 替换为实际的命令行程序路径
                                                                         SI_trimed.WorkingDirectory = currentDirectory + "analysis\" ' 替换为实际的运行文件夹路径
                                                                         SI_trimed.CreateNoWindow = True
                                                                         SI_trimed.Arguments = "-in " + """" + TextBox1.Text + "\aligned\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                         SI_trimed.Arguments += " -out " + """" + TextBox1.Text + "\trimed\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta" + """"
                                                                         SI_trimed.Arguments += " -automated1"
                                                                         If terminalonly Then
                                                                             SI_trimed.Arguments += " -terminalonly"
                                                                         End If
                                                                         Dim process_trimed As Process = Process.Start(SI_trimed)
                                                                         process_trimed.WaitForExit()
                                                                         process_trimed.Close()
                                                                     End If
                                                                 End If
                                                             End Sub)
        PB_value = -1
        'MsgBox("Analysis completed!")
        Dim result As DialogResult = MessageBox.Show("Analysis has been completed. Would you like to view the results file?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        ' 根据用户的选择执行相应的操作
        If result = DialogResult.Yes Then
            Process.Start("explorer.exe", """" + out_dir.Replace("/", "\") + "\trimed" + """")
        End If
    End Sub

    Private Sub 重构ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 重构ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            out_dir = TextBox1.Text
            timer_id = 4
            PB_value = 0
            Dim my_input As String = InputBox("Input threshold value: 0.01-0.99. Increasing the threshold value results in a greater number of ambiguities.", "Input", 0.75)
            Dim con_level As Single
            If Not Single.TryParse(my_input, con_level) Then
                Exit Sub
            End If
            Dim th1 As New Thread(AddressOf do_consensus)
            th1.Start(con_level)
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Public Sub do_consensus(ByVal con_level As Single)
        If My.Computer.FileSystem.DirectoryExists(Path.Combine(out_dir, "results")) Then
            Dim consensus_dir As String = Path.Combine(out_dir, "consensus")
            My.Computer.FileSystem.CreateDirectory(consensus_dir)
            Dim count As Integer = 0
            Dim parallelOptions As New ParallelOptions()
            parallelOptions.MaxDegreeOfParallelism = max_thread
            Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                     'For i As Integer = 1 To refsView.Count
                                                                     count += 1
                                                                     PB_value = count / refsView.Count * 100
                                                                     If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                         Dim in_path_fasta As String = Path.Combine(out_dir, "results", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                                                                         Dim in_path_fq As String = Path.Combine(out_dir, "filtered", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fq")
                                                                         Dim out_path As String = consensus_dir
                                                                         Dim out_path_file As String = Path.Combine(consensus_dir, DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".sam")
                                                                         If File.Exists(in_path_fasta) AndAlso File.Exists(in_path_fq) Then
                                                                             Dim SI_minimap2 As New ProcessStartInfo()
                                                                             SI_minimap2.FileName = Path.Combine(currentDirectory, "analysis", "minimap2.exe")
                                                                             SI_minimap2.WorkingDirectory = Path.Combine(currentDirectory, "analysis")
                                                                             SI_minimap2.CreateNoWindow = True
                                                                             SI_minimap2.UseShellExecute = False
                                                                             SI_minimap2.RedirectStandardOutput = True
                                                                             SI_minimap2.Arguments = "-ax sr " + """" + in_path_fasta + """" + " " + """" + in_path_fq + """"

                                                                             Using process_minimap2 As Process = Process.Start(SI_minimap2)
                                                                                 Using reader As StreamReader = process_minimap2.StandardOutput
                                                                                     Dim my_result As String = reader.ReadToEnd()
                                                                                     File.WriteAllText(out_path_file, my_result)
                                                                                 End Using
                                                                                 process_minimap2.WaitForExit()
                                                                             End Using
                                                                             If File.Exists(out_path_file) Then
                                                                                 Dim SI_consensus As New ProcessStartInfo()
                                                                                 SI_consensus.FileName = Path.Combine(currentDirectory, "analysis", "build_consensus.exe")
                                                                                 SI_consensus.WorkingDirectory = out_path
                                                                                 SI_consensus.CreateNoWindow = True
                                                                                 SI_consensus.Arguments = "-i " + """" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".sam" + """" + " -c " + con_level.ToString + " -o " + """" + out_path + """"
                                                                                 Dim process_consensus As Process = Process.Start(SI_consensus)
                                                                                 process_consensus.WaitForExit()
                                                                                 process_consensus.Close()
                                                                             End If


                                                                         End If
                                                                     End If
                                                                 End Sub)
            PB_value = -1
            Dim result As DialogResult = MessageBox.Show("Analysis has been completed. Would you like to view the results file?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            ' 根据用户的选择执行相应的操作
            If result = DialogResult.Yes Then
                Process.Start("explorer.exe", """" + consensus_dir + """")
            End If
        End If
    End Sub

    Private Sub 重构序列ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 重构序列ToolStripMenuItem.Click
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        timer_id = 4
        PB_value = 0
        Dim my_input As String = InputBox("Input threshold value: 0.01-0.99. Increasing the threshold value results in a greater number of ambiguities.", "Input", 0.75)
        Dim con_level As Single
        If Not Single.TryParse(my_input, con_level) Then
            Exit Sub
        End If
        Dim th1 As New Thread(AddressOf batch_consensus)
        th1.Start(con_level)
    End Sub

    Public Sub batch_consensus(ByVal con_level As Single)
        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")

                Dim consensus_dir As String = Path.Combine(out_dir, "consensus")
                My.Computer.FileSystem.CreateDirectory(consensus_dir)
                Dim count As Integer = 0
                Dim parallelOptions As New ParallelOptions()
                parallelOptions.MaxDegreeOfParallelism = max_thread
                Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                         If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                             Dim in_path_fasta As String = Path.Combine(out_dir, "results", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                                                                             Dim in_path_fq As String = Path.Combine(out_dir, "filtered", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fq")
                                                                             Dim out_path As String = consensus_dir
                                                                             Dim out_path_file As String = Path.Combine(consensus_dir, DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".sam")

                                                                             If File.Exists(in_path_fasta) AndAlso File.Exists(in_path_fq) Then
                                                                                 Dim SI_minimap2 As New ProcessStartInfo()
                                                                                 SI_minimap2.FileName = Path.Combine(currentDirectory, "analysis", "minimap2.exe")
                                                                                 SI_minimap2.WorkingDirectory = Path.Combine(currentDirectory, "analysis")
                                                                                 SI_minimap2.CreateNoWindow = True
                                                                                 SI_minimap2.UseShellExecute = False
                                                                                 SI_minimap2.RedirectStandardOutput = True
                                                                                 SI_minimap2.Arguments = "-ax sr " + """" + in_path_fasta + """" + " " + """" + in_path_fq + """"

                                                                                 Using process_minimap2 As Process = Process.Start(SI_minimap2)
                                                                                     Using reader As StreamReader = process_minimap2.StandardOutput
                                                                                         Dim my_result As String = reader.ReadToEnd()
                                                                                         File.WriteAllText(out_path_file, my_result)
                                                                                     End Using
                                                                                     process_minimap2.WaitForExit()
                                                                                 End Using
                                                                                 If File.Exists(out_path_file) Then
                                                                                     Dim SI_consensus As New ProcessStartInfo()
                                                                                     SI_consensus.FileName = Path.Combine(currentDirectory, "analysis", "build_consensus.exe")
                                                                                     SI_consensus.WorkingDirectory = out_path
                                                                                     SI_consensus.CreateNoWindow = True
                                                                                     SI_consensus.Arguments = "-i " + """" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".sam" + """" + " -c " + con_level.ToString + " -o " + """" + out_path + """"
                                                                                     Dim process_consensus As Process = Process.Start(SI_consensus)
                                                                                     process_consensus.WaitForExit()
                                                                                     process_consensus.Close()
                                                                                     File.Delete(out_path_file)
                                                                                 End If
                                                                             End If
                                                                         End If
                                                                     End Sub)

            End If
        Next


        Dim supercontigs_dir As String = TextBox1.Text + "\supercontigs\"
        My.Computer.FileSystem.CreateDirectory(supercontigs_dir)
        Dim sw_namelist As New StreamWriter(Path.Combine(TextBox1.Text, "namelist.txt"), False, utf8WithoutBom)
        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                Dim temp_out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")
                folder_name = folder_name.Replace("-", "_").Replace(":", "_")
                Dim sw_res As New StreamWriter(Path.Combine(supercontigs_dir, folder_name + ".degenerated.fasta"), False, utf8WithoutBom)
                sw_namelist.WriteLine(folder_name)
                For i As Integer = 1 To refsView.Count
                    If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                        Dim consensus_path As String = temp_out_dir + "\consensus\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                        Dim sr_line As String = ""
                        If File.Exists(consensus_path) Then
                            Dim sr_res As New StreamReader(consensus_path)
                            sr_line = sr_res.ReadLine
                            sr_line = sr_res.ReadLine
                            If sr_line IsNot Nothing Then
                                If sr_line <> "" Then
                                    sw_res.WriteLine(">" + batch_i.ToString + " " + folder_name + "-" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString)
                                    sw_res.WriteLine(sr_line)
                                End If
                            End If
                            sr_res.Close()
                        End If
                    End If
                Next
                sw_res.Close()
            End If
        Next
        sw_namelist.Close()

        Dim combine_consensus_dir As String = TextBox1.Text + "\consensus\"
        My.Computer.FileSystem.CreateDirectory(combine_consensus_dir)
        For i As Integer = 1 To refsView.Count
            PB_value = i / refsView.Count * 100
            Dim sw_res As New StreamWriter(Path.Combine(combine_consensus_dir, DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"), False, utf8WithoutBom)
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                For batch_i As Integer = 1 To seqsView.Count
                    If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                        Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                        Dim temp_out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")
                        folder_name = folder_name.Replace("-", "_").Replace(":", "_")
                        Dim consensus_path As String = temp_out_dir + "\consensus\" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"
                        Dim sr_line As String = ""
                        If File.Exists(consensus_path) Then
                            Dim sr_res As New StreamReader(consensus_path)
                            sr_line = sr_res.ReadLine
                            sr_line = sr_res.ReadLine
                            If sr_line IsNot Nothing Then
                                If sr_line <> "" Then
                                    sw_res.WriteLine(">" + folder_name + "-" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString)
                                    sw_res.WriteLine(sr_line)
                                End If
                            End If
                            sr_res.Close()
                        End If
                    End If
                Next
            End If
            sw_res.Close()
        Next

        Dim sw_tg As New StreamWriter(Path.Combine(TextBox1.Text, "TargetSequences.fasta"), False, utf8WithoutBom)
        For i As Integer = 1 To refsView.Count
            Dim ref_file As String = Path.Combine(currentDirectory, "temp", "org_seq", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
            Using sr As New StreamReader(ref_file)
                While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()
                    If line.StartsWith(">") Then
                        line = line.Replace(DataGridView1.Rows(i - 1).Cells(2).Value.ToString, "").Replace("-", "_").Replace(" ", "_")
                        Do While line.EndsWith("_")
                            line = line.Substring(0, line.Length - 1)
                        Loop
                        line = line + "-" + DataGridView1.Rows(i - 1).Cells(2).Value.ToString
                    End If
                    sw_tg.WriteLine(line)
                End While
            End Using
        Next
        sw_tg.Close()
        PB_value = -1
        MsgBox("Analysis completed!")
    End Sub

    Private Sub PPDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PPDToolStripMenuItem.Click
        If File.Exists(Path.Combine(TextBox1.Text, "TargetSequences.fasta")) And File.Exists(Path.Combine(TextBox1.Text, "namelist.txt")) Then

            If File.Exists(Path.Combine(TextBox1.Text, "outgroup.txt")) Then
                Dim result As DialogResult = MessageBox.Show("Need to choose a different outgroup? If you want to start the analysis right away, click 'No'!", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Dim my_input As String = InputBox("Input ID of Outgroup", "ID of Outgroup", seqsView.Count)
                    Dim og_id As Integer
                    If Not Integer.TryParse(my_input, og_id) Then
                        Exit Sub
                    End If

                    Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(og_id - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(og_id - 1).Cells(3).Value.ToString))
                    folder_name = folder_name.Replace("-", "_").Replace(":", "_")
                    Dim sw_og As New StreamWriter(Path.Combine(TextBox1.Text, "outgroup.txt"), False, utf8WithoutBom)
                    sw_og.WriteLine(folder_name)
                    sw_og.Close()
                End If
            Else
                Dim my_input As String = InputBox("Input ID of Outgroup", "ID of Outgroup", seqsView.Count)
                Dim og_id As Integer
                If Not Integer.TryParse(my_input, og_id) Then
                    Exit Sub
                End If
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(og_id - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(og_id - 1).Cells(3).Value.ToString))
                folder_name = folder_name.Replace("-", "_").Replace(":", "_")
                Dim sw_og As New StreamWriter(Path.Combine(TextBox1.Text, "outgroup.txt"), False, utf8WithoutBom)
                sw_og.WriteLine(folder_name)
                sw_og.Close()
            End If

            If My.Computer.FileSystem.DirectoryExists(Path.Combine(TextBox1.Text, "PPD")) Then
                '    If Directory.GetFileSystemEntries(Path.Combine(TextBox1.Text, "PPD")).Length > 0 Then
                '        Dim result As DialogResult = MessageBox.Show("Clear the PPD output directory? If you are optimizing for previous results, please select 'NO'!", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                '        If result = DialogResult.Yes Then
                DeleteDir(Path.Combine(TextBox1.Text, "PPD"))
                '        End If
                '    End If
            End If
            RichTextBox1.Text = "Cite PPD:" + vbCrLf + " Zhou, W., Soghigian, J., Xiang, Q. 2022, A New Pipeline for Removing Paralogs in Target Enrichment Data. Systematic Biology, syab044. DOI: /10.1093/sysbio/syab044"
            Dim th1 As New Thread(AddressOf run_ppd)
            th1.Start(TextBox1.Text)

        End If

    End Sub
    Public Sub run_ppd(ByVal out_path As String)
        Dim SI_PPD As New ProcessStartInfo()
        SI_PPD.FileName = Path.Combine(currentDirectory, "analysis", "PPD.exe")
        SI_PPD.WorkingDirectory = Path.Combine(currentDirectory, "analysis")
        SI_PPD.CreateNoWindow = False
        SI_PPD.Arguments = "-ifa " + """" + Path.Combine(out_path, "supercontigs") + """"
        SI_PPD.Arguments += " -ina " + """" + Path.Combine(out_path, "namelist.txt") + """"
        SI_PPD.Arguments += " -iref " + """" + Path.Combine(out_path, "TargetSequences.fasta") + """"
        SI_PPD.Arguments += " -io " + """" + Path.Combine(out_path, "outgroup.txt") + """"
        SI_PPD.Arguments += " -o " + """" + Path.Combine(out_path, "PPD") + """"
        SI_PPD.Arguments += " -th " + max_thread.ToString

        Dim process_PPD As Process = Process.Start(SI_PPD)
        process_PPD.WaitForExit()
        process_PPD.Close()

    End Sub


    Private Sub 下载ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 下载ToolStripMenuItem.Click
        'form_config_ags.Show()

        cpg_down_mode = 6
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = False
        form_config_cp.Show()
    End Sub

    Private Sub 下载叶绿体基因组ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 下载叶绿体基因组ToolStripMenuItem.Click
        cpg_down_mode = 0
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = True
        form_config_cp.Show()
    End Sub

    Private Sub 下载植物线粒体ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 下载植物线粒体ToolStripMenuItem.Click
        cpg_down_mode = 2
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = True
        form_config_cp.Show()
    End Sub

    Private Sub 植物叶绿体基因组ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 植物叶绿体基因组ToolStripMenuItem.Click
        If check_data_count(1) = 0 Then
            MsgBox("Please select at least one sequencing data!")
            Exit Sub
        End If
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        cpg_down_mode = 1
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = False
        form_config_cp.Show()
    End Sub

    Private Sub 植物线粒体基因组ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 植物线粒体基因组ToolStripMenuItem.Click
        If check_data_count(1) = 0 Then
            MsgBox("Please select at least one sequencing data!")
            Exit Sub
        End If
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        Dim result As DialogResult = MessageBox.Show("To assemble the plant mitochondrial genome, it is necessary to already possess the chloroplast genome of the plant. Click 'Yes' to choose the chloroplast genome.", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        ' 根据用户的选择执行相应的操作
        If result = DialogResult.Yes Then
            Dim opendialog As New OpenFileDialog
            opendialog.Filter = "Fasta File(*.fasta)|*.fas;*.fasta;*.fa"
            opendialog.FileName = ""
            opendialog.DefaultExt = ".fas"
            opendialog.CheckFileExists = True
            opendialog.CheckPathExists = True
            Dim resultdialog As DialogResult = opendialog.ShowDialog()
            If resultdialog = DialogResult.OK Then
                File.Copy(opendialog.FileName, currentDirectory + "temp\cpg.fasta", True)
                cpg_down_mode = 3
                out_dir = TextBox1.Text.Replace("\", "/")
                form_config_cp.CheckBox1.Visible = False
                form_config_cp.Show()
            End If
        End If
    End Sub

    Private Sub 分离序列ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 分离序列ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            Dim refs_count As Integer = 0
            ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
            out_dir = TextBox1.Text
            DeleteDir(ref_dir)
            My.Computer.FileSystem.CreateDirectory(ref_dir)
            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    MergeFiles(Path.Combine(ref_dir, "barcode.fasta"), Path.Combine(currentDirectory, "temp", "org_seq", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"))
                End If
            Next
            Dim my_input As String = InputBox("Input accuracy level: 1-8", "Input", 4)
            Dim word_size As Integer
            If Not Integer.TryParse(my_input, word_size) Then
                MsgBox("Please enter the correct number!")
                Exit Sub
            End If
            If word_size >= 1 And word_size <= 8 Then
                Dim th1 As New Thread(AddressOf do_split_barcode)
                th1.Start(word_size)
            Else
                MsgBox("Please enter the correct number!")
            End If
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Public Sub do_split_barcode(ByVal word_size As Integer)

        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim SI_split_barcode As New ProcessStartInfo()
                SI_split_barcode.FileName = Path.Combine(currentDirectory, "analysis", "split_barcode.exe")
                SI_split_barcode.WorkingDirectory = Path.Combine(currentDirectory, "temp")
                SI_split_barcode.CreateNoWindow = True
                SI_split_barcode.Arguments = "-i " + """" + DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString + """" + " -r " + """" + ".\temp_refs\barcode.fasta" + """" + " -o " + """" + TextBox1.Text + """" + " -p " + max_thread.ToString + " -w " + (word_size + 11).ToString
                Dim process_split_barcode As Process = Process.Start(SI_split_barcode)
                process_split_barcode.WaitForExit()
                process_split_barcode.Close()
            End If
        Next
        MsgBox("Analysis completed!")
        PB_value = 0
    End Sub

    Private Sub 合并结果ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 合并结果ToolStripMenuItem1.Click
        If TextBox1.Text <> "" Then
            Dim refs_count As Integer = 0
            out_dir = TextBox1.Text
            Dim th1 As New Thread(AddressOf do_combine_barcode)
            th1.Start()
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Public Sub do_combine_barcode()
        For batch_i As Integer = 1 To seqsView.Count
            My.Computer.FileSystem.CreateDirectory(Path.Combine(out_dir, "clean_data"))
            My.Computer.FileSystem.CreateDirectory(Path.Combine(out_dir, "raw_data"))
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then

                Dim infolder As String = Path.Combine(out_dir, System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString))
                Dim fileList() As String = Directory.GetFileSystemEntries(infolder)  ' 遍历所有的文件和目录  
                For Each FileName As String In fileList
                    If FileName.EndsWith("_clean.fasta") Then
                        MergeFiles(Path.Combine(out_dir, "clean_data", Path.GetFileName(FileName)), FileName)
                    Else
                        MergeFiles(Path.Combine(out_dir, "raw_data", Path.GetFileName(FileName)), FileName)
                    End If
                Next

            End If
        Next
        MsgBox("Analysis completed!")
        PB_value = 0
    End Sub

    Private Sub 重建序列ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 重建序列ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            Dim refs_count As Integer = 0
            ref_dir = (currentDirectory + "temp\temp_refs\").Replace("\", "/")
            out_dir = TextBox1.Text
            DataGridView2.EndEdit()
            DeleteDir(ref_dir)
            My.Computer.FileSystem.CreateDirectory(ref_dir)
            For i As Integer = 1 To refsView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    MergeFiles(Path.Combine(ref_dir, "barcode_refs.fasta"), Path.Combine(currentDirectory, "temp", "org_seq", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta"))
                End If
            Next
            Dim level As String = InputBox("Input variability value: 1-4. Increasing the variability value results in a greater variation in the alternative sequences.", "Input", 2)
            If Int(level) >= 1 And Int(level) <= 4 Then
                Dim th1 As New Thread(AddressOf do_build_barcode)
                th1.Start(Int(level))
            Else
                MsgBox("Please enter the correct number!")
            End If
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub

    Public Sub do_build_barcode(ByVal level As Integer)
        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim SI_build_barcode As New ProcessStartInfo()
                SI_build_barcode.FileName = Path.Combine(currentDirectory, "analysis", "build_barcode.exe")
                SI_build_barcode.WorkingDirectory = Path.Combine(currentDirectory, "temp")
                SI_build_barcode.CreateNoWindow = True
                SI_build_barcode.Arguments = "-i " + """" + DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString + """" + " -r " + """" + ".\temp_refs\barcode_refs.fasta" + """" + " -o " + """" + TextBox1.Text + """" + " -p " + max_thread.ToString + " -l " + level.ToString
                Dim process_build_barcode As Process = Process.Start(SI_build_barcode)
                process_build_barcode.WaitForExit()
                process_build_barcode.Close()
            End If
        Next
        MsgBox("Analysis completed!")
        PB_value = 0
    End Sub

    Private Sub 哺乳动物线粒体基因组ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 哺乳动物线粒体基因组ToolStripMenuItem.Click
        cpg_down_mode = 7
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = True
        form_config_cp.Show()
    End Sub

    Private Sub 哺乳动物线粒体基因组ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 哺乳动物线粒体基因组ToolStripMenuItem1.Click
        If check_data_count(1) = 0 Then
            MsgBox("Please select at least one sequencing data!")
            Exit Sub
        End If
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        cpg_down_mode = 8
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = False
        form_config_cp.Show()
    End Sub

    Private Sub 哺乳动物线粒体基因组ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles 哺乳动物线粒体基因组ToolStripMenuItem2.Click
        If Directory.GetFileSystemEntries(TextBox1.Text).Length > 0 And DebugToolStripMenuItem.Checked = False Then
            Dim result As DialogResult = MessageBox.Show("The result folder is not empty. Are you sure to proceed?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then
                Exit Sub
            End If
        End If
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        cpg_down_mode = 9
        out_dir = TextBox1.Text.Replace("\", "/")
        form_config_cp.CheckBox1.Visible = False
        form_config_cp.Show()
    End Sub

    Private Sub EnglishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnglishToolStripMenuItem.Click
        If language = "EN" Then
            to_ch()
        Else
            to_en()
        End If
        settings("language") = language
    End Sub


    Private Sub RichTextBox1_DoubleClick(sender As Object, e As EventArgs) Handles RichTextBox1.DoubleClick
        If File.Exists(TextBox1.Text + "\log.txt") Then
            Using sr As New StreamReader(TextBox1.Text + "\log.txt")
                RichTextBox1.Text = sr.ReadToEnd
            End Using
        End If
    End Sub

    Private MenuClicked As String
    Private Sub ConfirmClickedHandler()
        Select Case MenuClicked
            Case "filter"
                menu_filter()
            Case "refilter"
                menu_refilter()
            Case "assemble"
                menu_assemble()
            Case "batch_auto_assemble"
                menu_batch_auto_assemble()
            Case "auto_assemble"
                menu_auto_assemble()
            Case "iteration"
                menu_iteration()
            Case "null"
            Case Else
        End Select
    End Sub

    Private Sub 多拷贝检测ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 多拷贝检测ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            DataGridView1.EndEdit()
            DataGridView2.EndEdit()
            DataGridView1.Refresh()
            DataGridView2.Refresh()
            out_dir = TextBox1.Text
            timer_id = 4
            PB_value = 0
            Dim th1 As New Thread(AddressOf check_paralogs)
            th1.Start()
        Else
            MsgBox("Please select an output folder!")
        End If
    End Sub
    Public Sub check_paralogs()
        If My.Computer.FileSystem.DirectoryExists(Path.Combine(out_dir, "results")) Then
            Dim paralogs_dir As String = Path.Combine(out_dir, "paralogs")
            DeleteDir(paralogs_dir)
            My.Computer.FileSystem.CreateDirectory(paralogs_dir)
            Dim count As Integer = 0
            Dim parallelOptions As New ParallelOptions()
            parallelOptions.MaxDegreeOfParallelism = max_thread
            Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                     'For i As Integer = 1 To refsView.Count
                                                                     count += 1
                                                                     PB_value = count / refsView.Count * 100
                                                                     If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                                                                         Dim in_path_fasta As String = Path.Combine(out_dir, "results", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                                                                         Dim in_path_fq As String = Path.Combine(out_dir, "filtered", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fq")
                                                                         Dim out_path As String = paralogs_dir
                                                                         Dim out_path_file As String = Path.Combine(paralogs_dir, DataGridView1.Rows(i - 1).Cells(2).Value.ToString + "_ref.fasta")
                                                                         If File.Exists(in_path_fasta) AndAlso File.Exists(in_path_fq) Then
                                                                             safe_copy(in_path_fasta, out_path_file, True)
                                                                             Dim SI_build_barcode As New ProcessStartInfo()
                                                                             SI_build_barcode.FileName = Path.Combine(currentDirectory, "analysis", "build_barcode.exe")
                                                                             SI_build_barcode.WorkingDirectory = Path.Combine(currentDirectory, "temp")
                                                                             SI_build_barcode.CreateNoWindow = True
                                                                             SI_build_barcode.Arguments = "-i " + """" + in_path_fq + """" + " -o " + """" + out_path + """" + " -p " + max_thread.ToString + " -c 0.25,0.3,0.35,0.4,0.45,0.5,0.55,0.6,0.65,0.7,0.75 -l 4 -m 1"
                                                                             Dim process_build_barcode As Process = Process.Start(SI_build_barcode)
                                                                             process_build_barcode.WaitForExit()
                                                                             process_build_barcode.Close()
                                                                         End If
                                                                     End If
                                                                 End Sub)
            PB_value = -1
            Dim result As DialogResult = MessageBox.Show("Analysis has been completed. Would you like to view the results file?", "Confirm Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            ' 根据用户的选择执行相应的操作
            If result = DialogResult.Yes Then
                Process.Start("explorer.exe", """" + paralogs_dir + """")
            End If
        End If
    End Sub

    Private Sub 分离重建ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 分离重建ToolStripMenuItem.Click
        form_config_barcode.Show()
    End Sub

    Private Sub 旁系同源检测ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 旁系同源检测ToolStripMenuItem.Click

        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        timer_id = 4
        PB_value = 0
        Dim th1 As New Thread(AddressOf batch_paralog)
        th1.Start()
    End Sub

    Public Sub batch_paralog()
        For batch_i As Integer = 1 To seqsView.Count
            PB_value = batch_i / seqsView.Count * 100
            If DataGridView2.Rows(batch_i - 1).Cells(0).FormattedValue.ToString = "True" Then
                Dim folder_name As String = make_out_name(System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(2).Value.ToString), System.IO.Path.GetFileNameWithoutExtension(DataGridView2.Rows(batch_i - 1).Cells(3).Value.ToString))
                out_dir = (TextBox1.Text + "\" + batch_i.ToString + "_" + folder_name).Replace("\", "/")

                Dim paralogs_dir As String = Path.Combine(out_dir, "paralogs")
                My.Computer.FileSystem.CreateDirectory(paralogs_dir)
                Dim count As Integer = 0
                Dim parallelOptions As New ParallelOptions()
                parallelOptions.MaxDegreeOfParallelism = max_thread
                Parallel.For(1, refsView.Count + 1, parallelOptions, Sub(i)
                                                                         If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then

                                                                             Dim in_path_fasta As String = Path.Combine(out_dir, "results", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fasta")
                                                                             Dim in_path_fq As String = Path.Combine(out_dir, "filtered", DataGridView1.Rows(i - 1).Cells(2).Value.ToString + ".fq")
                                                                             Dim out_path As String = paralogs_dir
                                                                             Dim out_path_file As String = Path.Combine(paralogs_dir, DataGridView1.Rows(i - 1).Cells(2).Value.ToString + "_ref.fasta")

                                                                             If File.Exists(in_path_fasta) AndAlso File.Exists(in_path_fq) Then

                                                                                 safe_copy(in_path_fasta, out_path_file, True)
                                                                                 Dim SI_build_barcode As New ProcessStartInfo()
                                                                                 SI_build_barcode.FileName = Path.Combine(currentDirectory, "analysis", "build_barcode.exe")
                                                                                 SI_build_barcode.WorkingDirectory = Path.Combine(currentDirectory, "temp")
                                                                                 SI_build_barcode.CreateNoWindow = True
                                                                                 SI_build_barcode.Arguments = "-i " + """" + in_path_fq + """" + " -o " + """" + out_path + """" + " -p " + max_thread.ToString + " -c 0.25,0.3,0.35,0.4,0.45,0.5,0.55,0.6,0.65,0.7,0.75 -l 4 -m 1"
                                                                                 Dim process_build_barcode As Process = Process.Start(SI_build_barcode)
                                                                                 process_build_barcode.WaitForExit()
                                                                                 process_build_barcode.Close()
                                                                             End If
                                                                         End If
                                                                     End Sub)

            End If
        Next
        PB_value = -1
        MsgBox("Analysis completed!")
    End Sub

    Private Sub 全选ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 全选ToolStripMenuItem.Click
        For i As Integer = 1 To seqsView.Count
            DataGridView2.Rows(i - 1).Cells(0).Value = True
        Next
        DataGridView2.RefreshEdit()
    End Sub

    Private Sub 反选ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles 反选ToolStripMenuItem1.Click
        Dim sel_count As Integer = 0
        For i As Integer = 1 To seqsView.Count
            If DataGridView2.Rows(i - 1).Cells(0).Value = True Then
                DataGridView2.Rows(i - 1).Cells(0).Value = False
            Else
                DataGridView2.Rows(i - 1).Cells(0).Value = True
                sel_count += 1
            End If
        Next
        DataGridView1.RefreshEdit()
    End Sub

    Private Sub 全不选ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 全不选ToolStripMenuItem.Click
        For i As Integer = 1 To seqsView.Count
            DataGridView2.Rows(i - 1).Cells(0).Value = False
        Next
        DataGridView2.RefreshEdit()
    End Sub

    Private Sub 统计结果ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 统计结果ToolStripMenuItem.Click
        Dim SI_build_summary As New ProcessStartInfo()
        SI_build_summary.FileName = Path.Combine(currentDirectory, "analysis", "build_summary.exe")
        SI_build_summary.WorkingDirectory = Path.Combine(currentDirectory, "temp")
        SI_build_summary.CreateNoWindow = True
        SI_build_summary.Arguments = "-b " + """" + TextBox1.Text + """"
        Dim process_build_summary As Process = Process.Start(SI_build_summary)
        process_build_summary.WaitForExit()
        process_build_summary.Close()
        MsgBox("Please check the 'summary.csv' in the output folder.")
    End Sub

    Private Sub 用迭代覆盖ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 用迭代覆盖ToolStripMenuItem.Click
        If TextBox1.Text <> "" Then
            If refsView.Count > 0 Then
                DataGridView1.EndEdit()
                DataGridView2.EndEdit()
                DataGridView1.Refresh()
                DataGridView2.Refresh()
                Dim th1 As New Thread(AddressOf cover_with_iteration)
                th1.Start()
            End If
        Else
            MsgBox("Please select an output folder!")
        End If

    End Sub
    Private Sub cover_with_iteration()
        For i As Integer = 1 To refsView.Count
            PB_value = i / refsView.Count * 100
            Dim gene_name As String = DataGridView1.Rows(i - 1).Cells(2).Value.ToString
            If File.Exists(TextBox1.Text + "\iteration\contigs_all\" + gene_name + ".fasta") Then
                safe_copy(TextBox1.Text + "\iteration\contigs_all\" + gene_name + ".fasta", TextBox1.Text + "\contigs_all\" + gene_name + ".fasta", True)
            End If
            If File.Exists(TextBox1.Text + "\iteration\results\" + gene_name + ".fasta") Then
                safe_copy(TextBox1.Text + "\iteration\results\" + gene_name + ".fasta", TextBox1.Text + "\results\" + gene_name + ".fasta", True)
            End If
            If File.Exists(TextBox1.Text + "\iteration\filtered\" + gene_name + ".fq") Then
                safe_copy(TextBox1.Text + "\iteration\filtered\" + gene_name + ".fq", TextBox1.Text + "\filtered\" + gene_name + ".fq", True)
            End If
            If File.Exists(TextBox1.Text + "\iteration\large_files\" + gene_name + ".fq") Then
                safe_copy(TextBox1.Text + "\iteration\large_files\" + gene_name + ".fq", TextBox1.Text + "\large_files\" + gene_name + ".fq", True)
            End If
        Next
        DeleteDir(TextBox1.Text + "\iteration")
        PB_value = 0
    End Sub

    Private Sub 过滤参数设定ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 过滤参数设定ToolStripMenuItem.Click
        DataGridView1.EndEdit()
        DataGridView2.EndEdit()
        DataGridView1.Refresh()
        DataGridView2.Refresh()
        MenuClicked = "null"
        form_config_basic.GroupBox2.Enabled = True
        form_config_basic.GroupBox3.Enabled = False
        form_config_basic.GroupBox4.Enabled = False
        form_config_basic.NumericUpDown1.Value = 16
        form_config_basic.Show()
    End Sub
End Class
