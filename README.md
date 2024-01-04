
# Introduction
**中文版的使用说明见[此处](chinese_README.md)**

 ![](images/main_page.jpg)

GeneMiner2 is a comprehensive toolkit designed for phylogenomic genomics. Its main functionalities include:

- Mining single-copy nuclear genes, plastid genes, and other molecular markers from next-generation sequencing data.
- Aligning multiple molecular markers, sequencing, constructing concatenated and ancestral phylogenetic trees.
- Assembling and annotating animal and plant plastid genomes.
Users can complete all tasks from obtaining NGS data to establishing phylogenetic trees within GeneMiner2.


GeneMiner2 efficiently and precisely extracts molecular markers from second-generation sequencing data, leveraging reference genes from closely related species. Additionally, it offers functionalities for plastid genome assembly, gene sequence decomposition in GenBank files, and the identification of paralogs.

# Citations
GeneMiner2 is based on our previously developed GeneMiner and Easy353 software and integrates excellent tools such as Blast, Minimap2, Fasttree, Muscle5, Mafft, Astral, PDD, PGA, NOVOPlasty, and OrthoFinder. Please cite the literature prompted by the software when using the corresponding functionalities.

The work on GeneMiner2 has not yet been published. Currently, please cite our paper on GeneMiner:

- Pulin Xie, Yongling Guo, Yue Teng, Wenbin Zhou, Yan Yu. 2024. GeneMiner: a tool for extracting phylogenetic markers from next-generation sequencing data. Molecular Ecology Resources. DOI: 10.1111/1755-0998.13924

If mining angiosperm 353 genes, please cite our paper on Easy353:
- Zhang Z, Xie P, Guo Y, Zhou W, Liu E, Yu Y. 2022. Easy353: A tool to get Angiosperms353 genes for phylogenomic research. Molecular Biology and Evolution 39(12): msac261.


# Install
The .NET 6.0 Desktop Runtime x64 must be installed on the computer for proper software functionality.
[.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-6.0.21-windows-x64-installer)


You could download from [SourceForge](https://sourceforge.net/projects/geneminer/files/) or [our site](http://life-bioinfo.tpddns.cn:8445/database/app/GeneMiner/).

**Note: Avoid installing the software on a portable hard drive.**


# Demo

[DEMO1 - A Quick Tutorial For General Use (Part 1)](/DEMO/DEMO1/)



[DEMO2 - For Obtaining Organelle Genes (Genome) (Part 2)](/DEMO/DEMO2/)


  
[DEMO3 - For Obtaining Single Copy Genes](DEMO/DEMO3/)



[DEMO4 - For Obtaining Angiosperms353 Genes](DEMO/DEMO4/)


# More Details
For detailed instructions and tutorials, visit [here](manual/ZH_CN/readmeall.md).


# Command Line (cmd)

For users interested in the command line version, please refer to our [Easy353](https://github.com/plant720/Easy353) and [GeneMiner](https://github.com/sculab/GeneMiner)


# Tutorials

# Part 1 :How to get specific gene

This section demonstrates how to extract specific genes from second-generation sequencing files of Arabidopsis thaliana, using Arabidopsis lyrata gene sequences as a reference.


### Data Preparation


- **Sequencing Data**: Second-generation sequencing data files, in .gz or .fq format.


- **Reference Sequence**: Reference gene sequences from closely related species in fasta or genbank format.
 

**Load Data**
 

Click **[File>Load Sequencing Files]** to select sequencing data file.


![](images/load_file.jpg)


![](images/chinese_ex_import.jpg)
**Note**:  For paired sequencing files, select two (even-numbered) data files simultaneously. Single files will be treated as single-end sequencing data.


Click **[File>Load Reference]** to select  fasta format reference sequence files. Multiple files can be chosen at once.


![](images/refs.jpg)


Reference sequences in gb format can also be imported.Confirm to report the file as a gene list.


Optionally, expand the left or right intronic regions [Extend Left][Extend Right].
![](images/gb.jpg)

The imported files display details such as reference sequence ID, gene name, number of sequences, and average sequence length.
![](images/chinese_ref_xiangxi.jpg)


### Running Programme


Click **[Analyse>Filter&Assemble]**  to run the program with default parameters.

![](images/analyse_eng.jpg)

![](images/basic_option.jpg)



**Note:** Do not manually close the command line window; it will close automatically once the process is complete.

**Note:**[Analysis>**Trim With Reference**]If the reference sequence originates from a transcriptome and the sequencing data is of shallow depth, this trimming process is recommended. 

### View Results


Click **"Open"**  to view the results.


![](images/right_eng.jpg)

---
# Part 2 :How to Get Plastid Genome
This example demonstrates the process of mining chloroplast and mitochondrial genomes from second-generation sequencing data of *Arabidopsis thaliana*.


**Note:** Assembly of the Plant Chloroplast Genome is required before the Plant Mitochondrial Genome assembly.


## Step 1 :Plant Chlororplast Genome


### Data Preparation


**Sequencing Data**: Second-generation sequencing data files in .gz or .fq format.



**Load Data**


Click **[File>Load Sequencing Files]** to select sequencing data files.

![](images/load_seq.jpg)



### Running Programme


Click[**Analyse>Plant_Chloroplast_Genome**]to download the reference genome of closely related species.

![](images/step1.jpg)

![](images/download_genome.jpg)


Click **OK** and proceed with the default parameters for assembly.

![](images/Novoplasty.jpg)



### View Results


Click **"Open"** to view the results. Assembled files are stored in the **Organelle** directory in both gb and fasta formats. 

![](images/right_eng.jpg)

Specific genes can be extracted from the genome by loading and exporting the gb file:
You can extract specific genes from the genome by loading and exporting the gb file: **[File>Load_reference]**.**[File>Export_reference]**

---


## Step 2: Plant Mitochondrial Genome


Follow the same process as the Plant Chloroplast Genome assembly. However, initially **import the results of the previous Plant Chloroplast Genome** assembly.

![](images/mito.jpg)

---

# Questions


**1.** **Does sequencing data require the removal of junctions and low-quality reads?**


It's recommended to use the high-quality (HQ) version of the data provided by the sequencing company. Using low-quality data may result in less optimal extraction outcomes. If HQ data is not available, removing junctions and low-quality reads is advisable to enhance results.




**2.** **Causes and solutions for the absence of assemble result.**


*The selected reference sequence may not be sufficiently related. (Find a more closely related sequence manually).

*The sequencing data might not be deep enough. (Consider reducing the filter K value or extending the [Reads/File(M)]).

*If the assembly results are not ideal, re-analyze using [Analysis > Iteration].

*Consider reducing the Kmer value during assembly.

**Note:**  Lowering the Kmer value might affect accuracy. Manual discernment of false positive sequences is recommended.


**Note:**  It's advisable to set the Kmer value to be greater than 17 and ensure it's an odd number.




**3.** **What is the memory requirement of the software?**


The software requires minimal memory. Adjust the [Threads] setting according to your computer's memory capacity.




**4.** **How to obtain the intron sequence data?**


Firstly, acquire the gb file via [Analyse > Plant Chloroplast Genome], then import it using [File > Load Reference]. Confirm to report the file as a gene list. Determine the intron region length using [Extend Left] and [Extend Right].

**Note:** If the imported reference CDS includes an internal vacant intron region, the software's analysis will include this vacant intron.




**5.** **No result for  [Combine & Trim]?**


[Batch > Combine & Trim] requires: sequencing data, reference sequences, and a catalog folder where results exist.

**Note:** The ID number in the result folder(3) should match the imported sequencing file ID(2).  
*Ensure there is no Chinese catalog folder.

![](images/trim_details.jpg)




**6.** **No results for PPD?**


Ensure the selection of three or more species for batch extraction. Ensure there is no Chinese catalog folder.




**7.** **No results for Mitochondrial Genome?**


For mitochondrial genome assembly, it's recommended to utilize the full read length of the sequencing file. Uncheck the [Reads/File(M)] checkbox at [Analysis > Filter] to analyze the full read length data.


**7.** **No results for Mitochondrial Genome?**


# Contact
If you have any questions, suggestions, or comments about GeneMiner, feel free to contact Xinyi_Yu2021@163.com.




