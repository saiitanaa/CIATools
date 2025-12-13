#include <windows.h>
#include <stdio.h>
#include <string.h>

int main() {
    char root_path[MAX_PATH];
    GetModuleFileName(NULL, root_path, MAX_PATH);

    char *last;
    while (1) {
        char marker[MAX_PATH];
        snprintf(marker, MAX_PATH, "%s\\root_path", root_path);

        if (GetFileAttributes(marker) != INVALID_FILE_ATTRIBUTES) {
            printf("Root folder: %s\n", root_path);
            char exe_path[MAX_PATH];
            snprintf(exe_path, MAX_PATH, "%s\\CIATools_HUD\\CIATools_HUD\\bin\\Debug\\net8.0-windows\\CIATools_HUD.exe", root_path);
            system(exe_path);
        }

        last = strrchr(root_path, '\\');
        if (!last) break;
        *last = '\0';
    }

    return 0;
}
