using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.ImagePointProcessors
{
    public sealed class GrayscaleAverageMethodProcessor : ImageProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            for (var i = 0; i < pixelData.Length; i += bytesPerPixel)
            {
                var b = pixelData[i];
                var g = pixelData[i + 1];
                var r = pixelData[i + 2];
                var avgPixelValue = (byte)((b + g + r) / 3);
                pixelData[i] = avgPixelValue;
                pixelData[i + 1] = avgPixelValue;
                pixelData[i + 2] = avgPixelValue;
            }
            return pixelData;
        }
    }
}
