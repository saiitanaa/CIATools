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

        public string rootFolder = AppDomain.CurrentDomain.BaseDirectory;

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

        public void OnRSFCREATOR(object sender, RoutedEventArgs e)
        {
            var linux_path = Path.Combine(rootFolder, "RSF-Creator", "RSF-Creator");
            var win_path = Path.Combine(rootFolder, "RSF-Creator", "RSF-Creator.exe");

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
            string currentVersion = "7.0.0";
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