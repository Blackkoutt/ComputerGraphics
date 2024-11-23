using System.Windows;
using System.Windows.Media;

namespace Gk_01.Models
{
    public sealed class Rectangle : CustomPath
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                shapeType = ShapeTypeEnum.Rectangle.ToString();

                PathGeometry geometry = new PathGeometry();

                var startPoint = CharacteristicPoints.Values.FirstOrDefault();
                var endPoint = CharacteristicPoints.Values.Skip(1).FirstOrDefault();
                PathFigure figure = new PathFigure
                {
                    StartPoint = startPoint,
                    IsClosed = true 
                };

                LineSegment rightTop = new LineSegment
                {
                    Point = new Point(endPoint.X, startPoint.Y)
                };

                LineSegment rightBottom = new LineSegment
                {
                    Point = endPoint 
                };

                LineSegment leftBottom = new LineSegment
                {
                    Point = new Point(startPoint.X, endPoint.Y) 
                };

                figure.Segments.Add(rightTop);
                figure.Segments.Add(rightBottom);
                figure.Segments.Add(leftBottom);
        
                geometry.Figures.Add(figure);

                return geometry;
            }
        }
    }
}
