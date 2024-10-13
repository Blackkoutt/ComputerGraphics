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

                PathFigure figure = new PathFigure
                {
                    StartPoint = StartPoint,
                    IsClosed = true 
                };

                LineSegment rightTop = new LineSegment
                {
                    Point = new Point(EndPoint.X, StartPoint.Y)
                };

                LineSegment rightBottom = new LineSegment
                {
                    Point = EndPoint 
                };

                LineSegment leftBottom = new LineSegment
                {
                    Point = new Point(StartPoint.X, EndPoint.Y) 
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
