using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.ImagePointProcessors
{
    public sealed class MultiplicationProcessor : ImageProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            Parallel.For(0, pixelData.Length, i =>
            {
                pixelData[i] = (byte)Math.Clamp(pixelData[i] * value, 0, 255);
            });
            return pixelData;
        }
    }
}
