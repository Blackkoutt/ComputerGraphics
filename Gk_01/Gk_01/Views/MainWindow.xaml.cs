using Gk_01.ViewModels;
using System.Windows;

namespace Gk_01.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = MainWindowViewModel.Instance;
            DataContext = viewModel;
            viewModel.Canvas = mainCanvas;
        }
    }
}