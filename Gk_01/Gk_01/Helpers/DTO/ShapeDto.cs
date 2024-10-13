using System.Windows;

namespace Gk_01.Helpers.DTO
{
    public class ShapeDto
    {
        public string ShapeType { get; set; } = string.Empty;
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public string Stroke { get; set; } = string.Empty;
        public string Fill { get; set; } = string.Empty;
        public int StrokeTickness { get; set; }

    }
}
