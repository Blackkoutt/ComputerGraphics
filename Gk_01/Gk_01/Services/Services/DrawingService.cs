using Gk_01.Exceptions;
using Gk_01.Helpers.DTO;
using Gk_01.Models;
using Gk_01.Services.Interfaces;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gk_01.Services.Services
{
    public class DrawingService(IBezierCurveCalculatorService bezierCurveCalculatorService) : IDrawingService
    {
        private readonly IBezierCurveCalculatorService _bezierCurveCalculatorService = bezierCurveCalculatorService;
        private Canvas? _canvas;
        public Canvas Canvas { set { _canvas = value; } }    

        public void ClearCanvas() => _canvas!.Children.Clear();
        
        
        public IEnumerable<string> DrawShapes(IEnumerable<ShapeDto> shapeDtoList)
        {
            List<string> badShapeTypes = [];
            foreach (var shape in shapeDtoList)
            {
                var shapeType = Enum.TryParse(typeof(ShapeTypeEnum), shape.ShapeType, true, out var shapeTypeEnum);
                if (shapeType)
                {
                    DrawShape(
                      shapeType: (ShapeTypeEnum)shapeTypeEnum!,
                      controlPoints: shape.ControlPoints,
                      lineColor: (Color)ColorConverter.ConvertFromString(shape.Stroke),
                      fillColor: (Color)ColorConverter.ConvertFromString(shape.Fill),
                      lineThickness: shape.StrokeTickness);
                }
                else if (!badShapeTypes.Contains(shape.ShapeType))
                {
                    badShapeTypes.Add(shape.ShapeType);
                }
            }
            return badShapeTypes;
        }


        public CustomPath DrawRotationOrScallingPoint(Point clickPoint)
        {
            var radius = 8;
            List<Point> controlPoints = new List<Point>()
            {
                clickPoint, new Point(clickPoint.X + radius, clickPoint.Y)
            };
            var characteristicsPointsDict = CreateCharacteristicsPointDict(controlPoints);
            var point =  new Circle
            {
                CharacteristicPoints = characteristicsPointsDict,
                DefaultCharacteristicPoints = CopyDictionary(characteristicsPointsDict),
                Stroke = new SolidColorBrush(Colors.Blue),
                Fill = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 1,
            };
            _canvas!.Children.Add(point);
            return point;
        }

        public CustomPath DrawShape(ShapeTypeEnum? shapeType, List<Point> controlPoints, Color lineColor, Color fillColor, int lineThickness)
        {
            CustomPath shape;
            var characteristicsPointsDict = CreateCharacteristicsPointDict(controlPoints);
            switch (shapeType)
            {
   
                case ShapeTypeEnum.Rectangle:
                    shape = new Rectangle
                    {
                        CharacteristicPoints = characteristicsPointsDict,
                        DefaultCharacteristicPoints = CopyDictionary(characteristicsPointsDict),
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness,
                    };
                    break;
                case ShapeTypeEnum.Circle:
                    shape = new Circle
                    {
                        CharacteristicPoints = characteristicsPointsDict,
                        DefaultCharacteristicPoints = CopyDictionary(characteristicsPointsDict),
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness,
                    };
                    break;
                case ShapeTypeEnum.Line:
                    shape = new Line
                    {
                        CharacteristicPoints = characteristicsPointsDict,
                        DefaultCharacteristicPoints = CopyDictionary(characteristicsPointsDict),
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness,
                    };
                    break;
                case ShapeTypeEnum.Curve:
                    shape = new Curve
                    {
                        BezierCurveCalculatorService = _bezierCurveCalculatorService,
                        CharacteristicPoints = characteristicsPointsDict,
                        DefaultCharacteristicPoints = CopyDictionary(characteristicsPointsDict),
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness,
                    };
                    break;
                case ShapeTypeEnum.Polygon:
                    shape = new Polygon
                    {
                        CharacteristicPoints = characteristicsPointsDict,
                        DefaultCharacteristicPoints = CopyDictionary(characteristicsPointsDict),
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness,
                    };
                    break;
                default: throw new UnrecognizedShapeTypeException("Nie rozpoznano typu figury.");
            }
            _canvas!.Children.Add(shape);
            return shape;
        }

        private Dictionary<Guid, Point> CreateCharacteristicsPointDict(List<Point> points)
        {
            Dictionary<Guid, Point> pointsDict = [];
            foreach (var point in points)
            {
                pointsDict.Add(Guid.NewGuid(), point);
            }
            return pointsDict;
        }

        private Dictionary<Guid, Point> CopyDictionary(Dictionary<Guid,Point> dictionaryToCopy)
        {
            return dictionaryToCopy.ToDictionary(pair => pair.Key,
                 pair => new Point(pair.Value.X, pair.Value.Y));
        }
    }
}
