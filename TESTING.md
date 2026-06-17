# Testing Guide

## Unity Functional Tests

Check the following before training:

- The robotic arm resets to a stable neutral pose.
- Each joint moves smoothly with keyboard input.
- The brush tip reaches the virtual canvas.
- The brush leaves a visible line when near the canvas.
- Recording creates ML-Agents `.demo` files.
- `TrajectoryLogger` creates CSV files with brush coordinates.

## Behaviour Cloning Smoke Test

Train Style A first because it is the simplest trajectory.

```bash
mlagents-learn Python/configs/bc_style_a.yaml --run-id=bc_style_a_smoke --force
```

Expected outcome:

- Training starts without configuration errors.
- TensorBoard logs are created.
- An ONNX model is exported when training completes.

## Evaluation Test

Place demonstration CSV files in:

```text
Data/demonstrations/style_a_circle/
```

Place reproduction CSV files in:

```text
Data/reproductions/style_a_circle/
```

Run:

```bash
python Python/analysis/evaluate_trajectories.py
python Python/analysis/compare_styles.py
python Python/analysis/plot_results.py
```

Expected output:

- `Data/evaluation_results/results.csv`
- `Data/evaluation_results/style_summary.csv`
- Graphs in `Data/figures`
