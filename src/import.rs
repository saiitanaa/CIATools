/*use std::io;
use std::path::PathBuf;
use std::env;
use std::fs;

pub fn data_path() -> PathBuf {
    let binary_path = env::current_exe()
        .expect("Path Error !")
        .parent()
        .unwrap()
        .to_path_buf();

    binary_path.join("DATA").join("USER_FILES")
}

pub fn ensure_data_path() -> io::Result<PathBuf> {
    let dir = data_path();
    fs::create_dir_all(&dir)?;
    Ok(dir)
}

pub fn import_files(files: &[PathBuf]) -> io::Result<Vec<String>> {
    let import_path = ensure_data_path()?;
    let mut log = Vec::new();

    for file in files {
        let file_name = match file.file_name() {
            Some(name) => name,
            None => continue,
        };

        let dest_path = import_path.join(file_name);
        fs::copy(file, &dest_path)?;
        log.push(format!("{}", file_name.to_string_lossy()));
    }

    Ok(log)
}*/