using Gk_01.Helpers.ImagePointProcessing;

namespace Gk_01.Helpers.ImageProcessors.ImagePointProcessors
{
    public sealed class GrayscaleLuminosityProcessor : ImageProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            for (var i = 0; i < pixelData.Length; i += bytesPerPixel)
            {
                var b = pixelData[i];
                var g = pixelData[i + 1];
                var r = pixelData[i + 2];
                var pixelValue = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                pixelData[i] = pixelValue;
                pixelData[i + 1] = pixelValue;
                pixelData[i + 2] = pixelValue;
            }
            return pixelData;
        }
    }
}
