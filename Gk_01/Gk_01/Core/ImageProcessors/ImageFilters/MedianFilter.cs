using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.ImageFilters
{
    public class MedianFilter(int filterSize) : ImageProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var outputBitmap = new byte[pixelData.Length];
            pixelData.CopyTo(outputBitmap, 0);

            // Image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * bytesPerPixel;

                    List<int> rValues = new List<int>();
                    List<int> gValues = new List<int>();
                    List<int> bValues = new List<int>();

                    // Filter
                    for (int filterY = 0; filterY < filterSize; filterY++)
                    {
                        for (int filterX = 0; filterX < filterSize; filterX++)
                        {
                            // Neighbor pixel
                            int neighborX = x - filterSize / 2 + filterX;
                            int neighborY = y - filterSize / 2 + filterY;

                            if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                            {
                                int neighborIndex = (neighborY * width + neighborX) * bytesPerPixel;

                                rValues.Add(pixelData[neighborIndex]);
                                gValues.Add(pixelData[neighborIndex + 1]);
                                bValues.Add(pixelData[neighborIndex + 2]);
                            }
                        }
                    }

                    rValues.Sort();
                    gValues.Sort();
                    bValues.Sort();

                    // Median 
                    int medianR = rValues[rValues.Count / 2];
                    int medianG = gValues[gValues.Count / 2];
                    int medianB = bValues[bValues.Count / 2];

                    // Filtered bitmap
                    outputBitmap[pixelIndex] = (byte)medianR;
                    outputBitmap[pixelIndex + 1] = (byte)medianG;
                    outputBitmap[pixelIndex + 2] = (byte)medianB;

                    if (bytesPerPixel == 4)
                        outputBitmap[pixelIndex + 3] = pixelData[pixelIndex + 3];
                }
            }

            return outputBitmap;
        }

    }
}
