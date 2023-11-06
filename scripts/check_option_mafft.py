import os
import re
import argparse
import subprocess
import shutil

def count_sequence_for_gaps(input_path):
    with open(input_path, 'r') as file:
        content = file.read().split('>')[1:]  # 分割每个序列

    sequences = [(seq.partition('\n')[0], seq.partition('\n')[2].replace('\n', '')) for seq in content]  # 提取序列名称和序列部分
    name1, seq1 = sequences[0]
    name2, seq2 = sequences[1]
    gap_length1, gap_length2 = 0, 0
    gap_diff1, gap_diff2 = 1e9, 1e9
    # 检查第一条序列的开头是否有gap
    if seq1.startswith('-'):
        gap_length1 = len(seq1) - len(seq1.lstrip('-')) + len(seq2) - len(seq2.rstrip('-'))
        gap_diff1 = abs(len(seq1) - len(seq1.lstrip('-')) - (len(seq2) - len(seq2.rstrip('-'))))
    # 检查第一条序列的末尾是否有gap
    if seq1.endswith('-'):
        gap_length2 = len(seq1) - len(seq1.rstrip('-'))  + len(seq2) - len(seq2.lstrip('-'))
        gap_diff2 = abs(len(seq1) - len(seq1.rstrip('-'))  - (len(seq2) - len(seq2.lstrip('-'))))

    return max(gap_length1, gap_length2), min(gap_diff1, gap_diff2) + 1

def main():
    parser = argparse.ArgumentParser(description="基于mafft筛选最佳序列")
    parser.add_argument("-i", "--input", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\temp\NOVOPlasty", help="options存放的位置")
    parser.add_argument("-r", "--ref", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\temp\Arabidopsis_thaliana#NC_037304.fasta", help="参考序列的路径")
    parser.add_argument("-o", "--output", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\temp\NOVOPlasty\Option_best.fasta", help="结果文件的路径")

    args = parser.parse_args()

    
    folder_path = args.input
    pattern = re.compile(r'^Option.*\.fasta$')
    fasta_files = [file for file in os.listdir(folder_path) if pattern.match(file)]
    best_option = ""
    best_gap = (0,1e9)
    for input_file1 in fasta_files:
        input_file1 = os.path.join(folder_path,input_file1)
        input_file2 = args.ref  # 第二个FASTA文件
        output_file = "tmp_input.fasta"  # 合并后的FASTA文件
        with open(input_file1, "r") as file1:
            sequences_file1 = file1.readlines()
        with open(input_file2, "r") as file2:
            sequences_file2 = file2.readlines()
        merged_sequences = sequences_file1 + sequences_file2
        with open(output_file, "w") as output:
            output.writelines(merged_sequences)
        mafft_output = "tmp_output.fasta"
        # 构建命令字符串
        command = '"'+ r"..\analysis\mafft-win\mafft.bat" + '" ' + "--retree 1 --inputorder "  + f'"{output_file}"' + f' > "{mafft_output}"'
        try:
            subprocess.run(command, shell=True, check=True, timeout=300)
        except subprocess.CalledProcessError as e:
            print("MAFFT failed", e)
        current_count = count_sequence_for_gaps("tmp_output.fasta")
        if current_count[0] / current_count[1] > best_gap[0] / best_gap[1]:
            best_gap = current_count
            best_option = input_file1
    print(best_option)
    shutil.copy(best_option, os.path.join(folder_path,args.output))

if __name__ == "__main__":
    main()