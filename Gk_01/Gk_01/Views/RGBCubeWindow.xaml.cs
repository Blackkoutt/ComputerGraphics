using Gk_01.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            viewModel.VisualModelCube = visualModelCube;
            viewModel.Viewport3DCube = viewportCube;
            //viewModel.VisualModelTriangle = visualModelTriangle;
            viewModel.Canvas = canvas;
        }
    }
}
