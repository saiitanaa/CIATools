using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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
        private bool _Console = false;

        [ObservableProperty]
        private bool _AutoCloseScript = true;

        [ObservableProperty]
        private string _current_os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Win" : "Linux";

        public string rootFolder = AppDomain.CurrentDomain.BaseDirectory.Contains("bin")
            ? Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."))
            : AppDomain.CurrentDomain.BaseDirectory;

        public void ImportFiles(List<string> FilePath)
        {
            if (FilePath == null || !FilePath.Any())
            {
                Debug_output = "Import aborted: no file selected.";
                return;
            }

            string userPath = Path.Combine(rootFolder, "USER_FILES");
            if (!Directory.Exists(userPath))
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
                File.Create(Path.Combine(userPath, "FILE_PATH")).Close();

                Debug_output = "Clean: USER_FILES - Status: Done";

                if (Current_os == "Win")
                {
                    Process.Start(new ProcessStartInfo { FileName = "explorer.exe", Arguments = userPath, UseShellExecute = true });
                }
                else
                {
                    Process.Start(new ProcessStartInfo { FileName = "xdg-open", Arguments = $"\"{userPath}\"", UseShellExecute = true });
                }
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

            try
            {
                if (Current_os == "Win")
                {
                    Debug_output = AutoCloseScript ? "Auto Close Script: Enabled" : "Auto Close Script: Disabled";
                    string flag = AutoCloseScript ? "/c" : "/k";
                    Process.Start("cmd.exe", $"{flag} cd /d \"{execute_py}\" && py -3 import.py");
                }
                else
                {
                    Debug_output = "Build started (Linux)";
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "python3",
                        Arguments = "import.py",
                        WorkingDirectory = execute_py,
                        UseShellExecute = false
                    });
                }

                string userPath = Path.Combine(rootFolder, "USER_FILES");
                if (Current_os == "Win")
                {
                    Process.Start(new ProcessStartInfo { FileName = "explorer.exe", Arguments = userPath, UseShellExecute = true });
                }
                else
                {
                    Process.Start(new ProcessStartInfo { FileName = "xdg-open", Arguments = $"\"{userPath}\"", UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                Debug_output = $"Build Error: {ex.Message}";
            }
        }
    }
}