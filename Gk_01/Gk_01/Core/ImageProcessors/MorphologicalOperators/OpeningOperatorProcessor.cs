using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.MorphologicalOperators
{
    public sealed class OpeningOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var erosionProcessor = new ErosionOperatorProcessor();
            erosionProcessor.Size = size;
            erosionProcessor.StructuringElement = structuringElement;
            byte[] erodedPixels = erosionProcessor.ProcessImageBitmap(pixelData, width, height, bytesPerPixel);

            var dilatationProcessor = new DilatationOperatorProcessor();
            dilatationProcessor.Size = size;
            dilatationProcessor.StructuringElement = structuringElement;
            byte[] openedPixels = dilatationProcessor.ProcessImageBitmap(erodedPixels, width, height, bytesPerPixel);

            return openedPixels;
        }
    }
}
