using System.Windows.Media;

namespace Gk_01.Models
{
    public sealed class Circle : CustomPath
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                shapeType = ShapeTypeEnum.Circle.ToString();
                double radius = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));
                var ellipseGeometry = new EllipseGeometry(StartPoint, radius, radius);

                return ellipseGeometry;
            }
        }
    }
}
