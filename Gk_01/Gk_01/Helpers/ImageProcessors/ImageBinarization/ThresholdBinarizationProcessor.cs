using Gk_01.Helpers.ImagePointProcessing;

namespace Gk_01.Helpers.ImageProcessors.ImageBinarization
{
    public sealed class ThresholdBinarizationProcessor : ImageProcessor
    {
        protected sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var threshold = value;
            for (var i =0; i < pixelData.Length; i+=bytesPerPixel)
            {
                var b = pixelData[i];
                var g = pixelData[i + 1];
                var r = pixelData[i + 2];
                var grayScale = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                if (grayScale < threshold) 
                {
                    if (i + 2 < pixelData.Length)
                    { 
                        pixelData[i] = 0;
                        pixelData[i + 1] = 0;
                        pixelData[i + 2] = 0;
                    }
                } 
                else
                {
                    if(i + 2 < pixelData.Length)
                    {
                        pixelData[i] = 255;
                        pixelData[i + 1] = 255;
                        pixelData[i + 2] = 255;
                    }
                }
                if (bytesPerPixel == 4 && i + 3 < pixelData.Length) pixelData[i + 3] = 255;
            }
            return pixelData;
        }
    }
}
