using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        // --- GESTION DE L'AFFICHAGE DES PANNEAUX ---
        [ObservableProperty]
        private bool _isDropZoneVisible = true;

        [ObservableProperty]
        private bool _isConfirmationVisible = false;

        [ObservableProperty]
        private bool _isBuilding = false;

        [ObservableProperty]
        private string _current_os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Win" :
                                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Linux";

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

            // Basculer vers le panneau de confirmation
            IsDropZoneVisible = false;
            IsConfirmationVisible = true;
            IsBuilding = false;
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
                
                // Réinitialiser l'affichage
                IsDropZoneVisible = true;
                IsConfirmationVisible = false;
                IsBuilding = false;

                OpenFolder(userPath);
            }
            catch (Exception ex)
            {
                Debug_output = $"Clear Error: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task Build()
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

                prepareScript(rustScriptPath);

                // --- 1. AFFICHER LE CHARGEMENT "Compiling to CIA..." ---
                IsConfirmationVisible = false;
                IsBuilding = true;
                Debug_output = "Compiling to CIA...";

                Process? process = null;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    process = ScriptStartWin(rustScriptPath, importExecutable);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    process = ScriptStartLinux(rustScriptPath, importExecutable);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    process = ScriptStartOSX(rustScriptPath, importExecutable);
                }
                else
                {
                    Debug_output = "Build Error: unsupported OS.";
                    IsBuilding = false;
                    IsDropZoneVisible = true;
                    return;
                }

                // --- 2. ATTENDRE LA FIN DU PROCESSUS ---
                if (process != null)
                {
                    await process.WaitForExitAsync();
                }
                else
                {
                    await Task.Delay(1500); // Temps de repli si le processus est externe
                }

                Debug_output = "Build Completed!";
                OpenFolder(userPath);
            }
            catch (Exception ex)
            {
                Debug_output = $"Build Error: {ex.Message}";
            }
            finally
            {
                // --- 3. REVENIR À L'ÉTAT INITIAL ---
                IsBuilding = false;
                IsDropZoneVisible = true;
            }
        }

        private Process? ScriptStartWin(string rustScriptPath, string importExecutable)
        {
            Debug_output = AutoCloseScript
                ? "Rust build started - Auto close enabled"
                : "Rust build started - Auto close disabled";

            if (Console)
            {
                string flag = AutoCloseScript ? "/c" : "/k";

                return Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"{flag} \"\"{importExecutable}\"\"",
                    WorkingDirectory = rustScriptPath,
                    UseShellExecute = true
                });
            }

            return Process.Start(new ProcessStartInfo
            {
                FileName = importExecutable,
                WorkingDirectory = rustScriptPath,
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }

        private Process? ScriptStartLinux(string rustScriptPath, string importExecutable)
        {
            Debug_output = "Rust build started (Linux)";

            if (Console && TryStartLinuxTerminal(rustScriptPath, importExecutable, out var terminalProc))
            {
                return terminalProc;
            }

            return Process.Start(new ProcessStartInfo
            {
                FileName = importExecutable,
                WorkingDirectory = rustScriptPath,
                UseShellExecute = false
            });
        }

        private Process? ScriptStartOSX(string rustScriptPath, string importExecutable)
        {
            Debug_output = "Rust build started (macOS)";

            if (Console)
            {
                string command = $"cd {QuoteForBash(rustScriptPath)} && {QuoteForBash(importExecutable)}";
                if (!AutoCloseScript)
                {
                    command += "; echo; read -r -p 'Press Enter to close...'";
                }

                try
                {
                    string escapedCommand = command.Replace("\"", "\\\"");

                    var psi = new ProcessStartInfo
                    {
                        FileName = "osascript",
                        UseShellExecute = false
                    };

                    psi.ArgumentList.Add("-e");
                    psi.ArgumentList.Add($"tell application \"Terminal\" to do script \"{escapedCommand}\"");
                    psi.ArgumentList.Add("-e");
                    psi.ArgumentList.Add("tell application \"Terminal\" to activate");

                    return Process.Start(psi);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Mac Terminal start failed: {ex.Message}");
                }
            }

            return Process.Start(new ProcessStartInfo
            {
                FileName = importExecutable,
                WorkingDirectory = rustScriptPath,
                UseShellExecute = false
            });
        }

        private void prepareScript(string rustScriptPath)
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
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    MakeExecutable(Path.Combine(rustScriptPath, "import"));
                    MakeExecutable(Path.Combine(rustScriptPath, "compile"));
                    MakeExecutable(Path.Combine(rustScriptPath, "delete"));

                    MakeExecutable(Path.Combine(builderPath, "build.sh"));
                    MakeExecutable(Path.Combine(builderPath, "makeromOSX"));
                    MakeExecutable(Path.Combine(builderPath, "bannertool"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"prepareScript warning: {ex.Message}");
            }
        }

        private static void RemoveWindowsZoneIdentifierFromFolder(string folderPath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            if (!Directory.Exists(folderPath))
                return;

            string[] patterns = { "*.exe", "*.bat", "*.cmd" };

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
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            catch (Exception ex)
            {
                Debug.WriteLine($"Zone.Identifier remove failed for {filePath}: {ex.Message}");
            }
        }

        private static void MakeExecutable(string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
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

        private bool TryStartLinuxTerminal(string rustScriptPath, string importExecutable, out Process? process)
        {
            process = null;
            string command = $"cd {QuoteForBash(rustScriptPath)} && {QuoteForBash(importExecutable)}";

            if (!AutoCloseScript)
            {
                command += "; echo; read -r -p 'Press Enter to close...'";
            }

            string[] terminals = { "gnome-terminal", "konsole", "xfce4-terminal", "mate-terminal", "xterm" };

            foreach (string terminal in terminals)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = terminal,
                        UseShellExecute = false
                    };

                    if (terminal == "gnome-terminal" || terminal == "mate-terminal")
                    {
                        psi.ArgumentList.Add("--");
                        psi.ArgumentList.Add("bash");
                        psi.ArgumentList.Add("-lc");
                        psi.ArgumentList.Add(command);
                    }
                    else if (terminal == "konsole" || terminal == "xterm")
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

                    process = Process.Start(psi);
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
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "open",
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