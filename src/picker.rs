use std::{io, path::PathBuf};

#[cfg(target_os = "macos")]
pub fn pick_files() -> io::Result<Option<Vec<PathBuf>>> {
    use std::process::Command;

    let script = r#"
        set selectedFiles to choose file with multiple selections allowed
        set output to ""
        
        repeat with selectedFile in selectedFiles
            set output to output & POSIX path of selectedFile & linefeed
        end repeat
        
        return output
    "#;

    let output = Command::new("osascript")
        .args(["-e", script])
        .output()?;

    if !output.status.success() {
        return Ok(None);
    }

    let files = String::from_utf8_lossy(&output.stdout)
        .lines()
        .map(PathBuf::from)
        .filter(|path| path.exists())
        .collect::<Vec<_>>();

    if files.is_empty() {
        Ok(None)
    } else {
        Ok(Some(files))
    }
}

#[cfg(not(target_os = "macos"))]
pub fn pick_files() -> io::Result<Option<Vec<PathBuf>>> {
    Ok(rfd::FileDialog::new().pick_files())
}