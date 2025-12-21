# >> CIATools
The simplest tool for compiling to .cia!
_________________________________________

## What does the program do?

`CIATools.exe` launches `CIATools_HUD.exe`, located at `CIATools\CIATools_HUD\CIATools_HUD\bin\Debug\net8.0-windows`.
**For V3 and above**, `CIATools_HUD.exe` is located at `CIATools\CIATools_HUD\CIATools_HUD\bin\Release\net8.0-windows`

`CIATools_HUD.exe` is the program interface.

Then, when we click on Build in the HUD, it will `import.py` from `CIATools\script_import`. import.py moves all the files necessary for compiling the project.

Once finished, `import.py` will launch `compile.py` which is located in the same path.

compile.py will then launch `CIABUILDER.bat` from `CIATools\builder_files_sources`. Once the compilation is complete, it will launch `delete.py`, which is located in the same path as compile.py.

delete.py will delete the temporary files used for compilation to simplify the program's use.

**For compiling .3dsx files**

CIATools converts your .3dsx file to .3ds and .elf files.

Then it converts the .3ds and .elf files to .cia files.

## How to use CIATools?

**Very important! Please install Python and add it to your PATH.**

Simply drag and drop all the files needed to compile your .cia into this path `CIATools\USER_FILES`.

Then run CIATools.exe.

Note that if you are using version 1.0, you must rename your files as follows:

**This problem has been fixed in v2.0 and you no longer need to touch `CIABUILDER.bat`!**

//
- homebrew.rsf
- homebrew.elf
- icon.icn
- icon.png
- banner.bnr
- audio.wav
- homebrew.3ds

//

Optional but recommended:

I recommend modifying the `CIABUILDER.bat` file located in this path `CIATools\builder_files_sources` to properly configure your .cia (such as the 'Homebrew Name', 'Creator Name', file names, etc.).

Otherwise, this step is not mandatory, but you must rename your files as mentioned above. 



## Credit

Thanks to Manurocker95 for creating CIABUILDER.bat which was used in version 1.0

Original CIABUILDER.bat project: https://github.com/Manurocker95/CIABUILDER
