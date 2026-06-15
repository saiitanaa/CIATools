using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CIAToolsR.ViewModels;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ObjectiveC;
using System.Threading.Tasks;

namespace CIAToolsR.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _ = CheckUpdateAsync();

            DragDrop.AddDropHandler(this, OnDrop);
            DragDrop.AddDragOverHandler(this, OnDragOver);

            if (DataContext == null)
            {
                DataContext = new MainWindowViewModel();
            }
        }

        public static string FindRootPath()
        {
            string? dir = AppDomain.CurrentDomain.BaseDirectory;

            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, "root_path")))
                    return dir;

                dir = Directory.GetParent(dir)?.FullName;
            }

            throw new DirectoryNotFoundException("root_path ???");
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.DataTransfer.Contains(DataFormat.File)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            var files = e.DataTransfer.TryGetFiles();

            if (files != null)
            {
                var paths = files.Select(f => f.Path.LocalPath).ToList();

                if (paths.Count > 0 && DataContext is MainWindowViewModel vm)
                {
                    vm.ImportFiles(paths);
                }
            }
        }

        public async void ImportFiles(object sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "CIATools Import - Select Files",
                AllowMultiple = true
            });

            if (files.Count > 0 && DataContext is MainWindowViewModel vm)
            {
                var paths = files.Select(f => f.Path.LocalPath).ToList();
                vm.ImportFiles(paths);
            }
        }

        public void OnClearMenuClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ClearUserFiles();
            }
        }

        public async void OnSetAuthorName(object sender, RoutedEventArgs e)
        {
            var dialog = new Window
            {
                Width = 300,
                Height = 120,
                Title = "Set Homebrew Author"
            };

            var textBox = new TextBox();

            var button = new Button
            {
                Content = "Save"
            };

            button.Click += async (_, _) =>
            {
                await SaveCreatorAsync(FindRootPath(), textBox.Text ?? "");
                dialog.Close();
            };

            dialog.Content = new StackPanel
            {
                Margin = new Thickness(10),
                Children =
                {
                    textBox,
                    button
                }
            };

            await dialog.ShowDialog(this);
        }

        public static async Task SaveCreatorAsync(string rootPath, string creatorName)
        {
            string userFilesPath = Path.Combine(rootPath, "USER_FILES");
            Directory.CreateDirectory(userFilesPath);

            string creatorPath = Path.Combine(userFilesPath, "AUTHOR.txt");

            await File.WriteAllTextAsync(creatorPath, creatorName.Trim());
        }

        public void OnRestoreFILEPATH(object? sender, RoutedEventArgs e)
        {
            string rootPath = FindRootPath();
            string userFilesPath = Path.Combine(rootPath, "USER_FILES");
            string filePath = Path.Combine(userFilesPath, "FILE_PATH");
            try
            {
                Directory.CreateDirectory(userFilesPath);
                File.Create(filePath).Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed! : {ex.Message}");
            }
        }
        public void OnRestoreAuthor(object? sender, RoutedEventArgs e)
        {
            string rootPath = FindRootPath();
            string userFilesPath = Path.Combine(rootPath, "USER_FILES");
            string creatorPath = Path.Combine(userFilesPath, "AUTHOR.txt");
            try
            {
                Directory.CreateDirectory(userFilesPath);
                File.Create(creatorPath).Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed! : {ex.Message}");
            }
        }

        public void OnRSFCREATOR(object sender, RoutedEventArgs e)
        {
            var rootPath = FindRootPath();
            var rsfFolder = Path.Combine(rootPath, "RSF-Creator");

            var linux_path = Path.Combine(rsfFolder, "RSF-Creator");
            var win_path = Path.Combine(rsfFolder, "RSF-Creator.exe");

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string zoneIdentifierPath = $"{win_path}:Zone.Identifier";
                    if (File.Exists(zoneIdentifierPath))
                    {
                        File.Delete(zoneIdentifierPath);
                    }

                    Process.Start(new ProcessStartInfo(win_path)
                    {
                        UseShellExecute = true,
                        WorkingDirectory = rsfFolder
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    try
                    {
                        var chmodProcess = Process.Start(new ProcessStartInfo("chmod", $"+x \"{linux_path}\"")
                        {
                            UseShellExecute = false
                        });

                        if (chmodProcess == null)
                        {
                            Debug.WriteLine("Failed to start chmod.");
                        }
                        else
                        {
                            chmodProcess.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"chmod failed: {ex.Message}");
                    }

                    var psi = new ProcessStartInfo(linux_path)
                    {
                        UseShellExecute = false,
                        WorkingDirectory = rsfFolder
                    };
                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed start RSFCREATOR: {ex.Message}");
            }
        }

        public void OnSMDHCREATOR(object sender, RoutedEventArgs e)
        {
            var rootPath = FindRootPath();
            var smdhFolder = Path.Combine(rootPath, "SMDH-Creator");

            var linux_path = Path.Combine(smdhFolder, "SMDH-Creator");
            var win_path = Path.Combine(smdhFolder, "SMDH-Creator.exe");

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string zoneIdentifierPath = $"{win_path}:Zone.Identifier";
                    if (File.Exists(zoneIdentifierPath))
                    {
                        File.Delete(zoneIdentifierPath);
                    }

                    Process.Start(new ProcessStartInfo(win_path)
                    {
                        UseShellExecute = true,
                        WorkingDirectory = smdhFolder
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    try
                    {
                        var chmodProcess = Process.Start(new ProcessStartInfo("chmod", $"+x \"{linux_path}\"")
                        {
                            UseShellExecute = false
                        });

                        if (chmodProcess == null)
                        {
                            Debug.WriteLine("Failed to start chmod.");
                        }
                        else
                        {
                            chmodProcess.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"chmod failed: {ex.Message}");
                    }

                    var psi = new ProcessStartInfo(linux_path)
                    {
                        UseShellExecute = false,
                        WorkingDirectory = smdhFolder
                    };
                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed start SMDHCREATOR: {ex.Message}");
            }
        }

        private async Task CheckUpdateAsync()
        {
            await RunUpdateCheckLogicAsync(this);
        }

        private async Task RunUpdateCheckLogicAsync(Window ownerWindow)
        {
            string currentVersion = "8.2.1";
            var client = new GitHubClient(new ProductHeaderValue("CIAToolsR"));

            try
            {
                var latestRelease = await client.Repository.Release.GetLatest("saysaa", "CIATools");

                if (latestRelease != null)
                {
                    string latestVersion = latestRelease.TagName?.Replace("v", "").Trim() ?? "0.0.0";
                    var current = new Version(currentVersion);
                    var latest = new Version(latestVersion);

                    if (latest > current)
                    {
                        var dialog = new Window
                        {
                            Width = 320,
                            Height = 180,
                            Title = "CIATools Updater",
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };

                        var textBlock = new TextBlock
                        {
                            Text = $"New update available: {latestRelease.TagName} !",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(0, 30, 0, 20)
                        };

                        var button = new Button
                        {
                            Content = "Update",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Width = 100
                        };

                        button.Click += (_, _) =>
                        {
                            string downloadUrl = latestRelease.HtmlUrl;
                            OpenBrowser(downloadUrl);
                            dialog.Close();
                        };

                        var layout = new StackPanel();
                        layout.Children.Add(textBlock);
                        layout.Children.Add(button);

                        dialog.Content = layout;

                        await dialog.ShowDialog(ownerWindow);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Check update failed! : " + ex.Message);
            }
        }

        private void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
        }

        public void OnOpenGitHubClick(object? sender, RoutedEventArgs e)
        {
            OpenGitHub();
        }

        public void OnOpenGitHubPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenGitHub();
        }

        private void OpenGitHub()
        {
            var url = "https://github.com/saysaa/CIATools";

            try
            {
                OpenBrowser(url);
            }
            catch
            {
            }
        }

        public void OnOpenDiscord(object? sender, RoutedEventArgs e)
        {
            var url = "https://discord.gg/px7MGB2vhX";

            try
            {
                OpenBrowser(url);
            }
            catch
            {
            }
        }
    }
}