#include "lib.h"
#include "ciatools_api.h"
#include "cia_build.h"
#include "user_settings.h"
#include <_string.h>

int build_NCCH(user_settings *usrset);

int CIAToolsBuildCIA(
    const char *elfPath,
    const char *rsfPath,
    const char *iconPath,
    const char *bannerPath,
    const char *outputPath
)
{
    user_settings *set = calloc(1, sizeof(user_settings));

    if (!set)
        return -1;

    init_UserSettings(set);

    set->common.contentPath = calloc(CIA_MAX_CONTENT, sizeof(char *));

    if (!set->common.contentPath)
    {
        free_UserSettings(set);
        return -1;
    }

    InitKeys(&set->common.keys);

    SetDefaults(set);

    int result = SetKeys(&set->common.keys);

    if (result != 0)
        goto cleanup;

    set->common.rsfPath = strdup(rsfPath);
    set->ncch.elfPath = strdup(elfPath);
    set->ncch.iconPath = strdup(iconPath);
    set->ncch.bannerPath = strdup(bannerPath);
    set->common.outFileName = strdup(outputPath);

    if (!set->common.rsfPath ||
        !set->ncch.elfPath ||
        !set->ncch.iconPath ||
        !set->ncch.bannerPath ||
        !set->common.outFileName)
    {
        result = -1;
        goto cleanup;
    }

    set->common.outFileName_mallocd = true;

    set->common.outFormat = CIA;

    set->common.workingFileType = infile_ncch;

    result = GetRsfSettings(set);

    if (result != 0)
        goto cleanup;

    result = build_NCCH(set);

    if (result != 0)
        goto cleanup;

    result = build_CIA(set);

cleanup:
    free_UserSettings(set);

    return result;
}