import os
import csv
import argparse

def count_files(directory, extensions):
    """
    Count the number of files with specified extensions in a directory.
    :param directory: Path of the directory to search in.
    :param extensions: Tuple of file extensions to look for.
    :return: Number of files with specified extensions.
    """
    return sum(1 for item in os.listdir(directory) if os.path.isfile(os.path.join(directory, item)) and item.endswith(extensions))

def generate_stats_table(base_directory, output_filename):
    """
    Generate a statistics table for specific subdirectories within each child directory of the base directory and save it as a CSV file.
    :param base_directory: The base directory containing child directories to analyze.
    :param output_filename: The filename of the output CSV file.
    """
    if not os.path.isdir(base_directory):
        return "The provided directory does not exist."

    subdirs_to_check = [ 'filtered', 'results', 'consensus', 'paralogs']
    file_extensions = ('.fq', '.fasta')

    stats_table = []

    for child_dir in os.listdir(base_directory):
        child_dir_path = os.path.join(base_directory, child_dir)
        if os.path.isdir(child_dir_path):
            # Check if the child directory contains at least one directory from subdirs_to_check
            contains_subdir = any(os.path.isdir(os.path.join(child_dir_path, subdir)) for subdir in subdirs_to_check)
            if contains_subdir:
                row = [child_dir]
                for subdir in subdirs_to_check:
                    subdir_path = os.path.join(child_dir_path, subdir)
                    if os.path.isdir(subdir_path):
                        row.append(count_files(subdir_path, file_extensions))
                    else:
                        row.append(0)
                stats_table.append(row)

    # Save to CSV file
    summary_file_path = os.path.join(base_directory, output_filename)
    with open(summary_file_path, 'w', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(['Subdirectory'] + subdirs_to_check)
        writer.writerows(stats_table)

    return f"Summary saved to {summary_file_path}"

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Generate statistics table for directories.")
    parser.add_argument("-b", "--base_directory", type=str, default=r"D:\working\Develop\EasyMiner Develop\EasyMiner\bin\Debug\net6.0-windows\results", help="The base directory containing child directories to analyze.")
    parser.add_argument("-o", "--output_filename", type=str, default="summary.csv", help="The filename of the output CSV file.")
    args = parser.parse_args()

    result = generate_stats_table(args.base_directory, args.output_filename)
    print(result)
