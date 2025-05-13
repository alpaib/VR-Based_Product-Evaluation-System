import os
import pandas as pd
import sys

csv_folder = sys.argv[1]
csv_files = [file for file in os.listdir(csv_folder) if file.endswith("Eye-tracking.csv")]

# UPDATE hz (it must be the same as the value entered in Unity)
hz = 120.0
time_per_sample = 1 / hz

data_dict = {}

if not csv_files:
    print(f"No CSV files were found in {csv_folder}.")
else:
    print(f"The following CSV files were found in {csv_folder}:")
    print(csv_files)

for file in csv_files:
    file_path = os.path.join(csv_folder, file)
    print(f"\nProcessing: {file_path}")
    
    df = pd.read_csv(file_path)
    print(f"Initial DataFrame loaded:\n{df.head()}")

    if 'Volume' in df.columns:
        for i in range(1, len(df)):
            if df.at[i, 'Volume'] == 'Blink':
                df.at[i, 'Volume'] = df.at[i - 1, 'Volume']
                j = i + 1
                while j < len(df) and df.at[j, 'Volume'] == 'Blink':
                    df.at[j, 'Volume'] = df.at[i - 1, 'Volume']
                    j += 1
        
        print(f"DataFrame after Blink processing:\n{df.head()}")

        output_file_path = os.path.join(csv_folder, file.replace('.csv', '-PP.csv'))
        
        df.to_csv(output_file_path, index=False)
        print(f"Processed file saved: {output_file_path}")

        last_value = None
        current_time = 0
        time_data = {}

        for index, row in df.iterrows():
            volume = row['Volume']
            
            if volume != 'Blink':
                if volume != last_value:
                    if last_value is not None:
                        time_data[last_value] = time_data.get(last_value, 0) + current_time * time_per_sample
                        print(f"Accumulated time for '{last_value}': {time_data[last_value]} seconds")
                    current_time = 0
                
                current_time += 1
                last_value = volume
        
        if last_value is not None:
            time_data[last_value] = time_data.get(last_value, 0) + current_time * time_per_sample
            print(f"Final accumulated time for '{last_value}': {time_data[last_value]} seconds")

        print(f"Time data for file {file}: {time_data}")
        data_dict[file] = time_data

final_data = {}

final_data["File"] = list(data_dict.keys())

all_elements = set()
for time_data in data_dict.values():
    all_elements.update(time_data.keys())

all_elements = sorted(all_elements)

for element in all_elements:
    final_data[element] = [
        data_dict[file].get(element, 0)
        for file in data_dict
    ]

final_df = pd.DataFrame(final_data)
final_output_path = os.path.join(csv_folder, "Final-ET-Data.csv")
final_df.to_csv(final_output_path, index=False, sep=";")

print(f"\nFinal summary data:\n{final_df}")
print(f"File 'Final-ET-Data.csv' saved in {final_output_path}")
