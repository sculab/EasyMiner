Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Module Module_Function
    Public Function ReadSettings(filePath As String) As Dictionary(Of String, String)
        Dim settings As New Dictionary(Of String, String)()
        If File.Exists(filePath) Then
            Dim lines As String() = File.ReadAllLines(filePath)
            For Each line As String In lines
                Dim parts As String() = line.Split("="c)
                If parts.Length = 2 Then
                    Dim key As String = parts(0).Trim()
                    Dim value As String = parts(1).Trim()
                    settings(key) = value
                End If
            Next
        End If
        Return settings
    End Function

    ' 保存设置到文件
    Public Sub SaveSettings(filePath As String, settings As Dictionary(Of String, String))
        Dim lines As New List(Of String)()
        For Each kvp As KeyValuePair(Of String, String) In settings
            Dim line As String = $"{kvp.Key}={kvp.Value}"
            lines.Add(line)
        Next

        File.WriteAllLines(filePath, lines)
    End Sub

    Public Function new_line(ByVal is_LF As Boolean) As String
        If is_LF Then
            Return vbLf
        Else
            Return vbCrLf
        End If
    End Function

    Public Function Check_Mixed_AA(ByVal seq As String) As Boolean
        Dim mixed_AA() As String = {"R", "Y", "M", "K", "S", "W", "H", "B", "V", "D", "N"}
        For Each i As String In mixed_AA
            If seq.ToUpper.Contains(i) Then
                Return False
            End If
        Next
        Return True
    End Function
    Public Sub clean_fasta_file(ByVal inputFilePath As String, ByVal outputFilePath As String)
        Try
            Using inputFile As New StreamReader(inputFilePath)
                Using outputFile As New StreamWriter(outputFilePath)
                    Dim line As String
                    While (Not inputFile.EndOfStream)
                        line = inputFile.ReadLine()
                        If line.StartsWith(">") Then
                            ' 替换非文件夹名称中允许的字符为下划线
                            line = ReplaceInvalidCharacters(line.Substring(1))
                            ' 写入到输出文件
                            outputFile.WriteLine(">" & line)
                        Else
                            ' 直接写入到输出文件
                            outputFile.WriteLine(line)
                        End If
                    End While
                End Using
            End Using
            Console.WriteLine("文件处理完成。")
        Catch ex As Exception
            Console.WriteLine("发生错误：" & ex.Message)
        End Try
    End Sub

    Private Function ReplaceInvalidCharacters(ByVal inputString As String) As String
        ' 定义不允许在文件夹名称中出现的字符
        Dim invalidChars As String = Path.GetInvalidFileNameChars()

        ' 遍历输入字符串，替换非法字符为下划线
        For Each invalidChar As Char In invalidChars
            inputString = inputString.Replace(invalidChar, "_")
        Next

        Return inputString
    End Function
    Public Sub safe_copy(ByVal source As String, ByVal target As String, Optional ByVal overwrite As Boolean = True)
        If File.Exists(source) Then
            File.Copy(source, target, overwrite)
            Dim myFileInfo As FileInfo = New FileInfo(target)
            myFileInfo.IsReadOnly = False
        End If
    End Sub
    Public Sub DeleteDir(ByVal aimPath As String)
        If (aimPath(aimPath.Length - 1) <> Path.DirectorySeparatorChar) Then
            aimPath += Path.DirectorySeparatorChar
        End If  '判断待删除的目录是否存在,不存在则退出.  
        If (Not Directory.Exists(aimPath)) Then Exit Sub ' 
        Dim fileList() As String = Directory.GetFileSystemEntries(aimPath)  ' 遍历所有的文件和目录  
        For Each FileName As String In fileList
            If (Directory.Exists(FileName)) Then  ' 先当作目录处理如果存在这个目录就递归
                DeleteDir(aimPath + Path.GetFileName(FileName))
                Directory.Delete(aimPath + Path.GetFileName(FileName))
            Else  ' 否则直接Delete文件  
                Try
                    File.Delete(aimPath + Path.GetFileName(FileName))
                Catch ex As Exception
                End Try
            End If
        Next  '删除文件夹
    End Sub
    Public Sub format_path()
        Select Case TargetOS
            Case "linux"
                path_char = "/"
            Case "win32", "macos"
                path_char = "\"
            Case Else
                path_char = "\"
        End Select
        root_path = (Application.StartupPath + path_char).Replace(path_char + path_char, path_char)
        Dec_Sym = CInt("0").ToString("F1").Replace("0", "")
        If Dec_Sym <> "." Then
            MsgBox("Notice: We will use dat (.) as decimal quotation instead of comma (,). We recommand to change your system's number format to English! ")
        End If
    End Sub
    Public Sub MergeFiles(ByVal firstFilePath As String, ByVal secondFilePath As String)
        Try
            Using firstFileWriter As New StreamWriter(firstFilePath, True) ' "True" appends to the file
                Using secondFileReader As New StreamReader(secondFilePath)
                    Dim line As String = ""
                    While (InlineAssignHelper(line, secondFileReader.ReadLine())) IsNot Nothing
                        firstFileWriter.WriteLine(line)
                    End While
                End Using
                'firstFileWriter.Write(vbCrLf)
            End Using
            Console.WriteLine("Files merged successfully.")
        Catch ex As Exception
            Console.WriteLine("An error occurred: " & ex.Message)
        End Try
    End Sub
    Private Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
    Public Function refresh_file()
        Dim length_min As Integer = 1000000000.0
        Dim length_max As Integer = 0
        Dim newrow(7) As String
        Dim seq_count As Integer = 0
        refsView.AllowNew = True
        Dim fileList() As String = Directory.GetFileSystemEntries(root_path + "temp\org_seq")
        For Each FileName As String In fileList
            If FileName.EndsWith(".fasta") Then
                seq_count += 1
                refsView.AddNew()
                Dim ref_count As Integer = 0
                Dim ref_length As Integer = 0
                Dim sr As New StreamReader(FileName)
                Dim line As String = sr.ReadLine
                Do
                    If line.StartsWith(">") Then
                        ref_count += 1
                    Else
                        ref_length += line.Length
                    End If
                    line = sr.ReadLine
                Loop Until line Is Nothing
                sr.Close()
                newrow(0) = refsView.Count
                newrow(1) = System.IO.Path.GetFileNameWithoutExtension(FileName)
                newrow(2) = ref_count
                newrow(3) = CInt(ref_length / ref_count)
                newrow(4) = ""
                newrow(5) = ""
                newrow(6) = ""
                newrow(7) = ""
                refsView.Item(refsView.Count - 1).Row.ItemArray = newrow
                If length_min > CInt(ref_length / ref_count) Then
                    length_min = CInt(ref_length / ref_count)
                End If
                If length_max < CInt(ref_length / ref_count) Then
                    length_max = CInt(ref_length / ref_count)
                End If
            End If
        Next
        refsView.AllowNew = False
        refsView.AllowEdit = True

        Return {length_min, length_max}
    End Function
    Public Function GetLineCount(filePath As String) As Integer
        Try
            Using reader As New StreamReader(filePath)
                Dim lineCount As Integer = 0
                While Not reader.EndOfStream
                    reader.ReadLine()
                    lineCount += 1
                End While
                Return lineCount
            End Using
        Catch ex As Exception
            MessageBox.Show($"无法打开文件: {ex.Message}")
            Return -1 ' 如果发生错误，返回-1表示失败
        End Try
    End Function

    Private ReadOnly client As HttpClient = New HttpClient()
    Private Async Function GetResponseTime(url As String) As Task(Of Long)
        Dim httpClient As New HttpClient()
        Dim stopwatch As New Stopwatch()

        Try
            stopwatch.Start()
            Dim response As HttpResponseMessage = Await httpClient.GetAsync(url)
            ' 不需要做任何事情，只是获取响应时间
        Catch ex As HttpRequestException
            ' 处理异常
            MessageBox.Show($"无法获取 {url} 的响应时间：{ex.Message}")
        Finally
            stopwatch.Stop()
        End Try

        Return stopwatch.ElapsedMilliseconds
    End Function
    Public Async Function get_genome_data(ByVal database_type As String, ByVal file_type As String, ByVal gb_id As String) As Task(Of String)
        Try
            Dim data_folder As String = currentDirectory & "database\" & database_type & "_" & file_type & "\" & gb_id.Substring(0, Math.Min(2, gb_id.Length)) & "\" & gb_id.Substring(0, Math.Min(5, gb_id.Length)) & "\"
            Dim file_path As String = data_folder & gb_id & "." & file_type

            If Not File.Exists(file_path) Then
                Dim source_file As String = database_url & database_type & "_" & file_type & "/" & gb_id.Substring(0, Math.Min(2, gb_id.Length)) & "/" & gb_id.Substring(0, Math.Min(5, gb_id.Length)) & "/" & gb_id & "." & file_type
                My.Computer.FileSystem.CreateDirectory(data_folder)

                Dim response = Await client.GetAsync(source_file)
                response.EnsureSuccessStatusCode()

                Dim content = Await response.Content.ReadAsByteArrayAsync()
                File.WriteAllBytes(file_path, content)
            End If
            Return file_path
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Sub build_ann(ByVal input1 As String, ByVal input2 As String, ByVal gb_file As String, ByVal output_file As String, ByVal WorkingDirectory As String)
        Dim SI_build_ann As New ProcessStartInfo()
        SI_build_ann.FileName = currentDirectory + "analysis\build_ann.exe" ' 替换为实际的命令行程序路径
        SI_build_ann.WorkingDirectory = WorkingDirectory ' 替换为实际的运行文件夹路径
        SI_build_ann.CreateNoWindow = False
        SI_build_ann.Arguments = "-i1 " + """" + input1 + """" + " -i2 " + """" + input2 + """" + " -gb " + """" + gb_file + """" + " -o " + """" + output_file + """"
        Dim process_build_ann As Process = New Process()
        process_build_ann.StartInfo = SI_build_ann
        process_build_ann.Start()
        process_build_ann.WaitForExit()
        process_build_ann.Close()
    End Sub
End Module
