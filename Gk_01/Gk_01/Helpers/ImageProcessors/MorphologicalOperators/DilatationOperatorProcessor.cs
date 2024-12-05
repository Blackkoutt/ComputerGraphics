using Gk_01.Helpers.ImagePointProcessing;
using SixLabors.ImageSharp.PixelFormats;

namespace Gk_01.Helpers.ImageProcessors.MorphologicalOperators
{
    public class DilatationOperatorProcessor : ImageOperatorProcessor
    {
        protected override byte ProcessPixelWithMorphologicalOperator(byte[] pixelData, int width, int height, int imageX, int imageY)
        {
            byte maxPixel = 0;
            int halfSize = (size - 1) / 2;

            for (int ky = -halfSize; ky <= halfSize; ky++)
            {
                for (int kx = -halfSize; kx <= halfSize; kx++)
                {
                    int nx = imageX + kx;
                    int ny = imageY + ky;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && structuringElement[ky + halfSize, kx + halfSize])
                    {
                        int neighborIndex = (ny * width + nx) * 4;
                        byte brightness = pixelData[neighborIndex];
                        maxPixel = Math.Max(maxPixel, brightness);
                    }
                }
            }
            return maxPixel;
        }
    }
}
