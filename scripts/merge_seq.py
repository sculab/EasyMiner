import os
from Bio import SeqIO
import argparse

def merge_sequences(input_folder, output_file, exts, missingchar):
    species_sequences = {}
    sequence_index = 0
    total_count = 0
    # Traverse through all FASTA files in the folder
    for filename in os.listdir(input_folder):
        if os.path.splitext(filename)[-1].lower() in exts.split(","):
            total_count += 1
    sequence_length = [0] * total_count
    for filename in os.listdir(input_folder):
        if os.path.splitext(filename)[-1].lower() in exts.split(","):
            filepath = os.path.join(input_folder, filename)
            for record in SeqIO.parse(filepath, "fasta"):
                sequence = str(record.seq)
                species = record.name
                if species not in species_sequences:
                    species_sequences[species] = [""] * total_count
                species_sequences[species][sequence_index] = sequence
                sequence_length[sequence_index] = max(sequence_length[sequence_index], len(sequence))
            sequence_index += 1

    # Fill in missing sequences
    for species, sequences in species_sequences.items():
        for i in range(len(sequences)):
            if sequences[i] == "":
                sequences[i] = missingchar * sequence_length[i]

    # Write merged sequences to a new FASTA file
    with open(output_file, "w") as output_handle:
        for species, sequences in species_sequences.items():
            output_handle.write(f">{species}\n{''.join(sequences)}\n")
    if len(species_sequences) > 0:
        with open(create_new_filename(output_file), 'w') as partition_file:
            partition_file.write('#nexus\nbegin sets;\n')
            first_value = next(iter(species_sequences.values()))
            start_pos, end_pos = 0, 0
            for i in range(len(first_value)):
                start_pos = end_pos + 1
                end_pos += len(first_value[i])
                if end_pos-start_pos>=0:
                    partition_file.write(f'charset part{i+1}= {start_pos} - {end_pos};\n')
            partition_file.write('end;')
            



def create_new_filename(original_file_path, new_suffix="_partition.txt"):
    # 分割文件名和扩展名
    file_name_without_extension, _ = os.path.splitext(original_file_path)

    # 创建新文件名
    new_file_name = file_name_without_extension + new_suffix

    return new_file_name
if __name__ == "__main__":
    pars = argparse.ArgumentParser(formatter_class=argparse.RawDescriptionHelpFormatter, description=''' 按照序列名合并多个fasta文件中的所有序列 ''')
    pars.add_argument('-input', metavar='<str>', type=str, help='''input folder.''', required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results\combined_trimed")
    pars.add_argument('-output', metavar='<str>', type=str, help='''output folder''', required=False, default='merge.fasta')
    pars.add_argument('-exts', metavar='<str>', type=str, help='''file extensions''', required=False, default='.fasta,.fas,.fa')
    pars.add_argument('-missing', metavar='<str>', type=str, help='''character to fill missing sequences''', required=False, default='N')
    args = pars.parse_args()
    input_folder = args.input  # Replace with the folder path containing FASTA files
    output_file = args.output
    exts = args.exts
    missingchar = args.missing
    merge_sequences(input_folder, output_file, exts, missingchar)
    print("Merging completed.")
