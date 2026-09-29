use std::ffi::{CString, c_char};
use std::io;
use std::path::Path;

unsafe extern "C" {
    fn ciatools_make_icn(
        title: *const c_char,
        publisher: *const c_char,
        icon: *const c_char,
        output: *const c_char,
    ) -> i32;
}

pub fn make_icn(title: &str, publisher: &str, icon: &Path, output: &Path) -> io::Result<()> {
    let title = CString::new(title)
        .map_err(|_| io::Error::new(io::ErrorKind::InvalidInput, "Invalid title"))?;

    let publisher = CString::new(publisher)
        .map_err(|_| io::Error::new(io::ErrorKind::InvalidInput, "Invalid publisher"))?;

    let icon = CString::new(icon.to_string_lossy().as_bytes())
        .map_err(|_| io::Error::new(io::ErrorKind::InvalidInput, "Invalid icon path"))?;

    let output = CString::new(output.to_string_lossy().as_bytes())
        .map_err(|_| io::Error::new(io::ErrorKind::InvalidInput, "Invalid output path"))?;

    let result = unsafe {
        ciatools_make_icn(
            title.as_ptr(),
            publisher.as_ptr(),
            icon.as_ptr(),
            output.as_ptr(),
        )
    };

    if result != 0 {
        return Err(io::Error::other("bannertool failed to create SMDH"));
    }
    Ok(())
}