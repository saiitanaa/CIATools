#include "cmd.h"

extern "C" int ciatools_make_smdh(
    const char* title,
    const char* publisher,
    const char* icon,
    const char* output
) {
    char* argv[] = {
        const_cast<char*>("bannertool"),
        const_cast<char*>("makesmdh"),
        const_cast<char*>("-s"),
        const_cast<char*>(title),
        const_cast<char*>("-l"),
        const_cast<char*>(title),
        const_cast<char*>("-p"),
        const_cast<char*>(publisher),
        const_cast<char*>("-i"),
        const_cast<char*>(icon),
        const_cast<char*>("-o"),
        const_cast<char*>(output),
    };

    return cmd_process_command(
        sizeof(argv) / sizeof(argv[0]),
        argv
    );
}