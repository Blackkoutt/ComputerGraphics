using System.Windows.Media;

namespace Gk_01.Models
{
    public class Polygon : CustomPath
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                shapeType = ShapeTypeEnum.Polygon.ToString();

                if (CharacteristicPoints.Count < 3)
                    return Geometry.Empty;

                PathGeometry geometry = new PathGeometry();

                PathFigure figure = new PathFigure
                {
                    StartPoint = CharacteristicPoints.Values.First(),
                    IsClosed = true, 
                    IsFilled = true 
                };

                foreach (var point in CharacteristicPoints.Values.Skip(1))
                {
                    LineSegment lineSegment = new LineSegment
                    {
                        Point = point
                    };
                    figure.Segments.Add(lineSegment);
                }

                geometry.Figures.Add(figure);

                return geometry;
            }
        }
    }
}
