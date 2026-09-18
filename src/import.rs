use std::{fs, io, path::PathBuf};

pub fn import_files(user_files_path: PathBuf) -> io::Result<Vec<String>> {
    let Some(files) = crate::picker::pick_files()? else {
        return Ok(Vec::new());
    };

    let mut imported = Vec::new();

    for file in files {
        let Some(filename) = file.file_name() else {
            continue;
        };

        let destination = user_files_path.join(filename);

        fs::copy(&file, &destination)?;
        imported.push(filename.to_string_lossy().into_owned());
    }

    Ok(imported)
}
