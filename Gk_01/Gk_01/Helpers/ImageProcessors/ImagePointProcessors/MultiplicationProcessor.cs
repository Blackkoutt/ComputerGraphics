using Gk_01.Helpers.ImagePointProcessing;

namespace Gk_01.Helpers.ImageProcessors.ImagePointProcessors
{
    public sealed class MultiplicationProcessor : ImageProcessor
    {
        protected sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            Parallel.For(0, pixelData.Length, i =>
            {
                pixelData[i] = (byte)(pixelData[i] * value);
            });
            return pixelData;
        }
    }
}
