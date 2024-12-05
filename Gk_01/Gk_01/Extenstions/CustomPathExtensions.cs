using Gk_01.Helpers.DTO;
using Gk_01.Models;

namespace Gk_01.Extenstions
{
    public static class CustomPathExtensions
    {
        public static ShapeDto AsDto(this CustomPath geometryPath)
        {
            return new ShapeDto
            {
                ControlPoints = geometryPath.CharacteristicPoints.Values.ToList(),
                ShapeType = geometryPath.ShapeType,
                Stroke = geometryPath.Stroke.ToString(),
                Fill = geometryPath.Fill.ToString(),
                StrokeTickness = (int)geometryPath.StrokeThickness
            };
        }
    }
}
