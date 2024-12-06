using Gk_01.Helpers.ImagePointProcessing;

namespace Gk_01.Helpers.ImageProcessors
{
    public abstract class ImageAutoBinarizationProcessor : ImageProcessor
    {
        protected int threshold;
        public int Threshold => threshold;
        public abstract override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0);
    }
}
