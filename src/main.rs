mod bannertool;
mod delete;
mod icncreator;
mod import;
mod make;
mod makerom;
mod picker;
mod rsfcreator;
mod titleid;
mod uniqueid;
mod utils;


use std::{fs, io, env, path::PathBuf, process::Command};
use crossterm::event::{self, Event, KeyCode, KeyEventKind};
use ratatui::{
    DefaultTerminal, Frame,
    buffer::Buffer,
    layout::{Constraint, Direction, Layout, Rect},
    style::{Color, Stylize},
    symbols::border,
    text::Line,
    widgets::{Block, Paragraph, Widget},
};

use crate::import::import_files;
use crate::rsfcreator::rsf_config;

use arboard::Clipboard;

const VERSION: &str = "v26.4.2";

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

    project: String,
    project_input: String,
    add_project: bool,

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
    let project_folder = bin_path.join("DATA").join("PROJECT_DATA");
    fs::create_dir_all(&user_files)?;
    fs::create_dir_all(&project_folder)?;
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
                    self.output.push(format!("[?] Latest GitHub release: {}", release.tag_name));
                }

                Err(_) => {
                    self.output.push("[UPDATE] Failed to parse!".to_string());
                }
            },

            Err(_) => {
                self.output.push("[!] Check update failed!".to_string());
            }
        }
    }

fn draw(&self, frame: &mut Frame) {
    let main_layout = Layout::default()
        .direction(Direction::Vertical)
        .margin(1)
        .constraints([
            Constraint::Min(0), 
            Constraint::Length(10), 
        ])
        .split(frame.area());

    let top_layout = Layout::default()
        .direction(Direction::Horizontal)
        .constraints([
            Constraint::Percentage(40), // Console
            Constraint::Percentage(60), // CIAToolsN
        ])
        .split(main_layout[0]);

    // Console/output
    let output_lines: Vec<Line> = std::iter::once(Line::from(""))
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
        top_layout[1],
    );

    // Main UI
    frame.render_widget(self, top_layout[0]);
    let input_layout = Layout::default()
        .direction(Direction::Horizontal)
        .constraints([
            Constraint::Percentage(25), // Creator Tools
            Constraint::Percentage(20), // Editor Tools
            Constraint::Percentage(30), // Project Manager
            Constraint::Percentage(25), // CIATools Options
        ])
        .split(main_layout[1]);

    // Creator Tools
    frame.render_widget(
        Paragraph::new(vec![
            Line::from("[1] Import HB Files"),
            Line::from("[2] Create RSF"),
            Line::from("[3] Create ICN"),
            Line::from("[5] Create TitleID"),
        ])
        .block(
            Block::bordered()
                .fg(Color::White)
                .title(" Creator Tools ".bold()),
        ),
        input_layout[0],
    );

    // Editor Tools
    frame.render_widget(
        Paragraph::new(vec![
            Line::from("[6] Edit RSF"),
            Line::from("[4] Set Author"),
        ])
        .block(
            Block::bordered()
                .fg(Color::White)
                .title(" Editor Tools ".bold()),
        ),
        input_layout[1],
    );

    // Project Manager
    frame.render_widget(
        Paragraph::new(vec![
            Line::from("[C] Make CIA"),
            Line::from("[S] Save Project"),
            Line::from("[9] Open USER_FILES"),
            Line::from("[0] Open PROJECT_DATA"),
            //rm -rf boum zone
            Line::from(""),
            Line::from("DANGER ZONE").fg(Color::Red).bold(),
            Line::from("[O] Wipe USER_FILES"),
            Line::from("[J] Wipe PROJECT_DATA"),
        ])
        .block(
            Block::bordered()
                .fg(Color::White)
                .title(" Project Manager ".bold()),
        ),

        input_layout[2],
    );

    // CIATools Options
    frame.render_widget(
        Paragraph::new(vec![
            Line::from("[L] List Project Folder").fg(Color::White),
            Line::from("[K] Clear Console").fg(Color::White),
            Line::from("[Q] Quit").fg(Color::White),
            Line::from(""),
            Line::from("[D] Download Latest Version").fg(Color::Green),
            Line::from("[H] Using CIAToolsN").fg(Color::White),
            Line::from("[P] Report Bug").fg(Color::White),
            Line::from("[R] Submit a request <3").fg(Color::White),
        ])
        .block(
            Block::bordered()
                //.bold()
                .fg(Color::LightCyan)
                .title(" Options ".bold()),
        ),
        input_layout[3],
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

            if self.add_project {
                match key.code {
                    KeyCode::Enter => {
                        self.project = self.project_input.clone();
                        save_project(&self.project)?;
                        self.add_project = false;
                        self.output.push(format!("[+] Name: {}", self.project));
                    }

                    KeyCode::Backspace => {
                        self.project_input.pop();
                    }

                    KeyCode::Char(c) => {
                        self.project_input.push(c);
                    }

                    KeyCode::Esc => {
                        self.add_project = false;
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
                                                self.output.push(format!("[+] ICN Created: {}", output.display()));
                                                self.icn_select_icon = false;
                                                self.icn_edit = false;
                                            }

                                            Err(error) => {
                                                self.output.push(format!("[!] Bannertool Failed: {}", error));
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
                        let mut clipboard = Clipboard::new().unwrap();

                        match clipboard.set_text(&self.uniqueid) {
                            Ok(()) => {
                                self.output.push("[+] UniqueID copied to clipboard.".to_string());
                            }

                            Err(error) => {
                                self.output.push(format!("[!] Clipboard error: {error}"));
                            }
                        }
                    }

                    KeyCode::Char('t') | KeyCode::Char('T') => {
                        let mut clipboard = Clipboard::new().unwrap();

                        match clipboard.set_text(&self.titleid) {
                            Ok(()) => {
                                self.output.push("[+] TitleID copied to clipboard.".to_string());
                            }

                            Err(error) => {
                                self.output.push(format!("[!] Clipboard error: {error}"));
                            }
                        }
                    }

                    KeyCode::Char('r') | KeyCode::Char('R') => {
                        self.titleid = crate::titleid::generate();
                        self.uniqueid = crate::uniqueid::from_title_id(&self.titleid);

                        self.output.push("[+] New TitleID Generated".to_string());
                    }

                    KeyCode::Esc => {
                        self.titleid_gen = false;
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
                    #[cfg(target_os = "macos")] {
                        self.output.push("[!] Redirect to USAGE.md".to_string());
                        Command::new("open")
                            .arg("https://github.com/saiitanaa/CIATools/blob/main/USAGE.md")
                            .status()
                            .ok();
                    }

                    #[cfg(target_os = "windows")] {
                        self.output.push("[!] Redirect to USAGE.md".to_string());
                        Command::new("explorer")
                            .arg("https://github.com/saiitanaa/CIATools/blob/main/USAGE.md")
                            .status()
                            .ok();
                    }

                    #[cfg(target_os = "linux")] {
                        self.output.push("[!] Redirect to USAGE.md".to_string());
                        Command::new("xdg-open")
                            .arg("https://github.com/saiitanaa/CIATools/blob/main/USAGE.md")
                            .status()
                            .ok();
                    }
                }

                KeyCode::Char('p') | KeyCode::Char('P') => {
                    #[cfg(target_os = "macos")]
                        self.output.push("[!] Redirect to ISSUES".to_string());
                        Command::new("open")
                            .arg("https://github.com/saiitanaa/CIATools/issues")
                            .status()
                            .ok();

                    #[cfg(target_os = "windows")]
                        self.output.push("[!] Redirect to ISSUES".to_string());
                        Command::new("explorer")
                            .arg("https://github.com/saiitanaa/CIATools/issues")
                            .status()
                            .ok();

                    #[cfg(target_os = "linux")]
                        self.output.push("[!] Redirect to ISSUES".to_string());
                        Command::new("xdg-open")
                            .arg("https://github.com/saiitanaa/CIATools/issues")
                            .status()
                            .ok();
                }  

                KeyCode::Char('r') | KeyCode::Char('R') => {
                    #[cfg(target_os = "macos")]
                    self.output.push("[!] Redirect to PR".to_string());
                    Command::new("open")
                        .arg("https://github.com/saiitanaa/CIATools/pulls")
                        .status()
                        .ok();
                    #[cfg(target_os = "windows")]
                        self.output.push("[!] Redirect to PR".to_string());
                        Command::new("explorer")
                            .arg("https://github.com/saiitanaa/CIATools/pulls")
                            .status()
                            .ok();

                    #[cfg(target_os = "linux")]
                        self.output.push("[!] Redirect to PR".to_string());
                        Command::new("xdg-open")
                            .arg("https://github.com/saiitanaa/CIATools/pulls")
                            .status()
                            .ok();
                }

                KeyCode::Char('k') | KeyCode::Char('K') => {
                    self.output.clear();
                }

                KeyCode::Char('l') | KeyCode::Char('L') => {
                    let project_dir = env::current_exe()
                        .unwrap()
                        .parent()
                        .unwrap()
                        .join("DATA/PROJECT_DATA");

                    match std::fs::read_dir(project_dir) {
                        Ok(entries) => {
                            for entry in entries.flatten() {
                                self.output.push(format!("- {}", entry.file_name().to_string_lossy()));
                            }
                        }
                        Err(e) => self.output.push(format!("[!] - {e}")),
                    }
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

                KeyCode::Char('S') | KeyCode::Char('s') => {
                    self.output.push("[+] Save DATA/PROJECT_DATA...".to_string());
                    self.add_project = true;
                }

                KeyCode::Char('5') => {
                    self.titleid_gen = true;
                    self.titleid = crate::titleid::generate();   
                }  

                KeyCode::Char('6') => {
                    if let Some(file) = std::fs::read_dir(user_files_path().unwrap())
                        .ok()
                        .and_then(|mut r| {
                            r.find_map(|e| {
                                let p = e.ok()?.path();
                                (p.extension()? == "rsf").then_some(p)
                            })
                        })
                    {
                        #[cfg(target_os = "windows")]
                            Command::new("notepad").arg(&file).status().ok();

                        #[cfg(target_os = "macos")]
                            Command::new("open").arg("-e").arg(&file).status().ok();

                        #[cfg(target_os = "linux")]
                            Command::new("xdg-open").arg(&file).status().ok();
                    }
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
                                self.output.push(format!(r"[+] CIA created :  {}", output.display()));
                            } else {
                                self.output.push(format!("[!] makerom failed with code: {result}"));
                            }
                        }

                        Err(error) => {
                            self.output.push(format!("[!] {error}"));
                        }
                    }
                }

                KeyCode::Char('O') | KeyCode::Char('o') => match user_files_path() {
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

                KeyCode::Char('J') | KeyCode::Char('j') => match project_folder_path() {
                    Ok(path) => {
                        match crate::delete::clean_project_data(path) {
                            Ok(()) => {
                                self.output.push("[-] PROJECT_DATA cleaned.".to_string());
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

                KeyCode::Char('D') | KeyCode::Char('d') => {
                    #[cfg(all(target_os = "macos", target_arch = "aarch64"))]
                        Command::new("curl")
                            .args(["-L","-s","-f","-o","CIAToolsN",
                                "https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-OSX-aarch64",
                            ])
                            .stdout(std::process::Stdio::null())
                            .stderr(std::process::Stdio::null())
                            .status()?;

                    #[cfg(all(target_os = "windows", target_arch = "aarch64"))]
                        Command::new("curl.exe")
                            .args(["-L","-s","-f","-o","CIAToolsN.exe",
                                "https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Win-aarch64.exe",
                            ])
                            .stdout(std::process::Stdio::null())
                            .stderr(std::process::Stdio::null())
                            .status()?;

                    #[cfg(all(target_os = "windows", target_arch = "x86_64"))]        
                        Command::new("curl.exe")
                            .args(["-L","-s","-f","-o","CIAToolsN.exe",
                                "https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Win-x64.exe",
                            ])
                            .stdout(std::process::Stdio::null())
                            .stderr(std::process::Stdio::null())
                            .status()?;

                    #[cfg(all(target_os = "linux", target_arch = "x86_64"))]
                        Command::new("curl")
                            .args(["-L","-s","-f","-o","CIAToolsN",
                                "https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Linux-x64",
                            ])
                            .stdout(std::process::Stdio::null())
                            .stderr(std::process::Stdio::null())
                            .status()?;

                    #[cfg(all(target_os = "linux", target_arch = "aarch64"))]
                        Command::new("curl")
                            .args(["-L","-s","-f","-o","CIAToolsN",
                                "https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Linux-x64",
                            ])
                            .stdout(std::process::Stdio::null())
                            .stderr(std::process::Stdio::null())
                            .status()?;
                }

                KeyCode::Char('9') => {
                    match user_files_path() {
                        Ok(path) => {
                            #[cfg(target_os = "macos")]
                            let result = Command::new("open").arg(&path).spawn();

                            #[cfg(target_os = "linux")]
                            let result = Command::new("xdg-open").arg(&path).spawn();

                            #[cfg(target_os = "windows")]
                            let result = Command::new("explorer").arg(&path).spawn();

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

                KeyCode::Char('0') => {
                    match project_folder_path() {
                        Ok(path) => {
                            #[cfg(target_os = "macos")]
                            let result = Command::new("open").arg(&path).spawn();

                            #[cfg(target_os = "linux")]
                            let result = Command::new("xdg-open").arg(&path).spawn();

                            #[cfg(target_os = "windows")]
                            let result = Command::new("explorer").arg(&path).spawn();

                            match result {
                                Ok(_) => {
                                    self.output.push("[+] Open PROJECT_DATA".to_string());
                                }

                                Err(error) => {
                                    self.output.push(format!("[!] Failed to open PROJECT_DATA: {error}"));
                                }
                            }
                        }
                                Err(error) => {
                                    self.output.push(format!("[!] Failed to open PROJECT_DATA: {error}"));
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

fn project_folder_path() -> io::Result<PathBuf> {
    let project_path = std::env::current_exe()?
        .parent()
        .ok_or_else(|| {
            io::Error::other("[!] Project Folder PATH error!")
        })?
        .to_path_buf();

    let project_folder = project_path.join("DATA").join("PROJECT_DATA");
    fs::create_dir_all(&project_folder)?;
    Ok(project_folder)
}

fn load_author() -> io::Result<String> {
    let path = user_files_path()?.join("AUTHOR.txt");

    if path.exists() {
        return Ok(fs::read_to_string(path)?.trim().to_string());
    }
    Ok(String::new())
}

fn save_author(author: &str) -> io::Result<()> {
    let path = user_files_path()?.join("AUTHOR.txt");
    fs::write(path, author)?;
    Ok(())
}

fn save_project(project: &str) -> io::Result<()> {
    let path = project_folder_path()?.join(project);

    fs::create_dir_all(&path)?;

    let user_files = user_files_path()?;

    for entry in fs::read_dir(&user_files)? {
        let entry = entry?;
        let source = entry.path();
        let destination = path.join(entry.file_name());

        fs::rename(source, destination)?;
    }

    Ok(())
}


impl Widget for &App {
    fn render(
        self,
        area: Rect,
        buf: &mut Buffer,
    ) {
        let hostname = hostname::get().map(|h| h.to_string_lossy().into_owned()).unwrap_or_else(|_| "unknown".to_string());
        let mut lines = vec![Line::from(""), Line::from(format!("Bonjour, {hostname} !")), Line::from(format!("{VERSION} - Saiitanaa"))];

        if self.editing_author {
            lines.push(Line::from(""));
            lines.push(Line::from(format!("Enter author: {}", self.author_input)).fg(Color::White));

            lines.push(Line::from("Enter: Next    Esc: Cancel").fg(Color::DarkGray));
        } else if !self.author.is_empty() {
            lines.push(Line::from(""));
            lines.push(Line::from(format!("Author defined: {}", self.author)).fg(Color::White));
        }

        if self.add_project {
            lines.push(Line::from(""));
            lines.push(Line::from(format!("Add a project name : {}", self.project_input)).fg(Color::White));
            lines.push(Line::from("Enter: Confirm    Esc: Cancel").fg(Color::DarkGray));
        } else if !self.project.is_empty() {
            lines.push(Line::from(""));
            lines.push(Line::from(format!("Project name defined: {}", self.project)).fg(Color::White));
        }

        if self.rsf_edit {
            lines.push(Line::from(""));
            lines.push(Line::from("RSF-Creator").bold().fg(Color::Yellow));
            lines.push(Line::from(""));

            let (prompt, example) = match self.rsf_field {
                0 => ("Title:", "(Ex: My Homebrew)"),
                1 => ("CompanyCode:", "(Ex: SAAA)"),
                2 => ("ProductCode:", "(Ex: CTR-P-XXXX)"),
                3 => ("RomFs Path:", "(Ex: ./romfs)"),
                4 => ("UniqueId:", "No UniqueID? Press 5"),
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
            lines.push(Line::from("TitleID & UniqueID Creator").bold().fg(Color::Yellow));
            lines.push(Line::from(""));
            lines.push(Line::from("TitleID:"));
            lines.push(Line::from(self.titleid.as_str()).fg(Color::White));
            lines.push(Line::from(""));
            lines.push(Line::from("UniqueID:"));
            lines.push(Line::from(crate::uniqueid::from_title_id(&self.titleid)).fg(Color::White));
            lines.push(Line::from(""));
            lines.push(Line::from("Enter: Copy UniqueID  T: Copy TitleID").fg(Color::DarkGray));
            lines.push(Line::from("R: New  Esc: Cancel").fg(Color::DarkGray));
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
                    .title(" Main UI ".bold())
                    .border_set(border::THICK),
            )
            .render(area, buf);
    }
}