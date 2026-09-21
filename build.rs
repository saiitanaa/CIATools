use std::fs;
use std::path::Path;

fn collect_c_files(dir: &Path, build: &mut cc::Build) {
    if let Ok(entries) = fs::read_dir(dir) {
        for entry in entries.flatten() {
            let path = entry.path();

            if path.is_dir() {
                collect_c_files(&path, build);
            } else if path.extension().and_then(|s| s.to_str()) == Some("c") {
                if path.file_name().and_then(|s| s.to_str()) != Some("makerom.c") {
                    build.file(path);
                }
            }
        }
    }
}

fn collect_cpp_files(dir: &Path, build: &mut cc::Build) {
    if let Ok(entries) = fs::read_dir(dir) {
        for entry in entries.flatten() {
            let path = entry.path();

            if path.is_dir() {
                collect_cpp_files(&path, build);
            } else if path.extension().and_then(|s| s.to_str()) == Some("cpp") {
                if path.file_name().and_then(|s| s.to_str()) != Some("main.cpp") {
                    build.file(path);
                }
            }
        }
    }
}

fn main() {
    let mut makerom = cc::Build::new();

    makerom
        .std("c11")
        .include("makerom/src")
        .include("makerom/deps/libyaml/include")
        .include("makerom/deps/libmbedtls/include")
        .include("makerom/deps/libblz/include");

    if cfg!(target_env = "msvc") {
        makerom
            .flag("/wd4996")
            .flag("/wd4244")
            .flag("/wd4245");
    }

    collect_c_files(Path::new("makerom/src"), &mut makerom);
    collect_c_files(Path::new("makerom/deps/libblz/src"), &mut makerom);
    collect_c_files(Path::new("makerom/deps/libyaml/src"), &mut makerom);
    collect_c_files(Path::new("makerom/deps/libmbedtls/src"), &mut makerom);

    makerom.compile("makerom_core");

    let mut bannertool = cc::Build::new();

    bannertool
        .cpp(true)
        .define("VERSION_MAJOR", "1")
        .define("VERSION_MINOR", "0")
        .define("VERSION_MICRO", "0")
        .include("bannertool/source")
        .include("bannertool/source/3ds")
        .include("bannertool/source/pc");

    collect_cpp_files(
        Path::new("bannertool/source"),
        &mut bannertool,
    );

    bannertool.compile("bannertool_core");

    let mut bannertool_c = cc::Build::new();

    bannertool_c
        .std("c11")
        .include("bannertool/source")
        .include("bannertool/source/pc")
        .file("bannertool/source/pc/stb_image_impl.c")
        .file("bannertool/source/pc/stb_vorbis.c");

    bannertool_c.compile("bannertool_c");

    let out_dir = std::env::var("OUT_DIR").unwrap();

    println!("cargo:rustc-link-search=native={out_dir}");
    println!("cargo:rustc-link-arg-bins={out_dir}/libbannertool_core.a");
    println!("cargo:rustc-link-arg-bins={out_dir}/libbannertool_c.a");

    println!("cargo:rerun-if-changed=makerom");
    println!("cargo:rerun-if-changed=bannertool");

    #[cfg(target_os = "macos")]
    {
        println!("cargo:rustc-link-lib=c++");
        println!("cargo:rustc-link-arg-bins=-lc++");
    }

    #[cfg(target_os = "linux")]
    {
        println!("cargo:rustc-link-lib=stdc++");
    }

    #[cfg(all(target_os = "windows", target_env = "msvc"))]
    {
        //println!("cargo:rustc-link-lib=static=libcmt");
    }
}