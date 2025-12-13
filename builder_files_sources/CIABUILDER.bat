@echo off
bannertool.exe makebanner -i banner.png -a audio.wav -o banner.bnr
bannertool.exe makesmdh -s "NameOfYourHomebrew" -l "NameOfYourHomebrew" -p "NameOfTheCreator" -i icon.png  -o icon.icn
makerom -f cia -o homebrew.cia -DAPP_ENCRYPTED=false -rsf homebrew.rsf -target t -exefslogo -elf homebrew.elf -icon icon.icn -banner banner.bnr
makerom -f cci -o homebrew.3ds -DAPP_ENCRYPTED=true -rsf homebrew.rsf -target t -exefslogo -elf homebrew.elf -icon icon.icn -banner banner.bnr
echo Finished! 3DS and CIA have been built!
set BATCH_DIR=%~dp0
python "%BATCH_DIR%..\script_import\delete.py"
