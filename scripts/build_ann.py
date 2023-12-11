import argparse
import subprocess
from Bio import SeqIO
from Bio.Seq import Seq
from Bio.SeqRecord import SeqRecord
from Bio.SeqFeature import SeqFeature, FeatureLocation, CompoundLocation

def Write_Print(log_path, *log_str, sep = " "):
    line = sep.join(map(str,log_str)).strip()
    with open(log_path, 'a') as out:
        out.write(line)
        out.write('\n')
    print(line)

def update_annotations(mafft_output, ref_gb, output_file):
    # 从mafft_output中读取序列
    alignments = list(SeqIO.parse(mafft_output, "fasta"))
    seqA_with_gaps, seqB_with_gaps = alignments[0].seq, alignments[1].seq

    # 从ref_gb读取序列A的注释和不带gap的序列
    ref_record = SeqIO.read(ref_gb, "genbank")

    # 确保molecule_type存在
    molecule_type = ref_record.annotations.get("molecule_type", "DNA")

    # 删除序列B中的gap
    seqB_no_gaps = seqB_with_gaps.replace("-","")

    map_ab = {}
    gap_count_A, gap_count_B = 0, 0
    for i, (base_a, base_b) in enumerate(zip(seqA_with_gaps, seqB_with_gaps)):
        
        if base_a != '-':
            map_ab[i - gap_count_A + 1] = i - gap_count_B + 1
        if base_a == '-':
            gap_count_A += 1
        if base_b == '-':
            gap_count_B += 1


    updated_features = []
    for feature in ref_record.features:
        if 'translation' in feature.qualifiers:
            del feature.qualifiers['translation']
        if feature.type.lower() == 'source':
            new_start, new_end = map_ab[feature.location.start+1]-1, map_ab[feature.location.end] - 1
            feature.location = FeatureLocation(new_start, new_end, strand=feature.strand)
            if 'db_xref' in feature.qualifiers:
                del feature.qualifiers['db_xref']
            if 'specimen_voucher' in feature.qualifiers:
                feature.qualifiers['specimen_voucher'] = ['unknown']
            if 'country' in feature.qualifiers:
                del feature.qualifiers['country']
            if 'collection_date' in feature.qualifiers:
                del feature.qualifiers['collection_date']
            if 'organism' in feature.qualifiers:
                feature.qualifiers['organism'] = ['unknown']
            updated_features.append(feature)
            continue
        if len(feature.location.parts) > 1:
            exons = []
            total_len = 0
            for part in feature.location.parts:
                new_start, new_end = map_ab[part.start+1]-1, map_ab[part.end]
                exons.append(FeatureLocation(start=new_start, end=new_end, strand=part.strand))
                total_len += new_end - new_start
            if feature.type.lower() == 'cds':
                if total_len%3 != 0:
                    Write_Print(output_file + "_log.txt", "Please check", feature.qualifiers)
            feature.location = CompoundLocation(exons)
            updated_features.append(feature)
        else:
            # 映射特征位置
            new_start, new_end = map_ab[feature.location.start+1]-1, map_ab[feature.location.end]
            if new_start is not None and new_end is not None:
                feature.location = FeatureLocation(new_start, new_end, strand=feature.strand)
                updated_features.append(feature)
                if feature.type.lower() == 'cds':
                    if (new_end - new_start)%3 != 0:
                        Write_Print(output_file + "_log.txt", "Please check", feature.qualifiers)

    # 保存为新的GenBank文件
    new_record = SeqRecord(seqB_no_gaps, id="XX000000", name="XX000000", description="XX000000, complete genome")
    new_record.annotations["molecule_type"] = molecule_type
    new_record.features = updated_features
    SeqIO.write(new_record, output_file + '.gb', "genbank")
    SeqIO.write(new_record, output_file + '.fasta', "fasta-2line")


def adjust_sequence_for_gaps(input_path, output_path):
    try:
        with open(input_path, 'r') as file:
            content = file.read().split('>')[1:]  # 分割每个序列

        sequences = [(seq.partition('\n')[0], seq.partition('\n')[2].replace('\n', '')) for seq in content]  # 提取序列名称和序列部分

        if len(sequences) != 2:
            raise ValueError("FASTA文件应该包含两条序列。")

        name1, seq1 = sequences[0]
        name2, seq2 = sequences[1]

        # 检查第一条序列的开头是否有gap
        if seq1.startswith('-'):
            gap_length = len(seq1) - len(seq1.lstrip('-'))
            seq2 = seq2[gap_length:].rstrip('-') + seq2[:gap_length]
            seq1 = seq1.lstrip('-')

        # 检查第一条序列的末尾是否有gap
        if seq1.endswith('-'):
            gap_length = len(seq1) - len(seq1.rstrip('-'))
            seq2 = seq2[-gap_length:] + seq2[:-gap_length].lstrip('-')
            seq1 = seq1.rstrip('-')

        # 保存修改后的序列
        modified_content = f">{name1}\n{seq1}\n>{name2}\n{seq2}\n"
        with open(output_path, 'w') as file:
            file.write(modified_content)

        print("FASTA文件处理成功！")
    except Exception as e:
        print(f"发生错误：{e}")

def main():
    parser = argparse.ArgumentParser(description="调整FASTA文件中的序列，根据第一条序列的gap来调整第二条序列。")
    parser.add_argument("-i1", "--input1", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results\4_Bupleurum_commelynoideum_HQ_R\NOVOPlasty\Bupleurum_scorzonerifolium#MT534601.fasta", help="输入FASTA文件的路径")
    parser.add_argument("-i2", "--input2", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results\4_Bupleurum_commelynoideum_HQ_R\NOVOPlasty\Circularized_assembly_1_Project1.fasta", help="输入FASTA文件的路径")
    parser.add_argument("-gb", "--genbank", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results\4_Bupleurum_commelynoideum_HQ_R\NOVOPlasty\ref_gb.gb", help="GB文件的路径")
    parser.add_argument("-o", "--output", required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results\4_Bupleurum_commelynoideum_HQ_R\NOVOPlasty\output", help="结果文件的路径")

    args = parser.parse_args()

    # 定义输入文件名
    input_file1 = args.input1  # 第一个FASTA文件
    input_file2 = args.input2  # 第二个FASTA文件
    output_file = "cpg_input.fasta"  # 合并后的FASTA文件
    with open(input_file1, "r") as file1:
        sequences_file1 = file1.readlines()
    with open(input_file2, "r") as file2:
        sequences_file2 = file2.readlines()
    merged_sequences = sequences_file1 + sequences_file2
    with open(output_file, "w") as output:
        output.writelines(merged_sequences)

    mafft_output = "cpg_output.fasta"
    # 构建命令字符串
    command = '"'+ r"..\analysis\mafft-win\mafft.bat" + '" ' + "--retree 1 --inputorder "  + f'"{output_file}"' + f' > "{mafft_output}"'
    try:
        subprocess.run(command, shell=True, check=True, timeout=300)
    except subprocess.CalledProcessError as e:
        print("MAFFT failed", e)
        return -1
    adjust_sequence_for_gaps("cpg_output.fasta", "cpg_input.fasta")
    try:
        subprocess.run(command, shell=True, check=True, timeout=300)
    except subprocess.CalledProcessError as e:
        print("MAFFT failed", e)
        return -1
    update_annotations(mafft_output, args.genbank, args.output)



if __name__ == "__main__":
    main()