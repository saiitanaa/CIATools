#!/usr/bin/env bash
set -e

# Se placer dans le répertoire du script
cd "$(dirname "$0")"
WORK_DIR="$(pwd)"

echo "Searching files in: $WORK_DIR"

# ============================
# 1) DETECT ELF OR 3DSX
# ============================

ELF_FILE="$(ls *.elf 2>/dev/null | head -n 1 || true)"
THREEDSX_FILE="$(ls *.3dsx 2>/dev/null | head -n 1 || true)"

if [ -z "$ELF_FILE" ] && [ -z "$THREEDSX_FILE" ]; then
    echo "No .elf or .3dsx file found!"
    exit 1
fi

if [ -n "$ELF_FILE" ]; then
    HOME_NAME="${ELF_FILE%.*}"
else
    HOME_NAME="${THREEDSX_FILE%.*}"

    echo "Found 3DSX: $THREEDSX_FILE"
    echo "Converting .3dsx to .elf..."

    if [ -x "./3dsxtool" ]; then
        ./3dsxtool "$THREEDSX_FILE" "$HOME_NAME.elf"
    elif command -v 3dsxtool >/dev/null 2>&1; then
        3dsxtool "$THREEDSX_FILE" "$HOME_NAME.elf"
    else
        echo "ERROR: 3dsxtool not found!"
        exit 1
    fi

    if [ ! -f "$HOME_NAME.elf" ]; then
        echo "ERROR: Failed to generate ELF!"
        exit 1
    fi

    ELF_FILE="$HOME_NAME.elf"
fi

echo "Using ELF: $ELF_FILE"
echo "Homebrew name: $HOME_NAME"

# ============================
# 2) BANNER / ICON / AUDIO
# ============================

BANNER_FILE="$(find . -maxdepth 1 -type f -iname "*.png" ! -iname "icon*.png" | head -n 1 | sed 's|^\./||')"
ICON_FILE="$(find . -maxdepth 1 -type f -iname "icon*.png" | head -n 1 | sed 's|^\./||')"
AUDIO_FILE="$(find . -maxdepth 1 -type f -iname "*.wav" | head -n 1 | sed 's|^\./||')"

if [ -z "$BANNER_FILE" ]; then
    echo "No banner PNG found!"
    exit 1
fi

if [ -z "$ICON_FILE" ]; then
    echo "No icon PNG found!"
    exit 1
fi

if [ -z "$AUDIO_FILE" ]; then
    echo "No WAV audio found!"
    exit 1
fi

echo "Banner: $BANNER_FILE"
echo "Icon: $ICON_FILE"
echo "Audio: $AUDIO_FILE"

# ============================
# 3) ROMFS
# ============================

if [ ! -d "romfs" ]; then
    mkdir romfs
fi

# ============================
# 4) CREATOR / AUTHOR
# ============================

CREATOR="NameOfTheCreator"

if [ -f "AUTHOR.txt" ]; then
    CREATOR="$(head -n 1 AUTHOR.txt)"
fi

if [ -z "$CREATOR" ]; then
    CREATOR="PleaseDefineCreatorName"
fi

echo "Creator: $CREATOR"

# ============================
# 5) CHECK TOOLS
# ============================

if [ -x "./bannertool" ]; then
    BANNERTOOL="./bannertool"
elif command -v bannertool >/dev/null 2>&1; then
    BANNERTOOL="bannertool"
else
    echo "ERROR: bannertool not found!"
    exit 1
fi

if [ -x "./makerom" ]; then
    MAKEROM="./makerom"
elif command -v makerom >/dev/null 2>&1; then
    MAKEROM="makerom"
else
    echo "ERROR: makerom not found!"
    exit 1
fi

if [ ! -f "homebrew.rsf" ]; then
    echo "ERROR: homebrew.rsf not found!"
    exit 1
fi

# ============================
# 6) BUILD
# ============================

echo "Building $HOME_NAME..."

"$BANNERTOOL" makebanner \
    -i "$BANNER_FILE" \
    -a "$AUDIO_FILE" \
    -o banner.bnr

"$BANNERTOOL" makesmdh \
    -s "$HOME_NAME" \
    -l "$HOME_NAME" \
    -p "$CREATOR" \
    -i "$ICON_FILE" \
    -o icon.icn

"$MAKEROM" -f cia \
    -o "$HOME_NAME.cia" \
    -DAPP_ENCRYPTED=false \
    -rsf homebrew.rsf \
    -target t \
    -exefslogo \
    -elf "$ELF_FILE" \
    -icon icon.icn \
    -banner banner.bnr

"$MAKEROM" -f cci \
    -o "$HOME_NAME.3ds" \
    -DAPP_ENCRYPTED=true \
    -rsf homebrew.rsf \
    -target t \
    -exefslogo \
    -elf "$ELF_FILE" \
    -icon icon.icn \
    -banner banner.bnr

echo "Files ready."

# ============================
# 7) CLEANUP optionnel
# ============================

# DELETE_SCRIPT="../PYSCRIPT/delete.py"
# if [ -f "$DELETE_SCRIPT" ]; then
#     python3 "$DELETE_SCRIPT"
# fi