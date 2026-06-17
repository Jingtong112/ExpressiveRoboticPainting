#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."
MLAGENTS_BIN="${MLAGENTS_BIN:-/opt/anaconda3/envs/mlagents/bin/mlagents-learn}"

"$MLAGENTS_BIN" Python/configs/bc_smoke.yaml \
  --run-id=bc_smoke_style_a \
  --force \
  --results-dir results \
  --time-scale 10
