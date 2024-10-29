using Gk_01.ViewModels;
using System.Windows;

namespace Gk_01.Views
{
    /// <summary>
    /// Logika interakcji dla klasy RGBCubeWindow.xaml
    /// </summary>
    public partial class RGBCubeWindow : Window
    {
        public RGBCubeWindow()
        {
            InitializeComponent();
            var viewModel = RGBCubeViewModel.Instance;
            DataContext = viewModel;
            viewModel.VisualModel = visualModel;
        }
    }
}
