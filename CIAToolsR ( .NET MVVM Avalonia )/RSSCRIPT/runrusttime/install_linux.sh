#!/usr/bin/env bash
set -euo pipefail

cargo build --release --bins
cp target/release/import ../import
cp target/release/compile ../compile
cp target/release/delete ../delete
chmod +x ../import ../compile ../delete

echo "Linux executables installed in RSSCRIPT/"
