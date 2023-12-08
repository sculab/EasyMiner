﻿Imports System.Reflection.Emit
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button

Module Module_Language
    Public Sub to_en()
        form_main.Text = "EasyMiner" + " ver. " + version
        form_main.文件ToolStripMenuItem.Text = "File"
        form_main.载入参考序列ToolStripMenuItem.Text = "Load References"
        form_main.测序文件ToolStripMenuItem.Text = "Load Sequencing Files"
        form_main.下载序列ToolStripMenuItem.Text = "Download References"
        form_main.下载叶绿体基因组ToolStripMenuItem.Text = "Plant Chloroplast Genome"
        form_main.下载植物线粒体ToolStripMenuItem.Text = "Plant Mitochondrial Genome"
        form_main.哺乳动物线粒体基因组ToolStripMenuItem.Text = "Animal Mitochondrial Genome"
        form_main.下载ToolStripMenuItem.Text = "Angiosperm 353 Genes"
        form_main.导出ToolStripMenuItem.Text = "Export List"
        form_main.导出参考序列ToolStripMenuItem.Text = "Export References"
        form_main.导出测序文件ToolStripMenuItem.Text = "Export Sequencing Files"
        form_main.刷新数据ToolStripMenuItem.Text = "Refresh Data"
        form_main.过滤ToolStripMenuItem.Text = "Filter"
        form_main.从头过滤ToolStripMenuItem.Text = "Filter from Scratch"
        form_main.进一步过滤ToolStripMenuItem.Text = "Further Filter"
        form_main.拼接ToolStripMenuItem.Text = "Assemble"
        form_main.全自动ToolStripMenuItem.Text = "Filter && Assemble"
        form_main.迭代ToolStripMenuItem.Text = "Iteration"
        form_main.迭代ToolStripMenuItem1.Text = "First Iteration"
        'form_main.重新拼接ToolStripMenuItem.Text = "Re-Assemble"
        'form_main.多次迭代ToolStripMenuItem.Text = "Multiple Iterations"
        form_main.重构ToolStripMenuItem.Text = "Generate Consensus"
        form_main.多拷贝检测ToolStripMenuItem.Text = "Paralog Detection"
        form_main.植物叶绿体基因组ToolStripMenuItem.Text = "Plant Chloroplast Genome"
        form_main.植物线粒体基因组ToolStripMenuItem.Text = "Plant Mitochondrial Genome"
        form_main.哺乳动物线粒体基因组ToolStripMenuItem1.Text = "Animal Mitochondrial Genome"
        form_main.长读条码ToolStripMenuItem.Text = "Long Read Analysis"
        form_main.分离序列ToolStripMenuItem.Text = "Separate Barcodes"
        form_main.合并结果ToolStripMenuItem1.Text = "Merge Results"
        form_main.重建序列ToolStripMenuItem.Text = "Reconstruct Sequences"
        form_main.分离重建ToolStripMenuItem.Text = "Separate && Reconstruct"
        form_main.批量ToolStripMenuItem1.Text = "Batch"
        form_main.过滤拼接ToolStripMenuItem.Text = "Filter && Assemble"
        form_main.合并结果ToolStripMenuItem.Text = "Merge Results"
        form_main.合并比对ToolStripMenuItem.Text = "Merge && Trim"
        form_main.重构序列ToolStripMenuItem.Text = "Generate Consensus"
        form_main.旁系同源检测ToolStripMenuItem.Text = "Paralog Detection"
        form_main.PPDToolStripMenuItem.Text = "PPD (for 353 only)"
        form_main.ToolStripMenuItem2.Text = "Plant Chloroplast Genome"
        form_main.ToolStripMenuItem3.Text = "Plant Mitochondrial Genome"
        form_main.哺乳动物线粒体基因组ToolStripMenuItem2.Text = "Animal Mitochondrial Genome"
        form_main.统计结果ToolStripMenuItem.Text = "Summary Statistics"
        form_main.三方工具ToolStripMenuItem.Text = "Other Tools"
        form_main.序列比对ToolStripMenuItem1.Text = "Sequence Alignment"
        form_main.切齐比对ToolStripMenuItem1.Text = "Trimmed Alignment"
        form_main.RichTextBox1.Text = "Double-click to view logs"
        form_main.ToolStripMenuItem4.Text = "Re-Assemble"
        form_main.分析ToolStripMenuItem.Text = "Analysis"
        form_main.GroupBox1.Text = "Output"
        form_main.Label3.Text = "Threads:"
        form_main.Button1.Text = "Change"
        form_main.Button2.Text = "Open"
        form_main.EnglishToolStripMenuItem.Text = "中文"

        form_config_barcode.Text = "Barcode"
        form_config_barcode.Label1.Text = "Data Folder:"
        form_config_barcode.Label2.Text = "Barcode:"
        form_config_barcode.Label3.Text = "Reference:"
        form_config_barcode.Button1.Text = "Browse"
        form_config_barcode.Button2.Text = "Browse"
        form_config_barcode.Button3.Text = "Browse"
        form_config_barcode.Button4.Text = "Cancel"
        form_config_barcode.Button5.Text = "OK"
        form_config_barcode.Label4.Text = "Variability:"
        form_config_barcode.CheckBox1.Text = "Recalculate Only"
        form_config_barcode.Label5.Text = "Accuracy:"

        form_config_basic.Text = "Basic Option"
        form_config_basic.GroupBox2.Text = "Filter"
        form_config_basic.CheckBox3.Text = "Reads/File (M)"
        form_config_basic.Label1.Text = "Filter K Value:"
        form_config_basic.Label2.Text = "Filter Step Size:"
        form_config_basic.CheckBox2.Text = "High-Speed"
        form_config_basic.GroupBox4.Text = "Assemble"
        form_config_basic.Label5.Text = "Fixed K:"
        form_config_basic.Label7.Text = "Err. Threshold:"
        form_config_basic.Label6.Text = "->"
        form_config_basic.CheckBox1.Text = "Auto-Estimate K"
        form_config_basic.GroupBox3.Text = "Further Filtering"
        form_config_basic.Label8.Text = "File Size Limit:"
        form_config_basic.Label4.Text = "Depth Limit:"
        form_config_basic.Button2.Text = "Cancel"
        form_config_basic.Button1.Text = "OK"
        form_config_basic.Label3.Text = "Boundary"

        form_config_cp.Text = "Download Data"
        form_config_cp.Button5.Text = "<<"
        form_config_cp.清空ToolStripMenuItem.Text = "Clear"
        form_config_cp.Button3.Text = ">>"
        form_config_cp.Label1.Text = "Include:"
        form_config_cp.全选ToolStripMenuItem.Text = "Select All"
        form_config_cp.Button2.Text = "Cancel"
        form_config_cp.Button1.Text = "OK"
        form_config_cp.CheckBox1.Text = "Download as Single Gene"
        form_config_cp.CheckBox2.Text = "Search Below Genus Level"

        form_config_plasty.Text = "NOVOPlasty"
        form_config_plasty.Label1.Text = "Type"
        form_config_plasty.Label2.Text = "Length"
        form_config_plasty.Label3.Text = "K-mer"
        form_config_plasty.Label4.Text = "Memory"
        form_config_plasty.Label5.Text = "Chloroplast Sequence"
        form_config_plasty.Label6.Text = "Reference Sequence"
        form_config_plasty.Button1.Text = "Browse"
        form_config_plasty.Button2.Text = "Browse"
        form_config_plasty.Button3.Text = "Cancel"
        form_config_plasty.Button4.Text = "OK"
        form_config_plasty.Label7.Text = "Insert Size"
        form_config_plasty.Label8.Text = "Read Length"
        form_config_plasty.Label9.Text = "Project"
        form_config_plasty.TextBox5.Text = "Genome"
        form_config_plasty.CheckBox1.Text = "Pre-filter with Reference Sequence"
        form_config_plasty.Button5.Text = "Clear"

        form_config_split.Text = "Split Sequences"
        form_config_split.Button2.Text = "Cancel"
        form_config_split.Button1.Text = "OK"
        form_config_split.Label4.Text = "Extend Left"
        form_config_split.Label1.Text = "Extend Right"
        form_config_split.Label3.Text = "Maximum Gene Length"
        form_config_split.Label2.Text = "Minimum Gene Length"
        form_config_split.CheckBox1.Text = "Exclude Exonic Regions"

        language = "EN"

    End Sub

    Public Sub to_ch()
        form_main.Text = "EasyMiner" + " ver. " + version
        form_main.文件ToolStripMenuItem.Text = "文件"
        form_main.载入参考序列ToolStripMenuItem.Text = "载入参考序列"
        form_main.测序文件ToolStripMenuItem.Text = "载入测序文件"
        form_main.下载序列ToolStripMenuItem.Text = "下载序列"
        form_main.下载叶绿体基因组ToolStripMenuItem.Text = "植物叶绿体基因组"
        form_main.下载植物线粒体ToolStripMenuItem.Text = "植物线粒体基因组"
        form_main.哺乳动物线粒体基因组ToolStripMenuItem.Text = "动物线粒体基因组"
        form_main.下载ToolStripMenuItem.Text = "被子植物353基因"
        form_main.导出ToolStripMenuItem.Text = "导出列表信息"
        form_main.导出参考序列ToolStripMenuItem.Text = "导出参考序列"
        form_main.导出测序文件ToolStripMenuItem.Text = "导出测序文件"
        form_main.刷新数据ToolStripMenuItem.Text = "刷新数据"
        form_main.过滤ToolStripMenuItem.Text = "过滤"
        form_main.从头过滤ToolStripMenuItem.Text = "从头过滤"
        form_main.进一步过滤ToolStripMenuItem.Text = "进一步过滤"
        form_main.拼接ToolStripMenuItem.Text = "拼接"
        form_main.全自动ToolStripMenuItem.Text = "过滤&&拼接"
        form_main.迭代ToolStripMenuItem.Text = "迭代"
        form_main.迭代ToolStripMenuItem1.Text = "运行迭代"
        form_main.重构ToolStripMenuItem.Text = "一致性重构"
        form_main.多拷贝检测ToolStripMenuItem.Text = "旁系同源检测"
        form_main.植物叶绿体基因组ToolStripMenuItem.Text = "植物叶绿体基因组"
        form_main.植物线粒体基因组ToolStripMenuItem.Text = "植物线粒体基因组"
        form_main.哺乳动物线粒体基因组ToolStripMenuItem1.Text = "动物线粒体基因组"
        form_main.长读条码ToolStripMenuItem.Text = "长读分析"
        form_main.分离序列ToolStripMenuItem.Text = "分离条码"
        form_main.合并结果ToolStripMenuItem1.Text = "合并结果"
        form_main.重建序列ToolStripMenuItem.Text = "重建序列"
        form_main.分离重建ToolStripMenuItem.Text = "分离&&重建"
        form_main.批量ToolStripMenuItem1.Text = "批量"
        form_main.过滤拼接ToolStripMenuItem.Text = "过滤&&拼接"
        form_main.合并结果ToolStripMenuItem.Text = "合并结果"
        form_main.合并比对ToolStripMenuItem.Text = "合并&&切齐"
        form_main.重构序列ToolStripMenuItem.Text = "一致性重构"
        form_main.旁系同源检测ToolStripMenuItem.Text = "旁系同源检测"
        form_main.PPDToolStripMenuItem.Text = "PPD (仅用于353)"
        form_main.ToolStripMenuItem2.Text = "植物叶绿体基因组"
        form_main.ToolStripMenuItem3.Text = "植物线粒体基因组"
        form_main.哺乳动物线粒体基因组ToolStripMenuItem2.Text = "动物线粒体基因组"
        form_main.统计结果ToolStripMenuItem.Text = "统计汇总结果"
        form_main.三方工具ToolStripMenuItem.Text = "其他"
        form_main.序列比对ToolStripMenuItem1.Text = "序列比对"
        form_main.切齐比对ToolStripMenuItem1.Text = "切齐比对"
        form_main.ToolStripMenuItem4.Text = "重新拼接"
        form_main.RichTextBox1.Text = "双击此处查看最近的日志"
        form_main.分析ToolStripMenuItem.Text = "分析"
        form_main.EnglishToolStripMenuItem.Text = "English"
        form_main.GroupBox1.Text = "输出目录"
        form_main.Label3.Text = "进程数量:"
        form_main.Button1.Text = "更改目录"
        form_main.Button2.Text = "打开目录"

        form_config_barcode.Text = "分析条码"
        form_config_barcode.Label1.Text = "数据文件夹:"
        form_config_barcode.Label2.Text = "条码序列:"
        form_config_barcode.Label3.Text = "参考序列:"
        form_config_barcode.Button1.Text = "浏览"
        form_config_barcode.Button2.Text = "浏览"
        form_config_barcode.Button3.Text = "浏览"
        form_config_barcode.Button4.Text = "取消"
        form_config_barcode.Button5.Text = "确定"
        form_config_barcode.Label4.Text = "变异级别:"
        form_config_barcode.CheckBox1.Text = "仅重新计算变异序列"
        form_config_barcode.Label5.Text = "精确度:"

        form_config_basic.Text = "基础设定"
        form_config_basic.GroupBox2.Text = "过滤"
        form_config_basic.CheckBox3.Text = "读长/文件(M)"
        form_config_basic.Label1.Text = "过滤K值："
        form_config_basic.Label2.Text = "过滤步长："
        form_config_basic.CheckBox2.Text = "高速(高内存占用)"
        form_config_basic.GroupBox4.Text = "拼接"
        form_config_basic.Label5.Text = "固定拼接K值："
        form_config_basic.Label7.Text = "错误阈值："
        form_config_basic.Label6.Text = "->"
        form_config_basic.CheckBox1.Text = "自动估算拼接K值(慢)"
        form_config_basic.GroupBox3.Text = "进一步过滤"
        form_config_basic.Label8.Text = "文件大小限制："
        form_config_basic.Label4.Text = "深度限制："
        form_config_basic.Button2.Text = "取消"
        form_config_basic.Label3.Text = "边界"
        form_config_basic.Button1.Text = "确定"

        form_config_cp.Text = "下载数据"
        form_config_cp.Button5.Text = "<<"
        form_config_cp.清空ToolStripMenuItem.Text = "清空"
        form_config_cp.Button3.Text = ">>"
        form_config_cp.Label1.Text = "包含类群: "
        form_config_cp.全选ToolStripMenuItem.Text = "全选"
        form_config_cp.Button2.Text = "取消"
        form_config_cp.Button1.Text = "确定"
        form_config_cp.CheckBox1.Text = "作为单个基因下载"
        form_config_cp.CheckBox2.Text = "不在属以上搜索"

        form_config_plasty.Text = "细胞器基因组"
        form_config_plasty.Label1.Text = "类型"
        form_config_plasty.Label2.Text = "长度"
        form_config_plasty.Label3.Text = "K-mer"
        form_config_plasty.Label4.Text = "最大允许内存"
        form_config_plasty.Label5.Text = "叶绿体序列(拼接线粒体)"
        form_config_plasty.Label6.Text = "参考序列(可选)"
        form_config_plasty.Button1.Text = "浏览"
        form_config_plasty.Button2.Text = "浏览"
        form_config_plasty.Button3.Text = "取消"
        form_config_plasty.Button4.Text = "确定"
        form_config_plasty.Label7.Text = "插入大小"
        form_config_plasty.Label8.Text = "读长大小"
        form_config_plasty.Label9.Text = "项目名"
        form_config_plasty.TextBox5.Text = "Genome"
        form_config_plasty.CheckBox1.Text = "使用参考序列进行预过滤"
        form_config_plasty.Button5.Text = "清空"

        form_config_split.Text = "分割序列"
        form_config_split.Button2.Text = "取消"
        form_config_split.Button1.Text = "确定"
        form_config_split.Label4.Text = "扩展边界左侧"
        form_config_split.Label1.Text = "扩展边界右侧"
        form_config_split.Label3.Text = "基因最大长度"
        form_config_split.Label2.Text = "基因最小长度"
        form_config_split.CheckBox1.Text = "去除外显子区"

        language = "CH"


    End Sub
End Module
