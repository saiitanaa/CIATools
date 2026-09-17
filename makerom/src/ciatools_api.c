#include "lib.h"
#include "ciatools_api.h"
#include "cia_build.h"
#include "user_settings.h"

int CIAToolsBuildCIA(
    const char *inputPath,
    const char *outputPath
)
{
    user_settings *set = calloc(1, sizeof(user_settings));

    if (!set)
        return -1;

    init_UserSettings(set);
    InitKeys(&set->common.keys);
    SetDefaults(set);

    int result = SetKeys(&set->common.keys);

    if (result != 0)
        goto cleanup;

    set->common.workingFileType = infile_ncch;
    set->common.workingFile.size = GetFileSize64((char *)inputPath);

    if (set->common.workingFile.size == 0) {
        result = -1;
        goto cleanup;
    }

    set->common.workingFile.buffer = ImportFile(
        (char *)inputPath,
        set->common.workingFile.size
    );

    if (!set->common.workingFile.buffer) {
        result = -1;
        goto cleanup;
    }

    set->common.outFileName = strdup(outputPath);

    if (!set->common.outFileName) {
        result = -1;
        goto cleanup;
    }

    set->common.outFileName_mallocd = true;
    set->common.outFormat = CIA;

    result = build_CIA(set);

cleanup:
    free_UserSettings(set);
    return result;
}