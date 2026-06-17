#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."
TENSORBOARD_BIN="${TENSORBOARD_BIN:-/opt/anaconda3/envs/mlagents/bin/tensorboard}"
"$TENSORBOARD_BIN" --logdir results --port 6006
