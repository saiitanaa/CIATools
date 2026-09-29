#define _LARGEFILE_SOURCE
#define _LARGEFILE64_SOURCE
#define _FILE_OFFSET_BITS 64

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdbool.h>
#include <ctype.h>
#include <time.h>
#include <math.h>
#include <wchar.h>
#include <inttypes.h>

#ifdef _WIN32

    #include <io.h>
    #include <direct.h>
    #include <windows.h>

    #define strcasecmp _stricmp
    #define strncasecmp _strnicmp

#else

    #include <strings.h>
    #include <unistd.h>
    #include <dirent.h>
    #include <sys/stat.h>
    #include <sys/types.h>

#endif

#include "types.h"
#include "utils.h"
#include "ctr_utils.h"
#include "crypto.h"
#include "keyset.h"
#include "user_settings.h"
#include "yaml_parser.h"
#include "rsf_settings.h"
