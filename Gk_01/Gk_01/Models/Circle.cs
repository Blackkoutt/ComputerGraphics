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
                var startPoint = CharacteristicPoints.Values.FirstOrDefault();
                var endPoint = CharacteristicPoints.Values.Skip(1).FirstOrDefault();
                double radius = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
                var ellipseGeometry = new EllipseGeometry(CharacteristicPoints.Values.FirstOrDefault(), radius, radius);

                return ellipseGeometry;
            }
        }
    }
}
