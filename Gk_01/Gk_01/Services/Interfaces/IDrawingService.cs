using Gk_01.Helpers.DTO;
using Gk_01.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gk_01.Services.Interfaces
{
    public interface IDrawingService
    {
        IEnumerable<string> DrawShapes(IEnumerable<ShapeDto> shapeDtoList);
        CustomPath DrawShape(ShapeTypeEnum? shapeType, List<Point> controlPoints, Color lineColor, Color fillColor, int lineThickness);
        Canvas Canvas { set; }
        void ClearCanvas();
    }
}
