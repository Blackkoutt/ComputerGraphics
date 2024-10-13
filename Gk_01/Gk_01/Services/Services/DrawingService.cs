using Gk_01.Exceptions;
using Gk_01.Helpers.DTO;
using Gk_01.Models;
using Gk_01.Services.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gk_01.Services.Services
{
    public class DrawingService : IDrawingService
    {
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
                      startPoint: shape.StartPoint,
                      endPoint: shape.EndPoint,
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


        public CustomPath DrawShape(ShapeTypeEnum? shapeType, Point startPoint, Point endPoint, Color lineColor, Color fillColor, int lineThickness)
        {
            CustomPath shape;
            switch (shapeType)
            {
                case ShapeTypeEnum.Rectangle:
                    shape = new Rectangle
                    {
                        StartPoint = startPoint,
                        EndPoint = endPoint,
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness
                    };
                    break;
                case ShapeTypeEnum.Circle:
                    shape = new Circle
                    {
                        StartPoint = startPoint,
                        EndPoint = endPoint,
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness
                    };
                    break;
                case ShapeTypeEnum.Line:
                    shape = new Line
                    {
                        StartPoint = startPoint,
                        EndPoint = endPoint,
                        Stroke = new SolidColorBrush(lineColor),
                        Fill = new SolidColorBrush(fillColor),
                        StrokeThickness = lineThickness
                    };
                    break;
                default: throw new UnrecognizedShapeTypeException("Nie rozpoznano typu figury.");
            }
            _canvas!.Children.Add(shape);
            return shape;
        }
    }
}
