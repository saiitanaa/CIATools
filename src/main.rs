mod import;
mod delete;
mod utils;
mod compile;
mod picker;
mod rsfcreator;

use std::{io, fs, path::PathBuf, path::Path};
use crossterm::{
    event::{self, Event, KeyCode, KeyEventKind},
};
use ratatui::{
    DefaultTerminal, Frame, buffer::Buffer,
    layout::{Constraint, Direction, Layout, Rect},
    style::{Color, Stylize},
    symbols::border,
    text::Line,
    widgets::{Block, Borders, Paragraph, Widget},
};

use crate::import::import_files;
use crate::rsfcreator::rsf_config;

fn main() -> io::Result<()> {
    print!("\x1b]0;CIATools v12.0.0\x07");
    set_directories()?;
    let author = load_author()?;
    ratatui::run(|terminal| {
        App {
            author,
            ..App::default()
        }
        .run(terminal)
    })
} //Good size : 120x32

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
    
impl App {
    pub fn run(&mut self, terminal: &mut DefaultTerminal) -> io::Result<()> {
        while !self.exit {
            terminal.draw(|frame| self.draw(frame))?;
            self.handle_events(terminal)?;
        }

        Ok(())
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
                Line::from("3 : Create SMDH"),
                Line::from("4 : Set Author"),
                Line::from("\n"),
                Line::from("C : Compile"),
                Line::from("0 : Clean USER_FILES"),
                Line::from("\n"),
                Line::from("Q : Quit"),
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
            Paragraph::new(output_lines)
                .style(Color::White)
                .block(
                    Block::bordered()
                        .bold()
                        .fg(Color::Magenta)
                        .title(" OUTPUT ".bold())
                        .border_set(border::THICK),
                ),
            outer_layout[1],
        );
    }


    fn handle_events(&mut self, terminal: &mut DefaultTerminal) -> io::Result<()> {
        if let Event::Key(key) = event::read()? {
            if key.kind == KeyEventKind::Press {

                // Author input mode
                if self.editing_author {
                    match key.code {
                        KeyCode::Enter => {
                            self.author = self.author_input.clone();
                            save_author(&self.author)?;
                            self.author_input.clear();
                            self.editing_author = false;
                            self.output
                                .push(format!("[+] Author set to: {}", self.author));
                        }

                        KeyCode::Backspace => {
                            self.author_input.pop();
                        }

                        KeyCode::Char(c) => {
                            self.author_input.push(c);
                        }

                        KeyCode::Esc => {
                            self.author_input.clear();
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

                            if self.rsf_field > 4 {
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

                // Key mode
                match key.code {
                    KeyCode::Char('q') | KeyCode::Char('Q') => {
                        self.exit = true;
                    }

                    KeyCode::Char('1') => {
                        self.output.push("[?] Import FileDialog".to_string());

                        ratatui::restore();

                        let result = user_files_path()
                            .and_then(import_files);

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
                        self.output.push("Start SMDH-Creator...".to_string());
                    }

                    KeyCode::Char('4') => {
                        self.author_input.clear();
                        self.editing_author = true;
                    }

                    KeyCode::Char('C') | KeyCode::Char('c') => {
                        self.output.push("[+] Compile HB...".to_string());
                    }

                    KeyCode::Char('0') => {
                        match user_files_path() {
                            Ok(path) => {
                                match crate::delete::clean_user_files(path) {
                                    Ok(()) => self.output.push("[-] USER_FILES cleaned.".to_string()),
                                    Err(error) => self.output.push(format!("[!] {error}")),
                                }
                            }
                            Err(error) => self.output.push(format!("[!] {error}")),
                        }
                    }
                    _ => {}
                }
            }
        }

        Ok(())
        }
    }

    fn user_files_path() -> io::Result<PathBuf> {
        let bin_path = std::env::current_exe()?
            .parent()
            .ok_or_else(|| io::Error::other("[!] Binary path error !?"))?
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
    fn render(self, area: Rect, buf: &mut Buffer) {
        let hostname = hostname::get()
            .map(|h| h.to_string_lossy().into_owned())
            .unwrap_or_else(|_| "unknown".to_string());

        let mut lines = vec![
            Line::from(""),
            Line::from(format!("HELLO !!! {hostname} 👋")),
        ];

        if self.editing_author {
            lines.push(Line::from(""));
            lines.push(Line::from(format!(
                "Enter author -> {}",
                self.author_input
            )));
        } else if !self.author.is_empty() {
            lines.push(Line::from(""));
            lines.push(Line::from(format!(
                "Author defined: {}",
                self.author
            )));
        }

        if self.rsf_edit {
            lines.push(Line::from(""));

            let (prompt, example) = match self.rsf_field {
                0 => ("Title:", "(Ex: The best Homebrew)"),
                1 => ("CompanyCode:", "(Ex: SAAA)"),
                2 => ("ProductCode:", "(Ex: CTR-P-XXXX)"),
                3 => ("RomFs Path:", "(Ex: ./romfs)"),
                4 => ("UniqueId:", "(Ex: 0x0004000000000000)"),
                5 => ("SaveDataSize:", "(Ex: 0x100000)"),
                6 => ("CpuSpeed:", "(Ex: 804)"),
                _ => ("", ""),
            };

            lines.push(
                Line::from(format!("{} {}", prompt, self.rsf_input))
                    .fg(Color::White),
            );

            lines.push(
                Line::from(example)
                    .fg(Color::DarkGray),
            );
        }

        Paragraph::new(lines)
            .centered()
            .block(
                Block::bordered()
                    .title(" CIATools -- Saiitanaa ".bold())
                    .border_set(border::THICK),
            )
            .render(area, buf);
    }
}