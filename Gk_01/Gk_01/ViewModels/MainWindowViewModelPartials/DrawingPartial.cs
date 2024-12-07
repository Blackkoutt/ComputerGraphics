using Gk_01.Enums;
using Gk_01.Models;
using Gk_01.Observable;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        private Guid? _currentCharacteriticsPointID;
        private int polygonAnglesCount = 4;

        private int p_x;
        private int p_y;

        private int curveDegree = 2;
        private int clickCount = 0;
        private int curvePointsCount = 20;
        private List<Point> controlPoints = [];

        private int lineThickness = 1;
        private SolidColorBrush selectedLineColor = Brushes.Black;
        private SolidColorBrush selectedFillColor = Brushes.Transparent;
        private ShapeTypeEnum? _currentShapeType;
        private CustomPath? _currentShape;

        // Commands
        public ICommand ChangeDrawingShapeCommand { get; set; }
        public ICommand DrawShapeCommand { get; set; }

        private void AddDrawingHandlers()
        {
            ChangeDrawingShapeCommand = new RelayCommand(ChangeDrawingShape);
            DrawShapeCommand = new RelayCommand(DrawShapeHandler);
        }

        private void PrepareToResizeShape(Guid pointId, Point clickPoint)
        {
            _currentCharacteriticsPointID = pointId;
            P_X = (int)clickPoint.X;
            P_Y = (int)clickPoint.Y;
            CanvasCursor = Cursors.ScrollSE;
            _isResizing = true;
        }

        private void ResizeShape(Point currentMousePosition)
        {
            P_X = (int)currentMousePosition.X;
            P_Y = (int)currentMousePosition.Y;
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

        private void ChangeDrawingShape(object parameter)
        {
            if (parameter is string shapeTypeString)
            {
                if (Enum.TryParse(typeof(ShapeTypeEnum), shapeTypeString, out var parseResult))
                {
                    _actualCanvasMode = CanvasMode.Paint;
                    CanvasCursor = Cursors.Cross;

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

        private void SetCurrentShape(CustomPath shapePath)
        {
            if (_currentShape != null) _currentShape.Stroke = selectedLineColor;
            _currentShape = shapePath;
            SelectedLineColor = (SolidColorBrush)_currentShape!.Stroke;
            SelectedFillColor = (SolidColorBrush)_currentShape!.Fill;
            LineThickness = (int)_currentShape!.StrokeThickness;
            _currentShape.Stroke = Brushes.Blue;
        }

        private void SetShapeDefaultAppearance()
        {
            _currentShape!.Stroke = selectedLineColor;
            _currentShape = null;
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
                if (_currentShape is Curve curve) curve.CurvePointsCount = curvePointsCount;
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
    }
}
