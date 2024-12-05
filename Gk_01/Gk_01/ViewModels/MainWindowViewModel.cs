using Gk_01.DI;
using Gk_01.Enums;
using Gk_01.Handlers;
using Gk_01.Helpers.ImagePointProcessing;
using Gk_01.Helpers.ImageProcessors;
using Gk_01.Helpers.ImageProcessors.ImageBinarization;
using Gk_01.Helpers.ImageProcessors.ImageFilters;
using Gk_01.Helpers.ImageProcessors.ImagePointProcessors;
using Gk_01.Helpers.ImageProcessors.MorphologicalOperators;
using Gk_01.Models;
using Gk_01.Observable;
using Gk_01.Services.Interfaces;
using Gk_01.Views;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Unity;

namespace Gk_01.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private static MainWindowViewModel? _instance = null;
        private ImagePointProcessingHandler _imagePointProcessingHandler;
        private HistogramViewModel _histogramViewModel;
        private Canvas? _canvas;
        private Image? _currentImage;
        private Image? _defaultImage;
        private WriteableBitmap? _originalImageWritableBitmap;
        private Stack<Image> imageProcessingUndoStack = [];
        private readonly int maxUndoOperations = 5;
        private Stack<Image> imageProcessingRedoStack = [];

        // Visibility
        private Visibility _curveDegreeVisibility = Visibility.Collapsed;
        private Visibility _curvePointsVisibility = Visibility.Collapsed;
        private Visibility _anglesCountVisibility = Visibility.Collapsed;
        private Visibility _translationVectorVisibility = Visibility.Collapsed;
        private Visibility _characteristicsPointVisibility = Visibility.Visible;
        private Visibility _rotationVectorVisibility = Visibility.Collapsed;
        private Visibility _rotationAngleVisibility = Visibility.Collapsed;
        private Visibility _scaleVisibility = Visibility.Collapsed;

        private Guid? _currentCharacteriticsPointID;
        private int polygonAnglesCount = 4;

        private int p_x;
        private int p_y;
        private int translation_X;
        private int translation_Y;
        private int rotationPoint_X;
        private int rotationPoint_Y;
        private int scalingPoint_X;
        private int scalingPoint_Y;

        private double scale_X;
        private double scale_Y;

        private double rotationAngle;

        private int curveDegree = 2;
        private int clickCount = 0;
        private List<Point> controlPoints = [];

        private int lineThickness = 1;
        private SolidColorBrush selectedLineColor = Brushes.Black;
        private SolidColorBrush selectedFillColor = Brushes.Transparent;
        private ShapeTypeEnum? _currentShapeType;
        private CustomPath? _currentShape;
        private bool _isResizing = false;
        private bool _isMoving = false;
        private bool _isRotateing = false;
        private bool _isScaling = false;
        private bool _isCanvasMoving = false;
        private bool _isSaved = true;
        private Point _mouseClickOnShapePosition;
        private Point _defaultRotationPosition;
        private Point _defaultScalingPosition;
        private Point _mouseClickOnCanvasPosition;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        private TransformGroup _canvasRenderTransform;
        private CanvasMode _actualCanvasMode = CanvasMode.Paint;
        private int imageDefaultLeft = 0;
        private int imageDefaultTop = 0;    

        private double zoomMax = 20;
        private double zoomMin = 0.5;
        private double zoomSpeed = 0.001;
        private double zoom = 1;

        // Structuring Element
        private int structuringElementSize = 3;

        private int curvePointsCount = 20;

        private Cursor _canvasCursor = Cursors.Cross;

        // Services
        private readonly IFileService _fileService;
        private readonly IDrawingService _drawingService;
        private readonly ITransformations2DService _transformations2DService;


        private CustomPath? _rotationPoint = null;
        private CustomPath? _scalingPoint = null;
        private Point? _defaultRotationPoint = null;
        public ICommand ChangeDrawingShapeCommand { get; set; }
        public ICommand DrawShapeCommand { get; set; }
        public ICommand CanvasMouseDownCommand { get; set; }
        public ICommand CanvasMouseUpCommand { get; set; }
        public ICommand CanvasMouseMoveCommand { get; set; }
        public ICommand SerializeCommand { get; set; }
        public ICommand DeserializeCommand { get; set; }
        public ICommand NewFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand CanvasMouseWheelCommand { get; set; }
        public ICommand LoadFileCommand { get; set; }
        public ICommand CanvasModeCommand { get; set; }
        public ICommand SaveFileCommand { get; set; }


        // Image points processing
        public ICommand ImageAdditionCommand { get; set; }
        public ICommand ImageSubtractionCommand { get; set; }
        public ICommand ImageMultiplicationCommand { get; set; }
        public ICommand ImageDivisionCommand { get; set; }
        public ICommand ImageChangeBrightnessCommand { get; set; }
        public ICommand ImageGrayscaleAverageMethodCommand { get; set; }
        public ICommand ImageGrayscaleLuminosityMethodCommand { get; set; }


        // Image filters
        public ICommand FilterAverageCommand { get; set; }
        public ICommand FilterMedianCommand { get; set; }
        public ICommand FilterVerticalSobelCommand { get; set; }
        public ICommand FilterHorizontalSobelCommand { get; set; }
        public ICommand FilterHighPassCommand { get; set; }
        public ICommand FilterGaussianCommand { get; set; }
        public ICommand FilterCustomCommand { get; set; }

        // Undo Redo
        public ICommand ResetImageCommand { get; set; }
        public ICommand RedoCommand { get; set; }


        // Histogram
        public ICommand HistogramCommand { get; set; }

        // Binarization
        public ICommand BinarizationThresholdCommand { get; set; }
        public ICommand BinarizationBlackSelectionCommand { get; set; }
        public ICommand BinarizationMeanIterativeSelectionCommand { get; set; }
        public ICommand BinarizationEntropySelectionCommand { get; set; }


        // Morphological operators 
        public ICommand DilatationOperatorCommand { get; set; }
        public ICommand ErosionOperatorCommand { get; set; }
        public ICommand OpenOperatorCommand { get; set; }
        public ICommand CloseOperatorCommand { get; set; }
        public ICommand HitOrMissOperatorCommand { get; set; }


        private ImageOperatorProcessor dilatationOperatorProcessor;
        private ImageOperatorProcessor erosionOperatorProcessor;
        private StructuringElementType _currentStructuringElementType = StructuringElementType.Square;

        private bool rotatePointSet = false;
        private bool scalingPointSet = false;

        public MainWindowViewModel(IFileService fileService, IDrawingService drawingService, ITransformations2DService transformations2DService)
        {
            ChangeDrawingShapeCommand = new RelayCommand(ChangeDrawingShape);
            DrawShapeCommand = new RelayCommand(DrawShapeHandler);
            CanvasMouseDownCommand = new RelayCommand(CanvasMouseDown);
            CanvasMouseUpCommand = new RelayCommand(CanvasMouseUp);
            CanvasMouseMoveCommand = new RelayCommand(CanvasMouseMove);
            SerializeCommand = new RelayCommand(SerializeObjects);
            DeserializeCommand = new RelayCommand(DeserializeObjects);
            NewFileCommand = new RelayCommand(NewFile);
            CloseCommand = new RelayCommand(Close);
            CanvasMouseWheelCommand = new RelayCommand(CanvasMouseWheel);
            LoadFileCommand = new RelayCommand(LoadGraphicFile);
            CanvasModeCommand = new RelayCommand(SetCanvasMode);
            SaveFileCommand = new RelayCommand(SaveFile);
            ResetImageCommand = new RelayCommand(ResetImage);
            RedoCommand = new RelayCommand(Redo);

            // Image points processing
            ImageAdditionCommand = SetImageProcessingCommandHandler(
                                    processor: new AdditionProcessor(),
                                    isDialog: true,
                                    title: "Dodawanie",
                                    labelText: "Wartość składnika: ");

            ImageSubtractionCommand = SetImageProcessingCommandHandler(
                                    processor: new SubtractionProcessor(),
                                    isDialog: true,
                                    title: "Odejmowanie",
                                    labelText: "Wartość odjemnika: ");

            ImageMultiplicationCommand = SetImageProcessingCommandHandler(
                                    processor: new MultiplicationProcessor(),
                                    isDialog: true,
                                    title: "Mnożenie",
                                    labelText: "Wartość mnożnika: ");

            ImageDivisionCommand = SetImageProcessingCommandHandler(
                                    processor: new DivisionProcessor(),
                                    isDialog: true,
                                    title: "Dzielenie",
                                    labelText: "Wartość dzielnika: ");

            ImageChangeBrightnessCommand = SetImageProcessingCommandHandler(
                                    processor: new BrightnessProcessor(),
                                    isDialog: true,
                                    title: "Zmiana jasności",
                                    labelText: "Wartość piksela: ");

            dilatationOperatorProcessor = new DilatationOperatorProcessor();
            DilatationOperatorCommand = SetImageProcessingCommandHandler(processor: dilatationOperatorProcessor);
            erosionOperatorProcessor = new ErosionOperatorProcessor();
            ErosionOperatorCommand = SetImageProcessingCommandHandler(processor: erosionOperatorProcessor);

            ImageGrayscaleAverageMethodCommand = SetImageProcessingCommandHandler(processor: new GrayscaleAverageMethodProcessor());
            ImageGrayscaleLuminosityMethodCommand = SetImageProcessingCommandHandler(processor: new GrayscaleLuminosityProcessor());
            FilterAverageCommand = SetImageProcessingCommandHandler(processor: new AverageFilter());
            FilterMedianCommand = SetImageProcessingCommandHandler(processor: new MedianFilter(3));
            FilterVerticalSobelCommand = SetImageProcessingCommandHandler(processor: new VerticalSobelFilter());
            FilterHorizontalSobelCommand = SetImageProcessingCommandHandler(processor: new HorizontalSobelFilter());
            FilterHighPassCommand = SetImageProcessingCommandHandler(processor: new HighPassFilter());
            FilterGaussianCommand = SetImageProcessingCommandHandler(processor: new GaussianFilter());

            FilterCustomCommand = new RelayCommand(param =>
                                  _imagePointProcessingHandler!.CustomFilter(param,
                                  defaultImage: _defaultImage,
                                  currentImage: _currentImage));

            HistogramCommand = new RelayCommand(param => _histogramViewModel!.ShowHistogram(param, currentImage: _currentImage));

            BinarizationThresholdCommand = SetImageProcessingCommandHandler(
                                    processor: new ThresholdBinarizationProcessor(),
                                    isDialog: true,
                                    title: "Binaryzacja",
                                    labelText: "Wartość progowa: ", 
                                    minValue: 0,
                                    maxValue: 255,
                                    defaultValue: 127);

            BinarizationBlackSelectionCommand = SetImageProcessingCommandHandler(
                                    processor: new BlackPercentSelectionBinarizationProcessor(),
                                    isDialog: true,
                                    title: "Binaryzacja",
                                    labelText: "Procent czarnych pikseli (%): ",
                                    minValue: 0,
                                    maxValue: 100,
                                    defaultValue: 50);

            BinarizationMeanIterativeSelectionCommand = new RelayCommand(param =>
                                                        _imagePointProcessingHandler!.AutoBinarizeImage(param,
                                                        imageProcessor: new MeanIterativeSelectionBinarizationProcessor(),
                                                        defaultImage: _defaultImage,
                                                        currentImage: _currentImage));

            BinarizationEntropySelectionCommand = new RelayCommand(param =>
                                                        _imagePointProcessingHandler!.AutoBinarizeImage(param,
                                                        imageProcessor: new EntropyBinarizationProcessor(),
                                                        defaultImage: _defaultImage,
                                                        currentImage: _currentImage));

            // Image filters
            _fileService = fileService;
            _drawingService = drawingService;
            _transformations2DService = transformations2DService;

            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _translateTransform = new TranslateTransform(0, 0);
            CanvasRenderTransform = new TransformGroup();
            CanvasRenderTransform.Children.Add(_scaleTransform);
            CanvasRenderTransform.Children.Add(_translateTransform);
        }

        private void SerializeObjects(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Serializuj jako",
                Filter = "Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml",
                FilterIndex = 1 // default txt
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    _fileService.SerializeToFile(filePath, _canvas!.Children);
                    _isSaved = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                                   "Błąd serializacji",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Error);
                }

            }
        }

        private void DeserializeObjects(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Deserializuj z",
                Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    var shapeDtoList = _fileService.DeserializeFromFile(filePath);
                    var badShapeTypes = _drawingService.DrawShapes(shapeDtoList);
                    if (badShapeTypes.Any())
                    {
                        MessageBox.Show($"Kilka figur zostało niewczytanych z powodu nieobsługiwanych typów: " +
                                        $"{string.Join(", ", badShapeTypes)}",
                                        "Błąd wczytywania",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}",
                                    "Błąd deserializacji",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        

        private void SetCurrentShape(CustomPath shapePath)
        {
            if (_currentShape != null) _currentShape.Stroke = selectedLineColor;
            _currentShape = shapePath;
            SelectedLineColor = ((SolidColorBrush)_currentShape!.Stroke);
            SelectedFillColor = ((SolidColorBrush)_currentShape!.Fill);
            LineThickness = (int)_currentShape!.StrokeThickness;
            _currentShape.Stroke = Brushes.Blue;
        }


        private void SetCanvasMode(object parameter)
        {
            if (parameter is string canvasModeString)
            {
                if (Enum.TryParse(typeof(CanvasMode), canvasModeString, out var parseResult))
                {
                    var canvasMode = (CanvasMode)parseResult;
                    switch (canvasMode)
                    {
                        case CanvasMode.Paint:
                            _actualCanvasMode = CanvasMode.Paint;
                            CanvasCursor = Cursors.Cross;
                            break;
                        case CanvasMode.Translate:
                            _actualCanvasMode = CanvasMode.Translate;
                            CanvasCursor = Cursors.SizeAll;
                            break;
                        case CanvasMode.Rotate:
                            Uri cursorUri = new Uri("pack://application:,,,/Assets/Rotate.cur");
                            _actualCanvasMode = CanvasMode.Rotate;
                            CanvasCursor = new Cursor(Application.GetResourceStream(cursorUri).Stream);
                            break;
                        case CanvasMode.Scaling:
                            _actualCanvasMode = CanvasMode.Scaling;
                            CanvasCursor = Cursors.SizeNWSE;
                            break;
                    }

                    if (_actualCanvasMode == CanvasMode.Translate)
                    {
                        TranslationVectorVisibility = Visibility.Visible;
                        CharacteristicsPointVisibility = Visibility.Collapsed;
                        RotationAngleVisibility = Visibility.Collapsed;
                        RotationVectorVisibility = Visibility.Collapsed;
                        ScaleVisibility = Visibility.Collapsed;
                    }
                    else if (_actualCanvasMode == CanvasMode.Rotate)
                    {
                        RotationVectorVisibility = Visibility.Visible;
                        RotationAngleVisibility = Visibility.Visible;
                        TranslationVectorVisibility = Visibility.Collapsed;
                        CharacteristicsPointVisibility = Visibility.Collapsed;
                        ScaleVisibility = Visibility.Collapsed;
                    }
                    else if (_actualCanvasMode == CanvasMode.Scaling)
                    {
                        ScaleVisibility = Visibility.Visible;
                        RotationVectorVisibility = Visibility.Collapsed;
                        RotationAngleVisibility = Visibility.Collapsed;
                        TranslationVectorVisibility = Visibility.Collapsed;
                        CharacteristicsPointVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TranslationVectorVisibility = Visibility.Collapsed;
                        RotationAngleVisibility = Visibility.Collapsed;
                        CharacteristicsPointVisibility = Visibility.Visible;
                        RotationVectorVisibility = Visibility.Collapsed;
                        ScaleVisibility = Visibility.Collapsed;
                    }
                }
            }
        }



        private void CanvasMouseDown(object parameter)
        {
            if (parameter is MouseButtonEventArgs e)
            {
                Point clickPosition = e.GetPosition(_canvas);
                var hitElement = _canvas!.InputHitTest(clickPosition) as UIElement;

                switch (_actualCanvasMode)
                {
                    case CanvasMode.Move:
                        _isCanvasMoving = true;
                        _mouseClickOnCanvasPosition = clickPosition;
                        break;
                    case CanvasMode.Paint:
                        if (_currentShapeType != null) DrawShapeByMouseClick(clickPosition);
                        if (hitElement != null && hitElement is CustomPath shapePath)
                        {
                            SetCurrentShape(shapePath);
                            //CheckWhatSegmentOfShapeWasClicked(shapePath, clickPosition);
                        }
                        else if (_currentShape != null)
                            SetShapeDefaultAppearance();
                        break;
                    case CanvasMode.Translate:
                        if(_currentShape != null)
                        {
                            _isMoving = true;
                            _mouseClickOnShapePosition = clickPosition;
                            //CanvasCursor = Cursors.SizeAll;
                        }
                        break;
                    case CanvasMode.Rotate:
                        if (_currentShape != null)
                        {
                            if (!rotatePointSet) AddRotationOrScalingPoint(clickPosition, TransformationPoint.Rotation);
                            else
                            {
                                _isRotateing = true;
                                _defaultRotationPosition = clickPosition;
                            } 
                        }
                        break;
                    case CanvasMode.Scaling:
                        if (_currentShape != null)
                        {
                            if (!scalingPointSet) AddRotationOrScalingPoint(clickPosition, TransformationPoint.Scaling);
                            else
                            {
                                _isScaling = true;
                                _defaultScalingPosition = clickPosition;
                            }
                        }
                        break;
                }
            }
        }
        private void AddRotationOrScalingPoint(Point clickPosition, TransformationPoint transformationPoint)
        {
            if(transformationPoint == TransformationPoint.Rotation)
            {
                _rotationPoint = _drawingService.DrawRotationOrScallingPoint(clickPosition);
                RotationPoint_X = (int)clickPosition.X;
                RotationPoint_Y = (int)clickPosition.Y;
                rotatePointSet = true;
            }
            else if (transformationPoint == TransformationPoint.Scaling) 
            {
                _scalingPoint = _drawingService.DrawRotationOrScallingPoint(clickPosition);
                ScalingPoint_X = (int)clickPosition.X;
                ScalingPoint_Y = (int)clickPosition.Y;
                scalingPointSet = true;
            }

        }


        private void CanvasMouseUp(object parameter)
        {
            if (_isResizing)
            {
                _isResizing = false;
                // _currentCharacteriticsPointID = null;
            }
            else if (_isMoving)
            {
                _isMoving = false;
                _transformations2DService.EndShapeTransformation(_currentShape!);
                //_currentShape!.EndShapeTransformation();
                _currentShape!.ReleaseMouseCapture();
            }
            else if (_isRotateing)
            {
                _isRotateing = false;
                rotatePointSet = false;
                _canvas!.Children.Remove(_rotationPoint);
                _rotationPoint = null;
                _transformations2DService.EndShapeTransformation(_currentShape!);
                //_currentShape!.EndShapeTransformation();
                _canvas!.ReleaseMouseCapture();
            }
            else if (_isScaling)
            {
                _isScaling = false;
                scalingPointSet = false;
                _canvas!.Children.Remove(_scalingPoint);
                _scalingPoint = null;
                _transformations2DService.EndShapeTransformation(_currentShape!);
                //_currentShape!.EndShapeTransformation();
                _canvas!.ReleaseMouseCapture();
            }
            else if (_isCanvasMoving)
            {
                foreach (UIElement child in _canvas!.Children)
                {
                    if (child is Image image)
                    {
                        imageDefaultLeft = (int)Canvas.GetLeft(image);
                        imageDefaultTop = (int)Canvas.GetTop(image);
                    }
                    if (child is CustomPath childPath)
                    {
                        _transformations2DService.EndShapeTransformation(childPath);
                        //childPath.EndShapeTransformation();
                    }
                }
                _isCanvasMoving = false;
                _canvas.ReleaseMouseCapture();
            }
            if (_actualCanvasMode == CanvasMode.Paint) CanvasCursor = Cursors.Cross;
            else if (_actualCanvasMode == CanvasMode.Translate) CanvasCursor = Cursors.SizeAll;
            if (_currentShape != null) _currentShape.ReleaseMouseCapture();
        }


        private void CheckWhatSegmentOfShapeWasClicked(CustomPath shapePath, Point mouseClickPoint)
        {
            bool isCharacteristicPoint = false;
            foreach (var (key, value) in shapePath.DrawingDictionary)
            {
                var geometryType = value.Type;
                var geometry = value.Figure.Geometry;

                if (geometryType == ShapeElement.CharacteristicPoint && geometry.FillContains(mouseClickPoint))
                {
                    PrepareToResizeShape(key, mouseClickPoint);
                    //var a = geometry is CustomPath;
                    isCharacteristicPoint = true;
                    break;
                }
            }
            /*if (!isCharacteristicPoint)
            {
                PrepareToMoveShape(mouseClickPoint);
            }*/
        }


        private void PrepareToResizeShape(Guid pointId, Point clickPoint)
        {
            _currentCharacteriticsPointID = pointId;
            P_X = (int)clickPoint.X;
            P_Y = (int)clickPoint.Y;
            CanvasCursor = Cursors.ScrollSE;
            _isResizing = true;
        }


        private void CanvasMouseMove(object parameter)
        {
            if (parameter is MouseEventArgs e)
            {
                Point currentMousePosition = e.GetPosition(_canvas);
                if (_isResizing)
                {
                    ResizeShape(currentMousePosition);
                }
                else if (_isMoving)
                {
                    _currentShape!.CaptureMouse();
                    Translation_X = (int)(currentMousePosition.X - _mouseClickOnShapePosition.X);
                    Translation_Y = (int)(currentMousePosition.Y - _mouseClickOnShapePosition.Y);
                }
                else if (_isRotateing)
                {
                    _canvas!.CaptureMouse();
                    var angleInRadians = Math.Atan2(currentMousePosition.Y - _defaultRotationPosition.Y, currentMousePosition.X - _defaultRotationPosition.X);
                    double angleDegrees = angleInRadians * (180 / Math.PI);
                    RotationAngle = angleDegrees;
                }
                else if (_isScaling)
                {
                    _canvas!.CaptureMouse();
                    Scale_X = Math.Abs(ScalingPoint_X - currentMousePosition.X) / Math.Abs(ScalingPoint_X - _defaultScalingPosition.X);
                    Scale_Y = Math.Abs(ScalingPoint_Y - currentMousePosition.Y) / Math.Abs(ScalingPoint_Y - _defaultScalingPosition.Y);
                }
                else if (_isCanvasMoving)
                {
                    _canvas!.CaptureMouse();
                    var deltaX = (int)(currentMousePosition.X - _mouseClickOnCanvasPosition.X);
                    var deltaY = (int)(currentMousePosition.Y - _mouseClickOnCanvasPosition.Y);
                    foreach (UIElement child in _canvas.Children)
                    {
                        if (child is Image image)
                        {
                            Canvas.SetLeft(image, imageDefaultLeft + deltaX);
                            Canvas.SetTop(image, imageDefaultTop + deltaY);
                        }
                        if (child is CustomPath childPath)
                        {
                            var moveVector = new Vector(deltaX, deltaY);
                            //childPath.TranslateShape(moveVector);
                            _transformations2DService.TranslateShape(childPath, moveVector);
                        }
                    }
                }

            }
        }
        private void ResizeShape(Point currentMousePosition)
        {
            P_X = (int)currentMousePosition.X;
            P_Y = (int)currentMousePosition.Y;
        }



       

        private void SetShapeDefaultAppearance()
        {
            _currentShape!.Stroke = selectedLineColor;
            _currentShape = null;
        }

        private void ChangeDrawingShape(object parameter)
        {
            if (parameter is string shapeTypeString)
            {
                if (Enum.TryParse(typeof(ShapeTypeEnum), shapeTypeString, out var parseResult))
                {
                    _currentShapeType = (ShapeTypeEnum)parseResult;
                    if (_currentShape != null && _currentShape.ShapeType != _currentShapeType.ToString())
                    {
                        _canvas!.Children.Remove(_currentShape);
                        _currentShape = _drawingService.DrawShape(_currentShapeType,
                                                  new List<Point> { _currentShape.StartPoint, _currentShape.EndPoint },
                                                  ((SolidColorBrush)_currentShape.Stroke).Color,
                                                  ((SolidColorBrush)_currentShape.Fill).Color,
                                                  (int)_currentShape.StrokeThickness);
                        _currentShapeType = null;
                    }

                    if (_currentShapeType == ShapeTypeEnum.Curve)
                    {
                        CurveDegreeVisibility = Visibility.Visible;
                        CurvePointsVisibility = Visibility.Visible;
                        AnglesCountVisibility = Visibility.Collapsed;
                    }
                    else if (_currentShapeType == ShapeTypeEnum.Polygon)
                    {
                        CurveDegreeVisibility = Visibility.Collapsed;
                        CurvePointsVisibility = Visibility.Collapsed;
                        AnglesCountVisibility = Visibility.Visible;
                    }
                    else
                    {
                        TranslationVectorVisibility = Visibility.Collapsed;
                        RotationVectorVisibility = Visibility.Collapsed;
                        CharacteristicsPointVisibility = Visibility.Visible;
                        CurveDegreeVisibility = Visibility.Collapsed;
                        CurvePointsVisibility = Visibility.Collapsed;
                        AnglesCountVisibility = Visibility.Collapsed;
                        RotationAngleVisibility = Visibility.Collapsed;
                        ScaleVisibility = Visibility.Collapsed;
                    }
                    controlPoints.Clear();
                    clickCount = 0;
                }

            }
        }

        private RelayCommand SetImageProcessingCommandHandler(ImageProcessor processor, bool isDialog = false, string title = "", string labelText = "", int minValue = int.MinValue, int maxValue = int.MaxValue, int defaultValue = 0)
        {
            if (isDialog)
            {
                return new RelayCommand(param =>
                    _imagePointProcessingHandler!.ShowDialog(param,
                    imageProcessor: processor,
                    defaultImage: _defaultImage,
                    currentImage: _currentImage,
                    title: title,
                    labelText: labelText,
                    minValue: minValue,
                    maxValue: maxValue,
                    defaultValue: defaultValue
                ));
            }
            else
            {
                return new RelayCommand(param =>
                   _imagePointProcessingHandler!.ProcessImage(param,
                   imageProcessor: processor,
                   defaultImage: _defaultImage,
                   currentImage: _currentImage
               ));
            }
        }

        private void Redo(object parameter)
        {
            // TO DO
        }

        private void ResetImage(object parameter)
        {
            if(_originalImageWritableBitmap != null)
            {
                _currentImage.Source = _originalImageWritableBitmap;
                _defaultImage.Source = _originalImageWritableBitmap;
                _canvas.Children.Clear();
                _canvas.Children.Add(_currentImage);         
            }
        }

        public void OnInit()
        {
            _imagePointProcessingHandler = ImagePointProcessingHandler.Instance;
            _histogramViewModel = HistogramViewModel.Instance;
            _imagePointProcessingHandler.Canvas = _canvas;
            _imagePointProcessingHandler.CloseProcessingDialogEvent += EndImageProcessing;
            _imagePointProcessingHandler.ProcessedImageEvent += EndImageProcessing;
        }

        private void EndImageProcessing(object? sender, EventArgs e)
        {
            if(_defaultImage != null && _currentImage != null)
            {
                _defaultImage = new Image
                {
                    Source = (_currentImage.Source as BitmapSource)?.Clone()
                };
            }
        }

        private void SaveFile(object parameter)
        {
            if (_currentImage != null)
            {
                Dictionary<FileType, string> fileTypeDictionary = new Dictionary<FileType, string>
                {
                    { FileType.PPM_P3, "PPM P3 files (*.ppm)|*.ppm" },
                    { FileType.PPM_P6, "PPM P6 files (*.ppm)|*.ppm" },
                    { FileType.JPEG, "JPEG files (*.jpeg;*.jpg)|*.jpeg;*.jpg" }
                };
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Zapisz jako",
                    Filter = string.Join("|", fileTypeDictionary.Values),
                    FilterIndex = 1 // default 
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var filePath = saveFileDialog.FileName;
                        var fileType = fileTypeDictionary.ElementAt(saveFileDialog.FilterIndex - 1).Key;
                        int? compressionLevel = null;
                        if (fileType == FileType.JPEG)
                        {
                            SelectCompressionLevelDialog optionDialog = new SelectCompressionLevelDialog();
                            var viewModel = optionDialog.DataContext as SelectCompressionLevelDialogViewModel;
                            if (optionDialog.ShowDialog() == true && viewModel != null)
                                compressionLevel = viewModel.SelectedOption;
                        }
                        _fileService.SaveImage(_currentImage, filePath, fileType, compressionLevel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                           "Błąd zapisu pliku graficznego",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Płótno nie zawiera żadnych obrazów",
                                "",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private async void LoadGraphicFile(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Otwórz plik graficzny",
                Filter = "PPM files (*.ppm)|*.ppm|JPEG files (*.jpeg;*.jpg)|*.jpeg;*.jpg",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    var image = await _fileService.LoadImage(filePath);
                    _currentImage = image;
                    _defaultImage = new Image
                    {
                        Source = (image.Source as BitmapSource)?.Clone()
                    };

                    var originalImageBitmapSource = _currentImage.Source as BitmapSource;
                    _originalImageWritableBitmap = new WriteableBitmap(originalImageBitmapSource);

                    _drawingService.ClearCanvas();
                    _canvas!.Children.Add(image);
                    imageDefaultLeft = 0;
                    imageDefaultTop = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                       "Błąd wczytywania pliku graficznego",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);
                }
            }
        }

        private void CanvasMouseWheel(object parameter)
        {
            if (parameter is MouseWheelEventArgs e)
            {
                double newZoom = zoom + zoomSpeed * e.Delta;
                if (newZoom < zoomMin) newZoom = zoomMin;
                if (newZoom > zoomMax) newZoom = zoomMax;

                Point mouseCanvasPos = e.GetPosition(_canvas);

                foreach (UIElement child in _canvas!.Children)
                {
                    if (child is Image image)
                    {
                        TransformGroup transformGroup;
                        if (image.RenderTransform is TransformGroup existingTransformGroup)
                        {
                            transformGroup = existingTransformGroup;
                        }
                        else
                        {
                            transformGroup = new TransformGroup();
                            transformGroup.Children.Add(new ScaleTransform(1, 1));
                            transformGroup.Children.Add(new TranslateTransform(0, 0));
                            image.RenderTransform = transformGroup;
                        }

                        var scaleTransform = (ScaleTransform)transformGroup.Children[0];
                        var translateTransform = (TranslateTransform)transformGroup.Children[1];

                        double scaleFactor = newZoom / zoom;

                        scaleTransform.ScaleX = newZoom;
                        scaleTransform.ScaleY = newZoom;

                        translateTransform.X = (mouseCanvasPos.X - translateTransform.X) * (1 - scaleFactor) + translateTransform.X;
                        translateTransform.Y = (mouseCanvasPos.Y - translateTransform.Y) * (1 - scaleFactor) + translateTransform.Y;
                    }
                }
                zoom = newZoom;
            }
        }

        private void DrawShapeHandler(object parameter)
        {
            if (_currentShapeType is not null)
            {
               /* _drawingService.DrawShape(
                        shapeType: _currentShapeType,
                        startPoint: new Point(p0_x, p0_y),
                        endPoint: new Point(p1_x, p1_y),
                        lineColor: selectedLineColor.Color,
                        fillColor: selectedFillColor.Color,
                        lineThickness: lineThickness);
                _currentShapeType = null;
                _isSaved = false;*/
            }
        }

        private void DrawShapeByMouseClick(Point clickPosition)
        {
            controlPoints.Add(clickPosition);
            clickCount++;


            bool isCurveReady = _currentShapeType == ShapeTypeEnum.Curve && clickCount == curveDegree + 1;
            bool isPolygonReady = _currentShapeType == ShapeTypeEnum.Polygon && clickCount == polygonAnglesCount;
            bool isOtherShapeReady = _currentShapeType != ShapeTypeEnum.Curve
                                     && _currentShapeType != ShapeTypeEnum.Polygon
                                     && clickCount == 2;

            if (isCurveReady || isPolygonReady || isOtherShapeReady)
            {
                _drawingService.DrawShape(
                        shapeType: _currentShapeType,
                        controlPoints: controlPoints,
                        lineColor: selectedLineColor.Color,
                        fillColor: selectedFillColor.Color,
                        lineThickness: lineThickness);
                _currentShapeType = null;
                _isSaved = false;
                clickCount = 0;
                controlPoints.Clear();
            }
        }


  



      

        private void Close(object parameter)
        {
            if (!_isSaved)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Czy na pewno chcesz zakończyć działanie programu? Masz niezapisane zmiany.",
                    "Potwierdzenie zamknięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }
            Application.Current.Shutdown();
        }

        private void NewFile(object parameter)
        {
            _drawingService.ClearCanvas();
            _currentImage = null;
            _defaultImage = null;
            P_X = 0;
            P_Y = 0;
            LineThickness = 1;
            SelectedLineColor = Brushes.Black;
            SelectedFillColor = Brushes.Transparent;
            _currentCharacteriticsPointID = null;
            _currentShape = null;
            _isResizing = false;
            _isMoving = false;
            _isSaved = true;
        }
        


        public TransformGroup CanvasRenderTransform
        {
            get { return _canvasRenderTransform; }
            set
            {
                _canvasRenderTransform = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush SelectedLineColor
        {
            get { return selectedLineColor; }
            set
            {
                selectedLineColor = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.Stroke = selectedLineColor;
            }
        }

        public SolidColorBrush SelectedFillColor
        {
            get { return selectedFillColor; }
            set
            {
                selectedFillColor = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.Fill = selectedFillColor;
            }
        }

        public Canvas Canvas 
        { 
            set 
            { 
                _canvas = value;
                _drawingService.Canvas = value;
            }
        }
        public Cursor CanvasCursor
        {
            get { return _canvasCursor; }
            set
            {
                _canvasCursor = value;
                OnPropertyChanged();
            }
        }

        public int LineThickness
        {
            get { return lineThickness; }
            set
            {
                lineThickness = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.StrokeThickness = lineThickness;
            }
        }

        private void RotateShape(double angleInRadians)
        {
            if (_currentShape != null)
            {
                var rotationPoint = new Point(RotationPoint_X, RotationPoint_Y);
                _transformations2DService.RotateShape(_currentShape!, rotationPoint, angleInRadians);
               // _currentShape!.RotateShape(rotationPoint, RotationAngle);
                _isSaved = false;
            }
        }


        public double RotationAngle
        {
            get { return rotationAngle; }
            set
            {
                rotationAngle = value;
                double rotationAngleInRadians = rotationAngle * (Math.PI / 180);
                RotateShape(rotationAngleInRadians);
                OnPropertyChanged();
            }
        }

        private void ScaleShape()
        {
            if (_currentShape != null)
            {
                var scalingPoint = new Point(ScalingPoint_X, ScalingPoint_Y);
                _transformations2DService.ScaleShape(_currentShape, scalingPoint, Scale_X, scale_Y);
                _isSaved = false;
            }
        }

        public double Scale_X
        {
            get { return scale_X; }
            set
            {
                scale_X = value;
                ScaleShape();
                OnPropertyChanged();
            }
        }
        public double Scale_Y
        {
            get { return scale_Y; }
            set
            {
                scale_Y = value;
                ScaleShape();
                OnPropertyChanged();
            }
        }

        public int RotationPoint_X
        {
            get { return rotationPoint_X; }
            set
            {
                rotationPoint_X = value;
                OnPropertyChanged();
            }
        }
        public int RotationPoint_Y
        {
            get { return rotationPoint_Y; }
            set
            {
                rotationPoint_Y = value;
                OnPropertyChanged();
            }
        }

        public int ScalingPoint_X
        {
            get { return scalingPoint_X; }
            set
            {
                scalingPoint_X = value;
                OnPropertyChanged();
            }
        }
        public int ScalingPoint_Y
        {
            get { return scalingPoint_Y; }
            set
            {
                scalingPoint_Y = value;
                OnPropertyChanged();
            }
        }

        private void TranslateShape()
        {
            if (_currentShape != null)
            {
                var translateVector = new Vector(Translation_X, Translation_Y);
                _transformations2DService.TranslateShape(_currentShape, translateVector);
               // _currentShape!.TranslateShape(translateVector);
                _isSaved = false;
            }
        }

        public int Translation_X
        {
            get { return translation_X; }
            set 
            {
                translation_X = value;
                TranslateShape();
                OnPropertyChanged();
            }
        }
        public int Translation_Y
        {
            get { return translation_Y; }
            set
            {
                translation_Y = value;
                TranslateShape();
                OnPropertyChanged();
            }
        }

        public int P_X
        {
            get { return p_x; }
            set
            {
                p_x = value;
                OnPropertyChanged();
                if (_currentShape != null && _currentCharacteriticsPointID != null)
                {
                    _currentShape.SetPointX((Guid)_currentCharacteriticsPointID, p_x);
                }

            }
        }
        public int P_Y
        {
            get { return p_y; }
            set
            {
                p_y = value;
                OnPropertyChanged();
                if (_currentShape != null && _currentCharacteriticsPointID != null)
                {
                    _currentShape.SetPointY((Guid)_currentCharacteriticsPointID, p_y);
                }

            }
        }

        public int CurveDegree
        {
            get { return curveDegree; }
            set
            {
                curveDegree = value;
                OnPropertyChanged();
            }
        }

        public int CurvePointsCount
        {
            get { return curvePointsCount; }
            set
            {
                curvePointsCount = value;
                OnPropertyChanged();
                if(_currentShape is Curve curve) curve.CurvePointsCount = curvePointsCount;
            }
        }

        public int PolygonAnglesCount
        {
            get { return polygonAnglesCount; }
            set
            {
                polygonAnglesCount = value;
                OnPropertyChanged();
            }
        }


        public Visibility RotationAngleVisibility
        {
            get => _rotationAngleVisibility;
            set
            {
                _rotationAngleVisibility = value;
                OnPropertyChanged();
            }
        }


        public Visibility RotationVectorVisibility
        {
            get => _rotationVectorVisibility;
            set
            {
                _rotationVectorVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility ScaleVisibility
        {
            get => _scaleVisibility;
            set
            {
                _scaleVisibility = value;
                OnPropertyChanged();
            }
        }



        public Visibility CharacteristicsPointVisibility
        {
            get => _characteristicsPointVisibility;
            set
            {
                _characteristicsPointVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility TranslationVectorVisibility
        {
            get => _translationVectorVisibility;
            set
            {
                _translationVectorVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CurveDegreeVisibility
        {
            get => _curveDegreeVisibility;
            set
            {
                _curveDegreeVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CurvePointsVisibility
        {
            get => _curvePointsVisibility;
            set
            {
                _curvePointsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility AnglesCountVisibility
        {
            get => _anglesCountVisibility;
            set
            {
                _anglesCountVisibility = value;
                OnPropertyChanged();
            }
        }

        public int StructuringElementSize
        {
            get => structuringElementSize;
            set
            {
                structuringElementSize = value;
                dilatationOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                erosionOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                OnPropertyChanged();
            }
        }

        public static MainWindowViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = DIContainer.GetContainer().Resolve<MainWindowViewModel>();
                    return _instance;
                }
                else
                    return _instance;
            }
        }
    }
}
