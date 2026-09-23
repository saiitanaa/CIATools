mod bannertool;
mod delete;
mod icncreator;
mod import;
mod make;
mod makerom;
mod picker;
mod rsfcreator;
mod titleid;
mod utils;

use std::{fs, io, path::PathBuf, process::Command};
use crossterm::event::{self, Event, KeyCode, KeyEventKind};
use ratatui::{
    DefaultTerminal, Frame,
    buffer::Buffer,
    layout::{Constraint, Direction, Layout, Rect},
    style::{Color, Stylize},
    symbols::border,
    text::Line,
    widgets::{Block, Borders, Paragraph, Widget},
};

use crate::import::import_files;
use crate::rsfcreator::rsf_config;

const VERSION: &str = "v26.3.0";

fn main() -> io::Result<()> {
    print!("\x1b]0;CIAToolsN {}\x07", VERSION);

    set_directories()?;

    let romfs_path = std::env::current_dir()?.join("romfs");
    fs::create_dir_all(&romfs_path)?;

    let author = load_author()?;

    let mut app = App {
        author,
        ..App::default()
    };

    app.update();

    let result = ratatui::run(|terminal| app.run(terminal));

    if romfs_path.exists() {
        fs::remove_dir_all(&romfs_path)?;
    }

    result
}

#[derive(Debug, Default)]
pub struct App {
    exit: bool,
    output: Vec<String>,

    author: String,
    author_input: String,
    editing_author: bool,

    rsf_edit: bool,
    rsf_field: usize,
    rsf_input: String,
    rsf_config: rsf_config,

    icn_edit: bool,
    icn_field: usize,
    icn_language: usize,
    icn_input: String,
    icn_file: icncreator::IcnFile,
    icn_select_language: bool,
    icn_select_icon: bool,

    titleid_gen: bool,
    titleid: String,
    uniqueid_gen: bool,
    uniqueid: String,
}

fn set_directories() -> io::Result<PathBuf> {
    let bin_path = std::env::current_exe()?
        .parent()
        .ok_or_else(|| io::Error::other("Get binary error!"))?
        .to_path_buf();

    let user_files = bin_path.join("DATA").join("USER_FILES");

    fs::create_dir_all(&user_files)?;

    Ok(user_files)
}

fn find_file_with_extension(directory: &PathBuf, extension: &str) -> io::Result<PathBuf> {
    for entry in fs::read_dir(directory)? {
        let path = entry?.path();

        if path
            .extension()
            .and_then(|ext| ext.to_str())
            .is_some_and(|ext| ext.eq_ignore_ascii_case(extension))
        {
            return Ok(path);
        }
    }

    Err(io::Error::new(
        io::ErrorKind::NotFound,
        format!("No .{extension} file found"),
    ))
}

impl App {
    pub fn run(&mut self, terminal: &mut DefaultTerminal) -> io::Result<()> {
        terminal.draw(|frame| self.draw(frame))?;
        while !self.exit {
            self.handle_events(terminal)?;
            terminal.draw(|frame| self.draw(frame))?;
        }

        Ok(())
    }

    fn update(&mut self) {
        use serde::Deserialize;

        #[derive(Deserialize)]
        struct Release {
            tag_name: String,
        }

        match reqwest::blocking::Client::new()
            .get("https://api.github.com/repos/saiitanaa/CIATools/releases/latest")
            .header("User-Agent", "CIATools")
            .send()
        {
            Ok(response) => match response.json::<Release>() {
                Ok(release) => {
                    self.output.push(format!("[!] Latest release: {}", release.tag_name));

                    if release.tag_name != VERSION {
                        self.output.push(r"[+] New update available :)".to_string());
                    } else {
                        self.output.push("[+] Up to date ;3".to_string());
                    }
                }

                Err(_) => {
                    self.output.push("[!] Failed to parse!".to_string());
                }
            },

            Err(_) => {
                self.output.push("[!] Check update failed!".to_string());
            }
        }
    }

    fn draw(&self, frame: &mut Frame) {
        let outer_layout = Layout::default()
            .direction(Direction::Horizontal)
            .margin(1)
            .constraints([
                Constraint::Percentage(40),
                Constraint::Percentage(40),
                Constraint::Percentage(20),
            ])
            .split(frame.area());

        frame.render_widget(self, outer_layout[0]);

        frame.render_widget(
            Paragraph::new(vec![
                Line::from("1 : Import HB Files"),
                Line::from("2 : Create RSF"),
                Line::from("3 : Create ICN"),
                Line::from("4 : Set Author"),
                Line::from("5 : Create TitleID"),
                Line::from("6 : Create UniqueID"),
                Line::from(""),
                Line::from(r"C : Make ¯\_(ツ)_/¯"),
                Line::from("0 : Clean USER_FILES"),
                Line::from("9 : Open USER_FILES"),
                Line::from(""),
                Line::from("K : Clear Console"),
                Line::from("Q : Quit"),
                Line::from("H : Help !!"),
                Line::from(""),
                Line::from("Y : Check Updates"),
            ])
            .block(
                Block::new()
                    .bold()
                    .fg(Color::LightBlue)
                    .title(" INPUT ".bold())
                    .borders(Borders::ALL),
            ),
            outer_layout[2],
        );

        let output_lines: Vec<Line> = std::iter::once(Line::from(">_ "))
            .chain(self.output.iter().map(|s| Line::from(s.as_str())))
            .collect();

        frame.render_widget(
            Paragraph::new(output_lines).style(Color::White).block(
                Block::bordered()
                    .bold()
                    .fg(Color::Magenta)
                    .title(" CONSOLE ".bold())
                    .border_set(border::THICK),
            ),
            outer_layout[1],
        );
    }

    fn handle_events(&mut self, terminal: &mut DefaultTerminal) -> io::Result<()> {
        if let Event::Key(key) = event::read()? {
            if key.kind != KeyEventKind::Press {
                return Ok(());
            }

            if self.editing_author {
                match key.code {
                    KeyCode::Enter => {
                        self.author = self.author_input.clone();
                        save_author(&self.author)?;
                        self.editing_author = false;
                        self.output.push(format!("[+] Author set to: {}", self.author));
                    }

                    KeyCode::Backspace => {
                        self.author_input.pop();
                    }

                    KeyCode::Char(c) => {
                        self.author_input.push(c);
                    }

                    KeyCode::Esc => {
                        self.editing_author = false;
                    }

                    _ => {}
                }

                return Ok(());
            }

            if self.rsf_edit {
                match key.code {
                    KeyCode::Char(c) => {
                        self.rsf_input.push(c);
                    }

                    KeyCode::Backspace => {
                        self.rsf_input.pop();
                    }

                    KeyCode::Enter => {
                        match self.rsf_field {
                            0 => self.rsf_config.title = self.rsf_input.clone(),
                            1 => self.rsf_config.companyCode = self.rsf_input.clone(),
                            2 => self.rsf_config.productCode = self.rsf_input.clone(),
                            3 => self.rsf_config.romFsPath = self.rsf_input.clone(),
                            4 => self.rsf_config.uniqueId = self.rsf_input.clone(),
                            5 => self.rsf_config.saveDataSize = self.rsf_input.clone(),
                            6 => self.rsf_config.cpuSpeed = self.rsf_input.clone(),
                            _ => {}
                        }

                        self.rsf_input.clear();
                        self.rsf_field += 1;

                        if self.rsf_field > 6 {
                            self.rsf_edit = false;

                            let content = self.rsf_config.generate();
                            let path = user_files_path()?.join("config.rsf");

                            fs::write(path, content)?;
                            self.output.push("[+] RSF file created.".to_string());
                        }
                    }

                    KeyCode::Esc => {
                        self.rsf_edit = false;
                        self.rsf_input.clear();
                    }

                    _ => {}
                }

                return Ok(());
            }

            if self.icn_edit {
                if self.icn_select_language {
                    match key.code {
                        KeyCode::Left => {
                            if self.icn_language == 0 {
                                self.icn_language = 11;
                            } else {
                                self.icn_language -= 1;
                            }
                        }

                        KeyCode::Right => {
                            self.icn_language = (self.icn_language + 1) % 12;
                        }

                        KeyCode::Enter => {
                            self.icn_select_language = false;
                        }

                        KeyCode::Esc => {
                            self.icn_edit = false;
                            self.icn_select_language = false;
                            self.icn_input.clear();
                        }

                        _ => {}
                    }

                    return Ok(());
                }

                if self.icn_select_icon {
                    match key.code {
                        KeyCode::Enter => {
                            ratatui::restore();

                            let result = crate::picker::pick_files();

                            *terminal = ratatui::init();

                            match result {
                                Ok(Some(files)) => {
                                    if let Some(path) = files.first() {
                                        let title = self
                                            .icn_file
                                            .get_short_description(
                                                self.icn_language,
                                            );

                                        let publisher = self
                                            .icn_file
                                            .get_publisher(
                                                self.icn_language,
                                            );

                                        let output = user_files_path()?.join("icon.icn");

                                        match crate::bannertool::make_icn(
                                            &title,
                                            &publisher,
                                            path,
                                            &output,
                                        ) {
                                            Ok(()) => {
                                                self.output.push(format!("[+] ICN created: {}", output.display()));
                                                self.icn_select_icon = false;
                                                self.icn_edit = false;
                                            }

                                            Err(error) => {
                                                self.output.push(format!("[!] bannertool failed: {}", error));
                                            }
                                        }
                                    }
                                }

                                Ok(None) => {
                                    self.output.push("[!] No icon selected.".to_string());
                                }

                                Err(error) => {
                                    self.output.push(format!("[!] Icon picker error: {}", error));
                                }
                            }
                        }

                        KeyCode::Esc => {
                            self.icn_select_icon = false;
                            self.icn_edit = false;
                        }

                        _ => {}
                    }

                    return Ok(());
                }

                match key.code {
                    KeyCode::Esc => {
                        self.icn_edit = false;
                        self.icn_input.clear();
                    }

                    KeyCode::Backspace => {
                        self.icn_input.pop();
                    }

                    KeyCode::Enter => match self.icn_field {
                        0 => {
                            self.icn_file.set_short_description(self.icn_language, &self.icn_input);
                            self.icn_input.clear();
                            self.icn_field = 1;
                        }

                        1 => {
                            self.icn_file.set_long_description(self.icn_language, &self.icn_input);
                            self.icn_input.clear();
                            self.icn_field = 2;
                        }

                        2 => {
                            self.icn_file.set_publisher(self.icn_language, &self.icn_input);
                            self.icn_input.clear();
                            self.icn_select_icon = true;
                        }

                        _ => {}
                    },

                    KeyCode::Char(c) => {
                        self.icn_input.push(c);
                    }

                    _ => {}
                }

                return Ok(());
            }

            if self.titleid_gen {
                match key.code {
                    KeyCode::Enter => {
                               
                    }

                    KeyCode::Char('r') | KeyCode::Char('R') => {
                        self.titleid = crate::titleid::generate();
                    }

                    KeyCode::Esc => {
                        self.titleid_gen = false;
                    }
                    _ => {}
                }
                return Ok(());
            }

            if self.uniqueid_gen {
                match key.code {
                    KeyCode::Enter => {}

                    KeyCode::Esc => {
                        self.uniqueid_gen = false;
                    }
                    _ => {}
                }
                return Ok(());
            }

            match key.code {
                KeyCode::Char('q') | KeyCode::Char('Q') => {
                    self.exit = true;
                }

                KeyCode::Char('h') | KeyCode::Char('H') => {
                    #[cfg(target_os = "macos")]
                    {
                        self.output.push("[!] NO PROBLEM !!".to_string());
                        Command::new("open")
                            .arg("https://github.com/saiitanaa/CIATools/blob/main/README.md")
                            .status()
                            .ok();
                    }

                    #[cfg(target_os = "windows")]
                    {
                        self.output.push("[!] NO PROBLEM !!!".to_string());
                        Command::new("start")
                            .arg("https://github.com/saiitanaa/CIATools/blob/main/README.md")
                            .status()
                            .ok();
                    }

                    #[cfg(target_os = "linux")]
                    {
                        self.output.push("[!] NO PROBLEM !!!!".to_string());
                        Command::new("xdg-open")
                            .arg("https://github.com/saiitanaa/CIATools/blob/main/README.md")
                            .status()
                            .ok();
                    }
                }

                KeyCode::Char('k') | KeyCode::Char('K') => {
                    self.output.clear();
                }

                KeyCode::Char('y') | KeyCode::Char('Y') => {
                    self.update();
                }

                KeyCode::Char('1') => {
                    self.output.push("[?] Import FileDialog".to_string());
                    ratatui::restore();

                    let result = user_files_path().and_then(import_files);
                    *terminal = ratatui::init();

                    match result {
                        Ok(files) => {
                            for file in files {
                                self.output.push(format!("[+] {file}"));
                            }
                        }

                        Err(error) => {
                            self.output.push(format!("[!] {error}"));
                        }
                    }
                }

                KeyCode::Char('2') => {
                    self.rsf_edit = true;
                    self.rsf_field = 0;
                    self.rsf_input.clear();
                    self.rsf_config = rsf_config::default();
                }

                KeyCode::Char('3') => {
                    self.icn_edit = true;
                    self.icn_select_language = true;
                    self.icn_select_icon = false;
                    self.icn_field = 0;
                    self.icn_language = 1;
                    self.icn_input.clear();
                    self.icn_file = icncreator::IcnFile::new();
                }

                KeyCode::Char('4') => {
                    self.editing_author = true;
                }

                KeyCode::Char('5') => {
                    self.titleid_gen = true;
                    self.titleid = crate::titleid::generate();   
                }

                KeyCode::Char('6') => {
                    self.uniqueid_gen = true;
                }

                KeyCode::Char('C') | KeyCode::Char('c') => {
                    self.output.push("[+] CIA Compiling...".to_string());
                    match user_files_path() {
                        Ok(user_files) => {
                            let elf = match find_file_with_extension(&user_files, "elf") {
                                    Ok(path) => path,

                                    Err(error) => {
                                        self.output.push(format!("[!] {error}"));
                                        return Ok(());
                                    }
                                };

                            let rsf =
                                match find_file_with_extension(
                                    &user_files,
                                    "rsf",
                                ) {
                                    Ok(path) => path,

                                    Err(error) => {
                                        self.output.push(format!("[!] {error}"));
                                        return Ok(());
                                    }
                                };

                            let icon =
                                match find_file_with_extension(
                                    &user_files,
                                    "icn",
                                ) {
                                    Ok(path) => path,

                                    Err(error) => {
                                        self.output.push(format!("[!] {error}"));
                                        return Ok(());
                                    }
                                };

                            let banner = find_file_with_extension(&user_files, "bin")
                            .or_else(|_| {
                                find_file_with_extension(&user_files, "bnr")
                            })
                            .ok();

                            let output = elf.with_extension("cia");
                            self.output.push(format!("[+] Building CIA: {}", elf.display()));

                            let result = crate::makerom::build_cia(elf.to_string_lossy().as_ref(), rsf.to_string_lossy().as_ref(), icon.to_string_lossy().as_ref(),
                                banner
                                    .as_ref()
                                    .map(|path| {
                                        path.to_string_lossy().to_string()
                                    })
                                    .as_deref(),
                                output.to_string_lossy().as_ref(),
                            );

                            if result == 0 {
                                self.output.push(format!("[+] CIA created: {}", output.display()));
                            } else {
                                self.output.push(format!("[!] makerom failed with code: {result}"));
                            }
                        }

                        Err(error) => {
                            self.output.push(format!("[!] {error}"));
                        }
                    }
                }

                KeyCode::Char('0') => match user_files_path() {
                    Ok(path) => {
                        match crate::delete::clean_user_files(path) {
                            Ok(()) => {
                                self.output.push("[-] USER_FILES cleaned.".to_string());
                            }

                            Err(error) => {
                                self.output.push(format!("[!] {error}"));
                            }
                        }
                    }

                    Err(error) => {
                        self.output.push(format!("[!] {error}"));
                    }
                },

                KeyCode::Char('9') => {
                    match user_files_path() {
                        Ok(path) => {
                            #[cfg(target_os = "macos")]
                            let result =
                                Command::new("open").arg(&path).spawn();

                            #[cfg(target_os = "linux")]
                            let result =
                                Command::new("xdg-open").arg(&path).spawn();

                            #[cfg(target_os = "windows")]
                            let result =
                                Command::new("explorer").arg(&path).spawn();

                            match result {
                                Ok(_) => {
                                    self.output.push("[+] Open USER_FILES".to_string());
                                }

                                Err(error) => {
                                    self.output.push(format!("[!] Failed to open USER_FILES: {error}"));
                                }
                            }
                        }

                        Err(error) => {
                            self.output.push(format!("[!] Failed to open USER_FILES: {error}"));
                        }
                    }
                }

                _ => {}
            }
        }

        Ok(())
    }
}

fn user_files_path() -> io::Result<PathBuf> {
    let bin_path = std::env::current_exe()?
        .parent()
        .ok_or_else(|| {
            io::Error::other("[!] Binary path error !?")
        })?
        .to_path_buf();

    let user_files = bin_path.join("DATA").join("USER_FILES");

    fs::create_dir_all(&user_files)?;
    Ok(user_files)
}

fn load_author() -> io::Result<String> {
    let path = user_files_path()?.join("author.txt");

    if path.exists() {
        return Ok(fs::read_to_string(path)?.trim().to_string());
    }
    Ok(String::new())
}

fn save_author(author: &str) -> io::Result<()> {
    let path = user_files_path()?.join("author.txt");
    fs::write(path, author)?;
    Ok(())
}

impl Widget for &App {
    fn render(
        self,
        area: Rect,
        buf: &mut Buffer,
    ) {
        let hostname = hostname::get().map(|h| h.to_string_lossy().into_owned()).unwrap_or_else(|_| "unknown".to_string());
        let mut lines = vec![Line::from(""), Line::from(format!("Hi ! {hostname} 👋"))];

        if self.editing_author {
            lines.push(Line::from(""));
            lines.push(Line::from(format!("Enter author: {}", self.author_input)).fg(Color::White));

            lines.push(Line::from("Enter: Next    Esc: Cancel").fg(Color::DarkGray));
        } else if !self.author.is_empty() {
            lines.push(Line::from(""));
            lines.push(Line::from(format!("Author defined: {}", self.author)).fg(Color::White));
        }

        if self.rsf_edit {
            lines.push(Line::from(""));
            lines.push(Line::from("RSF-Creator").bold().fg(Color::Yellow));
            lines.push(Line::from(""));

            let (prompt, example) = match self.rsf_field {
                0 => ("Title:", "(Ex: The best Homebrew)"),
                1 => ("CompanyCode:", "(Ex: SAAA)"),
                2 => ("ProductCode:", "(Ex: CTR-P-XXXX)"),
                3 => ("RomFs Path:", "(Ex: ./romfs)"),
                4 => ("UniqueId:", "(Ex: 0x7BE00)"),
                5 => ("SaveDataSize:", "(Ex: 128KB)"),
                6 => ("CpuSpeed:", "(804MHz New 3DS, 268MHz Old 3DS)"),
                _ => ("", ""),
            };

            lines.push(Line::from(format!("{} {}", prompt, self.rsf_input)).fg(Color::White));
            lines.push(Line::from(example).fg(Color::DarkGray));
            lines.push(Line::from(""));
            lines.push(Line::from("Enter: Next    Esc: Cancel").fg(Color::DarkGray));
        }

        if self.titleid_gen {
            lines.push(Line::from(""));
            lines.push(Line::from("TitleID Creator").bold().fg(Color::Yellow));
            lines.push(Line::from(""));
            lines.push(Line::from(self.titleid.as_str()).fg(Color::White));
            lines.push(Line::from(""));
            lines.push(Line::from("Enter: Copy  R: New  Esc: Cancel").fg(Color::DarkGray));
        }

        if self.uniqueid_gen {
            lines.push(Line::from(""));
            lines.push(Line::from("UniqueID Creator").bold().fg(Color::Yellow));
            lines.push(Line::from(""));
            lines.push(Line::from("Enter: Select    Esc: Cancel"));
        }

        if self.icn_edit {
            lines.push(Line::from(""));

            if self.icn_select_language {
                lines.push(Line::from("ICN Creator").bold().fg(Color::Yellow));
                lines.push(Line::from(""));
                lines.push(Line::from(format!("Language: {}", icncreator::ICN_LANGUAGES[self.icn_language])).fg(Color::Yellow));
                lines.push(Line::from(""));
                lines.push(Line::from("<- / -> Change language").fg(Color::DarkGray));
                lines.push(Line::from("Enter: Select    Esc: Cancel").fg(Color::DarkGray));
            } else if self.icn_select_icon {
                lines.push(Line::from("ICN Creator").bold().fg(Color::Yellow));
                lines.push(Line::from(""));
                lines.push(Line::from("Icon").bold().fg(Color::Yellow));
                lines.push(Line::from(""));
                lines.push(Line::from("Select an image for the ICN icon.").fg(Color::White));
                lines.push(Line::from("PNG / JPG / WebP").fg(Color::DarkGray));
                lines.push(Line::from(""));
                lines.push(Line::from("Enter: Select icon").fg(Color::DarkGray));
                lines.push(Line::from("Esc: Cancel").fg(Color::DarkGray));
            } else {
                let (prompt, example) = match self.icn_field {
                    0 => ("Title:", "(Ex: My Homebrew)"),
                    1 => (
                        "Description:",
                        "(Ex: My awesome 3DS application)",
                    ),
                    2 => ("Publisher:", "(Ex: Saiitanaa)"),
                    _ => ("", ""),
                };

                lines.push(Line::from("ICN Creator").bold().fg(Color::Yellow));
                lines.push(Line::from(""));
                lines.push(Line::from(format!("Language: {}", icncreator::ICN_LANGUAGES[self.icn_language])).fg(Color::Yellow));
                lines.push(Line::from(""));
                lines.push(Line::from(format!("{} {}", prompt, self.icn_input)).fg(Color::White));
                lines.push(Line::from(example).fg(Color::DarkGray));
                lines.push(Line::from(""));
                lines.push(Line::from("Enter: Next    Esc: Cancel").fg(Color::DarkGray));
            }
        }

        Paragraph::new(lines)
            .centered()
            .block(
                Block::bordered()
                    .title(" CIAToolsN -- Saiitanaa ".bold())
                    .border_set(border::THICK),
            )
            .render(area, buf);
    }
}