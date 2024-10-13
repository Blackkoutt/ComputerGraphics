using Gk_01.DI;
using Gk_01.Enums;
using Gk_01.Models;
using Gk_01.Observable;
using Gk_01.Services.Interfaces;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace Gk_01.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private static MainWindowViewModel? _instance = null;
        private Canvas? _canvas;
        private int p0_x;
        private int p0_y;
        private int p1_x;
        private int p1_y;
        private int lineThickness = 1;
        private Color selectedLineColor = Colors.Black;
        private Color selectedFillColor = Colors.Transparent;
        private Point? firstClickPoint;
        private ShapeTypeEnum? _currentShapeType;
        private CustomPath? _currentShape;
        private ShapeElement? _currentCharacteristicPoint;
        private bool _isResizing = false;
        private bool _isMoving = false;
        private bool _isSaved = true;
        private Point _mouseClickOnShapePosition;
        private Point _defaultShape_P0;
        private Point _defaultShape_P1;
        private Cursor _canvasCursor = Cursors.Cross;
        private readonly IFileService _fileService;
        private readonly IDrawingService _drawingService;
        public ICommand ChangeDrawingShapeCommand { get; set; }
        public ICommand DrawShapeCommand { get; set; }
        public ICommand CanvasMouseDownCommand { get; set; }
        public ICommand CanvasMouseUpCommand { get; set; }
        public ICommand CanvasMouseMoveCommand { get; set; }
        public ICommand SerializeCommand { get; set; }
        public ICommand DeserializeCommand { get; set; }
        public ICommand NewFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public MainWindowViewModel(IFileService fileService, IDrawingService drawingService)
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
            _fileService = fileService;
            _drawingService = drawingService;
        }

        private void DrawShapeHandler(object parameter)
        {
            if (_currentShapeType is not null)
            {
                _drawingService.DrawShape(
                        shapeType: _currentShapeType,
                        startPoint: new Point(p0_x, p0_y),
                        endPoint: new Point(p1_x, p1_y),
                        lineColor: selectedLineColor,
                        fillColor: selectedFillColor,
                        lineThickness: lineThickness);
                _currentShapeType = null;
                _isSaved = false;
            }
        }

        private void DrawShapeByMouseClick(Point clickPosition)
        {
            if (firstClickPoint == null)
                firstClickPoint = clickPosition;
            else
            {
                var secondClickPoint = clickPosition;
                _drawingService.DrawShape(
                    shapeType: _currentShapeType,
                    startPoint: (Point)firstClickPoint,
                    endPoint: secondClickPoint,
                    lineColor: selectedLineColor,
                    fillColor: selectedFillColor,
                    lineThickness: lineThickness);
                _currentShapeType = null;
                firstClickPoint = null;
                _isSaved = false;
            }
        }


        private void CanvasMouseDown(object parameter)
        {
            if (parameter is MouseButtonEventArgs e)
            {
                Point clickPosition = e.GetPosition(_canvas);
                var hitElement = _canvas!.InputHitTest(clickPosition) as UIElement;

                if (_currentShapeType != null)
                    DrawShapeByMouseClick(clickPosition);

                if (firstClickPoint == null && hitElement != null && hitElement is CustomPath shapePath)
                {
                    // Set previous shape color to default color
                    if (_currentShape != null)
                        _currentShape.Stroke = new SolidColorBrush(selectedLineColor);

                    _currentShape = shapePath;
                    LoadClickedShapeInfoAndChangeColor(shapePath);

                    CheckWhatSegmentOfShapeWasClicked(shapePath, clickPosition);
                }
                else if (_currentShape != null)
                    SetShapeDefaultAppearance();
            }
        }

        private void LoadClickedShapeInfoAndChangeColor(CustomPath shapePath)
        {
            // Load clicked shape info
            P0_X = (int)_currentShape!.StartPoint.X;
            P0_Y = (int)_currentShape!.StartPoint.Y;
            P1_X = (int)_currentShape!.EndPoint.X;
            P1_Y = (int)_currentShape!.EndPoint.Y;
            SelectedLineColor = ((SolidColorBrush)_currentShape!.Stroke).Color;
            SelectedFillColor = ((SolidColorBrush)_currentShape!.Fill).Color;
            LineThickness = (int)_currentShape!.StrokeThickness;

            // Set clicked shape color
            _currentShape.Stroke = Brushes.Blue;
        }


        private void CheckWhatSegmentOfShapeWasClicked(CustomPath shapePath, Point mouseClickPoint)
        {
            foreach (var (element, drawing) in shapePath.DrawingDictionary)
            {
                var geometry = drawing.Geometry;
                bool isCharactertisticPoint = (element == ShapeElement.P0_Point || element == ShapeElement.P1_Point);

                // If characteristic point was clicked
                if (isCharactertisticPoint && geometry.FillContains(mouseClickPoint))
                {
                    PrepareToResizeShape(element);
                    break;
                }

                // If shape was clicked
                else if (element == ShapeElement.Figure && geometry.FillContains(mouseClickPoint))
                {
                    PrepareToMoveShape(mouseClickPoint);
                    break;
                }
            }
        }

        private void PrepareToResizeShape(ShapeElement characteristicPoint)
        {
            _currentCharacteristicPoint = characteristicPoint; // P0 or P1
            CanvasCursor = Cursors.SizeNWSE;
            _isResizing = true;
        }

        private void PrepareToMoveShape(Point mouseClickPoint)
        {
            _isMoving = true;
            _mouseClickOnShapePosition = mouseClickPoint;
            _defaultShape_P0 = new Point(_currentShape!.StartPoint.X, _currentShape!.StartPoint.Y);
            _defaultShape_P1 = new Point(_currentShape!.EndPoint.X, _currentShape!.EndPoint.Y);
            CanvasCursor = Cursors.SizeAll;
            _currentShape.CaptureMouse();
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
                if (_isMoving)
                {
                    MoveShape(currentMousePosition);
                }
            
            }
        }

        private void ResizeShape(Point currentMousePosition)
        {
            if (_currentCharacteristicPoint == ShapeElement.P0_Point)
            {
                P0_X = (int)currentMousePosition.X;
                P0_Y = (int)currentMousePosition.Y;
                _isSaved = false;

            }
            else if (_currentCharacteristicPoint == ShapeElement.P1_Point)
            {
                P1_X = (int)currentMousePosition.X;
                P1_Y = (int)currentMousePosition.Y;
                _isSaved = false;
            }
        }

        private void MoveShape(Point currentMousePosition)
        {
            var deltaX = (int)(currentMousePosition.X - _mouseClickOnShapePosition.X);
            var deltaY = (int)(currentMousePosition.Y - _mouseClickOnShapePosition.Y);
            P0_X = (int)_defaultShape_P0.X + deltaX;
            P0_Y = (int)_defaultShape_P0.Y + deltaY;
            P1_X = (int)_defaultShape_P1.X + deltaX;
            P1_Y = (int)_defaultShape_P1.Y + deltaY;
            _isSaved = false;
        }

        private void CanvasMouseUp(object parameter)
        {
            _isResizing = false;
            _isMoving = false;
            CanvasCursor = Cursors.Cross;
            if (_currentShape != null) _currentShape.ReleaseMouseCapture();
        }   

        private void SetShapeDefaultAppearance()
        {
            _currentShape!.Stroke = new SolidColorBrush(selectedLineColor);
            _currentShape = null;
        }

        private void ChangeDrawingShape(object parameter)
        {
            if(parameter is string shapeTypeString)
            {
                if (Enum.TryParse(typeof(ShapeTypeEnum), shapeTypeString, out var parseResult))
                {
                    _currentShapeType = (ShapeTypeEnum)parseResult;
                    if(_currentShape != null && _currentShape.ShapeType != _currentShapeType.ToString())
                    {
                        _canvas!.Children.Remove(_currentShape);
                        _currentShape = _drawingService.DrawShape(_currentShapeType,
                                                  _currentShape.StartPoint,
                                                  _currentShape.EndPoint,
                                                  ((SolidColorBrush)_currentShape.Stroke).Color,
                                                  ((SolidColorBrush)_currentShape.Fill).Color,
                                                  (int)_currentShape.StrokeThickness);
                        firstClickPoint = null;
                        _currentShapeType = null;
                    }
                }
                    
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
            P0_X = 0;
            P0_Y = 0;
            P1_X = 0;
            P1_Y = 0;
            LineThickness = 1;
            SelectedLineColor = Colors.Black;
            SelectedFillColor = Colors.Transparent;
            firstClickPoint = null;
            _currentShapeType = null;
            _currentShape = null;
            _currentCharacteristicPoint = null;
            _isResizing = false;
            _isMoving = false;
            _isSaved = true;
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

        public Color SelectedLineColor
        {
            get { return selectedLineColor; }
            set
            {
                selectedLineColor = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.Stroke = new SolidColorBrush(selectedLineColor);
            }
        }

        public Color SelectedFillColor
        {
            get { return selectedFillColor; }
            set
            {
                selectedFillColor = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.Fill = new SolidColorBrush(selectedFillColor);
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
        public int P0_X
        {
            get { return p0_x; }
            set
            {
                p0_x = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.SetStartPointX(p0_x);
            }
        }
        public int P0_Y
        {
            get { return p0_y; }
            set
            {
                p0_y = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.SetStartPointY(p0_y);
            }
        }
        public int P1_X
        {
            get { return p1_x; }
            set
            {
                p1_x = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.SetEndPointX(p1_x);
            }
        }
        public int P1_Y
        {
            get { return p1_y; }
            set
            {
                p1_y = value;
                OnPropertyChanged();
                if (_currentShape != null) _currentShape.SetEndPointY(p1_y);
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
