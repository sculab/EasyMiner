# Directory Annotation

## Output Files

**contigs_all**: All possible assembly results.

**filtered**: The fq files obtained after filtering.

**iteration**: Files obtained from the first or multiple iterations, with filenames and meanings consistent with the parent folder.

**large_files**: Original fq files that exceed the depth or file size limits during further filtering. If all filtering results are within limits, this folder will not appear.

**log.txt**: Log file.

#### **results: The sequence with the highest weight in the assembly results, i.e., the final result.**

**kmer_dict_k31.dict**: kmer dictionary file, formatted as: kmer fragment (in hexadecimal), kmer count (in hexadecimal).

**result_dict.txt**: Results file, formatted as: gene name, sequence assembly status, number of reads assembled.

**ref_reads_count_dict.txt**: Total number of kmers split from each reference gene sequence.

**result_dict.txt**: Results file, formatted as: gene name, sequence assembly status, number of reads assembled.

#### Organelle: Assembly results of the organelle genomes.

* **Gennome_cp.fasta**: Plant chloroplast genome assembly result.

* **Gennome_cp.gb**: Annotated plant chloroplast genome assembly result.

* **Gennome_mito_plant.fasta**: Plant mitochondrial genome assembly result.

* **temp**: Files from the organelle genome assembly process that were not completed due to terminal closure.

* **Gennome_mito.fasta**: Animal mitochondrial genome assembly result.



#### Generate Consensus  :

* **consensus**: Maps the result sequences to the filtered fq files. Sequences with a number of ambiguous bases above the set value will be retained.

* **supercontigs**: Consensus reconstruction result files, generating degenerate sequences using IUPAC codes, with degenerate bases marking SNP sites.

#### Multi Copy Detection :

* **paralogs**: Result files from paralogous gene screening. The _ref.fasta file stores paralogous genes, the csv file records the number of times different base positions are mapped, and the .pec.csv file records the frequency of base mutations.

**summary.csv**: Compilation of statistical results.

#### Batch- Analysis :

**Your Sequencing Filename**: A folder named after the sequencing sequence, storing the assembly results obtained from each sequencing sequence separately.

**combined_results**: Stores the combined result files.

**combined_trimmed**: Stores the result files after merging and trimming.

**combined_results.fasta**: Concatenated result file.

**combined_trimmed.fasta**: Trimmed concatenated result file.

**aligned**: Results of multiple sequence alignment.

#### Paralogous Detection (PPD) :

**PPD>result>supercontig>s8 _rm_paralogs> Final_kept_genes**: This is the final result file, for specific meanings of other files see [PPD GitHub](https://github.com/Bean061/putative_paralog#part2-trims-alignment-and-detectes-the-putative-paralogs).


 ---

# Software Annotion

## References Column Descriptions:

- **Select**: Whether to use this reference sequence.

- **ID**: Identifier number of the reference gene.

- **Name**: Name of the reference gene.

- **Ref. Count**: Number of times the reference gene appears.

- **Ref. Length**: Average length of the reference gene (in base pairs).

- **Filter Depth**: Depth of sequencing reads filtered using the reference gene. Calculated as:
  - Filter Depth = (Reads sequencing length * Number of filtered reads) / Average length of reference sequences.

- **Assemble State**: Status of sequence assembly, which includes:
  - **no reads**: No reads filtered; consider lowering the filter K-value or providing a closer reference sequence.
  - **distant references**: Reference sequence too distant; provide a closer reference sequence.
  - **insufficient reads**: Too few reads filtered; consider lowering the filter K-value or providing a closer reference sequence.
  - **no seed**: Unable to find a suitable seed; consider lowering the assembly K-value or providing a closer reference sequence.
  - **no contigs**: No assembly results.
  - **low quality**: Low accuracy of results, reads insufficient to cover assembled results.
  - **success**: Assembly successful.

- **Ass. Length**: Length of the assembly result.

- **Ass. Depth**: Depth of reads coverage of the assembly result, calculated as:
  - Ass. Depth = (Reads sequencing length * Number of reads used for assembly) / Length of assembly result.

- **Max.Diff.**: Maximum divergence obtained from pairwise comparison of genes from different species after [Merge & Trim]. Right-click and select Max. Diff. to filter out low-quality result sequences based on maximum divergence, with a default filter set at 0.1. Once checked, selected sequences can be re-merged and trimmed for phylogenetic tree construction using the filtered and trimmed results.



## Sequences Column Descriptions:

- **Select**: Whether to use this set of data files.

- **Data1**: Left end (end 1) of the sequencing file.

- **Data2**: Right end (end 2) of the sequencing file; if it is single-end sequencing, it automatically matches the content in Data1.

**Note**: The batch functionality is designed to analyze sequencing sequences from different species.