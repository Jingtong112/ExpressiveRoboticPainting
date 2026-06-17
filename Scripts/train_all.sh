#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

MLAGENTS_BIN="${MLAGENTS_BIN:-/opt/anaconda3/envs/mlagents/bin/mlagents-learn}"

"$MLAGENTS_BIN" Python/configs/bc_style_a.yaml --run-id=bc_style_a --force --results-dir results
"$MLAGENTS_BIN" Python/configs/bc_style_b.yaml --run-id=bc_style_b --force --results-dir results
"$MLAGENTS_BIN" Python/configs/bc_style_c.yaml --run-id=bc_style_c --force --results-dir results
"$MLAGENTS_BIN" Python/configs/bc_combined.yaml --run-id=bc_combined --force --results-dir results

echo "Copy exported ONNX models from results/*/RoboticPaintingAgent.onnx into UnityProject/Assets/ML-Agents/Models/."
