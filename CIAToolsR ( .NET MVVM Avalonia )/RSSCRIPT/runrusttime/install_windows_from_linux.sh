#!/usr/bin/env bash
set -euo pipefail

rustup target add x86_64-pc-windows-gnu
cargo build --release --target x86_64-pc-windows-gnu --bins
cp target/x86_64-pc-windows-gnu/release/import.exe ../import.exe
cp target/x86_64-pc-windows-gnu/release/compile.exe ../compile.exe
cp target/x86_64-pc-windows-gnu/release/delete.exe ../delete.exe

echo "Windows executables installed in RSSCRIPT/"
