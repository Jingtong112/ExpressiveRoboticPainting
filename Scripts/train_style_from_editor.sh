#!/usr/bin/env bash
set -euo pipefail

if [ "$#" -ne 1 ]; then
  echo "Usage: Scripts/train_style_from_editor.sh a|b|c|combined"
  exit 1
fi

cd "$(dirname "$0")/.."
MLAGENTS_BIN="${MLAGENTS_BIN:-/opt/anaconda3/envs/mlagents/bin/mlagents-learn}"

case "$1" in
  a)
    CONFIG="Python/configs/bc_style_a.yaml"
    RUN_ID="bc_style_a"
    ;;
  b)
    CONFIG="Python/configs/bc_style_b.yaml"
    RUN_ID="bc_style_b"
    ;;
  c)
    CONFIG="Python/configs/bc_style_c.yaml"
    RUN_ID="bc_style_c"
    ;;
  combined)
    CONFIG="Python/configs/bc_combined.yaml"
    RUN_ID="bc_combined"
    ;;
  *)
    echo "Unknown style: $1"
    exit 1
    ;;
esac

"$MLAGENTS_BIN" "$CONFIG" \
  --run-id="$RUN_ID" \
  --force \
  --results-dir results \
  --time-scale 10
