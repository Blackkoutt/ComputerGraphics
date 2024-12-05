namespace Gk_01.Helpers.ImageProcessors.MorphologicalOperators
{
    public sealed class ErosionOperatorProcessor : ImageOperatorProcessor
    {
        protected override byte ProcessPixelWithMorphologicalOperator(byte[] pixelData, int width, int height, int imageX, int imageY)
        {
            byte minPixel = 255;
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
                        minPixel = Math.Min(minPixel, brightness);
                    }
                }
            }
            return minPixel;
        }
    }
}
