using Gk_01.Core.ImageProcessors;
using Gk_01.Core.ImageProcessors.MorphologicalOperators;

namespace Gk_01.Core.ImageProcessors.MorphologicalOperators
{
    public sealed class ClosingOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var dilatationProcessor = new DilatationOperatorProcessor();
            dilatationProcessor.Size = size;
            dilatationProcessor.StructuringElement = structuringElement;
            byte[] dilatedPixels = dilatationProcessor.ProcessImageBitmap(pixelData, width, height, bytesPerPixel);

            var erosionProcessor = new ErosionOperatorProcessor();
            erosionProcessor.Size = size;
            erosionProcessor.StructuringElement = structuringElement;
            byte[] closedPixels = erosionProcessor.ProcessImageBitmap(dilatedPixels, width, height, bytesPerPixel);

            return closedPixels;
        }
    }
}
