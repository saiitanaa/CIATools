@echo off
setlocal enabledelayedexpansion

REM -- Set current directory
set "WORK_DIR=%~dp0"

REM -- Find the ELF file and its name
set "ELF_FILE="
set "HOME_NAME="
for %%f in ("%WORK_DIR%*.elf") do (
    set "ELF_FILE=%%~nxf"
    set "HOME_NAME=%%~nf"
)
if not defined ELF_FILE (
    echo No .elf file found in the folder!
    pause
    exit /b
)
echo Found ELF: %ELF_FILE%
echo Homebrew name: %HOME_NAME%

REM -- Find the banner image (any png except icon)
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
echo Banner detected: %BANNER_FILE%

REM -- Find the icon image (icon*.png)
set "ICON_FILE="
for %%f in ("%WORK_DIR%icon*.png") do (
    set "ICON_FILE=%%~nxf"
)
if not defined ICON_FILE (
    echo No icon PNG found!
    pause
    exit /b
)
echo Icon detected: %ICON_FILE%

REM -- Find the WAV audio file
set "AUDIO_FILE="
for %%f in ("%WORK_DIR%*.wav") do (
    set "AUDIO_FILE=%%~nxf"
)
if not defined AUDIO_FILE (
    echo No WAV audio found for the banner!
    pause
    exit /b
)
echo Audio detected: %AUDIO_FILE%

REM -- Ensure romfs folder exists
if not exist "%WORK_DIR%romfs" (
    mkdir "%WORK_DIR%romfs"
    echo Created romfs folder.
)

echo Building %HOME_NAME%...

REM -- Create banner and icon files
bannertool.exe makebanner -i "%WORK_DIR%%BANNER_FILE%" -a "%WORK_DIR%%AUDIO_FILE%" -o banner.bnr
bannertool.exe makesmdh -s "%HOME_NAME%" -l "%HOME_NAME%" -p "NameOfTheCreator" -i "%WORK_DIR%%ICON_FILE%" -o icon.icn

REM -- Build CIA and 3DS files
makerom -f cia -o "%HOME_NAME%.cia" -DAPP_ENCRYPTED=false -rsf homebrew.rsf -target t -exefslogo -elf "%ELF_FILE%" -icon icon.icn -banner banner.bnr
makerom -f cci -o "%HOME_NAME%.3ds" -DAPP_ENCRYPTED=true -rsf homebrew.rsf -target t -exefslogo -elf "%ELF_FILE%" -icon icon.icn -banner banner.bnr

echo Done! CIA and 3DS files are ready!

REM -- Run cleanup script if exists
set "DELETE_SCRIPT=%WORK_DIR%..\script_import\delete.py"
if exist "%DELETE_SCRIPT%" (
    python "%DELETE_SCRIPT%"
) else (
    echo Cleanup script not found!
)

pause
