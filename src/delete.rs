use std::fs;
use std::io;
use std::path::Path;

pub fn clean_user_files<P: AsRef<Path>>(path: P) -> io::Result<()> {
    let path = path.as_ref();

    if path.exists() {
        fs::remove_dir_all(path)?;
    }

    fs::create_dir_all(path)?;

    Ok(())
}