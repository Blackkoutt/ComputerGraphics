namespace Gk_01.Core.ImageProcessors
{
    public abstract class ImageAutoBinarizationProcessor : ImageProcessor
    {
        protected int threshold;
        public int Threshold => threshold;
        public abstract override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0);
    }
}
