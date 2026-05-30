<img width="1000" height="200" alt="Plan de travail 1" src="https://github.com/user-attachments/assets/7d9efa5e-888b-4106-9aed-cb090627dc36" />

# CIATools

The all-in-one tool for compiling your projects into `.cia` format with ease.

### About
`CIAToolsR` is a complete rewrite of the original tool, designed to be more stable, faster, and above all, **cross-platform**. We have migrated to an **Avalonia UI** architecture for a fluid experience on both Windows and Linux, while automating the entire compilation chain to save you from dealing with manual script handling.

---

### OLD vs Renewed: What’s changed?
* **Cross-platform**: Runs natively on Windows and Linux.
* **Clean Architecture**: Migrated to .NET MVVM, code completely rewritten.
* **Modern Interface**: Material Design look, clean and readable.
* **Full Control**: Integrated debug console, real-time log management, and automation options for scripts.

---

### Usage Guide

1. **Prerequisites**
   - Python must be installed and added to your `PATH`.
   - Use the "Install Python dependencies" button in the app to configure the environment automatically.

2. **Workflow**
   - Place your assets (icons, banners, binaries) in the `/USER_FILES` folder.
   - Launch the application.
   - Configure your preferences via the interface (Console, Auto-close, etc.).
   - Click **Build CIA** and let the tool handle the rest.
  
3. **Windows**
   - Locate `CIAToolsR.exe` and open it.
  
4. **Linux**
   - Open a terminal and run these commands: `cd ~/CIAToolsR-linux64` and `chmod +x CIAToolsR` and `./CIAToolsR`.

---

### Compile

**For Windows** : `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

**For Linux** : `dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true`

---

### Credits
* **Manurocker95** for the original base of `CIABUILDER.bat`.
