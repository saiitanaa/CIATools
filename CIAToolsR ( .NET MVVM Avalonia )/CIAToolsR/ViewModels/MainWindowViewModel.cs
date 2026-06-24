using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CIAToolsR.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _debug_output = "";

        [ObservableProperty]
        private bool _Console = true;

        [ObservableProperty]
        private bool _AutoCloseScript = true;

        [ObservableProperty]
        private string _current_os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Win" : "Linux";

        private static string FindRootPath()
        {
            string? dir = AppDomain.CurrentDomain.BaseDirectory;

            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, "root_path")) ||
                    Directory.Exists(Path.Combine(dir, "RSSCRIPT")) ||
                    Directory.Exists(Path.Combine(dir, "USER_FILES")))
                {
                    return dir;
                }

                dir = Directory.GetParent(dir)?.FullName;
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public string rootFolder = FindRootPath();

        public void ImportFiles(List<string> filePath)
        {
            if (filePath == null || !filePath.Any())
            {
                Debug_output = "Import aborted: no file selected.";
                return;
            }

            string userPath = Path.Combine(rootFolder, "USER_FILES");
            Directory.CreateDirectory(userPath);

            foreach (string file in filePath)
            {
                try
                {
                    string importFile = Path.Combine(userPath, Path.GetFileName(file));
                    File.Copy(file, importFile, true);
                    Debug_output = "Import successful";
                }
                catch (Exception ex)
                {
                    Debug_output = $"Error: {ex.Message}";
                }
            }
        }

        [RelayCommand]
        public void ClearUserFiles()
        {
            try
            {
                string userPath = Path.Combine(rootFolder, "USER_FILES");

                if (Directory.Exists(userPath))
                {
                    Directory.Delete(userPath, true);
                }

                Directory.CreateDirectory(userPath);

                File.WriteAllText(Path.Combine(userPath, "AUTHOR.txt"), "");
                File.WriteAllText(Path.Combine(userPath, "FILE_PATH"), "");

                Debug_output = "Clean: USER_FILES - Status: Done";
                OpenFolder(userPath);
            }
            catch (Exception ex)
            {
                Debug_output = $"Clear Error: {ex.Message}";
            }
        }

        [RelayCommand]
        public void Build()
        {
            string rustScriptPath = Path.Combine(rootFolder, "RSSCRIPT");
            string userPath = Path.Combine(rootFolder, "USER_FILES");

            string importExecutable = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(rustScriptPath, "import.exe")
                : Path.Combine(rustScriptPath, "import");

            try
            {
                Directory.CreateDirectory(userPath);

                if (!Directory.Exists(rustScriptPath))
                {
                    Debug_output = $"Build Error: RSSCRIPT folder not found: {rustScriptPath}";
                    return;
                }

                if (!File.Exists(importExecutable))
                {
                    Debug_output = $"Build Error: Rust import executable not found: {importExecutable}";
                    return;
                }

                PrepareRustExecutables(rustScriptPath);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    StartRustToolWindows(rustScriptPath, importExecutable);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    StartRustToolLinux(rustScriptPath, importExecutable);
                }
                else
                {
                    Debug_output = "Build Error: unsupported OS.";
                    return;
                }

                OpenFolder(userPath);
            }
            catch (Exception ex)
            {
                Debug_output = $"Build Error: {ex.Message}";
            }
        }

        private void StartRustToolWindows(string rustScriptPath, string importExecutable)
        {
            Debug_output = AutoCloseScript
                ? "Rust build started - Auto close enabled"
                : "Rust build started - Auto close disabled";

            if (Console)
            {
                string flag = AutoCloseScript ? "/c" : "/k";

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"{flag} \"\"{importExecutable}\"\"",
                    WorkingDirectory = rustScriptPath,
                    UseShellExecute = true
                });

                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = importExecutable,
                WorkingDirectory = rustScriptPath,
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }

        private void StartRustToolLinux(string rustScriptPath, string importExecutable)
        {
            Debug_output = "Rust build started (Linux)";

            if (Console && TryStartLinuxTerminal(rustScriptPath, importExecutable))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = importExecutable,
                WorkingDirectory = rustScriptPath,
                UseShellExecute = false
            });
        }

        private void PrepareRustExecutables(string rustScriptPath)
        {
            try
            {
                string builderPath = Path.Combine(rustScriptPath, "builder_files_sources");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    RemoveWindowsZoneIdentifierFromFolder(rustScriptPath);
                    RemoveWindowsZoneIdentifierFromFolder(builderPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    MakeExecutable(Path.Combine(rustScriptPath, "import"));
                    MakeExecutable(Path.Combine(rustScriptPath, "compile"));
                    MakeExecutable(Path.Combine(rustScriptPath, "delete"));

                    MakeExecutable(Path.Combine(builderPath, "build.sh"));
                    MakeExecutable(Path.Combine(builderPath, "makerom"));
                    MakeExecutable(Path.Combine(builderPath, "bannertool"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PrepareRustExecutables warning: {ex.Message}");
            }
        }

        private static void RemoveWindowsZoneIdentifierFromFolder(string folderPath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            if (!Directory.Exists(folderPath))
                return;

            string[] patterns =
            {
                "*.exe",
                "*.bat",
                "*.cmd"
            };

            foreach (string pattern in patterns)
            {
                foreach (string file in Directory.EnumerateFiles(folderPath, pattern, SearchOption.TopDirectoryOnly))
                {
                    RemoveWindowsZoneIdentifier(file);
                }
            }
        }

        private static void RemoveWindowsZoneIdentifier(string filePath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            if (!File.Exists(filePath))
                return;

            try
            {
                File.Delete($"{filePath}:Zone.Identifier");
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Zone.Identifier remove failed for {filePath}: {ex.Message}");
            }
        }

        private static void MakeExecutable(string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;

            if (!File.Exists(path))
                return;

            try
            {
                var chmodInfo = new ProcessStartInfo
                {
                    FileName = "chmod",
                    UseShellExecute = false
                };

                chmodInfo.ArgumentList.Add("+x");
                chmodInfo.ArgumentList.Add(path);

                using Process? chmodProcess = Process.Start(chmodInfo);
                chmodProcess?.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"chmod failed for {path}: {ex.Message}");
            }
        }

        private bool TryStartLinuxTerminal(string rustScriptPath, string importExecutable)
        {
            string command = $"cd {QuoteForBash(rustScriptPath)} && {QuoteForBash(importExecutable)}";

            if (!AutoCloseScript)
            {
                command += "; echo; read -r -p 'Press Enter to close...'";
            }

            string[] terminals =
            {
                "gnome-terminal",
                "konsole",
                "xfce4-terminal",
                "mate-terminal",
                "xterm"
            };

            foreach (string terminal in terminals)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = terminal,
                        UseShellExecute = false
                    };

                    if (terminal == "gnome-terminal")
                    {
                        psi.ArgumentList.Add("--");
                        psi.ArgumentList.Add("bash");
                        psi.ArgumentList.Add("-lc");
                        psi.ArgumentList.Add(command);
                    }
                    else if (terminal == "konsole")
                    {
                        psi.ArgumentList.Add("-e");
                        psi.ArgumentList.Add("bash");
                        psi.ArgumentList.Add("-lc");
                        psi.ArgumentList.Add(command);
                    }
                    else if (terminal == "xfce4-terminal")
                    {
                        psi.ArgumentList.Add("--command");
                        psi.ArgumentList.Add($"bash -lc {QuoteForBash(command)}");
                    }
                    else if (terminal == "mate-terminal")
                    {
                        psi.ArgumentList.Add("--");
                        psi.ArgumentList.Add("bash");
                        psi.ArgumentList.Add("-lc");
                        psi.ArgumentList.Add(command);
                    }
                    else if (terminal == "xterm")
                    {
                        psi.ArgumentList.Add("-e");
                        psi.ArgumentList.Add("bash");
                        psi.ArgumentList.Add("-lc");
                        psi.ArgumentList.Add(command);
                    }

                    Process.Start(psi);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Terminal start failed ({terminal}): {ex.Message}");
                }
            }

            return false;
        }

        private static string QuoteForBash(string value)
        {
            return "'" + value.Replace("'", "'\"'\"'") + "'";
        }

        private void OpenFolder(string path)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{path}\"",
                        UseShellExecute = true
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = $"\"{path}\"",
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Debug_output = $"Open folder error: {ex.Message}";
            }
        }
    }
}