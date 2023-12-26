import os
import re
import argparse
import subprocess
import shutil

def main():
    parser = argparse.ArgumentParser(description="基于Blast筛选Novoplasty产生的几个option中哪个是最佳序列")
    parser.add_argument("-i", "--input", required=False, default=r"..\temp\my_target.fasta", help="选项文件夹的路径")
    parser.add_argument("-r", "--ref", required=False, default=r"..\temp\my_ref.fasta", help="参考序列的路径")
    parser.add_argument("-o", "--output", required=False, default=r"..\temp\trimed.fasta", help="结果文件的路径")
    parser.add_argument("-b", "--output_blast", required=False, default=r"..\temp", help="结果文件的路径")

    args = parser.parse_args()
    subject_file = args.ref
    query_file = args.input
    output_file = args.output
    output_db = args.output_blast + r"\blast_db"  # 指定数据库名称
    output_blast = args.output_blast + r"\blast_output.txt"

    if os.path.exists(output_db + ".nhr"): os.remove(output_db + ".nhr")
    if os.path.exists(output_db + ".nhr"): os.remove(output_db + ".nin")
    if os.path.exists(output_db + ".nhr"): os.remove(output_db + ".nsq")
    if os.path.exists(output_blast): os.remove(output_blast)

    makeblastdb_cmd = [r"..\analysis\makeblastdb.exe", "-in", subject_file, "-dbtype", "nucl", "-out", output_db]
    print(" ".join(makeblastdb_cmd))
    subprocess.run(makeblastdb_cmd, check=True)
    blastn_cmd = [r"..\analysis\blastn.exe", "-query", query_file, "-db", output_db, "-out", output_blast, "-outfmt", "6", "-evalue", "10"]
    subprocess.run(blastn_cmd, check=True)
    if os.path.exists(output_blast):
        fragments = merge_fragments(get_fragments(output_blast))
        with open(query_file, 'r') as file:
            header = next(file)
            sequence = file.read().replace('\n', '')
        if len(fragments) >= 1:
            combined_sequence = extract_and_combine_fragments(sequence, fragments)
            with open(output_file, 'w') as file:
                file.write(header )
                file.write(combined_sequence + '\n')
        else:
            if os.path.exists(output_file):
                shutil.remove(output_file)


def extract_and_combine_fragments(sequence, fragments):
    """
    Extract and combine fragments from a sequence based on given fragment positions.
    """
    combined_sequence = ''
    for start, end in fragments:
        # Extract the fragment (end is inclusive, hence end + 1)
        fragment = sequence[start-1:end]
        combined_sequence += fragment
    return combined_sequence

def merge_fragments(fragments):
    # Sort fragments by their starting positions
    fragments.sort(key=lambda x: x[0])
    merged_fragments = []
    current_fragment = fragments[0]
    for fragment in fragments[1:]:
        # If there is an overlap or the fragments are adjacent, merge them
        if fragment[0] <= current_fragment[1]:
            current_fragment[1] = max(current_fragment[1], fragment[1])
        else:
            # Add the current merged fragment to the list
            merged_fragments.append(current_fragment)
            current_fragment = fragment
    # Add the last merged fragment to the list
    merged_fragments.append(current_fragment)
    return merged_fragments
 

def get_fragments(blast_output_file):
    fragments = []
    with open(blast_output_file, "r") as blast_output:
        lines = blast_output.readlines()
        for line in lines:
            parts = line.split("\t")
            if int(parts[6]) < int(parts[7]):
                fragments.append([int(parts[6]), int(parts[7])])
    return fragments

if __name__ == "__main__":
    main()
