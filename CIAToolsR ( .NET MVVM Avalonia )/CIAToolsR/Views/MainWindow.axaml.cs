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

            throw new DirectoryNotFoundException("Impossible de trouver root_path");
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
                Title = "Author Name"
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
            string userFilesPath = Path.Combine(rootPath, "builder_files_sources");
            Directory.CreateDirectory(userFilesPath);

            string creatorPath = Path.Combine(userFilesPath, "AUTHOR.txt");

            await File.WriteAllTextAsync(creatorPath, creatorName.Trim());
        }

        public void OnRSFCREATOR(object sender, RoutedEventArgs e)
        {
            var linux_path = Path.Combine(FindRootPath(), "RSF-Creator", "RSF-Creator");
            var win_path = Path.Combine(FindRootPath(), "RSF-Creator", "RSF-Creator.exe");

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(win_path) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    var psi = new ProcessStartInfo(linux_path)
                    {
                        UseShellExecute = false
                    };
                    Process.Start(psi);
                }
            }
            catch
            {
            }
        }

        private async Task CheckUpdateAsync()
        {
            await RunUpdateCheckLogicAsync();
        }

        public async void OnCheckUpdateAsyncClick(object? sender, RoutedEventArgs e)
        {
            await RunUpdateCheckLogicAsync();
        }

        private async Task RunUpdateCheckLogicAsync()
        {
            string currentVersion = "7.1.1";
            var client = new GitHubClient(new ProductHeaderValue("CIAToolsR"));

            try
            {
                var latestRelease = await client.Repository.Release.GetLatest("saysaa", "CIATools");

                if (latestRelease != null)
                {
                    string latestVersion = latestRelease.TagName.Replace("v", "").Trim();
                    var current = new Version(currentVersion);
                    var latest = new Version(latestVersion);

                    if (latest > current)
                    {
                        string downloadUrl = latestRelease.HtmlUrl;
                        OpenBrowser(downloadUrl);
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

        public void OnOpenGitHub(object? sender, PointerPressedEventArgs e)
        {
            var url = "https://github.com/saysaa/CIATools";

            try
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
            catch
            {
            }
        }
    }
}