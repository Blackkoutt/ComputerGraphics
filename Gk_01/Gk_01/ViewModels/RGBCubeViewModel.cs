using Gk_01.DI;
using Gk_01.Observable;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using Unity;
using System.Windows.Shapes;

namespace Gk_01.ViewModels
{
    public class RGBCubeViewModel : BaseViewModel
    {
        private static RGBCubeViewModel? _instance = null;
        private ModelVisual3D? _visualModel;
        public ICommand LoadedCommand { get; }
        public RGBCubeViewModel()
        {
            LoadedCommand = new RelayCommand(CreateRGBCube);
        }
        public ModelVisual3D? VisualModel
        {
            get { return _visualModel; }
            set { _visualModel = value; }
        }

        private WriteableBitmap GenerateColorMixingImage(Color xColor, Color yColor, Color maxColor)
        {
            int width = 255;
            int height = 255;

            // Tworzymy bitmapę do zapisu pikseli
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = width * 4; // Ilość bajtów na wiersz (4 bajty na piksel dla BGRA32)
            byte[] pixelData = new byte[height * stride];

            byte r = maxColor.R, g = maxColor.G, b = maxColor.B;

            var a = 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (xColor == Colors.Red) r = (byte)x;
                    else if (xColor == Colors.Lime) g = (byte)x;
                    else if (xColor == Colors.Blue) b = (byte)x;

                    if (yColor == Colors.Red) r = (byte)y;
                    else if (yColor == Colors.Lime) g = (byte)y;
                    else if (yColor == Colors.Blue) b = (byte)y;

                    int index = (y * stride) + (x * 4);

                    // BGRA - kolejność dla formatu Bgra32
                    pixelData[index + 0] = b;   // B
                    pixelData[index + 1] = g;  // G
                    pixelData[index + 2] = r;    // R
                    pixelData[index + 3] = 255;    // A (przezroczystość, ustawiona na pełną widoczność)
                }
            }

            // Zapisujemy dane pikseli do bitmapy
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return bitmap;
        }

        private void CreateRGBCube(object parameter)
        {
            var firstBitmap = GenerateColorMixingImage(Colors.Lime, Colors.Blue, Colors.Red);
            var secondBitmap = GenerateColorMixingImage(Colors.Blue, Colors.Red, Colors.Lime);
            var thirdBitmap = GenerateColorMixingImage(Colors.Lime, Colors.Red, Colors.Blue);

            var fourthBitmap = GenerateColorMixingImage(Colors.Red, Colors.Blue, Colors.Black);
            var fifthBitmap = GenerateColorMixingImage(Colors.Lime, Colors.Blue, Colors.Black);
            var sixthBitmap = GenerateColorMixingImage(Colors.Lime, Colors.Red, Colors.Black);

            // Tworzenie kostki
            var cube = new Model3DGroup();

            cube.Children.Add(CreateFace(new Point3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), firstBitmap));    // front
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), fifthBitmap));   // back
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fourthBitmap));    // left
            cube.Children.Add(CreateFace(new Point3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), secondBitmap));   // right
            cube.Children.Add(CreateFace(new Point3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), thirdBitmap));  // top
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), sixthBitmap));    // bottom


            var ambientLight = new AmbientLight(Colors.LightGray);

            cube.Children.Add(ambientLight);

            _visualModel!.Content = cube;
            //viewport.Children.Add(modelVisual3D);
        }

        private GeometryModel3D CreateFace(Point3D origin, Vector3D width, Vector3D height, BitmapSource bitmapSource)
        {
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(origin);
            mesh.Positions.Add(origin + width);
            mesh.Positions.Add(origin + width + height);
            mesh.Positions.Add(origin + height);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            // Ustaw współrzędne tekstur
            mesh.TextureCoordinates.Add(new Point(0, 0)); // Wierzchołek 0 (lewy górny)
            mesh.TextureCoordinates.Add(new Point(1, 0)); // Wierzchołek 1 (prawy górny)
            mesh.TextureCoordinates.Add(new Point(1, 1)); // Wierzchołek 2 (prawy dolny)
            mesh.TextureCoordinates.Add(new Point(0, 1)); // Wierzchołek 3 (lewy dolny)

            var material = new DiffuseMaterial(new ImageBrush(bitmapSource));
            var geometryModel = new GeometryModel3D(mesh, material);
            geometryModel.BackMaterial = material; // Opcjonalnie dla tylnych ścianek
            return geometryModel;
        }

        public static RGBCubeViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = DIContainer.GetContainer().Resolve<RGBCubeViewModel>();
                    return _instance;
                }
                else
                    return _instance;
            }
        }
    }
}
