import os

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

print("[compile.py] | Debug rootPath = %s" % root_path)
cmd_CIABUILDER = os.path.join(root_path, "USER_FILES")
delete_NECESSARY_FILES = os.path.join(root_path, "PYSCRIPT", "delete.py")
os.chdir(cmd_CIABUILDER)
os.system("start CIABUILDER.bat")
os.chdir(delete_NECESSARY_FILES)
os.system("python delete.py")