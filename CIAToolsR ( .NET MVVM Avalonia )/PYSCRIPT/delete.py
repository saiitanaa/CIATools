import os
import sys
from utils import FIND_root_path

root_path = FIND_root_path()
is_windows = sys.platform.startswith('win')

# On détermine les extensions à nettoyer selon l'OS
ext = ".exe" if is_windows else ""
sh_ext = ".bat" if is_windows else ".sh"

print("[delete.py] | Debug rootPath = %s" % root_path)
cmd_delete_files = os.path.join(root_path, "USER_FILES")
os.chdir(cmd_delete_files)

# La liste des fichiers temporaires à dégager
files_to_delete = [f"build{sh_ext}", f"bannertool{ext}", f"makerom{ext}"]

for f in files_to_delete:
    if os.path.exists(f):
        try:
            os.remove(f)
            print(f"Successfully deleted: {f}")
        except Exception as e:
            print(f"Error deleting {f}: {e}")

print("Cleanup process finished!")