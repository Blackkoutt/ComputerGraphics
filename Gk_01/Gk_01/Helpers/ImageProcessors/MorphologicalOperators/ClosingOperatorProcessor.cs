namespace Gk_01.Helpers.ImageProcessors.MorphologicalOperators
{
    public sealed class ClosingOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var dilatationProcessor = new DilatationOperatorProcessor();
            dilatationProcessor.Size = this.size;
            dilatationProcessor.StructuringElement = this.structuringElement;
            byte[] dilatedPixels = dilatationProcessor.ProcessImageBitmap(pixelData, width, height, bytesPerPixel);

            var erosionProcessor = new ErosionOperatorProcessor();
            erosionProcessor.Size = this.size;
            erosionProcessor.StructuringElement = this.structuringElement;
            byte[] closedPixels = erosionProcessor.ProcessImageBitmap(dilatedPixels, width, height, bytesPerPixel);
          
            return closedPixels;
        }
    }
}
