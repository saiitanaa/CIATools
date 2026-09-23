use image::GenericImageView;
use std::fs;
use std::io::{self, Write};
use std::path::{Path, PathBuf};

const MAGIC_SMDH: u32 = 0x48444D53;
const TILE_ORDER: [u8; 64] = [
    0, 1, 8, 9, 2, 3, 10, 11, 16, 17, 24, 25, 18, 19, 26, 27, 4, 5, 12, 13, 6, 7, 14, 15, 20, 21,
    28, 29, 22, 23, 30, 31, 32, 33, 40, 41, 34, 35, 42, 43, 48, 49, 56, 57, 50, 51, 58, 59, 36, 37,
    44, 45, 38, 39, 46, 47, 52, 53, 60, 61, 54, 55, 62, 63,
];

pub const ICN_LANGUAGES: [&str; 16] = [
    "Japanese",
    "English",
    "French",
    "German",
    "Italian",
    "Spanish",
    "Simplified Chinese",
    "Korean",
    "Dutch",
    "Portuguese",
    "Russian",
    "Traditional Chinese",
    "[!] Unused Slot 12",
    "[!] Unused Slot 13",
    "[!] Unused Slot 14",
    "[!] Unused Slot 15",
];

#[derive(Clone, Copy, Debug)]
struct SmdhTitle {
    short_description: [u16; 0x40],
    long_description: [u16; 0x80],
    publisher: [u16; 0x40],
}

impl Default for SmdhTitle {
    fn default() -> Self {
        Self {
            short_description: [0; 0x40],
            long_description: [0; 0x80],
            publisher: [0; 0x40],
        }
    }
}

#[derive(Clone, Copy, Debug, Default)]
struct SmdhSettings {
    game_ratings: [u8; 0x10],
    region_lock: u32,
    match_maker_id: [u8; 0x0C],
    flags: u32,
    eula_version: u16,
    reserved: u16,
    default_frame: u32,
    cec_id: u32,
}

#[derive(Debug)]
pub struct IcnFile {
    titles: [SmdhTitle; 16],
    settings: SmdhSettings,
    reserved: [u8; 0x08],
    small_icon_data: [u16; 24 * 24],
    big_icon_data: [u16; 48 * 48],
}

impl Default for IcnFile {
    fn default() -> Self {
        Self::new()
    }
}

impl IcnFile {
    pub fn new() -> Self {
        let mut smdh = Self {
            titles: [SmdhTitle::default(); 16],
            settings: SmdhSettings::default(),
            reserved: [0; 0x08],
            small_icon_data: [0; 24 * 24],
            big_icon_data: [0; 48 * 48],
        };

        smdh.fill_default_icons();

        smdh
    }

    pub fn load_icon<P: AsRef<Path>>(&mut self, path: P) -> io::Result<()> {
        let image = image::open(path)
            .map_err(|error| io::Error::other(format!("[!] Failed to load icon: {error}")))?;

        let big = image.resize_exact(48, 48, image::imageops::FilterType::Lanczos3);

        let mut small = image::RgbaImage::new(24, 24);

        for y in 0..24 {
            for x in 0..24 {
                let mut r = 0u32;
                let mut g = 0u32;
                let mut b = 0u32;
                let mut a = 0u32;

                for oy in 0..2 {
                    for ox in 0..2 {
                        let pixel = big.get_pixel(x * 2 + ox, y * 2 + oy);

                        r += pixel[0] as u32;
                        g += pixel[1] as u32;
                        b += pixel[2] as u32;
                        a += pixel[3] as u32;
                    }
                }

                small.put_pixel(
                    x,
                    y,
                    image::Rgba([(r / 4) as u8, (g / 4) as u8, (b / 4) as u8, (a / 4) as u8]),
                );
            }
        }

        encode_icon(
            &image::DynamicImage::ImageRgba8(small),
            &mut self.small_icon_data,
        );
        encode_icon(&big, &mut self.big_icon_data);

        Ok(())
    }

    pub fn valid(data: &[u8]) -> bool {
        data.len() >= 4 && u32::from_le_bytes([data[0], data[1], data[2], data[3]]) == MAGIC_SMDH
    }

    pub fn get_short_description(&self, language: usize) -> String {
        decode_text(&self.titles[language].short_description)
    }

    pub fn set_short_description(&mut self, language: usize, value: &str) {
        encode_text(value, &mut self.titles[language].short_description);
    }

    pub fn get_long_description(&self, language: usize) -> String {
        decode_text(&self.titles[language].long_description)
    }

    pub fn set_long_description(&mut self, language: usize, value: &str) {
        encode_text(value, &mut self.titles[language].long_description);
    }

    pub fn get_publisher(&self, language: usize) -> String {
        decode_text(&self.titles[language].publisher)
    }

    pub fn set_publisher(&mut self, language: usize, value: &str) {
        encode_text(value, &mut self.titles[language].publisher);
    }

    pub fn load<P: AsRef<Path>>(path: P) -> io::Result<Self> {
        let data = fs::read(path)?;

        if !Self::valid(&data) {
            return Err(io::Error::new(
                io::ErrorKind::InvalidData,
                "[!] Invalid SMDH file",
            ));
        }

        let mut cursor = 0;

        let magic = read_u32(&data, &mut cursor)?;

        let _version = read_u16(&data, &mut cursor)?;

        let _reserved = read_u16(&data, &mut cursor)?;

        if magic != MAGIC_SMDH {
            return Err(io::Error::new(
                io::ErrorKind::InvalidData,
                "[!] Invalid SMDH magic",
            ));
        }

        let mut smdh = Self::new();

        for title in &mut smdh.titles {
            for value in &mut title.short_description {
                *value = read_u16(&data, &mut cursor)?;
            }

            for value in &mut title.long_description {
                *value = read_u16(&data, &mut cursor)?;
            }

            for value in &mut title.publisher {
                *value = read_u16(&data, &mut cursor)?;
            }
        }

        smdh.settings
            .game_ratings
            .copy_from_slice(read_bytes(&data, &mut cursor, 0x10)?);

        smdh.settings.region_lock = read_u32(&data, &mut cursor)?;

        smdh.settings
            .match_maker_id
            .copy_from_slice(read_bytes(&data, &mut cursor, 0x0C)?);

        smdh.settings.flags = read_u32(&data, &mut cursor)?;

        smdh.settings.eula_version = read_u16(&data, &mut cursor)?;

        smdh.settings.reserved = read_u16(&data, &mut cursor)?;

        smdh.settings.default_frame = read_u32(&data, &mut cursor)?;

        smdh.settings.cec_id = read_u32(&data, &mut cursor)?;

        smdh.reserved
            .copy_from_slice(read_bytes(&data, &mut cursor, 0x08)?);

        for value in &mut smdh.small_icon_data {
            *value = read_u16(&data, &mut cursor)?;
        }

        for value in &mut smdh.big_icon_data {
            *value = read_u16(&data, &mut cursor)?;
        }

        Ok(smdh)
    }

    pub fn save<P: AsRef<Path>>(&self, path: P) -> io::Result<()> {
        let mut file = fs::File::create(path)?;

        write_u32(&mut file, MAGIC_SMDH)?;

        write_u16(&mut file, 0)?;

        write_u16(&mut file, 0)?;

        for title in &self.titles {
            for value in title.short_description {
                write_u16(&mut file, value)?;
            }

            for value in title.long_description {
                write_u16(&mut file, value)?;
            }

            for value in title.publisher {
                write_u16(&mut file, value)?;
            }
        }

        file.write_all(&self.settings.game_ratings)?;

        write_u32(&mut file, self.settings.region_lock)?;

        file.write_all(&self.settings.match_maker_id)?;

        write_u32(&mut file, self.settings.flags)?;

        write_u16(&mut file, self.settings.eula_version)?;

        write_u16(&mut file, self.settings.reserved)?;

        write_u32(&mut file, self.settings.default_frame)?;

        write_u32(&mut file, self.settings.cec_id)?;

        file.write_all(&self.reserved)?;

        for value in self.small_icon_data {
            write_u16(&mut file, value)?;
        }

        for value in self.big_icon_data {
            write_u16(&mut file, value)?;
        }

        Ok(())
    }

    pub fn save_to_user_files(&self, filename: &str) -> io::Result<PathBuf> {
        let user_files = std::env::current_exe()?
            .parent()
            .ok_or_else(|| io::Error::other("Binary path error"))?
            .join("DATA")
            .join("USER_FILES");

        fs::create_dir_all(&user_files)?;

        let path = user_files.join(filename);

        self.save(&path)?;

        Ok(path)
    }

    fn fill_default_icons(&mut self) {
        for pixel in &mut self.small_icon_data {
            *pixel = encode_rgb565(40, 40, 40, 255);
        }

        for pixel in &mut self.big_icon_data {
            *pixel = encode_rgb565(40, 40, 40, 255);
        }
    }
}

fn encode_icon(image: &image::DynamicImage, destination: &mut [u16]) {
    let rgba = image.to_rgba8();

    let width = rgba.width() as usize;
    let height = rgba.height() as usize;

    for tile_y in (0..height).step_by(8) {
        for tile_x in (0..width).step_by(8) {
            let tile_index = (tile_y / 8) * (width / 8) + (tile_x / 8);
            let destination_offset = tile_index * 64;

            for (index, &pixel_index) in TILE_ORDER.iter().enumerate() {
                let pixel_index = pixel_index as usize;

                let x = tile_x + (pixel_index % 8);
                let y = tile_y + (pixel_index / 8);

                let pixel = rgba.get_pixel(x as u32, y as u32);

                destination[destination_offset + index] =
                    encode_rgb565(pixel[0], pixel[1], pixel[2], pixel[3]);
            }
        }
    }
}

fn encode_rgb565(r: u8, g: u8, b: u8, a: u8) -> u16 {
    let r = ((r as u32 * a as u32) / 255) as u8;
    let g = ((g as u32 * a as u32) / 255) as u8;
    let b = ((b as u32 * a as u32) / 255) as u8;

    let r = (r >> 3) as u16;
    let g = (g >> 2) as u16;
    let b = (b >> 3) as u16;

    (r << 11) | (g << 5) | b
}

fn encode_text(text: &str, destination: &mut [u16]) {
    destination.fill(0);

    for (index, value) in text.encode_utf16().take(destination.len()).enumerate() {
        destination[index] = value;
    }
}

fn decode_text(source: &[u16]) -> String {
    let length = source
        .iter()
        .position(|value| *value == 0)
        .unwrap_or(source.len());

    String::from_utf16_lossy(&source[..length])
}

fn read_u16(data: &[u8], cursor: &mut usize) -> io::Result<u16> {
    let bytes = read_bytes(data, cursor, 2)?;

    Ok(u16::from_le_bytes([bytes[0], bytes[1]]))
}

fn read_u32(data: &[u8], cursor: &mut usize) -> io::Result<u32> {
    let bytes = read_bytes(data, cursor, 4)?;

    Ok(u32::from_le_bytes([bytes[0], bytes[1], bytes[2], bytes[3]]))
}

fn read_bytes<'a>(data: &'a [u8], cursor: &mut usize, count: usize) -> io::Result<&'a [u8]> {
    if *cursor + count > data.len() {
        return Err(io::Error::new(
            io::ErrorKind::UnexpectedEof,
            "[!] Unexpected end of SMDH file",
        ));
    }

    let bytes = &data[*cursor..*cursor + count];

    *cursor += count;

    Ok(bytes)
}

fn write_u16<W: Write>(writer: &mut W, value: u16) -> io::Result<()> {
    writer.write_all(&value.to_le_bytes())
}

fn write_u32<W: Write>(writer: &mut W, value: u32) -> io::Result<()> {
    writer.write_all(&value.to_le_bytes())
}
