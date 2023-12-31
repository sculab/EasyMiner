<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Config_Tree
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Config_Tree))
        GroupBox1 = New GroupBox()
        RadioButton2 = New RadioButton()
        RadioButton1 = New RadioButton()
        Label1 = New Label()
        NumericUpDown1 = New NumericUpDown()
        GroupBox2 = New GroupBox()
        RadioButton4 = New RadioButton()
        RadioButton3 = New RadioButton()
        Button2 = New Button()
        Button1 = New Button()
        TextBox1 = New TextBox()
        DataGridView1 = New DataGridView()
        CheckBox1 = New CheckBox()
        GroupBox1.SuspendLayout()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox2.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(RadioButton2)
        GroupBox1.Controls.Add(RadioButton1)
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(NumericUpDown1)
        GroupBox1.Location = New Point(12, 9)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(363, 87)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "建树类型"
        ' 
        ' RadioButton2
        ' 
        RadioButton2.AutoSize = True
        RadioButton2.Location = New Point(6, 54)
        RadioButton2.Name = "RadioButton2"
        RadioButton2.Size = New Size(185, 21)
        RadioButton2.TabIndex = 1
        RadioButton2.Text = "构建溯祖树(FastTree+Astral)"
        RadioButton2.UseVisualStyleBackColor = True
        ' 
        ' RadioButton1
        ' 
        RadioButton1.AutoSize = True
        RadioButton1.Checked = True
        RadioButton1.Location = New Point(6, 27)
        RadioButton1.Name = "RadioButton1"
        RadioButton1.Size = New Size(147, 21)
        RadioButton1.TabIndex = 0
        RadioButton1.TabStop = True
        RadioButton1.Text = "构建串联树 (FastTree)"
        RadioButton1.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(213, 29)
        Label1.Name = "Label1"
        Label1.Size = New Size(69, 17)
        Label1.TabIndex = 2
        Label1.Text = "Bootstrap:"
        ' 
        ' NumericUpDown1
        ' 
        NumericUpDown1.Location = New Point(288, 27)
        NumericUpDown1.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        NumericUpDown1.Name = "NumericUpDown1"
        NumericUpDown1.Size = New Size(69, 23)
        NumericUpDown1.TabIndex = 3
        NumericUpDown1.Value = New Decimal(New Integer() {100, 0, 0, 0})
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(RadioButton4)
        GroupBox2.Controls.Add(RadioButton3)
        GroupBox2.Location = New Point(12, 102)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(363, 87)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "矩阵类型"
        ' 
        ' RadioButton4
        ' 
        RadioButton4.AutoSize = True
        RadioButton4.Location = New Point(6, 54)
        RadioButton4.Name = "RadioButton4"
        RadioButton4.Size = New Size(146, 21)
        RadioButton4.TabIndex = 2
        RadioButton4.Text = "使用切齐后的数据矩阵"
        RadioButton4.UseVisualStyleBackColor = True
        ' 
        ' RadioButton3
        ' 
        RadioButton3.AutoSize = True
        RadioButton3.Checked = True
        RadioButton3.Location = New Point(6, 27)
        RadioButton3.Name = "RadioButton3"
        RadioButton3.Size = New Size(122, 21)
        RadioButton3.TabIndex = 1
        RadioButton3.TabStop = True
        RadioButton3.Text = "使用原始数据矩阵"
        RadioButton3.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Button2.Location = New Point(653, 363)
        Button2.Name = "Button2"
        Button2.Size = New Size(75, 30)
        Button2.TabIndex = 48
        Button2.Text = "取消"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Button1.Location = New Point(572, 363)
        Button1.Name = "Button1"
        Button1.Size = New Size(75, 30)
        Button1.TabIndex = 47
        Button1.Text = "确定"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' TextBox1
        ' 
        TextBox1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        TextBox1.Location = New Point(12, 195)
        TextBox1.Multiline = True
        TextBox1.Name = "TextBox1"
        TextBox1.ReadOnly = True
        TextBox1.Size = New Size(363, 162)
        TextBox1.TabIndex = 49
        TextBox1.Text = resources.GetString("TextBox1.Text")
        ' 
        ' DataGridView1
        ' 
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Location = New Point(381, 36)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.RowTemplate.Height = 25
        DataGridView1.Size = New Size(347, 321)
        DataGridView1.TabIndex = 50
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Location = New Point(381, 12)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(131, 21)
        CheckBox1.TabIndex = 51
        CheckBox1.Text = "选择外类群(有根树)"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' Config_Tree
        ' 
        AutoScaleDimensions = New SizeF(7F, 17F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(737, 424)
        ControlBox = False
        Controls.Add(CheckBox1)
        Controls.Add(DataGridView1)
        Controls.Add(TextBox1)
        Controls.Add(Button2)
        Controls.Add(Button1)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "Config_Tree"
        StartPosition = FormStartPosition.CenterScreen
        Text = "建树设置"
        TopMost = True
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).EndInit()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents RadioButton2 As RadioButton
    Friend WithEvents RadioButton1 As RadioButton
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents RadioButton4 As RadioButton
    Friend WithEvents RadioButton3 As RadioButton
    Friend WithEvents Label1 As Label
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents CheckBox1 As CheckBox
End Class
