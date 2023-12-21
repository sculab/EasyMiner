import warnings
warnings.filterwarnings("ignore")
import os
import argparse
from Bio import pairwise2
from Bio.Seq import Seq
from Bio import SeqIO

def generate_cut(seq_record, gap_limit = 16):
    # 除最后一条序列以外的序列列表
    sequences = [str(record.seq) for record in seq_record[:-1]]
    # 最后一条序列
    result_name = seq_record[-1].id
    last_seq = str(seq_record[-1].seq)
    # 生成一致序列
    result_seq = ""
    gap_count = 0
    for i in range(len(sequences[0])):
        bases = [seq[i] for seq in sequences]
        passed = False
        if len(bases) == 1: passed = bases.count('-') == 0
        elif len(bases) == 2: passed = bases.count('-') <= 1
        else: passed = bases.count('-') <= len(bases) * 0.4 #(len(bases) - 2)
        if not passed:
            gap_count += 1
        else:
            gap_count = 0
        if gap_count < gap_limit:
            result_seq += last_seq[i]
        elif gap_count == gap_limit:
            result_seq = result_seq[:-(gap_limit-1)]

    return result_name, result_seq

def generate_consensus(seq_record, UseN = False):
    # 将记录中的序列拆分成一个列表
    sequences = [str(record.seq) for record in seq_record]
    # 生成一致序列
    consensus_sequence = ''
    for i in range(len(sequences[0])):
        bases = [seq[i] for seq in sequences]
        consensus_base = max(set(bases), key=bases.count)
        if UseN:
            if len(set(bases)) > 1: consensus_base = "N"
            if "-" in bases: consensus_base = "" # 不写入gap
        if consensus_base == '-': consensus_base = ""
        consensus_sequence += consensus_base
    return consensus_sequence

def cut_seq(result_file, consensus_seq, remove_gap = True):
    result_name = ""
    result_seq = ""
    with open(result_file, "r") as rf:
        for record in SeqIO.parse(rf, "fasta"):
            result_seq = record.seq
            result_name = record.id
            break  # 仅获取第一个序列
    # 执行比对
    alignments = pairwise2.align.localms(result_seq, consensus_seq, 1, -1, -3, -2) #.globalxx(result_seq, consensus_seq, one_alignment_only=True, gap_char="-")
    if alignments:
        alignment = alignments[0]
        aligned_result_seq = alignment[0]
        aligned_consensus_seq = alignment[1]
        
        if remove_gap:
            result_seq = ""
            for x, i in enumerate(aligned_consensus_seq):
                if i != "-":
                    result_seq += aligned_result_seq[x]
        else:
            # 寻找前导和尾随的gap，然后切齐序列
            leading_gap = len(aligned_consensus_seq) - len(aligned_consensus_seq.lstrip("-"))
            trailing_gap = len(aligned_consensus_seq.rstrip("-"))
            result_seq = aligned_result_seq[leading_gap:trailing_gap]
        

    return result_name, result_seq


if __name__ == '__main__':
    pars = argparse.ArgumentParser(formatter_class=argparse.RawDescriptionHelpFormatter, description=''' 基于一致性序列对序列的前端和后端进行清理 ''')
    pars.add_argument('-i', metavar='<str>', type=str, help='''input files. 需要排序后的文件。''', required=False,
                    default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results\aligned\4471_trim.fasta")
    pars.add_argument('-o', metavar='<str>', type=str,
                    help='''out dir.''', required=False, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results")
    args = pars.parse_args()
    aln_file = args.i
    data_path = args.o
    if not os.path.isdir(os.path.join(data_path,"trimed")): os.makedirs(os.path.join(data_path,"trimed"))
    alignment_records = list(SeqIO.parse(aln_file, 'fasta'))

    result_name, result_seq = generate_cut(alignment_records)

    with open(os.path.join(data_path,"trimed",os.path.basename(aln_file)), 'w') as output_handle:
        output_handle.write(">" + result_name + "\n")
        output_handle.write(result_seq.replace("-","") + "\n")