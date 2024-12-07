using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.MorphologicalOperators
{
    public sealed class ErosionOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        private byte Erode(byte[] pixelData, int width, int height, int imageX, int imageY)
        {
            byte minPixel = 255;
            int halfSize = (size - 1) / 2;

            for (int ky = -halfSize; ky <= halfSize; ky++)
            {
                for (int kx = -halfSize; kx <= halfSize; kx++)
                {
                    int nx = imageX + kx;
                    int ny = imageY + ky;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && structuringElement[ky + halfSize, kx + halfSize] == 1)
                    {
                        int neighborIndex = (ny * width + nx) * 4;
                        byte brightness = pixelData[neighborIndex];
                        minPixel = Math.Min(minPixel, brightness);
                    }
                }
            }
            return minPixel;
        }

        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var copyPixelData = new byte[pixelData.Length];
            pixelData.CopyTo(copyPixelData, 0);

            byte[] resultPixels = new byte[pixelData.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    var processedPixel = Erode(pixelData, width, height, x, y);
                    int pixelIndex = (y * width + x) * 4;
                    resultPixels[pixelIndex] = processedPixel;     // B
                    resultPixels[pixelIndex + 1] = processedPixel; // G
                    resultPixels[pixelIndex + 2] = processedPixel; // R
                    resultPixels[pixelIndex + 3] = copyPixelData[pixelIndex + 3]; // Alpha
                }
            }
            return resultPixels;
        }
    }
}
