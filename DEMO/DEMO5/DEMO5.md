# Tutorial 5 - Construct Phylogenetic Trees

---
## Batch Assembly of 353 Genes


### Data Preparation


- **[Sequencing Data](DATA/PLANT/GENE/)**: Second-generation sequencing data files, in .gz or .fq format.



### Loading the data files

Click [File>Load Sequencing Files] select sequencing data file.


Click [File>Download Reference] download fasta format reference sequences from closely related species. 


The imported files display details such as reference sequence ID, gene name, number of sequences, and average sequence length.


### Obtaining Genes
Click [Batch > Filter & Assemble] to obtain 353 Genes.

**NOTE:** Do not manually close the command line window; it will close automatically once the process is complete.



![](gif/GENOME_GENE2.gif)


Click [Open] to access the  results located in the 'results' folder.

---

## Genomes


### Data Preparation


- **[Sequencing Data](DATA/PLANT/GENOME/)**: Second-generation sequencing data files, in .gz or .fq format.

### Loading the data files


Click [File>Load Sequencing Files] select sequencing data file.


### Obtaining Plant Chloroplast Genome


Click [Analyse>Plant Chloroplast Genome] to download the reference genome of closely related species.

Click [OK] proceed with the default parameters for assembly.


NOTE: For importing multiple pairs of sequencing files, select [Batch > Plant Chloroplast Genome] to extract.


![](gif/GENOME2.gif)


Click [Open] to view the  results located in the 'Organelle' folder named "Gennome_cp.fasta".




### Obtaining Plant Mitochondrial Genome


Follow the same process as the Plant Chloroplast Genome assembly. However, initially **import the results of the previous Plant Chloroplast Genome** assembly.

Click [Analyse>Plant Mitochondrial Genome] to download the reference genome of closely related species.

Click [OK] proceed with the default parameters for assembly.


![](gif/GENOME_MITO.gif)


Click [Open] to view the  results located in the 'Organelle' folder named "Gennome_mito_plant.fasta"




---
