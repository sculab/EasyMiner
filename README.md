
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
You could download from [SourceForge](https://sourceforge.net/projects/geneminer/files/) or [our site](http://life-bioinfo.tpddns.cn:8445/database/app/GeneMiner/).

**Note: Avoid installing the software on a portable hard drive.**

**For macOS**: MATO is packaged with Wineskin to run on macOS. Please try to use the latest version of MacOS. Due to limited conditions, MATO has not been tested on all macOS versions. If you meet error like "XXX is damaged" You can type this in terminal:
- xattr -cr /location_of_MATO.app
- Example: xattr -cr /Applications/MATO.app

# Tutorials

[Tutorial 1 - A Quick Tutorial For General Use](/DEMO/DEMO1/DEMO1.md)



[Tutorial 2 - For Obtaining Organelle Genes (Genome)](/DEMO/DEMO2/DEMO2.md)


  
[Tutorial 3 - For Obtaining Single Copy Genes](DEMO/DEMO3/DEMO3.md)




[Tutorial 4 - For Obtaining Angiosperms353 Genes](DEMO/DEMO4/DEMO4.md)


# More Details
For detailed instructions and tutorials, visit [here](manual/ZH_CN/readmeall.md).


# Command Line (cmd)

For users interested in the command line version, please refer to our [Easy353](https://github.com/plant720/Easy353) and [GeneMiner](https://github.com/sculab/GeneMiner)






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



**8.** **Want to Run Multiple GeneMiner Simultaneously?**

**NOTE:**  Do not open and operate multiple GeneMiner windows in the same directory.
To expedite the extraction process, you can run multiple GeneMiner concurrently, provided your computer has sufficient memory. Create a copy of the GeneMiner folder and run the additional instance from the duplicated directory. (Ensure that the folder path does not contain any Chinese characters).


# Contact
If you have any questions, suggestions, or comments about GeneMiner, feel free to contact Xinyi_Yu2021@163.com.




