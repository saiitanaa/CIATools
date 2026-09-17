use std::ffi::CString;
use std::os::raw::c_char;

unsafe extern "C" {
    fn CIAToolsBuildCIA(
        input_path: *const c_char,
        output_path: *const c_char,
    ) -> i32;
}

pub fn build_cia(input_path: &str, output_path: &str) -> i32 {
    let input = CString::new(input_path).expect("[!] Invalid inputPath");
    let output = CString::new(output_path).expect("[!] Invalid outputPath");
    unsafe {
        CIAToolsBuildCIA(input.as_ptr(), output.as_ptr())
    }
}