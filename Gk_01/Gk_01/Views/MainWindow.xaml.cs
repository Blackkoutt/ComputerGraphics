using Gk_01.ViewModels.MainWindowViewModelPartials;
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
            viewModel.OnInit();
        }
    }
}