@echo off
setlocal enabledelayedexpansion

REM -- Force le script à travailler là où il se trouve physiquement
cd /d "%~dp0"
set "WORK_DIR=%cd%\"

echo Searching files in: %WORK_DIR%

REM ============================
REM 1) DETECT ELF OR 3DSX
REM ============================

set "ELF_FILE="
set "HOME_NAME="
set "THREEDSX_FILE="

REM -- Try to find ELF first
for %%f in ("%WORK_DIR%*.elf") do (
    if not defined ELF_FILE (
        set "ELF_FILE=%%~nxf"
        set "HOME_NAME=%%~nf"
    )
)

REM -- If no ELF, try 3DSX
if not defined ELF_FILE (
    for %%f in ("%WORK_DIR%*.3dsx") do (
        if not defined THREEDSX_FILE (
            set "THREEDSX_FILE=%%~nxf"
            set "HOME_NAME=%%~nf"
        )
    )
)

REM -- Error if nothing found
if not defined ELF_FILE if not defined THREEDSX_FILE (
    echo ERROR: No .elf or .3dsx file found!
    pause
    exit /b 1
)

REM ============================
REM 2) CONVERT 3DSX -> ELF IF NEEDED
REM ============================

if not defined ELF_FILE (
    echo Found 3DSX: %THREEDSX_FILE%
    echo Converting .3dsx to .elf...

    if exist "%WORK_DIR%3dsxtool.exe" (
        set "THREEDSTOOL=%WORK_DIR%3dsxtool.exe"
    ) else (
        set "THREEDSTOOL=3dsxtool.exe"
    )

    "%THREEDSTOOL%" "%WORK_DIR%%THREEDSX_FILE%" "%WORK_DIR%%HOME_NAME%.elf"

    if not exist "%WORK_DIR%%HOME_NAME%.elf" (
        echo ERROR: Failed to generate ELF!
        pause
        exit /b 1
    )

    set "ELF_FILE=%HOME_NAME%.elf"
)

echo Using ELF: %ELF_FILE%
echo Homebrew name: %HOME_NAME%

REM ============================
REM 3) CHECK TOOLS
REM ============================

if exist "%WORK_DIR%makerom.exe" (
    set "MAKEROM=%WORK_DIR%makerom.exe"
) else if exist "%WORK_DIR%makerom" (
    set "MAKEROM=%WORK_DIR%makerom"
) else (
    set "MAKEROM=makerom.exe"
)

if not exist "%WORK_DIR%homebrew.rsf" (
    echo ERROR: homebrew.rsf not found!
    pause
    exit /b 1
)

REM bannertool est seulement obligatoire si on doit générer banner.bnr ou icon.icn.
if exist "%WORK_DIR%bannertool.exe" (
    set "BANNERTOOL=%WORK_DIR%bannertool.exe"
) else (
    set "BANNERTOOL=bannertool.exe"
)

REM ============================
REM 4) ICON DYNAMIQUE
REM ============================

if exist "%WORK_DIR%icon.icn" (
    set "ICON_OUT=icon.icn"
    echo Using existing icon: icon.icn
) else (
    set "ICON_PNG="
    for %%f in ("%WORK_DIR%icon*.png") do (
        if not defined ICON_PNG set "ICON_PNG=%%~nxf"
    )

    if not defined ICON_PNG (
        echo ERROR: No icon.icn or icon*.png found!
        pause
        exit /b 1
    )

    set "CREATOR=NameOfTheCreator"
    if exist "%WORK_DIR%AUTHOR.txt" (
        set /p CREATOR=<"%WORK_DIR%AUTHOR.txt"
    )
    if "!CREATOR!"=="" (
        set "CREATOR=PleaseDefineCreatorName"
    )

    echo Generating icon.icn from: !ICON_PNG!
    echo Creator: !CREATOR!

    "%BANNERTOOL%" makesmdh -s "%HOME_NAME%" -l "%HOME_NAME%" -p "!CREATOR!" -i "%WORK_DIR%!ICON_PNG!" -o "%WORK_DIR%icon.icn"

    if not exist "%WORK_DIR%icon.icn" (
        echo ERROR: Failed to generate icon.icn!
        pause
        exit /b 1
    )

    set "ICON_OUT=icon.icn"
)

REM ============================
REM 5) BANNER DYNAMIQUE
REM ============================
REM Mode intelligent :
REM - Si banner.bin existe, on l'utilise directement. Utile pour une banniere 3D deja prete.
REM - Sinon, si banner.bnr existe, on l'utilise directement.
REM - Sinon, on genere banner.bnr avec bannertool depuis une image PNG + un WAV.

if exist "%WORK_DIR%banner.bin" (
    set "BANNER_OUT=banner.bin"
    echo Using 3D/prebuilt banner: banner.bin
) else if exist "%WORK_DIR%banner.bnr" (
    set "BANNER_OUT=banner.bnr"
    echo Using existing banner: banner.bnr
) else (
    set "BANNER_PNG="
    for %%f in ("%WORK_DIR%*.png") do (
        set "PNG_NAME=%%~nf"
        if /i not "!PNG_NAME:~0,4!"=="icon" (
            if not defined BANNER_PNG set "BANNER_PNG=%%~nxf"
        )
    )

    set "AUDIO_FILE="
    for %%f in ("%WORK_DIR%*.wav") do (
        if not defined AUDIO_FILE set "AUDIO_FILE=%%~nxf"
    )

    if not defined BANNER_PNG (
        echo ERROR: No banner.bin, banner.bnr, or banner PNG found!
        pause
        exit /b 1
    )

    if not defined AUDIO_FILE (
        echo ERROR: No WAV audio found! Needed to generate banner.bnr.
        pause
        exit /b 1
    )

    echo Generating 2D banner.bnr from:
    echo Banner image: !BANNER_PNG!
    echo Audio: !AUDIO_FILE!

    "%BANNERTOOL%" makebanner -i "%WORK_DIR%!BANNER_PNG!" -a "%WORK_DIR%!AUDIO_FILE!" -o "%WORK_DIR%banner.bnr"

    if not exist "%WORK_DIR%banner.bnr" (
        echo ERROR: Failed to generate banner.bnr!
        pause
        exit /b 1
    )

    set "BANNER_OUT=banner.bnr"
)

REM ============================
REM 6) ROMFS
REM ============================

if not exist "%WORK_DIR%romfs" (
    mkdir "%WORK_DIR%romfs"
)

REM ============================
REM 7) BUILD CIA
REM ============================

echo Building CIA: %HOME_NAME%.cia

"%MAKEROM%" -f cia -o "%WORK_DIR%%HOME_NAME%.cia" -rsf "%WORK_DIR%homebrew.rsf" -target t -exefslogo -elf "%WORK_DIR%%ELF_FILE%" -banner "%WORK_DIR%%BANNER_OUT%" -icon "%WORK_DIR%%ICON_OUT%"

if errorlevel 1 (
    echo ERROR: makerom failed!
    pause
    exit /b 1
)

echo CIA ready: %HOME_NAME%.cia
pause
