#include <stdlib.h>

#ifndef _WIN32
#ifndef __CYGWIN__
#define LIBICONV_PLUG
#endif
#include <iconv.h>
#endif

#include "oschar.h"

#ifdef _WIN32

typedef struct
{
	HANDLE handle;
	WIN32_FIND_DATAW data;
	struct _osdirent *entry;
	int first;
} _OSDIR;

struct _osdirent
{
	wchar_t d_name[MAX_PATH];
};

_OSDIR *os_opendir(const wchar_t *path)
{
	_OSDIR *dir = calloc(1, sizeof(_OSDIR));

	if (!dir)
		return NULL;

	dir->entry = calloc(1, sizeof(struct _osdirent));

	if (!dir->entry)
	{
		free(dir);
		return NULL;
	}

	wchar_t search_path[MAX_PATH];

	_snwprintf(
		search_path,
		MAX_PATH,
		L"%s\\*",
		path
	);

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
		if (!FindNextFileW(dir->handle, &dir->data))
			return NULL;
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

	return st.st_size;
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

	for (i = 0; str[i] != 0x0; i++);

	return i;
}

void utf16_fputs(const utf16char_t *str, FILE *out)
{
	oschar_t *_str = os_CopyConvertUTF16Str(str);

	os_fputs(_str, out);

	free(_str);
}

char *strcopy_8to8(const char *src)
{
	uint32_t src_len;
	char *dst;

	if (!src)
		return NULL;

	src_len = strlen(src);

	dst = calloc(src_len + 1, sizeof(char));

	if (!dst)
		return NULL;

	strncpy(dst, src, src_len);

	return dst;
}

utf16char_t *strcopy_8to16(const char *src)
{
	uint32_t src_len;
	uint32_t i;
	utf16char_t *dst;

	if (!src)
		return NULL;

	src_len = strlen(src);

	dst = calloc(src_len + 1, sizeof(utf16char_t));

	if (!dst)
		return NULL;

	for (i = 0; i < src_len; i++)
		dst[i] = src[i];

	return dst;
}

utf16char_t *strcopy_16to16(const utf16char_t *src)
{
	uint32_t src_len;
	uint32_t i;
	utf16char_t *dst;

	if (!src)
		return NULL;

	src_len = utf16_strlen(src);

	dst = calloc(src_len + 1, sizeof(utf16char_t));

	if (!dst)
		return NULL;

	for (i = 0; i < src_len; i++)
		dst[i] = src[i];

	return dst;
}

#ifndef _WIN32

utf16char_t *strcopy_UTF8toUTF16(const char *src)
{
	uint32_t src_len;
	uint32_t dst_len;
	size_t in_bytes;
	size_t out_bytes;
	utf16char_t *dst;
	char *in;
	char *out;

	if (!src)
		return NULL;

	src_len = strlen(src);
	dst_len = src_len + 1;

	dst = calloc(dst_len, sizeof(utf16char_t));

	if (!dst)
		return NULL;

	in = (char *)src;
	out = (char *)dst;

	in_bytes = src_len * sizeof(char);
	out_bytes = dst_len * sizeof(utf16char_t);

	iconv_t cd = iconv_open("UTF-16LE", "UTF-8");

	iconv(cd, &in, &in_bytes, &out, &out_bytes);

	iconv_close(cd);

	return dst;
}

char *strcopy_UTF16toUTF8(const utf16char_t *src)
{
	uint32_t src_len;
	uint32_t dst_len;
	size_t in_bytes;
	size_t out_bytes;
	char *dst;
	char *in;
	char *out;

	if (!src)
		return NULL;

	src_len = utf16_strlen(src);
	dst_len = src_len * 3;

	dst = calloc(dst_len, sizeof(char));

	if (!dst)
		return NULL;

	in = (char *)src;
	out = (char *)dst;

	in_bytes = src_len * sizeof(uint16_t);
	out_bytes = dst_len * sizeof(char);

	iconv_t cd = iconv_open("UTF-8", "UTF-16LE");

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
	uint32_t len;
	oschar_t *new_path;

	len = os_strlen(src) + os_strlen(add) + 0x10;

	new_path = calloc(len, sizeof(oschar_t));

#ifdef _WIN32
	_snwprintf(
		new_path,
		len,
		L"%s%c%s",
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

	return new_path;
}

oschar_t *os_AppendUTF16StrToPath(
	const oschar_t *src,
	const utf16char_t *add
)
{
	uint32_t len;
	oschar_t *new_path;
	oschar_t *_add;

	_add = os_CopyConvertUTF16Str(add);

	len = os_strlen(src) + os_strlen(_add) + 0x10;

	new_path = calloc(len, sizeof(oschar_t));

#ifdef _WIN32
	_snwprintf(
		new_path,
		len,
		L"%s%c%s",
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

	free(_add);

	return new_path;
}