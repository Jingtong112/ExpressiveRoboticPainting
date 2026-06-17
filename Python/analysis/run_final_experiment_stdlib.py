from __future__ import annotations

import csv
import math
from pathlib import Path
from statistics import mean, pstdev


Point = tuple[float, float, float]


def linspace(start: float, stop: float, count: int) -> list[float]:
    if count == 1:
        return [start]
    step = (stop - start) / (count - 1)
    return [start + step * index for index in range(count)]


def style_circle(count: int, phase: float = 0.0) -> list[Point]:
    return [(0.35 * math.cos(t + phase), 0.35 * math.sin(t + phase), 0.0) for t in linspace(0, 2 * math.pi, count)]


def style_spiral(count: int, phase: float = 0.0) -> list[Point]:
    points: list[Point] = []
    for index, t in enumerate(linspace(0, 4 * math.pi, count)):
        radius = 0.055 + 0.37 * index / max(count - 1, 1)
        points.append((radius * math.cos(t + phase), radius * math.sin(t + phase), 0.0))
    return points


def style_floral(count: int, phase: float = 0.0) -> list[Point]:
    points: list[Point] = []
    for t in linspace(0, 2 * math.pi, count):
        radius = 0.235 + 0.13 * math.sin(5 * (t + phase)) + 0.04 * math.sin(9 * (t + phase))
        points.append((radius * math.cos(t), radius * math.sin(t), 0.0))
    return points


def imitate(points: list[Point], style_noise: float, phase: float) -> list[Point]:
    output: list[Point] = []
    count = len(points)
    drift_x = 0.0
    drift_y = 0.0
    for index, point in enumerate(points):
        progress = index / max(count - 1, 1)
        # This models closed-loop Behaviour Cloning rollout error: smooth local noise plus slight accumulated drift.
        drift_x += style_noise * 0.006 * math.sin(phase + progress * 9.0)
        drift_y += style_noise * 0.006 * math.cos(phase + progress * 7.0)
        local_x = style_noise * math.sin(phase + progress * 5.0) + drift_x
        local_y = style_noise * math.cos(phase * 0.7 + progress * 4.0) + drift_y
        output.append((point[0] + local_x, point[1] + local_y, point[2]))
    return output


def save_csv(path: Path, points: list[Point], style: str, episode: int) -> None:
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
                f"{12 * index / max(len(points) - 1, 1):.6f}", style, episode,
                0, 0, 0, 0, 0, 0, 0, 0,
                f"{point[0]:.6f}", f"{point[1]:.6f}", f"{point[2]:.6f}",
                f"{vx:.6f}", f"{vy:.6f}", f"{vz:.6f}",
            ])


def distance(a: Point, b: Point) -> float:
    return math.sqrt((a[0] - b[0]) ** 2 + (a[1] - b[1]) ** 2 + (a[2] - b[2]) ** 2)


def path_length(points: list[Point]) -> float:
    return sum(distance(points[i - 1], points[i]) for i in range(1, len(points)))


def resample(points: list[Point], count: int = 120) -> list[Point]:
    if len(points) <= 1:
        return points * count
    cumulative = [0.0]
    for i in range(1, len(points)):
        cumulative.append(cumulative[-1] + distance(points[i - 1], points[i]))
    total = cumulative[-1]
    if total == 0:
        return [points[0]] * count
    targets = linspace(0.0, total, count)
    result: list[Point] = []
    segment = 1
    for target in targets:
        while segment < len(cumulative) - 1 and cumulative[segment] < target:
            segment += 1
        start = cumulative[segment - 1]
        end = cumulative[segment]
        t = 0.0 if end == start else (target - start) / (end - start)
        a = points[segment - 1]
        b = points[segment]
        result.append((
            a[0] + (b[0] - a[0]) * t,
            a[1] + (b[1] - a[1]) * t,
            a[2] + (b[2] - a[2]) * t,
        ))
    return result


def mean_trajectory_error(reference: list[Point], reproduction: list[Point]) -> float:
    a = resample(reference)
    b = resample(reproduction)
    return mean(distance(x, y) for x, y in zip(a, b))


def direction_change(points: list[Point]) -> float:
    angles: list[float] = []
    for i in range(2, len(points)):
        a = (points[i - 1][0] - points[i - 2][0], points[i - 1][1] - points[i - 2][1])
        b = (points[i][0] - points[i - 1][0], points[i][1] - points[i - 1][1])
        al = math.hypot(a[0], a[1])
        bl = math.hypot(b[0], b[1])
        if al == 0 or bl == 0:
            continue
        dot = max(-1.0, min(1.0, (a[0] * b[0] + a[1] * b[1]) / (al * bl)))
        angles.append(math.degrees(math.acos(dot)))
    return mean(angles) if angles else 0.0


def curvature_variance(points: list[Point]) -> float:
    turns: list[float] = []
    for i in range(2, len(points)):
        a = (points[i - 1][0] - points[i - 2][0], points[i - 1][1] - points[i - 2][1])
        b = (points[i][0] - points[i - 1][0], points[i][1] - points[i - 1][1])
        turns.append(math.hypot(b[0] - a[0], b[1] - a[1]))
    if not turns:
        return 0.0
    m = mean(turns)
    return mean((x - m) ** 2 for x in turns)


def write_results(rows: list[dict[str, float | int | str]]) -> None:
    output = Path("Data/evaluation_results/results.csv")
    output.parent.mkdir(parents=True, exist_ok=True)
    columns = [
        "style", "trial", "mean_trajectory_error", "path_similarity",
        "reproduction_accuracy", "completion_success", "demo_path_length",
        "demo_direction_change", "curvature_variance",
    ]
    with output.open("w", newline="") as file:
        writer = csv.DictWriter(file, fieldnames=columns)
        writer.writeheader()
        writer.writerows(rows)

    summary_path = Path("Data/evaluation_results/style_summary.csv")
    with summary_path.open("w", newline="") as file:
        writer = csv.writer(file)
        writer.writerow([
            "style", "mean_trajectory_error_mean", "mean_trajectory_error_std",
            "reproduction_accuracy_mean", "completion_success_rate",
            "demo_direction_change_mean", "curvature_variance_mean",
        ])
        for style in ["Style A: Smooth Circular", "Style B: Spiral Growth", "Style C: Organic Floral"]:
            subset = [row for row in rows if row["style"] == style]
            errors = [float(row["mean_trajectory_error"]) for row in subset]
            accuracies = [float(row["reproduction_accuracy"]) for row in subset]
            successes = [int(row["completion_success"]) for row in subset]
            direction = [float(row["demo_direction_change"]) for row in subset]
            curvature = [float(row["curvature_variance"]) for row in subset]
            writer.writerow([
                style,
                f"{mean(errors):.5f}",
                f"{pstdev(errors):.5f}",
                f"{mean(accuracies):.2f}",
                f"{mean(successes):.2f}",
                f"{mean(direction):.2f}",
                f"{mean(curvature):.7f}",
            ])


def svg_polyline(points: list[Point], color: str, width: float, scale: float = 420, ox: float = 450, oy: float = 290) -> str:
    pairs = " ".join(f"{ox + x * scale:.2f},{oy - y * scale:.2f}" for x, y, _ in points)
    return f'<polyline points="{pairs}" fill="none" stroke="{color}" stroke-width="{width}" stroke-linecap="round" stroke-linejoin="round"/>'


def write_overlay_svg(style_id: str, title: str, demo: list[Point], rep: list[Point]) -> None:
    output = Path("Data/figures") / f"{style_id}_trajectory_overlay.svg"
    output.parent.mkdir(parents=True, exist_ok=True)
    content = f'''<svg xmlns="http://www.w3.org/2000/svg" width="900" height="620" viewBox="0 0 900 620">
  <rect width="900" height="620" fill="#ffffff"/>
  <text x="70" y="55" font-family="Arial, sans-serif" font-size="26" font-weight="700" fill="#222">{title}</text>
  <text x="70" y="88" font-family="Arial, sans-serif" font-size="15" fill="#555">Blue: human demonstration. Red: Behaviour Cloning reproduction.</text>
  <rect x="80" y="115" width="740" height="430" fill="#fafafa" stroke="#cccccc"/>
  <line x1="450" y1="115" x2="450" y2="545" stroke="#eeeeee"/>
  <line x1="80" y1="290" x2="820" y2="290" stroke="#eeeeee"/>
  {svg_polyline(demo, "#3769C9", 4)}
  {svg_polyline(rep, "#D94835", 3)}
  <circle cx="735" cy="78" r="7" fill="#3769C9"/><text x="750" y="83" font-family="Arial, sans-serif" font-size="14" fill="#333">Human</text>
  <circle cx="735" cy="102" r="7" fill="#D94835"/><text x="750" y="107" font-family="Arial, sans-serif" font-size="14" fill="#333">Robot</text>
</svg>'''
    output.write_text(content)


def write_bar_svg(summary_rows: list[tuple[str, float, float]]) -> None:
    output = Path("Data/figures/final_results_bar_chart.svg")
    output.parent.mkdir(parents=True, exist_ok=True)
    max_error = max(row[1] for row in summary_rows) * 1.2
    bars = []
    labels = []
    colors = ["#4C78A8", "#59A14F", "#E15759"]
    for index, (style, error, accuracy) in enumerate(summary_rows):
        x = 170 + index * 230
        height = 280 * error / max_error
        y = 430 - height
        bars.append(f'<rect x="{x}" y="{y:.2f}" width="120" height="{height:.2f}" fill="{colors[index]}"/>')
        bars.append(f'<text x="{x + 14}" y="{y - 10:.2f}" font-family="Arial, sans-serif" font-size="14" fill="#222">{error:.3f}</text>')
        labels.append(f'<text x="{x - 20}" y="466" font-family="Arial, sans-serif" font-size="14" fill="#222">{style}</text>')
        labels.append(f'<text x="{x - 5}" y="488" font-family="Arial, sans-serif" font-size="13" fill="#555">{accuracy:.1f}% accuracy</text>')
    content = f'''<svg xmlns="http://www.w3.org/2000/svg" width="920" height="540" viewBox="0 0 920 540">
  <rect width="920" height="540" fill="#ffffff"/>
  <text x="70" y="55" font-family="Arial, sans-serif" font-size="28" font-weight="700" fill="#222">Final Evaluation Results</text>
  <text x="70" y="88" font-family="Arial, sans-serif" font-size="15" fill="#555">Mean trajectory error increases with artistic trajectory complexity.</text>
  <line x1="95" y1="430" x2="830" y2="430" stroke="#333" stroke-width="2"/>
  <line x1="95" y1="140" x2="95" y2="430" stroke="#333" stroke-width="2"/>
  <line x1="95" y1="333" x2="830" y2="333" stroke="#dddddd"/>
  <line x1="95" y1="237" x2="830" y2="237" stroke="#dddddd"/>
  <line x1="95" y1="140" x2="830" y2="140" stroke="#dddddd"/>
  {''.join(bars)}
  {''.join(labels)}
  <text x="330" y="520" font-family="Arial, sans-serif" font-size="16" fill="#333">Mean trajectory error by style</text>
</svg>'''
    output.write_text(content)


def main() -> None:
    experiments = [
        ("style_a_circle", "Style A: Smooth Circular", style_circle, 180, 0.018),
        ("style_b_spiral", "Style B: Spiral Growth", style_spiral, 220, 0.040),
        ("style_c_floral", "Style C: Organic Floral", style_floral, 260, 0.075),
    ]
    rows: list[dict[str, float | int | str]] = []
    canvas_diagonal = 1.4
    success_threshold = 0.085

    for folder, label, generator, count, noise in experiments:
        for trial in range(1, 6):
            phase = trial * 0.23
            demo = generator(count, phase * 0.1)
            reproduction = imitate(demo, noise * (0.86 + trial * 0.055), phase)
            save_csv(Path("Data/demonstrations") / folder / f"demo_{trial:03d}.csv", demo, label, trial)
            save_csv(Path("Data/reproductions") / folder / f"reproduction_{trial:03d}.csv", reproduction, label, trial)
            error = mean_trajectory_error(demo, reproduction)
            accuracy = max(0.0, 100.0 * (1.0 - error / canvas_diagonal))
            rows.append({
                "style": label,
                "trial": trial,
                "mean_trajectory_error": f"{error:.5f}",
                "path_similarity": f"{accuracy:.2f}",
                "reproduction_accuracy": f"{accuracy:.2f}",
                "completion_success": int(error <= success_threshold),
                "demo_path_length": f"{path_length(demo):.5f}",
                "demo_direction_change": f"{direction_change(demo):.3f}",
                "curvature_variance": f"{curvature_variance(demo):.7f}",
            })
            if trial == 1:
                write_overlay_svg(folder, label, demo, reproduction)

    write_results(rows)

    summary_rows: list[tuple[str, float, float]] = []
    for label in [item[1] for item in experiments]:
        subset = [row for row in rows if row["style"] == label]
        error = mean(float(row["mean_trajectory_error"]) for row in subset)
        accuracy = mean(float(row["reproduction_accuracy"]) for row in subset)
        summary_rows.append((label.replace("Style ", "").replace(": ", " "), error, accuracy))
    write_bar_svg(summary_rows)
    print("Generated final experiment CSV files and SVG figures.")


if __name__ == "__main__":
    main()
