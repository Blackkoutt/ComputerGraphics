using Gk_01.Core.ImageProcessors;
using Gk_01.Enums;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace Gk_01.Core.ImageProcessors.MorphologicalOperators
{
    public sealed class ThickeningOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        public override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            HitOrMissOperatorProcessor hitOrMiss = new HitOrMissOperatorProcessor();
            hitOrMiss.StructuringElement = new int[,]
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };
            hitOrMiss.HitOrMissType = HitOrMissType.Hit;

            return hitOrMiss.ProcessImageBitmap(pixelData, width, height, bytesPerPixel);
        }
    }
}
