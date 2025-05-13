import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import os
import sys

csv_folder = sys.argv[1]
csv_files = [file for file in os.listdir(csv_folder) if file.endswith("_Position.csv")]

for csv_filename in csv_files:
    data = pd.read_csv(os.path.join(csv_folder, csv_filename), delimiter=";")

    data["X"] = data["X"].str.replace(",", ".").astype(float)
    data["Y"] = data["Y"].str.replace(",", ".").astype(float)

    scale_factor = 4
    data["X"] = data["X"] * scale_factor
    data["Y"] = data["Y"] * scale_factor

    fig, ax = plt.subplots(figsize=(30, 24))

    image_filename = "SceneSnapshot.png"
    image_path = os.path.join(csv_folder, image_filename)
    environment_img = mpimg.imread(image_path)

    xmin, xmax, ymin, ymax = -20, 20, -20, 20
    ax.imshow(environment_img, extent=[-15, 15, -15, 15], aspect='auto')

    ax.scatter(data["X"], data["Y"], color='white', alpha=0.5, s=15, label='Player trajectory')
    ax.set_title("Player movement")
    ax.set_xlabel("X")
    ax.set_ylabel("Y")
    ax.axis("equal")
    ax.grid(False)

    ax.set_xlim(xmin, xmax)
    ax.set_ylim(ymin, ymax)

    ax.patch.set_facecolor('none')
    fig.patch.set_facecolor('none')

    output_folder = csv_folder
    output_filename = os.path.join(output_folder, os.path.splitext(csv_filename)[0] + "_drawing" + ".png")

    plt.savefig(output_filename, dpi=300, bbox_inches='tight', transparent=True)

# plt.show()
