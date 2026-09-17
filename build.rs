use std::fs;

fn main() {
    let mut build = cc::Build::new();

    build
        .std("c11")
        .include("makerom/src")
        .include("makerom/deps/libyaml/include")
        .include("makerom/deps/libmbedtls/include")
        .include("makerom/deps/libblz/include");

    for entry in fs::read_dir("makerom/src").unwrap() {
        let path = entry.unwrap().path();

        if path.extension().and_then(|x| x.to_str()) == Some("c")
            && path.file_name().and_then(|x| x.to_str()) != Some("makerom.c")
        {
            build.file(path);
        }
    }

build.compile("makerom_core");

let out_dir = std::env::var("OUT_DIR").unwrap();
println!("cargo:rustc-link-arg={out_dir}/libmakerom_core.a");
let manifest_dir = std::env::var("CARGO_MANIFEST_DIR").unwrap();

println!(
    "cargo:rustc-link-arg={manifest_dir}/makerom/deps/libmbedtls/bin/libmbedtls.a"
);
}