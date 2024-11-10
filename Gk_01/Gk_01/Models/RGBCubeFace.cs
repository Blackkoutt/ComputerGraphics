using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gk_01.Models
{
    public class RGBCubeFace
    {
        public Point3D Origin { get; set; }
        public Vector3D Width { get; set; }
        public Vector3D Height { get; set; }
        public RGBCubeMaterial Material { get; set; } = default!;
    }
}
