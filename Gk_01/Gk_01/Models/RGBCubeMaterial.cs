using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gk_01.Models
{
    public class RGBCubeMaterial
    {
        public Color XColor { get; set; }
        public Color YColor { get; set; }
        public Color MaxColor { get; set; }
        public DiffuseMaterial Material { get; set; } = default!;
    }
}
