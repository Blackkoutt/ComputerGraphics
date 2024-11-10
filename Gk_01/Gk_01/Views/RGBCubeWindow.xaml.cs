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
        private MeshGeometry3D triangleMesh;
        private MaterialGroup materialGroup;
        private Color triangleColor = Colors.Red;
        

        private byte r = 0;
        private byte g = 0;
        private byte b = 0;

        private Model3DGroup modelGroup;

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
