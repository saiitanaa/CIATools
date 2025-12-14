cmake_minimum_required(VERSION 3.16)
project(LAUNCHER C)

set(CMAKE_C_STANDARD 11)

add_executable(LAUNCHER
    main.c
    resource.rc
)
