namespace Gk_01.Helpers.ImageProcessors.MorphologicalOperators
{
    public sealed class DilatationOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        private byte Dilatate(byte[] pixelData, int width, int height, int imageX, int imageY)
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

        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var copyPixelData = new byte[pixelData.Length];
            pixelData.CopyTo(copyPixelData, 0);

            byte[] resultPixels = new byte[pixelData.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    var processedPixel = Dilatate(pixelData, width, height, x, y);
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
