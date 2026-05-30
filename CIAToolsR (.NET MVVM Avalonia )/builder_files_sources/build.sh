#!/bin/bash

# Se placer dans le répertoire du script
cd "$(dirname "$0")"
WORK_DIR=$(pwd)

# ============================
# 1) DETECT ELF OR 3DSX
# ============================

ELF_FILE=$(ls *.elf 2>/dev/null | head -n 1)
THREEDSX_FILE=$(ls *.3dsx 2>/dev/null | head -n 1)

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
    
    # Assure-toi que 3dsxtool est dans le même dossier ou dans ton PATH
    ./3dsxtool "$THREEDSX_FILE" "$HOME_NAME.elf"
    
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

# On cherche les fichiers (en évitant le mot 'icon' pour le banner)
BANNER_FILE=$(find . -maxdepth 1 -name "*.png" ! -name "icon*" | head -n 1 | xargs basename)
ICON_FILE=$(find . -maxdepth 1 -name "icon*.png" | head -n 1 | xargs basename)
AUDIO_FILE=$(find . -maxdepth 1 -name "*.wav" | head -n 1 | xargs basename)

if [ -z "$BANNER_FILE" ] || [ -z "$ICON_FILE" ] || [ -z "$AUDIO_FILE" ]; then
    echo "Missing files! Ensure you have a banner PNG, icon PNG, and a WAV file."
    exit 1
fi

# ============================
# 3) BUILD
# ============================

echo "Building $HOME_NAME..."

./bannertool makebanner -i "$BANNER_FILE" -a "$AUDIO_FILE" -o banner.bnr
./bannertool makesmdh -s "$HOME_NAME" -l "$HOME_NAME" -p "NameOfTheCreator" -i "$ICON_FILE" -o icon.icn

# Note : Assure-toi que makerom est bien dans le dossier ou installé dans ton PATH
./makerom -f cia -o "$HOME_NAME.cia" -DAPP_ENCRYPTED=false -rsf homebrew.rsf -target t -exefslogo -elf "$ELF_FILE" -icon icon.icn -banner banner.bnr
./makerom -f cci -o "$HOME_NAME.3ds" -DAPP_ENCRYPTED=true -rsf homebrew.rsf -target t -exefslogo -elf "$ELF_FILE" -icon icon.icn -banner banner.bnr

echo "Files ready."