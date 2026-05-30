import os
import shutil
import stat
import sys
import subprocess
from utils import FIND_root_path

root_path = FIND_root_path()
is_windows = sys.platform.startswith('win')

# Définition des extensions selon l'OS
ext = ".exe" if is_windows else ""
sh_ext = ".bat" if is_windows else ".sh"

# Fichiers sources et destinations
tools = ["makerom", "bannertool"]
src_dir = os.path.join(root_path, "builder_files_sources")
dst_dir = os.path.join(root_path, "USER_FILES")

print("debug: import.py -> Started\n")

build_script = f"build{sh_ext}"
shutil.copy(os.path.join(src_dir, build_script), os.path.join(dst_dir, build_script))
if not is_windows:
    os.chmod(os.path.join(dst_dir, build_script), stat.S_IRWXU)

for tool in tools:
    tool_file = f"{tool}{ext}"
    src_tool = os.path.join(src_dir, tool_file)
    dst_tool = os.path.join(dst_dir, tool_file)
    
    print(f"Import -> {tool_file} to USER_FILES")
    shutil.copy(src_tool, dst_tool)
    
    if not is_windows:
        os.chmod(dst_tool, stat.S_IRWXU)

print("\nImport finished, launching compile.py...")
cmd_cd = os.path.join(root_path, "PYSCRIPT")
os.chdir(cmd_cd)

subprocess.run([sys.executable, "compile.py"], check=True)