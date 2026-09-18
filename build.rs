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

fn main() {
    let mut build = cc::Build::new();

    build
        .std("c11")
        .include("makerom/src")
        .include("makerom/deps/libyaml/include")
        .include("makerom/deps/libmbedtls/include")
        .include("makerom/deps/libblz/include");

    if cfg!(target_env = "msvc") {
        build.flag("/wd4996").flag("/wd4244").flag("/wd4245");
    }

    collect_c_files(Path::new("makerom/src"), &mut build);
    collect_c_files(Path::new("makerom/deps/libblz/src"), &mut build);
    collect_c_files(Path::new("makerom/deps/libyaml/src"), &mut build);
    collect_c_files(Path::new("makerom/deps/libmbedtls/src"), &mut build);

    build.compile("makerom_core");
    println!("cargo:rustc-link-lib=static=makerom_core");
    println!("cargo:rerun-if-changed=makerom/src");
    println!("cargo:rerun-if-changed=makerom/deps");
    println!("cargo:rustc-link-search=native=/opt/homebrew/lib");
}