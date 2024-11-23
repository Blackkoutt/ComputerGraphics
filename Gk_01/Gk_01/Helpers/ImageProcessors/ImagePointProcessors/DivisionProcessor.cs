using Gk_01.Helpers.ImagePointProcessing;

namespace Gk_01.Helpers.ImageProcessors.ImagePointProcessors
{
    public sealed class DivisionProcessor : ImageProcessor
    {
        protected sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            if (value != 0)
            {
                Parallel.For(0, pixelData.Length, i =>
                {
                    pixelData[i] = (byte)Math.Clamp(pixelData[i] / value, 0, 255);
                });
            }
            return pixelData;
        }
    }
}
