use std::ffi::CString;
use std::os::raw::c_char;

unsafe extern "C" {
    fn CIAToolsBuildCIA(
        elf_path: *const c_char,
        rsf_path: *const c_char,
        icon_path: *const c_char,
        banner_path: *const c_char,
        output_path: *const c_char,
    ) -> i32;
}

pub fn build_cia(
    elf_path: &str,
    rsf_path: &str,
    icon_path: &str,
    banner_path: &str,
    output_path: &str,
) -> i32 {
    let elf = CString::new(elf_path).expect("[!] Invalid ELF path");
    let rsf = CString::new(rsf_path).expect("[!] Invalid RSF path");
    let icon = CString::new(icon_path).expect("[!] Invalid icon path");
    let banner = CString::new(banner_path).expect("[!] Invalid banner path");
    let output = CString::new(output_path).expect("[!] Invalid output path");

    unsafe {
        CIAToolsBuildCIA(
            elf.as_ptr(),
            rsf.as_ptr(),
            icon.as_ptr(),
            banner.as_ptr(),
            output.as_ptr(),
        )
    }
}