# GeneMiner2: A Comprehensive Toolkit for Phylogenomic Genomics
**[查看中文版的使用说明](README_zh_cn.md)**

# Introduction
 ![](images/main_page.jpg)

GeneMiner2 is a comprehensive toolkit designed for phylogenomic genomics. Its main functionalities include:
- Mining single-copy nuclear genes, plastid genes/genomes, and other molecular markers from next-generation sequencing data.
- Aligning and trimming multiple molecular markers, constructing concatenated and coalescent-based phylogenetic trees, and calibrating tree time.
- Identifying paralogs/multicopy genes.

Users can complete all tasks from obtaining NGS data to establishing phylogenetic trees within GeneMiner2.
# Citations
GeneMiner2 is based on our previously developed GeneMiner and Easy353 software and integrates excellent tools such as Blast, Minimap2, Fasttree, Muscle5, Mafft, Astral, PDD, PGA, NOVOPlasty, and OrthoFinder. Please cite the literature prompted by the software when using the corresponding functionalities.

The work on GeneMiner2 has not yet been published. Currently, please cite our paper on GeneMiner:

- Pulin Xie, Yongling Guo, Yue Teng, Wenbin Zhou, Yan Yu. 2024. GeneMiner: a tool for extracting phylogenetic markers from next-generation sequencing data. Molecular Ecology Resources. DOI: 10.1111/1755-0998.13924

If mining angiosperm 353 genes, please cite our paper on Easy353:
- Zhang Z, Xie P, Guo Y, Zhou W, Liu E, Yu Y. 2022. Easy353: A tool to get Angiosperms353 genes for phylogenomic research. Molecular Biology and Evolution 39(12): msac261.


# Install
**You could download GeneMiner from [SourceForge](https://sourceforge.net/projects/geneminer/files/) or [our site](http://life-bioinfo.tpddns.cn:8445/database/app/GeneMiner/).**

If you require the command-line version of gene mining tools, please visit:
- GeneMiner: https://github.com/sculab/GeneMiner
- Easy353: https://github.com/plant720/Easy353

You can also use the Python scripts in the scripts folder of GeneMiner2. These scripts offer all the core functionalities of GeneMiner2 and can be deployed on Windows, macOS, or Linux.

**For macOS**: GeneMiner is packaged with Wineskin to run on macOS. Please try to use the latest version of macOS. Due to limited conditions, GeneMiner has not been tested on all macOS versions. If you meet error like "XXX is damaged" You can type this in terminal:
- xattr -cr /location_of_GeneMinerO.app
- Example: xattr -cr /Applications/GeneMiner.app

Due to GeneMiner2 operating on macOS through Wine technology, its running efficiency is significantly lower compared to Windows. Therefore, it is not recommended to use GeneMiner2 on macOS for large-scale analyses.

**Note:** Avoid installing the software on a portable drive.

---


# Tutorials

[Tutorial 1 - A Quick Tutorial For General Use](/DEMO/DEMO1/DEMO1.md)


[Tutorial 2 - For Obtaining Organelle Genes (Genome)](/DEMO/DEMO2/DEMO2.md)

  
[Tutorial 3 - For Obtaining Single Copy Genes](DEMO/DEMO3/DEMO3.md)


[Tutorial 4 - For Obtaining Angiosperms353 Genes](DEMO/DEMO4/DEMO4.md)


# Command Line (cmd)

For users interested in the command line version, please refer to our [Easy353](https://github.com/plant720/Easy353) or [GeneMiner](https://github.com/sculab/GeneMiner)


# Questions
[Answer](FAQ.md)


# Contact
If you have any questions, suggestions, or comments about GeneMiner, feel free to contact Xinyi_Yu2021@163.com.




