namespace Gk_01.Helpers.ImageProcessors.MorphologicalOperators
{
    public sealed class OpeningOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var erosionProcessor = new ErosionOperatorProcessor();
            erosionProcessor.Size = this.size;
            erosionProcessor.StructuringElement = this.structuringElement;
            byte[] erodedPixels = erosionProcessor.ProcessImageBitmap(pixelData, width, height, bytesPerPixel);
            
            var dilatationProcessor = new DilatationOperatorProcessor();
            dilatationProcessor.Size = this.size;
            dilatationProcessor.StructuringElement = this.structuringElement;
            byte[] openedPixels = dilatationProcessor.ProcessImageBitmap(erodedPixels, width, height, bytesPerPixel);

            return openedPixels;
        }
    }
}
