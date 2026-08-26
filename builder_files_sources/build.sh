#!/usr/bin/env bash
set -e

# Se placer dans le répertoire du script
cd "$(dirname "$0")"
WORK_DIR="$(pwd)"

echo "Searching files in: $WORK_DIR"

# ============================
# 1) DETECT ELF OR 3DSX
# ============================

ELF_FILE="$(find . -maxdepth 1 -type f -iname "*.elf" | head -n 1 | sed 's|^\./||')"
THREEDSX_FILE="$(find . -maxdepth 1 -type f -iname "*.3dsx" | head -n 1 | sed 's|^\./||')"

if [ -z "$ELF_FILE" ] && [ -z "$THREEDSX_FILE" ]; then
    echo "ERROR: No .elf or .3dsx file found!"
    exit 1
fi

if [ -n "$ELF_FILE" ]; then
    HOME_NAME="${ELF_FILE%.*}"
else
    HOME_NAME="${THREEDSX_FILE%.*}"

    echo "Found 3DSX: $THREEDSX_FILE"
    echo "Converting .3dsx to .elf..."

    if [ -x "./3dsxtool" ]; then
        THREE_DSTOOL="./3dsxtool"
    elif command -v 3dsxtool >/dev/null 2>&1; then
        THREE_DSTOOL="3dsxtool"
    else
        echo "ERROR: 3dsxtool not found!"
        exit 1
    fi

    "$THREE_DSTOOL" "$THREEDSX_FILE" "$HOME_NAME.elf"

    if [ ! -f "$HOME_NAME.elf" ]; then
        echo "ERROR: Failed to generate ELF!"
        exit 1
    fi

    ELF_FILE="$HOME_NAME.elf"
fi

echo "Using ELF: $ELF_FILE"
echo "Homebrew name: $HOME_NAME"

# ============================
# 2) CHECK TOOLS
# ============================

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

# bannertool est seulement obligatoire si on doit générer banner.bnr ou icon.icn.
if [ -x "./bannertool" ]; then
    BANNERTOOL="./bannertool"
elif command -v bannertool >/dev/null 2>&1; then
    BANNERTOOL="bannertool"
else
    BANNERTOOL=""
fi

# ============================
# 3) ICON DYNAMIQUE
# ============================

if [ -f "icon.icn" ]; then
    ICON_OUT="icon.icn"
    echo "Using existing icon: $ICON_OUT"
else
    ICON_PNG="$(find . -maxdepth 1 -type f -iname "icon*.png" | head -n 1 | sed 's|^\./||')"

    if [ -z "$ICON_PNG" ]; then
        echo "ERROR: No icon.icn or icon*.png found!"
        exit 1
    fi

    if [ -z "$BANNERTOOL" ]; then
        echo "ERROR: bannertool not found! Needed to generate icon.icn from $ICON_PNG"
        exit 1
    fi

    CREATOR="NameOfTheCreator"

    if [ -f "AUTHOR.txt" ]; then
        CREATOR="$(head -n 1 AUTHOR.txt)"
    fi

    if [ -z "$CREATOR" ]; then
        CREATOR="PleaseDefineCreatorName"
    fi

    echo "Generating icon.icn from: $ICON_PNG"
    echo "Creator: $CREATOR"

    "$BANNERTOOL" makesmdh \
        -s "$HOME_NAME" \
        -l "$HOME_NAME" \
        -p "$CREATOR" \
        -i "$ICON_PNG" \
        -o icon.icn

    ICON_OUT="icon.icn"
fi

# ============================
# 4) BANNER DYNAMIQUE
# ============================
# Mode intelligent :
# - Si banner.bin existe, on l'utilise directement. Utile pour une bannière 3D déjà prête.
# - Sinon, si banner.bnr existe, on l'utilise directement.
# - Sinon, on génère banner.bnr avec bannertool depuis une image PNG + un WAV.

if [ -f "banner.bin" ]; then
    BANNER_OUT="banner.bin"
    echo "Using 3D/prebuilt banner: $BANNER_OUT"
elif [ -f "banner.bnr" ]; then
    BANNER_OUT="banner.bnr"
    echo "Using existing banner: $BANNER_OUT"
else
    BANNER_PNG="$(find . -maxdepth 1 -type f -iname "*.png" ! -iname "icon*.png" | head -n 1 | sed 's|^\./||')"
    AUDIO_FILE="$(find . -maxdepth 1 -type f -iname "*.wav" | head -n 1 | sed 's|^\./||')"

    if [ -z "$BANNER_PNG" ]; then
        echo "ERROR: No banner.bin, banner.bnr, or banner PNG found!"
        exit 1
    fi

    if [ -z "$AUDIO_FILE" ]; then
        echo "ERROR: No WAV audio found! Needed to generate banner.bnr."
        exit 1
    fi

    if [ -z "$BANNERTOOL" ]; then
        echo "ERROR: bannertool not found! Needed to generate banner.bnr."
        exit 1
    fi

    echo "Generating 2D banner.bnr from:"
    echo "Banner image: $BANNER_PNG"
    echo "Audio: $AUDIO_FILE"

    "$BANNERTOOL" makebanner \
        -i "$BANNER_PNG" \
        -a "$AUDIO_FILE" \
        -o banner.bnr

    BANNER_OUT="banner.bnr"
fi

# ============================
# 5) ROMFS
# ============================

if [ ! -d "romfs" ]; then
    mkdir romfs
fi

# ============================
# 6) BUILD CIA
# ============================

echo "Building CIA: $HOME_NAME.cia"

"$MAKEROM" -f cia \
    -o "$HOME_NAME.cia" \
    -rsf homebrew.rsf \
    -target t \
    -exefslogo \
    -elf "$ELF_FILE" \
    -banner "$BANNER_OUT" \
    -icon "$ICON_OUT"

echo "CIA ready: $HOME_NAME.cia"
