pub fn from_title_id(title_id: &str) -> String {
    let start = title_id.len().saturating_sub(5);
    format!("0x{}", &title_id[start..])
}