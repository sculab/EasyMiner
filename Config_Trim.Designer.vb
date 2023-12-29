<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Config_Trim
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        CheckBox1 = New CheckBox()
        Label1 = New Label()
        NumericUpDown1 = New NumericUpDown()
        Button2 = New Button()
        Button1 = New Button()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Checked = True
        CheckBox1.CheckState = CheckState.Checked
        CheckBox1.Location = New Point(12, 36)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(123, 21)
        CheckBox1.TabIndex = 0
        CheckBox1.Text = "只保留最长的匹配"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(12, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(99, 17)
        Label1.TabIndex = 1
        Label1.Text = "保留长度阈值(%)"
        ' 
        ' NumericUpDown1
        ' 
        NumericUpDown1.Location = New Point(215, 7)
        NumericUpDown1.Name = "NumericUpDown1"
        NumericUpDown1.Size = New Size(66, 23)
        NumericUpDown1.TabIndex = 2
        NumericUpDown1.Value = New Decimal(New Integer() {50, 0, 0, 0})
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(215, 64)
        Button2.Name = "Button2"
        Button2.Size = New Size(75, 30)
        Button2.TabIndex = 46
        Button2.Text = "取消"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(134, 64)
        Button1.Name = "Button1"
        Button1.Size = New Size(75, 30)
        Button1.TabIndex = 45
        Button1.Text = "确定"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Config_Trim
        ' 
        AutoScaleDimensions = New SizeF(7F, 17F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(302, 125)
        ControlBox = False
        Controls.Add(Button2)
        Controls.Add(Button1)
        Controls.Add(NumericUpDown1)
        Controls.Add(Label1)
        Controls.Add(CheckBox1)
        Name = "Config_Trim"
        StartPosition = FormStartPosition.CenterScreen
        Text = "切齐序列"
        TopMost = True
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
End Class
