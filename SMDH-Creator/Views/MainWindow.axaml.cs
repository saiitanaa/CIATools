using Avalonia.Controls;
using SMDH_Creator.ViewModels;

namespace SMDH_Creator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(this);
        }
    }
}