# Expressive Robotic Painting Through Imitation Learning

This project explores whether Behaviour Cloning can enable a simulated robotic manipulator to reproduce expressive artistic brush stroke styles from human demonstrations.

## Research Question

Can Behaviour Cloning enable a robotic manipulator to reproduce different expressive artistic motion styles from human demonstrations?

## Hypothesis

Simple motion styles will be reproduced more accurately than complex artistic styles. Trajectory complexity will negatively affect imitation learning performance.

## System Components

- Unity 6 robotic arm simulation
- Virtual canvas and brush end effector
- Manual demonstration recording
- Unity ML-Agents Behaviour Cloning pipeline
- ONNX model export and inference
- Quantitative evaluation scripts
- Visualisation tools for trajectory overlays, side-by-side comparison, and error heatmaps

## Artistic Styles

| Style | Description | Expected Complexity |
|---|---|---:|
| Style A | Layered circular ink style with wider black/grey wash-like brush quality | Low |
| Style B | Layered spiral growth style with strong deep-purple to bright-cyan gradient | Medium |
| Style C | Organic floral / butterfly-inspired expressive style with warm colour variation | High |

## Repository Structure

```text
UnityProject/        Unity 6 project assets and C# scripts
Python/configs/      ML-Agents Behaviour Cloning configuration files
Python/analysis/     Evaluation and plotting scripts
Data/                Demonstrations, reproductions, metrics, and figures
Docs/                ACM-style report material and guides
Scripts/             Training helper commands
```

## Quick Start

1. Open `UnityProject` in Unity 6.
2. Install ML-Agents from the Unity Package Manager.
3. Open `Assets/Scenes/DataCollectionScene.unity`.
4. Press Play to view the robotic painting system.
5. Press `1`, `2`, or `3` during Play Mode to preview the three artistic styles:
   - `1`: Layered circular ink style
   - `2`: Layered spiral growth expressive style
   - `3`: Organic floral / butterfly-inspired expressive style
6. Record demonstrations using ML-Agents Demonstration Recorder and `TrajectoryLogger`.
7. Train with:

```bash
cd ExpressiveRoboticPainting
python -m venv .venv
source .venv/bin/activate
pip install -r Python/requirements.txt
mlagents-learn Python/configs/bc_style_a.yaml --run-id=bc_style_a
```

8. Copy exported `.onnx` models into `UnityProject/Assets/ML-Agents/Models`.
9. Run inference in Unity and export reproduction trajectories.
10. Evaluate with:

```bash
python Python/analysis/evaluate_trajectories.py
python Python/analysis/compare_styles.py
python Python/analysis/plot_results.py
```

## Current Completed Artefacts

- Working Unity 6 robotic painting simulation
- Three artistic movement styles: layered ink circle, layered spiral, and butterfly-inspired organic floral
- Demonstration CSV logs in `Data/demonstrations/`
- ML-Agents `.demo` files in `UnityProject/Assets/ML-Agents/Demonstrations/`
- Trained ONNX models in `UnityProject/Assets/ML-Agents/Models/`
- Behaviour Cloning training outputs in `results/`
- Evaluation CSV files in `Data/evaluation_results/`
- Trajectory overlay and results figures in `Data/figures/`
- ACM-style report material in `Docs/`
- Docker setup and training helper scripts

## Final Evaluation Summary

| Style | Mean Trajectory Error | Reproduction Accuracy | Completion Success Rate |
|---|---:|---:|---:|
| Style A: Smooth Circular | 0.01684 | 98.80% | 1.00 |
| Style B: Spiral Growth | 0.04317 | 96.91% | 1.00 |
| Style C: Organic Floral | 0.08526 | 93.91% | 0.40 |

The results support the hypothesis that simpler motion styles are reproduced more accurately than complex artistic styles.

## Academic Contribution

The project contributes a simulated robotic painting system for studying expressive movement imitation. It compares Behaviour Cloning performance across three artistic trajectory classes and evaluates whether increased trajectory complexity reduces reproduction accuracy.
