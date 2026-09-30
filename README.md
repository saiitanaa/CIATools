<img width="1000" height="200" alt="BannerCIATools" src="https://github.com/user-attachments/assets/1c882709-0a5d-464e-b4dc-7455125c3a10" />

## CIA Tooling for Nintendo 3DS development

**`CIATools`** is **all-in-one tool** for compiling your **projects** into `.cia` format with ease.

### Features :
RSF File Creator, SMDH File Creator, ICN File Creator, Edit Author, 2D-3D banner support, 

No dependencies required "*except for contributions*", `makeROM` & `bannertool` librairies included, Actively maintained.

## Index

> Security : <a href="https://github.com/saiitanaa/CIATools/blob/main/AUDIT.md">Check Audit</a>

> Usage & Installation : <a href="https://github.com/saiitanaa/CIATools/blob/main/USAGE.md">Use CIATools</a>

> Contributing : <a href="#contribute-to-ciatools">Check</a>

> Credits : <a href="#credits-3">Check</a> | History : <a href="#history">Look</a>

## Downloads

| Platform | Architecture | Download |
|---|---|---|
| macOS | Aarch64 | [Download](https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-OSX-aarch64) |
| macOS | x64 | [Compile Required](#compiling) |
| Linux | x64 | [Download](https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Linux-x64) |
| Linux | Aarch64 | [Download](https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Linux-aarch64) |
| Windows | x64 | [Download](https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Win-x64.exe) |
| Windows | Aarch64 | [Download](https://github.com/saiitanaa/CIATools/releases/latest/download/CIAToolsN-Win-aarch64.exe) |

## What's changed with CIAToolsN? 

**Switching to the TUI** | **Faster** for **low-end** PC

**Rewrite to Full Rust** | **Native** standalone **binary** with no .NET runtime required

**Full cleaned code** | Better file organization, **Complete** code **overhaul** with `CIAToolsR`

**Better Compatibility** | The **makeROM** and **Bannertool** libraries are **included** in the same executable **binary** 

## Contribute to CIATools

### Dependencies

**Everything else in the `cargo tree` is just their transitive sub-dependencies**

| Crate          | Version | Role                                                                             |
| -------------- | ------: | -------------------------------------------------------------------------------- |
| **color-eyre** |   0.6.5 | Error handling with nice reports (backtrace, colors)                             |
| **crossterm**  |  0.29.0 | Cross-platform terminal (input, colors, raw mode)                                |
| **fs**         |   0.0.6 | Async file utilities (depends on `futures`)                                      |
| **hostname**   |   0.4.2 | Get the machine's hostname                                                       |
| **rand**       |     0.9 | Random number generation                                                         |
| **ratatui**    |  0.30.2 | TUI framework (terminal UI) — biggest dependency, includes widgets, layout, etc. |
| **reqwest**    |    0.12 | HTTP client for TitleDB/NUS Info API access                                      |
| **rfd**        |  0.14.1 | Native file dialogs ("Rust File Dialogs")                                        |
| **serde**      |       1 | Serialization/deserialization framework                                          |
| **serde_json** |       1 | JSON serialization/deserialization                                               |

---

### Project architecture

| File           | Utility                                                                          |
| -------------- | -------------------------------------------------------------------------------- |
| `src/main.rs` | Manages the user interface, inputs, and all features |
| `src/rsfcreator.rs` | Manages RSF Creator backend |
| `src/icncreator.rs` | Manages ICN Creator backend |
| `src/uniqueid.rs` | Generates a random UniqueID for **TitleID & UniqueID Creator** |
| `src/makerom.rs` | Manage makerom C librairies |
| `src/build.rs` | Manages compilation of the Final Binary |
| `makerom/` | MakeROM C libraries |
| `bannertool/` | Bannertool C libraries |

---

### Compile

Necessary file for **compilation** : `src/build.rs`

Make **sure** you're in the project's **root** directory

#### Release binary -> `cargo build --release`

#### Debug/Dev binary -> `cargo build` 

---

## History

**13-21 December 2025 | `CIATools`** -- Written in C# WinForms (only for Windows). Slow but stable (v1.0 -> v5)

[Original Branch](https://github.com/saiitanaa/CIATools/tree/winforms)

**29 May to 6 September 2026 | `CIAToolsR`** -- Written in C# (Windows, Linux, macOS). Fast but an interface that's too cluttered is less stable (v6-R -> v10.1.0)

[Original Branch](https://github.com/saiitanaa/CIATools/tree/ciatoolsr)

**Current | `CIAToolsN`** -- Written in Rust (Windows, Linux, macOS). Fast, TUI, Stable, Simple, Best compatibility (v26.0.0 and later...)


## Credits <3

Thanks for makeROM source code : https://github.com/3DSGuy/Project_CTR

Thanks for bannertool source code : https://github.com/diasurgical/bannertool

### AI Utilisation

AI is no longer used in the project