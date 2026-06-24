cargo build --release --bins
Copy-Item target\release\import.exe ..\import.exe -Force
Copy-Item target\release\compile.exe ..\compile.exe -Force
Copy-Item target\release\delete.exe ..\delete.exe -Force
Write-Host "Windows executables installed in RSSCRIPT/"
