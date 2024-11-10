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
using Gk_01.Models;

namespace Gk_01.ViewModels
{
    public class RGBCubeViewModel : BaseViewModel
    {
        private static RGBCubeViewModel? _instance = null;
        private Viewport3D? _viewport3DCube;
        private Polygon? triangle;
        private Canvas? _canvas;
        private ModelVisual3D? _visualModelCube;
        private ModelVisual3D? _visualModelTriangle;
        private List<RGBCubeFace> _cubeFaces;
        public ICommand LoadedCommand { get; }
        public ICommand ViewPortClickCommand { get; }

        private byte rrValue;
        private byte rgValue;
        private byte rbValue;

        private byte grValue;
        private byte ggValue;
        private byte gbValue;

        private byte brValue;
        private byte bgValue;
        private byte bbValue;

        private GeometryModel3D triangleModel;
        public RGBCubeViewModel()
        {
            _cubeFaces = [];
            LoadedCommand = new RelayCommand(CreateCubeAndTriangle);
            ViewPortClickCommand = new RelayCommand(ViewPortClick);
        }

        private void CreateCubeAndTriangle(object parameter)
        {
            CreateRGBCube();
            CreateTriangle();
        }

        private void ViewPortClick(object parameter)
        {
            if (parameter is MouseEventArgs e)
            {
                // Pobierz pozycję myszy w `Viewport3D`
                Point mousePosition = e.GetPosition(_viewport3DCube);

                // Przeprowadź `HitTest` w `Viewport3D`
                PointHitTestParameters hitParams = new PointHitTestParameters(mousePosition);
                VisualTreeHelper.HitTest(_viewport3DCube, null, ResultCallback, hitParams);
            }
        }


        private HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            // Sprawdź, czy wynik testu to `RayHitTestResult`
            if (result is RayHitTestResult rayResult)
            {
                var geometryHit = rayResult.ModelHit as GeometryModel3D;

                if (geometryHit != null)
                {
                    var material = geometryHit.Material;
                    var cubeFace = _cubeFaces.FirstOrDefault(f => f.Material.Material == material);
                    if (cubeFace != null)
                    {
                        Point3D hitPoint3D = rayResult.PointHit;
                        var X = Math.Clamp(hitPoint3D.X, 0, 1);
                        var Y = Math.Clamp(hitPoint3D.Y, 0, 1);
                        var Z = Math.Clamp(hitPoint3D.Z, 0, 1);

                        var a = 1;
                    }
                }
            }
            return HitTestResultBehavior.Stop;
        }


        private void CreateTriangle()
        {
            triangle = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(100, 0),  // Wierzchołek górny
                    new Point(0, 200),  // Wierzchołek lewy dolny
                    new Point(200, 200) // Wierzchołek prawy dolny
                },
                Fill = GenerateTriangleColor(),
            };

            // Dodanie trójkąta do Canvas
            Canvas!.Children.Clear();
            triangle.HorizontalAlignment = HorizontalAlignment.Center;
            triangle.VerticalAlignment = VerticalAlignment.Center;  
            Canvas.Children.Add(triangle);
        }

        private void UpdateTriangleColor()
        {
            var color = GenerateTriangleColor();
            triangle!.Fill =color;
        }

        private ImageBrush GenerateTriangleColor()
        {
            var defaultRR = (double)rrValue / 255;
            var defaultRG = (double)rgValue / 255;   
            var defaultRB = (double)rbValue / 255;

            var defaultGR = (double)grValue / 255;
            var defaultGG = (double)ggValue / 255;
            var defaultGB = (double)gbValue / 255;

            var defaultBR = (double)brValue / 255;
            var defaultBG = (double)bgValue / 255;
            var defaultBB = (double)bbValue / 255;

            double avgRB_GB = (rbValue + gbValue) / 2; // uśrednienie kanału B
            var diff_BB_avg = Math.Abs(bbValue - avgRB_GB);
            var step_BB = diff_BB_avg / 255;

            var is_BB_Graather_Than_avg_RB_GB = bbValue > avgRB_GB;

            int width = 255;
            int height = 255;

            // Tworzymy bitmapę do zapisu pikseli
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = width * 4; // Ilość bajtów na wiersz (4 bajty na piksel dla BGRA32)
            byte[] pixelData = new byte[height * stride];

            for (int y = 0; y < height; y++)
            {
                int invertedY = height - y - 1; // Odwrócona pozycja Y
                // Ustal podstawowy gradient dla koloru
                double r = rrValue;
                double g = rgValue;
                double b = avgRB_GB; // uśrednienie kanału B

                for (int x = 0; x < width; x++)
                {
                    int index = (invertedY * stride) + (x * 4);

                    // Modyfikacja w zależności od pozycji X dla płynnego gradientu
                    if (bbValue > avgRB_GB)
                        pixelData[index + 0] = (byte)Math.Floor(b + (step_BB * y));   // B
                    else
                        pixelData[index + 0] = (byte)Math.Floor(b - (step_BB * y));   // B

                    pixelData[index + 1] = (byte)Math.Floor(g - (defaultRG * x) + (defaultGG * x) + (defaultBG * y));   // G
                    pixelData[index + 2] = (byte)Math.Ceiling(r - (defaultRR * x) + (defaultGR * x) + (defaultBR * y)); // R
                    pixelData[index + 3] = 255; 
                }
            }

            // Zapisujemy dane pikseli do bitmapy
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

            Image imageControl = new Image();
            imageControl.Source = bitmap;

            return new ImageBrush(bitmap);
        }

        private ImageBrush GenerateColorMixingImage(Color xColor, Color yColor, Color maxColor)
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

            return new ImageBrush(bitmap);
        }
        private RGBCubeMaterial GetCubeMaterial(Color xColor, Color yColor, Color maxColor)
        {
            return new RGBCubeMaterial
            {
                XColor = xColor,
                YColor = yColor,
                MaxColor = maxColor, 
                Material = new DiffuseMaterial(GenerateColorMixingImage(xColor, yColor, maxColor))
            };          
        }


        


        private void CreateRGBCube()
        {
            var firstMaterial = GetCubeMaterial(Colors.Lime, Colors.Blue, Colors.Red);
            var secondMaterial = GetCubeMaterial(Colors.Blue, Colors.Red, Colors.Lime);
            var thirdMaterial = GetCubeMaterial(Colors.Lime, Colors.Red, Colors.Blue);
            var fourthMaterial = GetCubeMaterial(Colors.Red, Colors.Blue, Colors.Black);
            var fifthMaterial = GetCubeMaterial(Colors.Lime, Colors.Blue, Colors.Black);
            var sixthMaterial = GetCubeMaterial(Colors.Lime, Colors.Red, Colors.Black);

            // Tworzenie kostki
            var cube = new Model3DGroup();

            cube.Children.Add(CreateFace(new Point3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), firstMaterial));    // front
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), fifthMaterial));   // back
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fourthMaterial));    // left
            cube.Children.Add(CreateFace(new Point3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), secondMaterial));   // right
            cube.Children.Add(CreateFace(new Point3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), thirdMaterial));  // top
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), sixthMaterial));    // bottom

            var ambientLight = new AmbientLight(Colors.LightGray);

            cube.Children.Add(ambientLight);

            _visualModelCube!.Content = cube;
            //viewport.Children.Add(modelVisual3D);
        }

        private GeometryModel3D CreateFace(Point3D origin, Vector3D width, Vector3D height, RGBCubeMaterial material)
        {
            var cubeFace = new RGBCubeFace
            {
                Origin = origin,
                Width = width,
                Height = height,
                Material = material
            };
            _cubeFaces.Add(cubeFace);

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

            var geometryModel = new GeometryModel3D(mesh, material.Material);
           

            geometryModel.BackMaterial = material.Material; // Opcjonalnie dla tylnych ściane

            return geometryModel;
        }

        public Canvas? Canvas
        {
            get { return _canvas; }
            set { _canvas = value; }
        }

        public Viewport3D? Viewport3DCube
        {
            get { return _viewport3DCube; }
            set { _viewport3DCube = value; }
        }

        public ModelVisual3D? VisualModelCube
        {
            get { return _visualModelCube; }
            set { _visualModelCube = value; }
        }

        public ModelVisual3D? VisualModelTriangle
        {
            get { return _visualModelTriangle; }
            set { _visualModelTriangle = value; }
        }


        public byte RRValue
        {
            get { return rrValue; }
            set 
            {
                rrValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }
        public byte RGValue
        {
            get { return rgValue; }
            set
            {
                rgValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }
        public byte RBValue
        {
            get { return rbValue; }
            set
            {
                rbValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }

        public byte GRValue
        {
            get { return grValue; }
            set
            {
                grValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }
        public byte GGValue
        {
            get { return ggValue; }
            set
            {
                ggValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }
        public byte GBValue
        {
            get { return gbValue; }
            set
            {
                gbValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }

        public byte BRValue
        {
            get { return brValue; }
            set
            {
                brValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }
        public byte BGValue
        {
            get { return bgValue; }
            set
            {
                bgValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
        }
        public byte BBValue
        {
            get { return bbValue; }
            set
            {
                bbValue = value;
                OnPropertyChanged();
                UpdateTriangleColor();
            }
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
