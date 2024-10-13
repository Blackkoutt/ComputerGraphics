using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gk_01.Models
{
    public sealed class Line : CustomPath
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                shapeType = ShapeTypeEnum.Line.ToString();

                PathGeometry geometry = new PathGeometry();

                PathFigure figure = new PathFigure
                {
                    StartPoint = StartPoint,
                    IsClosed = false
                };

                LineSegment line = new LineSegment
                {
                    Point = EndPoint
                };

                figure.Segments.Add(line);

                geometry.Figures.Add(figure);

                return geometry;
            }
        }
    }
}
