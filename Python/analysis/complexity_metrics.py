from __future__ import annotations

import argparse
from pathlib import Path

import numpy as np
import pandas as pd


def load_points(path: Path) -> np.ndarray:
    df = pd.read_csv(path)
    return df[["brush_x", "brush_y", "brush_z"]].to_numpy(dtype=float)


def curvature_proxy(points: np.ndarray) -> float:
    if len(points) < 3:
        return 0.0
    vectors = np.diff(points, axis=0)
    norms = np.linalg.norm(vectors, axis=1, keepdims=True)
    unit = vectors / np.maximum(norms, 1e-9)
    turn = np.linalg.norm(np.diff(unit, axis=0), axis=1)
    return float(np.var(turn))


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input", type=Path, default=Path("Data/demonstrations"))
    parser.add_argument("--output", type=Path, default=Path("Data/evaluation_results/complexity.csv"))
    args = parser.parse_args()

    rows = []
    for path in args.input.glob("*/*.csv"):
        points = load_points(path)
        rows.append({
            "style": path.parent.name,
            "file": path.name,
            "num_points": len(points),
            "path_length": float(np.linalg.norm(np.diff(points, axis=0), axis=1).sum()) if len(points) > 1 else 0.0,
            "curvature_variance": curvature_proxy(points),
        })

    args.output.parent.mkdir(parents=True, exist_ok=True)
    pd.DataFrame(rows).to_csv(args.output, index=False)
    print(f"Saved complexity metrics to {args.output}")


if __name__ == "__main__":
    main()
