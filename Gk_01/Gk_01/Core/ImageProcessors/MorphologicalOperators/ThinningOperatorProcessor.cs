using Gk_01.Core.ImageProcessors;
using Gk_01.Enums;

namespace Gk_01.Core.ImageProcessors.MorphologicalOperators
{
    public sealed class ThinningOperatorProcessor : ImageMorphologicalOperatorProcessor
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

            hitOrMiss.HitOrMissType = HitOrMissType.Fit;

            return hitOrMiss.ProcessImageBitmap(pixelData, width, height, bytesPerPixel);
        }
    }
}
