Imports System.Text

Module Module_Var
    Public TargetOS As String = "win64"
    Public version As String = "1.0b build 20231221"
    Public exe_mode As String
    Public database_url As String
    Public settings As Dictionary(Of String, String)
    Public currentDirectory As String
    Public refsView As New DataView
    Public seqsView As New DataView
    Public ci As Globalization.CultureInfo = New Globalization.CultureInfo("en-us")
    Public path_char As String
    Public root_path As String
    Public lib_path As String
    Public Dec_Sym As String
    Public timer_id As Integer = 0
    Public waiting As Boolean = False
    Public current_file As String
    Public mydata_Dataset As New DataSet
    Public form_config_basic As New Config_Basic
    Public form_config_barcode As New Config_barcode
    Public form_config_plasty As New Config_Plasty
    Public form_config_cp As New Config_CP
    Public form_config_split As New Config_Split
    Public form_main As New Main_Form
    Public PB_value As Integer = 0
    Public info_text As String = ""
    Public language As String = "CH"
    Public add_data As Boolean = False
    Public data_type As String = ".fq"
    Public refs_type As String = "fasta"
    Public data_loaded As Boolean = False
    Public reads_length As Integer = 0
    Public max_thread As Integer = 8
    Public ref_dir, out_dir, q1, q2, k1, k2, sb As String
    Public utf8WithoutBom As New UTF8Encoding(False)
    Public cpg_down_mode As Integer = 0
    Public cpg_assemble_mode As Integer = 0

End Module
