using Gk_01.Enums;
using Gk_01.Models;
using Gk_01.Observable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Canvas mode
        private CanvasMode _actualCanvasMode = CanvasMode.Paint;
        private bool _isCanvasMoving = false;
        private bool _isResizing = false;
        private bool _isTranslateing = false;
        private bool _isRotateing = false;
        private bool _isScaling = false;

        private int imageDefaultLeft = 0;
        private int imageDefaultTop = 0;

        // Cursor
        private Cursor _canvasCursor = Cursors.Cross;

        // Positions
        private Point _mouseClickOnShapePosition;
        private Point _defaultRotationPosition;
        private Point _defaultScalingPosition;
        private Point _mouseClickOnCanvasPosition;

        // Canvas Transform
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        private TransformGroup _canvasRenderTransform;

        // Zoom
        private double zoomMax = 20;
        private double zoomMin = 0.5;
        private double zoomSpeed = 0.001;
        private double zoom = 1;

        // Commands
        public ICommand CanvasMouseDownCommand { get; set; }
        public ICommand CanvasMouseUpCommand { get; set; }
        public ICommand CanvasMouseMoveCommand { get; set; }
        public ICommand CanvasMouseWheelCommand { get; set; }
        public ICommand CanvasModeCommand { get; set; }

        private void AddCanvasTransformations()
        {
            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _translateTransform = new TranslateTransform(0, 0);
            CanvasRenderTransform = new TransformGroup();
            CanvasRenderTransform.Children.Add(_scaleTransform);
            CanvasRenderTransform.Children.Add(_translateTransform);
        }

        private void AddCanvasHandlers()
        {
            CanvasMouseDownCommand = new RelayCommand(CanvasMouseDown);
            CanvasMouseUpCommand = new RelayCommand(CanvasMouseUp);
            CanvasMouseMoveCommand = new RelayCommand(CanvasMouseMove);
            CanvasMouseWheelCommand = new RelayCommand(CanvasMouseWheel);
            CanvasModeCommand = new RelayCommand(SetCanvasMode);
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
                        case CanvasMode.Move:
                            _actualCanvasMode = CanvasMode.Move;
                            CanvasCursor = Cursors.SizeAll;
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
                            
                            var shapeType = shapePath.ShapeType;
                            if(shapeType == ShapeTypeEnum.Circle.ToString() ||
                               shapeType == ShapeTypeEnum.Rectangle.ToString() ||
                               shapeType == ShapeTypeEnum.Line.ToString())
                            {
                                CheckWhatSegmentOfShapeWasClicked(shapePath, clickPosition);
                            }
                        }
                        else if (_currentShape != null)
                            SetShapeDefaultAppearance();
                        break;
                    case CanvasMode.Translate:
                        StartTranslation(clickPosition);
                        break;
                    case CanvasMode.Rotate:
                        StartRotation(clickPosition);
                        break;
                    case CanvasMode.Scaling:
                        StartScalling(clickPosition);
                        break;
                }
            }
        }

        private void CanvasMouseMove(object parameter)
        {
            if (parameter is MouseEventArgs e)
            {
                Point currentMousePosition = e.GetPosition(_canvas);
                if (_isResizing) ResizeShape(currentMousePosition);
                else if (_isTranslateing) TranslateShape(currentMousePosition);
                else if (_isRotateing) RotateShape(currentMousePosition);
                else if (_isScaling) ScaleShape(currentMousePosition);
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


        private void CanvasMouseUp(object parameter)
        {
            if (_isResizing)
            {
                _isResizing = false;
                // _currentCharacteriticsPointID = null;
            }
            else if (_isTranslateing) EndTranslation();
            else if (_isRotateing) EndRotation();
            else if (_isScaling) EndScaling();
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

        private void CheckWhatSegmentOfShapeWasClicked(CustomPath shapePath, Point mouseClickPoint)
        {
            foreach (var (key, value) in shapePath.DrawingDictionary)
            {
                var geometryType = value.Type;
                var geometry = value.Figure.Geometry;

                if (geometryType == ShapeElement.CharacteristicPoint && geometry.FillContains(mouseClickPoint))
                {
                    PrepareToResizeShape(key, mouseClickPoint);
                    break;
                }
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
        public TransformGroup CanvasRenderTransform
        {
            get { return _canvasRenderTransform; }
            set
            {
                _canvasRenderTransform = value;
                OnPropertyChanged();
            }
        }
    }
}
