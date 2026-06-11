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
    set "ELF_FILE=%%~nxf"
    set "HOME_NAME=%%~nf"
)

REM -- If no ELF, try 3DSX
if not defined ELF_FILE (
    for %%f in ("%WORK_DIR%*.3dsx") do (
        set "THREEDSX_FILE=%%~nxf"
        set "HOME_NAME=%%~nf"
    )
)

REM -- Error if nothing found
if not defined ELF_FILE if not defined THREEDSX_FILE (
    echo No .elf or .3dsx file found!
    pause
    exit /b
)

REM ============================
REM 2) CONVERT 3DSX -> ELF IF NEEDED
REM ============================

if not defined ELF_FILE (
    echo Found 3DSX: %THREEDSX_FILE%
    echo Converting .3dsx to .elf...

    if not exist "%WORK_DIR%3dsxtool.exe" (
        echo ERROR: 3dsxtool.exe not found!
        pause
        exit /b
    )

    3dsxtool.exe "%WORK_DIR%%THREEDSX_FILE%" "%HOME_NAME%.elf"

    if not exist "%HOME_NAME%.elf" (
        echo ERROR: Failed to generate ELF!
        pause
        exit /b
    )

    set "ELF_FILE=%HOME_NAME%.elf"
)

echo Using ELF: %ELF_FILE%
echo Homebrew name: %HOME_NAME%

REM ============================
REM 3) BANNER / ICON / AUDIO
REM ============================

set "BANNER_FILE="
for %%f in ("%WORK_DIR%*.png") do (
    if /i "%%~nf" neq "icon" (
        set "BANNER_FILE=%%~nxf"
    )
)
if not defined BANNER_FILE (
    echo No banner PNG found!
    pause
    exit /b
)

set "ICON_FILE="
for %%f in ("%WORK_DIR%icon*.png") do (
    set "ICON_FILE=%%~nxf"
)
if not defined ICON_FILE (
    echo No icon PNG found!
    pause
    exit /b
)

set "AUDIO_FILE="
for %%f in ("%WORK_DIR%*.wav") do (
    set "AUDIO_FILE=%%~nxf"
)
if not defined AUDIO_FILE (
    echo No WAV audio found!
    pause
    exit /b
)

REM ============================
REM 4) ROMFS
REM ============================

if not exist "%WORK_DIR%romfs" (
    mkdir "%WORK_DIR%romfs"
)

REM ============================
REM 5) BUILD
REM ============================

echo Building %HOME_NAME%...

set "CREATOR=NameOfTheCreator"

if exist "AUTHOR.txt" (
    set /p CREATOR=<"AUTHOR.txt"
)

if "%CREATOR%"=="" (
    set "CREATOR=PleaseDefineCreatorName"
)

bannertool.exe makebanner -i "%WORK_DIR%%BANNER_FILE%" -a "%WORK_DIR%%AUDIO_FILE%" -o banner.bnr
bannertool.exe makesmdh -s "%HOME_NAME%" -l "%HOME_NAME%" -p "%CREATOR%" -i "%WORK_DIR%%ICON_FILE%" -o icon.icn

makerom -f cia -o "%HOME_NAME%.cia" -DAPP_ENCRYPTED=false -rsf homebrew.rsf -target t -exefslogo -elf "%ELF_FILE%" -icon icon.icn -banner banner.bnr
makerom -f cci -o "%HOME_NAME%.3ds" -DAPP_ENCRYPTED=true -rsf homebrew.rsf -target t -exefslogo -elf "%ELF_FILE%" -icon icon.icn -banner banner.bnr

echo Files ready.

REM -- 6) CLEANUP (Supprime cette partie ou commente-la)
REM set "DELETE_SCRIPT=%WORK_DIR%..\PYSCRIPT\delete.py"
REM if exist "%DELETE_SCRIPT%" (
REM     python "%DELETE_SCRIPT%"
REM )

pause
