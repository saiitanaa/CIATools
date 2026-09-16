mod import;
mod utils;
mod delete;
mod compile;

use std::io;
use crossterm::{
    event::{self, Event, KeyCode, KeyEventKind},
    execute,
    terminal::SetSize,
};
use ratatui::{
    DefaultTerminal, Frame, buffer::Buffer,
    layout::{Constraint, Direction, Layout, Rect},
    style::{Color, Stylize},
    symbols::border,
    text::Line,
    widgets::{Block, Borders, Paragraph, Widget},
};

const MIN_WIDTH: u16 = 120;
const MIN_HEIGHT: u16 = 32;

fn main() -> io::Result<()> {
    print!("\x1b]0;CIATools v12.0.0\x07");
        execute!(
        io::stdout(),
        SetSize(MIN_WIDTH, MIN_HEIGHT)
    )?;
    ratatui::run(|terminal| App::default().run(terminal))
}

#[derive(Debug, Default)]
pub struct App {
    exit: bool,
    output: Vec<String>,
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
                Line::from("1 : Select HB Files"),
                Line::from("2 : Create RSF-Files"),
                Line::from("3 : Create SMDH-Files"),
                Line::from("4 : Set author"),
                Line::from("C : Compile"),
                Line::from("0 : Clean USER_FILES"),
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
                match key.code {
                    KeyCode::Char('q') | KeyCode::Char('Q') => {
                        self.exit = true;
                    }
                    KeyCode::Char('1') => {
                        self.output.push("Import HB Files...".to_string());
                    }
                    KeyCode::Char('2') => {
                        self.output.push("Start RSF-Creator...".to_string());
                    }
                    KeyCode::Char('3') => {
                        self.output.push("Start SMDH-Creator...".to_string());
                    }
                    KeyCode::Char('4') => {
                        self.output.push("Set HB Author".to_string());
                    }
                    KeyCode::Char('C') | KeyCode::Char('c') => {
                        self.output.push("Compile HB...".to_string());
                    }
                    KeyCode::Char('0') => {
                        self.output.push("Clean USER_FILES...".to_string());
                    }
                    _ => {}
                }
            }
        }

        Ok(())
    }
}

impl Widget for &App {
    fn render(self, area: Rect, buf: &mut Buffer) {
        let hostname = hostname::get()
            .map(|h| h.to_string_lossy().into_owned())
            .unwrap_or_else(|_| "unknown".to_string());

        Paragraph::new(vec![
            Line::from(format!("HELLO !!!, {hostname} 👋")),
        ])
        .centered()
        .block(
            Block::bordered()
                .title(" >_ CIATools -- Saiitanaa ".bold())
                .border_set(border::THICK),
        )
        .render(area, buf);
    }
}