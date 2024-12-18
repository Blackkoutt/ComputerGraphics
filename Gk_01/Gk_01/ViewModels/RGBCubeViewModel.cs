﻿using Gk_01.DI;
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
    public enum RGBColorEnum
    {
        R,
        G,
        B
    }
    public class RGBCubeViewModel : BaseViewModel
    {
        private static RGBCubeViewModel? _instance = null;
        private Viewport3D? _viewport3DCube;
        private System.Windows.Shapes.Polygon? crossSectionSquare;
        private Canvas? _canvas;
        private ModelVisual3D? _visualModelCube;
        private ModelVisual3D? _visualModelTriangle;
        private List<RGBCubeFace> _cubeFaces;

        private RGBColorEnum currentMaxColor = RGBColorEnum.R;
        public ICommand LoadedCommand { get; }
        public ICommand ViewPortClickCommand { get; }
        public ICommand CheckMaxColorCommand { get; }

        private byte maxColorValue;

        public RGBCubeViewModel()
        {
            maxColorValue = 255;
            _cubeFaces = [];
            LoadedCommand = new RelayCommand(CreateCubeAndSquare);
            ViewPortClickCommand = new RelayCommand(ViewPortClick);
            CheckMaxColorCommand = new RelayCommand(CheckMaxColor);
        }

        private void CreateCubeAndSquare(object parameter)
        {
            CreateRGBCube();
            CreateCrossSectionSquare();
        }


        private void CreateCrossSectionSquare()
        {
            crossSectionSquare = new System.Windows.Shapes.Polygon
            {
                Points = new PointCollection
                {
                    new Point(0, 200),
                    new Point(200, 200),
                    new Point(200, 0),
                    new Point(0, 0),
                },
                Fill = GenerateColorMixingImage(Colors.Lime, Colors.Blue, Colors.Red)
            };

            ScaleTransform scaleTransform = new ScaleTransform
            {
                ScaleY = -1,
                CenterX = 100,
                CenterY = 100 
            };
            crossSectionSquare.RenderTransform = scaleTransform;

            Canvas!.Children.Clear();
            crossSectionSquare.HorizontalAlignment = HorizontalAlignment.Center;
            crossSectionSquare.VerticalAlignment = VerticalAlignment.Center;  
            Canvas.Children.Add(crossSectionSquare);
        }

        private void UpdateSquareColor()
        {
            if(currentMaxColor == RGBColorEnum.R)
                crossSectionSquare!.Fill = GenerateColorMixingImage(Colors.Lime, Colors.Blue, Color.FromRgb(MaxColorValue, 0, 0));
            
            if (currentMaxColor == RGBColorEnum.G)
                crossSectionSquare!.Fill = GenerateColorMixingImage(Colors.Blue, Colors.Red, Color.FromRgb(0, MaxColorValue, 0));
            
            if (currentMaxColor == RGBColorEnum.B)
                crossSectionSquare!.Fill = GenerateColorMixingImage(Colors.Lime, Colors.Red, Color.FromRgb(0, 0, MaxColorValue));
        }

        private ImageBrush GenerateColorMixingImage(Color xColor, Color yColor, Color maxColor)
        {
            int width = 255;
            int height = 255;

            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            // One max color r or g or b
            byte r = maxColor.R, g = maxColor.G, b = maxColor.B;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Others colors increase from 0 to 255
                    if (xColor == Colors.Red) r = (byte)x;
                    else if (xColor == Colors.Lime) g = (byte)x;
                    else if (xColor == Colors.Blue) b = (byte)x;

                    if (yColor == Colors.Red) r = (byte)y;
                    else if (yColor == Colors.Lime) g = (byte)y;
                    else if (yColor == Colors.Blue) b = (byte)y;

                    int index = (y * stride) + (x * 4);

                    // BGRA
                    pixelData[index + 0] = b;   
                    pixelData[index + 1] = g;
                    pixelData[index + 2] = r; 
                    pixelData[index + 3] = 255;  
                }
            }

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
            // Materials
            var firstMaterial = GetCubeMaterial(Colors.Lime, Colors.Blue, Colors.Red);
            var secondMaterial = GetCubeMaterial(Colors.Blue, Colors.Red, Colors.Lime);
            var thirdMaterial = GetCubeMaterial(Colors.Lime, Colors.Red, Colors.Blue);
            var fourthMaterial = GetCubeMaterial(Colors.Red, Colors.Blue, Colors.Black);
            var fifthMaterial = GetCubeMaterial(Colors.Lime, Colors.Blue, Colors.Black);
            var sixthMaterial = GetCubeMaterial(Colors.Lime, Colors.Red, Colors.Black);

            var cube = new Model3DGroup();

            // Cube faces
            cube.Children.Add(CreateFace(new Point3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), firstMaterial));    // front
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), fifthMaterial));   // back
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), fourthMaterial));    // left
            cube.Children.Add(CreateFace(new Point3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), secondMaterial));   // right
            cube.Children.Add(CreateFace(new Point3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), thirdMaterial));  // top
            cube.Children.Add(CreateFace(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), sixthMaterial));    // bottom

            // Light in scene
            var ambientLight = new AmbientLight(Colors.LightGray);

            cube.Children.Add(ambientLight);

            _visualModelCube!.Content = cube;
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
            
            // Corners
            mesh.Positions.Add(origin);
            mesh.Positions.Add(origin + width);
            mesh.Positions.Add(origin + width + height);
            mesh.Positions.Add(origin + height);

            // First triangle
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            // Second triangle
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            // Texture coordinates
            mesh.TextureCoordinates.Add(new Point(0, 0)); 
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(0, 1));

            var geometryModel = new GeometryModel3D(mesh, material.Material);          

            geometryModel.BackMaterial = material.Material;

            return geometryModel;
        }

        private void ViewPortClick(object parameter)
        {
            if (parameter is MouseEventArgs e)
            {
                Point mousePosition = e.GetPosition(_viewport3DCube);
                PointHitTestParameters hitParams = new PointHitTestParameters(mousePosition);
                VisualTreeHelper.HitTest(_viewport3DCube, null, ResultCallback, hitParams);
            }
        }


        private HitTestResultBehavior ResultCallback(HitTestResult result)
        {
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
        private void CheckMaxColor(object parameter)
        {
            if (parameter is string colorParameter)
            {
                if (Enum.TryParse(typeof(RGBColorEnum), colorParameter, out var result))
                {
                    currentMaxColor = (RGBColorEnum)result;
                    ApplySquareTransform();
                    UpdateSquareColor();
                }          
            }
        }

        private void ApplySquareTransform()
        {
            if(currentMaxColor == RGBColorEnum.R)
            {
                ScaleTransform scaleTransform = new ScaleTransform
                {
                    ScaleY = -1,
                    CenterX = 100,
                    CenterY = 100
                };
                crossSectionSquare!.RenderTransform = scaleTransform;
            }
            else if (currentMaxColor == RGBColorEnum.G)
            {
                ScaleTransform scaleTransform = new ScaleTransform
                {
                    ScaleX = -1,
                    ScaleY = -1,
                    CenterX = 100,
                    CenterY = 100
                };
                crossSectionSquare!.RenderTransform = scaleTransform;
            }
            else if(currentMaxColor == RGBColorEnum.B)
            {
                ScaleTransform scaleTransform = new ScaleTransform
                {
                    ScaleX = 1,
                    ScaleY = 1,
                    CenterX = 100,
                    CenterY = 100
                };
                crossSectionSquare!.RenderTransform = scaleTransform;
            }
        }

        public byte MaxColorValue
        {
            get { return maxColorValue; }
            set 
            {
                maxColorValue = value;
                OnPropertyChanged();
                UpdateSquareColor();
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
