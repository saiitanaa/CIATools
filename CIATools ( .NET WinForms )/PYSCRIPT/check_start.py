import os
import time

os.system("title CIATools - Check Start")
print("Installing dependencies...")
time.sleep(1)
print("Install: subprocess...")
os.system("pip install subprocess")
print("subprocess installed.")
print("Install: shutil...")
os.system("pip install shutil")
print("shutil installed.")
print("All dependencies installed.")

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
START_CIATools = os.path.join(root_path, "CIATools_HUD", "CIATools_HUD", "bin", "Release", "net8.0-windows")
os.chdir(START_CIATools)
os.system("start CIATools_HUD.exe")