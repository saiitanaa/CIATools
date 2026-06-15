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
                    Directory.Exists(Path.Combine(dir, "PYSCRIPT")) ||
                    Directory.Exists(Path.Combine(dir, "USER_FILES")))
                {
                    return dir;
                }

                dir = Directory.GetParent(dir)?.FullName;
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public string rootFolder = FindRootPath();

        public void ImportFiles(List<string> FilePath)
        {
            if (FilePath == null || !FilePath.Any())
            {
                Debug_output = "Import aborted: no file selected.";
                return;
            }

            string userPath = Path.Combine(rootFolder, "USER_FILES");
            Directory.CreateDirectory(userPath);

            foreach (string file in FilePath)
            {
                try
                {
                    string ImportFile = Path.Combine(userPath, Path.GetFileName(file));
                    File.Copy(file, ImportFile, true);
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
                if (Directory.Exists(userPath)) Directory.Delete(userPath, true);
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
            string execute_py = Path.Combine(rootFolder, "PYSCRIPT");
            string import_py = Path.Combine(execute_py, "import.py");
            string userPath = Path.Combine(rootFolder, "USER_FILES");

            try
            {
                Directory.CreateDirectory(userPath);

                if (!Directory.Exists(execute_py))
                {
                    Debug_output = $"Build Error: PYSCRIPT folder not found: {execute_py}";
                    return;
                }

                if (!File.Exists(import_py))
                {
                    Debug_output = $"Build Error: import.py not found: {import_py}";
                    return;
                }

                if (Current_os == "Win")
                {
                    Debug_output = AutoCloseScript ? "Auto Close Script: Enabled" : "Auto Close Script: Disabled";
                    string flag = AutoCloseScript ? "/c" : "/k";
                    Process.Start("cmd.exe", $"{flag} cd /d \"{execute_py}\" && py -3 import.py");
                }
                else
                {
                    Debug_output = "Build started (Linux)";

                    var psi = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/env",
                        WorkingDirectory = execute_py,
                        UseShellExecute = false
                    };

                    psi.ArgumentList.Add("python3");
                    psi.ArgumentList.Add("import.py");

                    Process.Start(psi);
                }

                OpenFolder(userPath);
            }
            catch (Exception ex)
            {
                Debug_output = $"Build Error: {ex.Message}";
            }
        }

        private void OpenFolder(string path)
        {
            try
            {
                if (Current_os == "Win")
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{path}\"",
                        UseShellExecute = true
                    });
                }
                else
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
