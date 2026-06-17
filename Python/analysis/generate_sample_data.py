from __future__ import annotations

import csv
import math
from pathlib import Path


def save(path: Path, points: list[tuple[float, float, float]], style: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", newline="") as file:
        writer = csv.writer(file)
        writer.writerow([
            "timestamp", "style", "episode",
            "joint_1_angle", "joint_2_angle", "joint_3_angle", "joint_4_angle",
            "joint_1_velocity", "joint_2_velocity", "joint_3_velocity", "joint_4_velocity",
            "brush_x", "brush_y", "brush_z", "brush_vx", "brush_vy", "brush_vz",
        ])
        previous = points[0]
        for index, point in enumerate(points):
            vx = point[0] - previous[0]
            vy = point[1] - previous[1]
            vz = point[2] - previous[2]
            previous = point
            writer.writerow([
                round(12 * index / max(len(points) - 1, 1), 6), style, 1,
                0, 0, 0, 0, 0, 0, 0, 0,
                round(point[0], 6), round(point[1], 6), round(point[2], 6),
                round(vx, 6), round(vy, 6), round(vz, 6),
            ])


def circle(count: int) -> list[tuple[float, float, float]]:
    return [(0.35 * math.cos(t), 0.35 * math.sin(t), 0.0) for t in linspace(0, 2 * math.pi, count)]


def spiral(count: int) -> list[tuple[float, float, float]]:
    points = []
    for index, t in enumerate(linspace(0, 4 * math.pi, count)):
        radius = 0.05 + (0.42 - 0.05) * index / max(count - 1, 1)
        points.append((radius * math.cos(t), radius * math.sin(t), 0.0))
    return points


def floral(count: int) -> list[tuple[float, float, float]]:
    points = []
    for t in linspace(0, 2 * math.pi, count):
        radius = 0.22 + 0.14 * math.sin(5 * t) + 0.035 * math.sin(9 * t)
        points.append((radius * math.cos(t), radius * math.sin(t), 0.0))
    return points


def perturb(points: list[tuple[float, float, float]], scale: float, phase: float) -> list[tuple[float, float, float]]:
    output = []
    count = len(points)
    for index, point in enumerate(points):
        t = phase * index / max(count - 1, 1)
        output.append((point[0] + scale * math.sin(t), point[1] + scale * math.cos(t * 0.7), point[2]))
    return output


def linspace(start: float, stop: float, count: int) -> list[float]:
    if count == 1:
        return [start]
    step = (stop - start) / (count - 1)
    return [start + step * index for index in range(count)]


def main() -> None:
    styles = {
        "style_a_circle": ("SmoothCircle", circle(180), 0.015, 3.0),
        "style_b_spiral": ("SpiralGrowth", spiral(220), 0.035, 5.0),
        "style_c_floral": ("OrganicFloral", floral(260), 0.065, 9.0),
    }

    for folder, (label, points, noise, phase) in styles.items():
        save(Path("Data/demonstrations") / folder / "demo_001.csv", points, label)
        save(Path("Data/reproductions") / folder / "reproduction_001.csv", perturb(points, noise, phase), label)

    print("Generated sample demonstration and reproduction CSV files.")


if __name__ == "__main__":
    main()
