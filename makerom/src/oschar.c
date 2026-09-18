#include <stdlib.h>
#include <string.h>
#include <wchar.h>

#ifdef _WIN32

#include <windows.h>
#include <sys/stat.h>
#include <direct.h>

#else

#ifndef CYGWIN
#define LIBICONV_PLUG
#endif

#include <iconv.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <dirent.h>
#include <unistd.h>

#endif

#include "oschar.h"

#ifdef _WIN32

struct _OSDIR
{
HANDLE handle;
WIN32_FIND_DATAW data;
struct _osdirent *entry;
int first;
};

_OSDIR *os_opendir(const wchar_t *path)
{
_OSDIR *dir;
wchar_t search_path[MAX_PATH];

if (!path)
	return NULL;

dir = calloc(1, sizeof(*dir));

if (!dir)
	return NULL;

dir->entry = calloc(1, sizeof(*dir->entry));

if (!dir->entry)
{
	free(dir);
	return NULL;
}

_snwprintf(
	search_path,
	MAX_PATH,
	L"%ls\\*",
	path
);

search_path[MAX_PATH - 1] = L'\0';

dir->handle = FindFirstFileW(
	search_path,
	&dir->data
);

if (dir->handle == INVALID_HANDLE_VALUE)
{
	free(dir->entry);
	free(dir);
	return NULL;
}

dir->first = 1;

return dir;

}

struct _osdirent *os_readdir(_OSDIR *dir)
{
if (!dir)
return NULL;

if (!dir->first)
{
	if (!FindNextFileW(
		dir->handle,
		&dir->data
	))
	{
		return NULL;
	}
}

dir->first = 0;

wcsncpy(
	dir->entry->d_name,
	dir->data.cFileName,
	MAX_PATH - 1
);

dir->entry->d_name[MAX_PATH - 1] = L'\0';

return dir->entry;

}

int os_closedir(_OSDIR *dir)
{
if (!dir)
return -1;

if (dir->handle != INVALID_HANDLE_VALUE)
	FindClose(dir->handle);

free(dir->entry);
free(dir);

return 0;

}

#endif

int os_fstat(const oschar_t *path)
{
struct _osstat st;

return os_stat(path, &st);

}

uint64_t os_fsize(const oschar_t *path)
{
struct _osstat st;

if (os_stat(path, &st) != 0)
	return 0;

return (uint64_t)st.st_size;

}

int os_makedir(const oschar_t *dir)
{
#ifdef _WIN32
return _wmkdir(dir);
#else
return mkdir(dir, 0777);
#endif
}

uint32_t utf16_strlen(const utf16char_t *str)
{
uint32_t i;

if (!str)
	return 0;

for (i = 0; str[i] != 0; i++)
	;

return i;

}

void utf16_fputs(const utf16char_t *str, FILE *out)
{
oschar_t *converted;

if (!str || !out)
	return;

converted = os_CopyConvertUTF16Str(str);

if (!converted)
	return;

os_fputs(converted, out);

free(converted);

}

char *strcopy_8to8(const char *src)
{
size_t src_len;
char *dst;

if (!src)
	return NULL;

src_len = strlen(src);

dst = calloc(src_len + 1, sizeof(*dst));

if (!dst)
	return NULL;

memcpy(dst, src, src_len);
dst[src_len] = '\0';

return dst;

}

utf16char_t *strcopy_8to16(const char *src)
{
size_t src_len;
size_t i;
utf16char_t *dst;

if (!src)
	return NULL;

src_len = strlen(src);

dst = calloc(src_len + 1, sizeof(*dst));

if (!dst)
	return NULL;

for (i = 0; i < src_len; i++)
	dst[i] = (utf16char_t)(unsigned char)src[i];

dst[src_len] = 0;

return dst;

}

utf16char_t *strcopy_16to16(const utf16char_t *src)
{
size_t src_len;
size_t i;
utf16char_t *dst;

if (!src)
	return NULL;

src_len = utf16_strlen(src);

dst = calloc(src_len + 1, sizeof(*dst));

if (!dst)
	return NULL;

for (i = 0; i < src_len; i++)
	dst[i] = src[i];

dst[src_len] = 0;

return dst;

}

#ifndef _WIN32

utf16char_t *strcopy_UTF8toUTF16(const char *src)
{
size_t src_len;
size_t dst_len;
size_t in_bytes;
size_t out_bytes;

utf16char_t *dst;
char *in;
char *out;
iconv_t cd;

if (!src)
	return NULL;

src_len = strlen(src);
dst_len = src_len + 1;

dst = calloc(dst_len, sizeof(*dst));

if (!dst)
	return NULL;

in = (char *)src;
out = (char *)dst;

in_bytes = src_len;
out_bytes = dst_len * sizeof(*dst);

cd = iconv_open("UTF-16LE", "UTF-8");

if (cd == (iconv_t)-1)
{
	free(dst);
	return NULL;
}

iconv(cd, &in, &in_bytes, &out, &out_bytes);
iconv_close(cd);

return dst;

}

char *strcopy_UTF16toUTF8(const utf16char_t *src)
{
size_t src_len;
size_t dst_len;
size_t in_bytes;
size_t out_bytes;

char *dst;
char *in;
char *out;
iconv_t cd;

if (!src)
	return NULL;

src_len = utf16_strlen(src);
dst_len = src_len * 3 + 1;

dst = calloc(dst_len, sizeof(*dst));

if (!dst)
	return NULL;

in = (char *)src;
out = dst;

in_bytes = src_len * sizeof(*src);
out_bytes = dst_len;

cd = iconv_open("UTF-8", "UTF-16LE");

if (cd == (iconv_t)-1)
{
	free(dst);
	return NULL;
}

iconv(cd, &in, &in_bytes, &out, &out_bytes);
iconv_close(cd);

return dst;

}

#endif

oschar_t *os_AppendToPath(
const oschar_t *src,
const oschar_t *add
)
{
size_t len;
oschar_t *new_path;

if (!src || !add)
	return NULL;

len = os_strlen(src) + os_strlen(add) + 0x10;

new_path = calloc(len, sizeof(*new_path));

if (!new_path)
	return NULL;

#ifdef _WIN32

_snwprintf(
	new_path,
	len,
	L"%ls%c%ls",
	src,
	OS_PATH_SEPARATOR,
	add
);

#else

snprintf(
	new_path,
	len,
	"%s%c%s",
	src,
	OS_PATH_SEPARATOR,
	add
);

#endif

new_path[len - 1] = 0;

return new_path;

}

oschar_t *os_AppendUTF16StrToPath(
const oschar_t *src,
const utf16char_t *add
)
{
size_t len;
oschar_t *new_path;
oschar_t *_add;

if (!src || !add)
	return NULL;

_add = os_CopyConvertUTF16Str(add);

if (!_add)
	return NULL;

len = os_strlen(src) + os_strlen(_add) + 0x10;

new_path = calloc(len, sizeof(*new_path));

if (!new_path)
{
	free(_add);
	return NULL;
}

#ifdef _WIN32

_snwprintf(
	new_path,
	len,
	L"%ls%c%ls",
	src,
	OS_PATH_SEPARATOR,
	_add
);

#else

snprintf(
	new_path,
	len,
	"%s%c%s",
	src,
	OS_PATH_SEPARATOR,
	_add
);

#endif

new_path[len - 1] = 0;

free(_add);

return new_path;

}