
# 输出结果注解


## 输出文件

**contigs_all**: 所有可能的组装结果。

**filtered**: 过滤后得到的fq文件。

**iteration**: 首次或多次迭代得到的文件，内部文件名和文件含义与上级文件夹相同。

**large_files**: 进一步过滤时超过深度限制或者文件大小限制的原始fq文件。如果所有过滤结果都在限制以内，则不会出现该文件夹。

**log.txt**: 日志文件。

#### **results：拼接结果中权重最大的序列，即最终结果。**

**kmer_dict_k31.dict**: kmer字典文件，格式为：kmer片段(十六进制)，kmer计数（十六进制）。

**result_dict.txt**: 结果文件，格式为：基因名，序列拼接状态，拼接上的reads数量。

**ref_reads_count_dict.txt**: 每个参考基因序列拆分成kmer的总条数。

**result_dict.txt**: 结果文件，格式为：基因名，序列拼接状态，拼接上的reads数量。

#### Organelle：细胞器基因组的拼接结果。

* **Gennome_cp.fasta**：植物叶绿体基因组拼接结果。

* **Gennome_cp.gb**：注释后的植物叶绿体基因组拼接结果。

* **Gennome_mito_plant.fasta**：植物线粒体基因组拼接结果。


* **temp**：因终端关闭终止分析，未完成的细胞器基因组的拼接过程文件。

* **Gennome_mito.fasta**：动物线粒体基因组拼接结果。



#### 一致性重构结果：

* **consensus**：将结果序列和过滤后的fq文件进行映射。存在设定值以上的模糊碱基数的序列将会被保留。

* **supercontigs**：一致性重构的结果文件，使用IPUAC代码生成的退化序列，使用简并碱基标注了SNP位点。

#### 多拷贝检测结果：

* **paralogs**：旁系同源基因筛选的结果文件，其中_ref.fasta文件储存旁系同源基因，csv文件记录不同位置碱基map出现的次数，.pec.csv文件记录碱基变异的频率。

**summary.csv**：统计汇总结果。



#### 批量分析结果：

**您的测序文件名**：以测序序列名命名的文件夹，储存每个测序序列分别得到的拼接结果。
    子文件夹**blast**：储存基于参考切齐后的结果序列。

**combined_results**：储存合并后的结果文件。

**combined_trimed**：储存合并并切齐后的结果文件。

**combined_results.fasta**: 串联结果文件。

**combined_trimed.fasta**： 修剪后的串联结果文件。


**aligned**: 多序列比对的结果。

#### 旁系同源检测（PPD）结果：

**PPD>result>supercontig>s8 _rm_paralogs> Final_kept_genes**：为最终结果文件，其他文件具体含义见[PPD github](https://github.com/Bean061/putative_paralog#part2-trims-alignment-and-detectes-the-putative-paralogs)。

 ---

# 软件（图形界面）结果显示

### 参考序列列表：

Select: 是否使用该条参考序列。

ID: 参考基因的编号。

Name: 参考基因的名称。

Ref. Count: 参考基因的数量。

Ref. Length: 参考基因的平均长度(bp)。

Filter Depth: 使用参考基因过滤后的深度。过滤深度(Filter Depth)=reads测序长度*过滤出的reads数量/参考序列的平均长度。

Assemble State: 序列拼接的状态，包括：

* no reads: 未过滤出reads，请降低过滤K值或者提供更近源的参考序列

* distant references: 参考序列过于远源，请提供更近源的参考序列

* insufficient reads: 过滤出的reads太少，请减小过滤K值或者提供更近源的参考序列

* no seed: 无法找到合适的种子，请减小拼接K值或者提供更近源的参考序列

* no contigs: 没有拼接出结果

* low quality: 结果准确度较低，reads 不足以覆盖拼接出的结果

* success: 拼接成功

Ass. Length: 拼接结果的长度

Ass. Depth: 拼接结果的reads覆盖深度=reads测序长度*用于拼接的reads数量/拼接结果的长度

Max.Diff.: 进行[合并&剪切]后不同物种基因两两比对得到的最大差异度。右击鼠标-最大差异度可以筛选最大差异来去除低质量结果序列。默认0.1筛选。勾选后可以对勾选序列重新[合并&切齐]，用筛选并切齐后结果进行系统发育树构建。

 

### 测序序列列表：

Select: 是否使用该组数据文件

Data1: 测序文件的左端(1端)

Data2: 测序文件的右端(2端)，如果是单端测序，则自动与Data1中的内容相同。

**注意:批量功能针对不同物种的测序序列进行分析。**