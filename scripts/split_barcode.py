import os
import sys
from sys import platform
import re
import argparse
import subprocess
import shutil
import gzip
from Bio import SeqIO
from multiprocessing import Pool
import multiprocessing

def sort_varieties(variety_dict):
    sorted_varieties = sorted(variety_dict.items(), key=lambda x: x[1], reverse=True)
    return sorted_varieties

def Write_Print(log_path, *log_str, sep = " "):
    """
    记录日志并打印
    """
    line = sep.join(map(str,log_str)).strip()
    with open(log_path, 'a') as out:
        out.write(line)
        out.write('\n')
    print(line)

def split_file(query_file, subject_file, word_size, seq_file_name, out_folder):
    output_db = seq_file_name 
    output_file = seq_file_name + '.tsv'
    makeblastdb_cmd = [r"..\analysis\makeblastdb.exe", "-in", subject_file if subject_file[0] == '"' else '"' + subject_file + '"', "-dbtype", "nucl", "-out", output_db]
    # print(" ".join(makeblastdb_cmd))
    print('Analysing' ,seq_file_name)
    subprocess.run(makeblastdb_cmd, check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    blastn_cmd = [r"..\analysis\blastn.exe", "-query", query_file if query_file[0] == '"' else '"' + query_file + '"', "-db", output_db, "-out", output_file, "-outfmt", "6", "-num_alignments", "1", "-max_hsps", "1", "-evalue", "10", "-word_size", str(word_size), "-num_threads", "8"]
    # print(" ".join(blastn_cmd))
    subprocess.run(" ".join(blastn_cmd), check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

    barcode_dict = {}
    get_barcode_dict(output_file, barcode_dict)
    
    os.remove(output_db + '.nsq')
    os.remove(output_db + '.nhr')
    os.remove(output_db + '.nin')
    os.remove(output_file)

    type_set = set(barcode_dict.values())
    file_dict = {}
    count_dict = {}
    for my_type in type_set:
        file_dict[my_type] = open(os.path.join(out_folder, my_type + "_raw.fasta"), "w")
        count_dict[my_type] = 1
    file_unknown = open(os.path.join(out_folder, "unknown.fasta"), "w")
    unknown_count = 1
    query_file = query_file[1:-1] if query_file[0] == '"' else query_file
    with open(query_file, "r") as fasta_file:
        for record in SeqIO.parse(fasta_file, "fasta"):
            sequence_name = record.id
            sequence_seq = str(record.seq)
            if sequence_name in barcode_dict:
                my_code = barcode_dict[sequence_name]
                sequence_name = sequence_name.replace("@","").replace("\\","_").replace("/","_")
                sequence_entry = f'>{sequence_name}\n{sequence_seq}\n'
                file_dict[my_code].write(sequence_entry)
                count_dict[my_code] += 1
            else:
                sequence_name = sequence_name.replace("@","").replace("\\","_").replace("/","_")
                file_unknown.write(f'>{sequence_name}\n{sequence_seq}\n')
                unknown_count += 1
    for x in file_dict.values(): x.close()
    file_unknown.close()
    barcode_sort = sort_varieties(count_dict)
    max_radio = 0
    max_id = -1
    for i in range(1, len(barcode_sort)):
        if max_radio < barcode_sort[i-1][1]/(barcode_sort[i][1]+16):
            max_radio = barcode_sort[i-1][1]/(barcode_sort[i][1]+16)
            max_id = i
    mix_file = os.path.join(out_folder, "mismatch.fasta")
    for i in range(max_id, len(barcode_sort)):
        raw_file = os.path.join(out_folder, barcode_sort[i][0] + "_raw.fasta")
        if os.path.exists(raw_file):
            with open(raw_file, "r") as query_f, open(mix_file, "a") as mix_f:
                for line in query_f:
                    mix_f.write(line)
            os.remove(raw_file)

    total_count = sum([i[1] for i in barcode_sort]) + unknown_count
    mismatch_count = sum([i[1] for i in barcode_sort[max_id:]])
    match_count = sum([i[1] for i in barcode_sort[:max_id]])
    Write_Print(os.path.join(out_folder, "log.txt"), "Results of", seq_file_name + ":")
    Write_Print(os.path.join(out_folder, "log.txt"), [i for i in barcode_sort[:max_id]])
    Write_Print(os.path.join(out_folder, "log.txt"), 
    'Recovery:', str(round((1-unknown_count/total_count)*100,2)) + "%",
    'Accuracy:', str(round((1-mismatch_count/(match_count+mismatch_count)*(len(barcode_sort)-1)/(len(barcode_sort)-max_id))*100,2)) + "%"
    )


def split_wo_combine(query_file, subject_file, word_size, out_folder):
    seq_file_name = os.path.basename(os.path.splitext(query_file)[0]).replace("|", "_").replace(" ", "_").replace(":", "_")
    os.makedirs(out_folder, exist_ok=True)
    split_file(query_file, subject_file, word_size, seq_file_name, out_folder)

def main():
    parser = argparse.ArgumentParser(description="基于Blast分割barcode")
    parser.add_argument("-i", "--input", required=False, default=r"E:\测试数据\HIVTEST\R003", help="测序数据文件夹的路径")
    parser.add_argument("-r", "--ref", required=False, default=r"E:\测试数据\HIVTEST\barcode.fasta", help="barcode序列的路径")
    parser.add_argument("-o", "--output", required=False, default=r'E:\测试数据\HIV', help="结果文件夹的路径")
    parser.add_argument('-w', "--word_size", required=False, type=int, default = 15, help='''word size''')
    parser.add_argument('-p', "--processes", required=False, type=int, default=8, help='Number of processes')
    args = parser.parse_args()

    subject_file = args.ref 
    word_size =  args.word_size
    if os.path.isdir(args.input):
        files_to_process = []
        for filename in os.listdir(args.input):
            if filename.endswith(".fa") or filename.endswith(".fas") or filename.endswith(".fasta") or filename.endswith(".fq") or filename.endswith(".fastq"):
                query_file = fq_to_fasta(os.path.join(args.input, filename))
                out_folder = os.path.join(args.output, os.path.splitext(filename)[0])
                if os.path.exists(out_folder):
                    shutil.rmtree(out_folder)
                files_to_process.append((query_file, subject_file, word_size, out_folder))
        # 创建进程池，根据需要设置进程数量
        with Pool(args.processes) as pool:
            pool.starmap(split_wo_combine, files_to_process)
        print("Combining results ...")
        combine_folder = os.path.join(args.output, "raw_data")
        if os.path.exists(combine_folder):
            shutil.rmtree(combine_folder)
        os.makedirs(combine_folder, exist_ok=True)
        for filename in os.listdir(args.input):
            if filename.endswith(".fa") or filename.endswith(".fas") or filename.endswith(".fasta") or filename.endswith(".fq") or filename.endswith(".fastq"):
                query_file = fq_to_fasta(os.path.join(args.input, filename))
                seq_file_name = os.path.basename(os.path.splitext(query_file)[0])
                out_folder = os.path.join(args.output, seq_file_name)
                for new_file in os.listdir(out_folder):
                    if new_file.endswith(".txt") or new_file.endswith(".fasta"):
                        combined_file = os.path.join(combine_folder, new_file)
                        with open(os.path.join(out_folder, new_file), "r") as query_f, open(combined_file, "a") as combined_f:
                            for line in query_f:
                                combined_f.write(line)
    else:
        query_file = fq_to_fasta(args.input)
        seq_file_name = os.path.basename(os.path.splitext(query_file)[0])
        out_folder = os.path.join(args.output, seq_file_name)
        if os.path.exists(out_folder):
            shutil.rmtree(out_folder)
        os.makedirs(out_folder, exist_ok=True)
        split_file(query_file, subject_file, word_size, seq_file_name, out_folder)
    
    print("Spliting subreads ...")
    barcode_list = []
    with open(subject_file, "r") as fasta_file:
        for record in SeqIO.parse(fasta_file, "fasta"):
            with open(os.path.join(args.output, record.id + "_barcode.fasta"), "w") as output_fasta:
                SeqIO.write(record, output_fasta, "fasta")
                barcode_list.append(record.id)

    out_folder = os.path.join(args.output, "clean_data") if os.path.isdir(args.input) else os.path.join(args.output, os.path.basename(os.path.splitext(query_file)[0]))
    if os.path.exists(out_folder) and os.path.isdir(args.input):
        shutil.rmtree(out_folder)
    os.makedirs(out_folder, exist_ok=True)

    files_to_process = []
    for barcode in barcode_list:
        query_file = os.path.join(args.output, "raw_data", barcode + "_raw.fasta") if os.path.isdir(args.input) else os.path.join(out_folder, barcode + "_raw.fasta")
        subject_file = os.path.join(args.output, barcode + "_barcode.fasta")
        if os.path.exists(query_file):
            files_to_process.append((query_file, subject_file, word_size, barcode, out_folder))
        else:
            os.remove(subject_file)
    with Pool(args.processes) as pool:
        pool.starmap(split_subreads, files_to_process)
    print("Analysis complete!")

def fq_to_fasta(input_file):
    # 检查文件扩展名
    if input_file.endswith('.fq.gz'):
        open_func = gzip.open
        output_file = './' + input_file.replace("\\","/").split('/')[-1].replace('.fq.gz', '.fasta')
    elif input_file.endswith('.fq'):
        open_func = open
        output_file = './' + input_file.replace("\\","/").split('/')[-1].replace('.fq', '.fasta')
    elif input_file.endswith('.fastq.gz'):
        open_func = gzip.open
        output_file = './' + input_file.replace("\\","/").split('/')[-1].replace('.fastq.gz', '.fasta')
    elif input_file.endswith('.fastq'):
        open_func = open
        output_file = './' + input_file.replace("\\","/").split('/')[-1].replace('.fastq', '.fasta')
    else:
        return input_file

    with open_func(input_file, 'rt') as fq, open(output_file, 'w') as fasta:
        while True:
            header = fq.readline().strip()
            if not header:
                break
            sequence = fq.readline().strip()
            fq.readline()  # + line (ignored)
            fq.readline()  # quality line (ignored)

            fasta_header = '>' + header[1:]  # Convert FASTQ header to FASTA header
            fasta.write(fasta_header + '\n')
            fasta.write(sequence + '\n')
    return output_file


def split_subreads(query_file, subject_file, word_size, seq_file_name, out_folder):
    output_db = seq_file_name 
    output_file = seq_file_name + '.tsv'
    makeblastdb_cmd = [r"..\analysis\makeblastdb.exe", "-in", subject_file if subject_file[0] == '"' else '"' + subject_file + '"', "-dbtype", "nucl", "-out", output_db]
    # print(" ".join(makeblastdb_cmd))
    print('Analysing' ,seq_file_name)
    subprocess.run(makeblastdb_cmd, check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    blastn_cmd = [r"..\analysis\blastn.exe", "-query", query_file if query_file[0] == '"' else '"' + query_file + '"', "-db", output_db, "-out", output_file, "-outfmt", "6", "-evalue", "10", "-word_size", str(word_size), "-num_threads", "8"]
    # print(" ".join(blastn_cmd))
    subprocess.run(" ".join(blastn_cmd), check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    subreads_dict = {}
    get_subreads_dict(output_file, subreads_dict)
    os.remove(subject_file)
    os.remove(output_db + '.nsq')
    os.remove(output_db + '.nhr')
    os.remove(output_db + '.nin')
    os.remove(output_file)
    splited_file = open(os.path.join(out_folder, seq_file_name + "_clean.fasta"), "w")
    query_file = query_file[1:-1] if query_file[0] == '"' else query_file
    with open(query_file, "r") as fasta_file:
        for record in SeqIO.parse(fasta_file, "fasta"):
            sequence_name = record.id
            sequence_seq = str(record.seq)
            if sequence_name in subreads_dict:
                subreads_list = sorted(subreads_dict[sequence_name]) + [len(sequence_seq)]
                for i in range(0, len(subreads_list) - 1, 1):
                    if subreads_list[i+1] - subreads_list[i] > 100:
                        sequence_entry = f'>{sequence_name}_{str(int(i/2))}\n{sequence_seq[subreads_list[i]:subreads_list[i+1]]}\n'
                        splited_file.write(sequence_entry)
            else:
                splited_file.write(record)
    splited_file.close()
def get_subreads_dict(blast_output_file, _subreads_dict):
    with open(blast_output_file, "r") as blast_output:
        lines = blast_output.readlines()
        for line in lines:
            temp_list = line.split("\t")
            if temp_list[0] in _subreads_dict:
                _subreads_dict[temp_list[0]] += [int(temp_list[6]), int(temp_list[7])]
            else:
                _subreads_dict[temp_list[0]] = [0, int(temp_list[6]), int(temp_list[7])]

def get_barcode_dict(blast_output_file, _barcode_dict):
    with open(blast_output_file, "r") as blast_output:
        lines = blast_output.readlines()
        for line in lines:
            _barcode_dict[line.split("\t")[0]] = line.split("\t")[1]

if __name__ == "__main__":
    if sys.platform.startswith('win'):
        multiprocessing.freeze_support()
    main()
