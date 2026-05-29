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