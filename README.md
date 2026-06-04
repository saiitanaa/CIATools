# CIATools | RSF-Creator

## RSF-Creator for CIATools

**RSF-Creator** is a tool designed to run on CIATools that allows you to **create .rsf configuration** files very quickly and easily.

By default, it grants **full permissions** to homebrew on the console to ensure there are no issues with your projects.

**CIATools repository**: https://github.com/saysaa/CIATools

---

## How to use ?

**Windows**
   - Locate `RSF-Creator.exe` and open it.
  
**Linux**
   - Open a terminal and run these commands: `cd ~/RSF-Creator.linux-x64` and `chmod +x RSF-Creator` and `./RSF-Creator`.

---

## Compile

**For Windows** : `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

**For Linux** : `dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true`

---
