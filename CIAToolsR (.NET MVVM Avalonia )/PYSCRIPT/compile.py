import os
import sys
import subprocess
from utils import FIND_root_path

root_path = FIND_root_path()
is_windows = sys.platform.startswith('win')

print("[compile.py] | Debug rootPath = %s" % root_path)

cmd_CIABUILDER = os.path.join(root_path, "USER_FILES")
delete_script = os.path.join(root_path, "PYSCRIPT", "delete.py")

os.chdir(cmd_CIABUILDER)

if is_windows:
    subprocess.run(["build.bat"], shell=True, check=True)
else:
    subprocess.run(["bash", "build.sh"], check=True)

subprocess.run([sys.executable, delete_script], check=True)