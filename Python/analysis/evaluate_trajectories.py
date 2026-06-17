from __future__ import annotations

import argparse
from pathlib import Path

import numpy as np
import pandas as pd


STYLE_ORDER = ["SmoothCircle", "SpiralGrowth", "OrganicFloral"]


def load_path(path: Path) -> np.ndarray:
    df = pd.read_csv(path)
    required = ["brush_x", "brush_y", "brush_z"]
    missing = [column for column in required if column not in df.columns]
    if missing:
        raise ValueError(f"{path} is missing columns: {missing}")
    return df[required].to_numpy(dtype=float)


def path_length(points: np.ndarray) -> float:
    if len(points) < 2:
        return 0.0
    return float(np.linalg.norm(np.diff(points, axis=0), axis=1).sum())


def resample(points: np.ndarray, count: int = 100) -> np.ndarray:
    if len(points) == 0:
        return np.empty((0, 3))
    if len(points) == 1:
        return np.repeat(points, count, axis=0)

    distances = np.linalg.norm(np.diff(points, axis=0), axis=1)
    cumulative = np.insert(np.cumsum(distances), 0, 0.0)
    total = cumulative[-1]
    if total == 0:
        return np.repeat(points[:1], count, axis=0)

    targets = np.linspace(0.0, total, count)
    output = np.zeros((count, 3), dtype=float)
    for dim in range(3):
        output[:, dim] = np.interp(targets, cumulative, points[:, dim])
    return output


def mean_trajectory_error(reference: np.ndarray, reproduction: np.ndarray) -> float:
    ref = resample(reference)
    rep = resample(reproduction)
    return float(np.linalg.norm(ref - rep, axis=1).mean())


def direction_change(points: np.ndarray) -> float:
    if len(points) < 3:
        return 0.0
    vectors = np.diff(points, axis=0)
    norms = np.linalg.norm(vectors, axis=1, keepdims=True)
    vectors = vectors / np.maximum(norms, 1e-9)
    dots = np.sum(vectors[:-1] * vectors[1:], axis=1)
    angles = np.degrees(np.arccos(np.clip(dots, -1.0, 1.0)))
    return float(np.mean(angles))


def evaluate_pair(style: str, demo_path: Path, reproduction_path: Path, canvas_diagonal: float) -> dict:
    demo = load_path(demo_path)
    reproduction = load_path(reproduction_path)
    error = mean_trajectory_error(demo, reproduction)
    similarity = max(0.0, 100.0 * (1.0 - error / canvas_diagonal))
    return {
        "style": style,
        "demonstration": demo_path.name,
        "reproduction": reproduction_path.name,
        "mean_trajectory_error": error,
        "path_similarity": similarity,
        "reproduction_accuracy": similarity,
        "completion_success": int(error <= 0.08),
        "demo_path_length": path_length(demo),
        "demo_direction_change": direction_change(demo),
    }


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--demonstrations", type=Path, default=Path("Data/demonstrations"))
    parser.add_argument("--reproductions", type=Path, default=Path("Data/reproductions"))
    parser.add_argument("--output", type=Path, default=Path("Data/evaluation_results/results.csv"))
    parser.add_argument("--canvas-diagonal", type=float, default=1.4)
    args = parser.parse_args()

    rows = []
    for style_dir in args.demonstrations.glob("*"):
        if not style_dir.is_dir():
            continue
        reproduction_dir = args.reproductions / style_dir.name
        if not reproduction_dir.exists():
            continue
        demo_files = sorted(style_dir.glob("*.csv"))
        reproduction_files = sorted(reproduction_dir.glob("*.csv"))
        for demo_path, reproduction_path in zip(demo_files, reproduction_files):
            rows.append(evaluate_pair(style_dir.name, demo_path, reproduction_path, args.canvas_diagonal))

    args.output.parent.mkdir(parents=True, exist_ok=True)
    pd.DataFrame(rows).to_csv(args.output, index=False)
    print(f"Saved {len(rows)} evaluation rows to {args.output}")


if __name__ == "__main__":
    main()
