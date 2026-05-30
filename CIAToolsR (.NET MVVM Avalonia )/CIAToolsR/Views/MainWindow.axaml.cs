using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CIAToolsR.ViewModels;
using System;
using System.IO;
using System.Linq;

namespace CIAToolsR.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataContext == null)
            {
                DataContext = new MainWindowViewModel();
            }
        }

        public string rootFolder = AppDomain.CurrentDomain.BaseDirectory.Contains("bin")
            ? Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."))
            : AppDomain.CurrentDomain.BaseDirectory;

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
    }
}