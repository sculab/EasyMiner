Imports System.Reflection.Emit
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button

Module Module_Language
    Public Sub to_en()
        form_main.Text = "EasyMiner" + " ver. " + version

        language = "EN"

    End Sub

    Public Sub to_ch()
        form_main.Text = "EasyMiner" + " ver. " + version

        language = "CH"


    End Sub
End Module
