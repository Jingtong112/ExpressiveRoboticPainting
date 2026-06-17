from __future__ import annotations

import argparse
from pathlib import Path

import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--results", type=Path, default=Path("Data/evaluation_results/results.csv"))
    parser.add_argument("--output-dir", type=Path, default=Path("Data/figures"))
    args = parser.parse_args()

    df = pd.read_csv(args.results)
    args.output_dir.mkdir(parents=True, exist_ok=True)
    sns.set_theme(style="whitegrid", context="paper")

    for metric, title, ylabel in [
        ("mean_trajectory_error", "Mean Trajectory Error by Style", "Mean error"),
        ("reproduction_accuracy", "Reproduction Accuracy by Style", "Accuracy (%)"),
        ("completion_success", "Completion Success Rate by Style", "Success rate"),
    ]:
        plt.figure(figsize=(7, 4))
        sns.barplot(data=df, x="style", y=metric, errorbar="sd", color="#4C78A8")
        plt.title(title)
        plt.xlabel("Artistic style")
        plt.ylabel(ylabel)
        plt.xticks(rotation=15, ha="right")
        plt.tight_layout()
        out = args.output_dir / f"{metric}.png"
        plt.savefig(out, dpi=200)
        plt.close()
        print(f"Saved {out}")


if __name__ == "__main__":
    main()
