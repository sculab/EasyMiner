# 介绍
GeneMiner2是一款为系统发育基因组学设计的全功能工具包，软件主要功能包括：
- 从二代测序数据中挖掘单拷贝核基因、质体基因等分子标记
- 将多个分子标记切齐、排序、建立串联和溯祖系统发育树
- 拼接注释动植物质体基因组
用户能够在GeneMiner2中完成从NGS数据获取到系统发育树建立的所有工作。

## 引用
GeneMiner2基于我们之前开发的GeneMiner和Easy353软件，并整合了Blast、Minimap2、Fasttree、Muscle5、Mafft、Astral、PDD、PGA、NOVOPlasty、OrthoFinder等优秀工具，请在使用对应功能时引用软件提示的文献。

GeneMiner2的工作尚未发表，目前请引用我们关于GeneMiner的论文：
- Pulin Xie, Yongling Guo, Yue Teng, Wenbin Zhou, Yan Yu. 2024. GeneMiner: a tool for extracting phylogenetic markers from next-generation sequencing data. Molecular Ecology Resources. DOI: 10.1111/1755-0998.13924
如果挖掘被子植物353基因，请引用我们关于Easy353的论文：
- Zhang Z, Xie P, Guo Y, Zhou W, Liu E, Yu Y. 2022. Easy353: A tool to get Angiosperms353 genes for phylogenomic research. Molecular Biology and Evolution 39(12): msac261.



# 安装和需求

![](images/main_page_chin.jpg)

软件基于.net平台开发，仅提供x64版本，需要在计算机上安装有.NET 6.0 Desktop Runtime x64。如果不满足需求，软件会在第一次运行时提醒您下载。您也可以从此处获取.NET 6.0 Desktop Runtime x64的安装包: 

https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-6.0.21-windows-x64-installer

GeneMiner的源代码均保存在github和Gitee上，您可以从下面的地址获取最新的安装包: 

[Index of /database/app/EasyMiner (tpddns.cn)](http://life-bioinfo.tpddns.cn:8445/database/app/EasyMiner/)

或

[EasyMiner download | SourceForge.net](https://sourceforge.net/projects/scueasyminer/)

如果您需要在macOS或Linux上使用命令行版本的基因挖掘工具，请访问: 

- GeneMiner: https://github.com/sculab/GeneMiner

- Easy353: https://github.com/plant720/Easy353

您也可以使用GeneMiner2上scripts文件夹中的python脚本，这些脚本提供了GeneMiner的所有核心功能，并可以在macOS或Linux上部署。

---

# 示例数据


[DEMO1](DEMO/DEMO1/): 用于 *Part1 基本操作* 的演示。基于目标物种的二代浅层测序，使用近缘种的参考序列获取目标因。
- 目标物种：拟南芥(*Arabidopsis thaliana*)
- 数据类型：全基因组测序（浅层测序），深度约2x
- 拟获取的目标基因：ITS、martK、psbA、rbcL、rps16
- 参考序列：来自近源物种琴叶拟南芥(*Arabidopsis lyrata*)的ITS、martK、psbA、rbcL、rps16基因


[DEMO2](DEMO/DEMO2/): 用于Part2的演示中。组装植物叶绿体和线粒体基因组所需数据，选用拟南芥二代测序数据。


[DEMO3](DEMO/DEMO3/): 动物线粒体基因组拼接所需测序序列，选用红原鸡（*Gallus gallus*）二代测序数据。


[DEMO4](DEMO/DEMO4/): 用于 **Part2 构建被子植物353(AGS353)基因集合系统发育树** 的演示。基于多个目标物种的二代浅层测序，使用Kew的被子植物353基因集获取所需数据，获取数据并构建串联树和溯祖树。
- 目标物种：来自唇形科香薷属的6个物种
- 数据类型：全基因组测序数据（浅层数据），深度约10x。（**注意：为了便于下载和演示，我们对原始数据进行了缩减，仅保留了运行本DEMO所需的reads**）
- 拟获取的目标基因：被子植物353基因集合的353条单拷贝基因
- 参考序列：GeneMiner






# 详细

有关软件的详细说明。请见： [here](manual/ZH_CN/readmeall.md)


---
# 使用方法

## Part 1：如何完成基因序列提取

该实例演示了利用来自琴叶拟南芥(*Arabidopsis lyrata*)的基因序列，从拟南芥(*Arabidopsis thaliana*)的二代测序文件中获取对应的基因。

### 数据准备:

**（1）测序数据**: 二代测序的数据文件，文件格式为.gz或.fq。GeneMiner主要针对短读长的测序文件（reads长度为100、150、300等）。一般而言，浅层基因组、转录组、全基因组的双端或者单端测序文件都可以使用。

**（2）参考序列**: 近源物种的参考基因序列文件。可以使用fasta或genbank格式。对于fasta格式，文件名通常为基因名，每个文件中可以包含多个不同物种的同一个基因。对于genbank，同一个gb文件中可以包含多个物种的多个基因，GeneMiner会自动按基因名进行分解和组合。


**载入数据**: 

![](images/load_all_chinese.jpg)

点击 **[文件>载入测序文件]** 选择测序数据文件。



![](images/chinese_ex_import.jpg)

 示例: 打开Arabidopsis_thaliana.1.fq.gz和Arabidopsis_thaliana.1.fq.gz两个文件。这两个数据文件是来自拟南芥(*Arabidopsis thaliana*)的双端二代测序文件，每个文件中保存了1M (2^20)条reads。

**注意**: 对于配对(paired)的序列文件，需要同时选中两个（偶数个）数据文件一起载入，如只选取一个，则会作为单端测序数据载入。

  点击 **[文件>载入参考序列]** 选择fasta格式的参考序列文件，可以一次选择多个参考序列文件。

![](images/refs.jpg)

示例: 载入 DEMO/A_lyrata/ 下的所有fasta文件 (ITS、martK、psbA、rbcL、rps16)，包括1个核基因和4个叶绿体基因的参考序列，所有这些序列来都自拟南芥同属的近缘种琴叶拟南芥(*A. lyrata*)。

 ![](images/refs_xiang.jpg)

导入文件后会显示参考序列的ID、基因名、序列数量、序列平均长度等信息。


### 运行程序

![](images/analyse_chinese.jpg)

点击[分析>过滤&拼接] 使用默认参数运行程序，等待程序运行结束。

**注意: 切勿手动关闭弹出的命令行窗口，请耐心等待窗口自动关闭。**

**注意**：[分析>**基于参考切齐**]可以根据参考序列比对上的部分进行切齐。如果参考序列是来自转录组，使用的测序数据是浅层，建议进行。切齐后会自动替代之前的结果序列，如果想重新得到完整序列结果需要进行[重新拼接]。

 

### 查看结果

![](images/right.jpg)

点击“打开目录”按钮，查看结果文件。拼接后的文件以fasta格式保存于results目录中。

---

## Part 2：如何完成基因组拼接

该实例演示了利用拟南芥(*Arabidopsis thaliana*)的二代测序文件，组装叶绿体和线粒体基因组。

## 组装叶绿体基因组

### 数据准备:

**测序数据**: 二代测序的数据文件，文件格式为.gz或.fq。




**载入数据**:  

点击[**文件>载入测序文件**]选择序列数据文件。


![](images/load_seq_chinese.jpg)



### 运行程序:  

点击[**分析>植物叶绿体基因组**]下载近缘物种参考基因组。 

![](images/analyse_genome_chinese.jpg)


下载近源物种的参考基因组。


![](images/download_chinese.jpg)



单击确定使用组装的默认参数。

![](images/Novoplasty.jpg)  



### 查看结果:  

点击**打开目录**查看结果。

![](images/right_page.jpg)

结果文件以gb和fasta两种格式存储**Organelle**目录中。 


重新将结果gb文件导入可以选择扩展cds边界，并导出所需的基因文件: 

**[文件>载入参考序列]**：导入结果gb文件，可以选择想要扩展的内含子边界。

**[文件>导出参考序列]**：可以勾选想要导出的基因组中的特定基因文件。导出为fasta格式。 

![](images/file_chinese.jpg)






## 组装线粒体基因组

遵循与植物叶绿体基因组组装相同的过程。但需要先导入刚刚得到的植物叶绿体基因组的组装结果。

---

# 常见问题


**1.** **测序数据是否需要去除接头和低质量reads？**


建议使用测序公司提供的HQ版本的数据，使用低质量数据可能导致提取结果效果不好。如果没有HQ数据建议去除接头和低质量reads。


**2.** **得不到结果序列可能原因和解决办法?**


* 选用的参考序列不够近源（手动查找更为近源的序列）
* 测序数据的深度太浅（可以尝试把过滤K值调低）
  * 降低kmer得到的结果可能精确度不够，对于假阳性等错误序列，需要自己手动筛选分辨
  * 过滤K值最小为17，且应设置为单数。
* 如果拼接结果不理想，可以尝试[分析>迭代]重新分析


**3.** **想要同时运行多个GeneMiner窗口？**


如果想加快提取速度，在保证电脑内存的情况下可以运行多个GeneMiner窗口。**注意**：不要在同一个文件夹下打开多个窗口并行运行，可以复制GeneMiner所在的文件夹，并在副本文件中运行第二个窗口。（文件夹路径不能有中文名）


**4.** **我该如何获取内含子序列数据？**


首先通过叶绿体基因组组装得到完整的gb文件，之后再将gb文件导入，勾选去除外显子区域，并在扩展边界长度选择您需要的内含子区长度。
**注意：在进行拼接时，如果参考序列（外显子序列）的内部空缺内含子区域，软件拼接结果会包含中间空缺的内含子。**


**5.** **没有切齐的结果？**


[批量>合并&切齐]需要同时存在：导入的测序文件、导入的参考序列文件、存在结果的结果目录文件夹。
**注意**：结果文件夹中的测序序列ID号要与导入的测序文件ID相对应。


**6.** **PPD没有结果？**


请保证选择了三个及以上的物种类群进行批量提取。
保证无中文目录文件夹。


**7.** **为什么没有线粒体基因组的组装结果？**


建议将测序文件的完整读长长度用于线粒体基因组组装。您可以在[分析>过滤]处取消勾选[读取/文件(M)]，以分析完整读取长度数据。
**注意：进行批量-质体基因组组装时，电脑需要预留出足够大的储蓄空间。**

**8.** **请不要将软件安装在移动硬盘中。**

**9.** **如果文件（序列）正在被GeneMiner使用，请不要打开该文件，以免程序冲突造成闪退。**

**9.** **基于参考切齐得到的结果不全，并且有长有短？**
基于参考切齐是基于中位数*设定阈值，得到的长度如果小于中位数*设定阈值，那么将会被去除。


# 联系方式
有关GeneMiner任何建议、问题，请联系邮箱
Xinyi_Yu2021@163.com.