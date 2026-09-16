<img width="1000" height="200" alt="BannerCIATools" src="https://github.com/user-attachments/assets/1c882709-0a5d-464e-b4dc-7455125c3a10" />

## The CIA Tooling for Nintendo 3DS development

**`CIATools`** is **all-in-one tool** for compiling your **projects** into `.cia` format with ease.

### Included :
RSF File Creator, SMDH File Creator, Edit Author, 2D-3D banner support, No dependencies required, 

#### What's Changed ?

**Switching to the CLI interface**. Faster for low-end PC and Better compatibility

**Passage to full Rust**. Very (very) **simple** for **compile** CIATools 

**Full cleaned code**. Cleaned obselete code & functions. Fast and light

## Usage



### History

**13-21 December 2025 | `CIATools`** -- Written in C# WinForms (only for Windows). Slow but stable (v1.0 -> v5)

[Original Branch](https://github.com/saiitanaa/CIATools/tree/winforms)

**29 May to 6 September 2026 | `CIAToolsR`** -- Written in C# (Windows, Linux, macOS). Fast but an interface that's too cluttered is less stable (v6-R -> v10.1.0)

[Original Branch](https://github.com/saiitanaa/CIATools/tree/ciatoolsr)

**Actual | `CIAToolsN`** -- Written in Rust (Windows, Linux, macOS). Very Fast, CLI, Stable, Simple, Best compatibility (v12.0.0 and later...)

## Downloads

You have Windows 10 - 11 ? Download here ➡️
[<kbd>X64</kbd>](https://github.com/saiitanaa/CIATools/releases)

You have macOS ? Download here ➡️
[<kbd>ARM64</kbd>](https://github.com/saiitanaa/CIATools/releases) - [<kbd>X64</kbd>](https://github.com/saiitanaa/CIATools/releases)

You have Linux ? Download here ➡️
[<kbd>ARM64</kbd>](https://github.com/saiitanaa/CIATools/releases) - [<kbd>X64</kbd>](https://github.com/saiitanaa/CIATools/releases)

## Compiling

Dependencies of **CIAToolsN** (everything else in the `cargo tree` is just their transitive sub-dependencies):

| Crate | Version | Role |
|---|---|---|
| **color-eyre** | 0.6.5 | Error handling with nice reports (backtrace, colors) |
| **crossterm** | 0.29.0 | Cross-platform terminal (input, colors, raw mode) |
| **fs** | 0.0.6 | Async file utilities (depends on `futures`) |
| **hostname** | 0.4.2 | Get the machine's hostname |
| **ratatui** | 0.30.2 | TUI framework (terminal UI) — biggest dependency, includes widgets, layout, etc. |
| **rfd** | 0.14.1 | Native file dialogs ("Rust File Dialogs") |

#### AI Utilisation & Transparancy

Artificial intelligence was **used** only for complex compilation **errors**. It was **not** used to generate assets or ready-made code.