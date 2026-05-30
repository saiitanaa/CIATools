import os
import subprocess

os.system("title CIATools - Launcher")
print("Check installing subprocess...")
os.system("pip install subprocess")
print("Check installing shutil...")
os.system("pip install shutil")
def CREATE_root_path():

    root_PATH = os.path.abspath(os.path.dirname(__file__))

    return root_PATH
root_PATH = CREATE_root_path()
print("Check -> root_path\n")
open(os.path.join(root_PATH, "root_path"), "w").close()

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

FILE_PATH_create = os.path.join(root_path, "USER_FILES")
os.chdir(FILE_PATH_create)
open(os.path.join("FILE_PATH"), "w").close()
START_CIATools = os.path.join(root_path, "CIATools_HUD", "CIATools_HUD", "bin", "Release", "net8.0-windows")
os.chdir(START_CIATools)
os.system("start CIATools_HUD.exe")