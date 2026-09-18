<img width="1000" height="200" alt="BannerCIATools" src="https://github.com/user-attachments/assets/1c882709-0a5d-464e-b4dc-7455125c3a10" />

## The CIA Tooling for Nintendo 3DS development

**`CIATools`** is **all-in-one tool** for compiling your **projects** into `.cia` format with ease.

### Included :
RSF File Creator, SMDH File Creator, Edit Author, 2D-3D banner support, No dependencies required, 

### What's Changed ?

**Switching to the CLI interface**. Faster for low-end PC and Better compatibility

**Passage to full Rust**. Very (very) **simple** for **compile** CIATools 

**Full cleaned code**. Cleaned obselete code & functions. Fast and light

**Console**. Yes, finally a veritable console for debug !

**100% Native**. The makeROM and Bannertool libraries are included in the same executable file 

## Usage

**Welcome to the interface!** To get started, **import the files** needed to create your **Homebrew** in `.cia` format.

To do this, **press** `1`. 

<img width="720" height="318" alt="image" src="https://github.com/user-attachments/assets/ae2fe529-c421-44e5-892a-5a68662e53e9" />

**You're now in the file explorer!** Select all the **files** needed for **compilation**.

**Don't have** an `.rsf` or `.smdh` **file**? No problem—we'll **create them for you**!

To do this, **press** `2` for RSF or `3` for SMDH.

#### For RSF File :

<img width="990" height="612" alt="image" src="https://github.com/user-attachments/assets/a06837b4-ae8f-42c2-8ee4-58797e25fd5d" />

#### For SMDH File: 

<img width="990" height="612" alt="image" src="https://github.com/user-attachments/assets/02f9769c-2772-4f35-9581-8851875d7643" />

**Perfect! Now let's define an author.** 

To do this, **press** `4`.

<img width="990" height="612" alt="image" src="https://github.com/user-attachments/assets/841ebbab-cc0e-4cad-8e40-a88f41967169" />

**Now we can compile the homebrew!**

To do this, **press** `C`.

<img width="990" height="612" alt="image" src="https://github.com/user-attachments/assets/1d26c2b3-e04b-4e7a-830f-1153029c6ca7" />



### History

**13-21 December 2025 | `CIATools`** -- Written in C# WinForms (only for Windows). Slow but stable (v1.0 -> v5)

[Original Branch](https://github.com/saiitanaa/CIATools/tree/winforms)

**29 May to 6 September 2026 | `CIAToolsR`** -- Written in C# (Windows, Linux, macOS). Fast but an interface that's too cluttered is less stable (v6-R -> v10.1.0)

[Original Branch](https://github.com/saiitanaa/CIATools/tree/ciatoolsr)

**Actual | `CIAToolsN`** -- Written in Rust (Windows, Linux, macOS). Very Fast, CLI, Stable, Simple, Best compatibility (v12.0.0 and later...)

## Downloads

You have **Windows** ? ➡️
[<kbd>X64</kbd>](https://github.com/saiitanaa/CIATools/releases)

You have **macOS** ? ➡️
[<kbd>ARM64</kbd>](https://github.com/saiitanaa/CIATools/releases) - [<kbd>X64</kbd>](https://github.com/saiitanaa/CIATools/releases)

You have **Linux** ? ➡️
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