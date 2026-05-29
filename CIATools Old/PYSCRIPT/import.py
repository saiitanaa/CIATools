import os
import shutil
import subprocess

script_dir = os.path.dirname(os.path.abspath(__file__))
destination_path = os.path.join(script_dir, "..", "USER_FILES")

CIABUILDER_source = os.path.join(script_dir, "..", "builder_files_sources", "CIABUILDER.bat")
makerom_source = os.path.join(script_dir, "..", "builder_files_sources", "makerom.exe")
bannertool_source = os.path.join(script_dir, "..", "builder_files_sources", "bannertool.exe")
destination = destination_path

print("debug: import.py -> Started\n")
print("Import -> CIABUILDER.bat to USER_FILES")
shutil.copy(CIABUILDER_source, destination)
print("Import -> makerom.exe to USER_FILES")
shutil.copy(makerom_source, destination)
print("Import -> bannertool.exe to USER_FILES")
shutil.copy(bannertool_source, destination)

def FIND_root_path():

    root_path = os.path.abspath(os.path.dirname(__file__))

    while root_path:
        marker = os.path.join(root_path, "root_path")
        if os.path.isfile(marker):
            return root_path

        parent = os.path.dirname(root_path)
        if parent == root_path:
            break
        root_path = parent

    return None 

root_path = FIND_root_path()

print("[import.py] | Debug rootPath = %s" % root_path)
cmd_cd = os.path.join(root_path, "PYSCRIPT")
os.chdir(cmd_cd)
subprocess.run(["python", "compile.py"])