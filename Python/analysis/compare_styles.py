from __future__ import annotations

import argparse
from pathlib import Path

import pandas as pd


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--results", type=Path, default=Path("Data/evaluation_results/results.csv"))
    parser.add_argument("--output", type=Path, default=Path("Data/evaluation_results/style_summary.csv"))
    args = parser.parse_args()

    df = pd.read_csv(args.results)
    summary = df.groupby("style", as_index=False).agg(
        mean_trajectory_error_mean=("mean_trajectory_error", "mean"),
        mean_trajectory_error_std=("mean_trajectory_error", "std"),
        reproduction_accuracy_mean=("reproduction_accuracy", "mean"),
        completion_success_rate=("completion_success", "mean"),
        demo_direction_change_mean=("demo_direction_change", "mean"),
        demo_path_length_mean=("demo_path_length", "mean"),
    )
    args.output.parent.mkdir(parents=True, exist_ok=True)
    summary.to_csv(args.output, index=False)
    print(summary.to_string(index=False))


if __name__ == "__main__":
    main()
