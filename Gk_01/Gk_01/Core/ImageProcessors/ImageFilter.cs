namespace Gk_01.Core.ImageProcessors
{
    public abstract class ImageFilter : ImageProcessor
    {
        protected abstract (int[] Filter, int FilterSize) GetFilter();
        protected int FilterSum => GetFilter().Filter.Sum();
        public override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            var outputBitmap = new byte[pixelData.Length];
            pixelData.CopyTo(outputBitmap, 0);

            var getFilterResult = GetFilter();
            var Filter = getFilterResult.Filter;
            var FilterSize = getFilterResult.FilterSize;

            // Image bitmap
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixelIndex = (y * width + x) * bytesPerPixel;
                    var r = 0;
                    var g = 0;
                    var b = 0;

                    // Filter
                    for (int filterY = 0; filterY < FilterSize; filterY++)
                    {
                        for (int filterX = 0; filterX < FilterSize; filterX++)
                        {
                            // Neighbor pixel position
                            var neighborX = x - FilterSize / 2 + filterX;
                            int neighborY = y - FilterSize / 2 + filterY;

                            // Reflection on edges
                            neighborX = Math.Clamp(neighborX, 0, width - 1);
                            neighborY = Math.Clamp(neighborY, 0, height - 1);

                            int neighborIndex = (neighborY * width + neighborX) * bytesPerPixel;

                            // Neighbor pixel RGB
                            int neighborValueR = pixelData[Math.Clamp(neighborIndex, 0, pixelData.Length - 1)];
                            int neighborValueG = pixelData[Math.Clamp(neighborIndex + 1, 0, pixelData.Length - 1)];
                            int neighborValueB = pixelData[Math.Clamp(neighborIndex + 2, 0, pixelData.Length - 1)];

                            // Filtering pixel
                            r += neighborValueR * Filter[filterY * FilterSize + filterX];
                            g += neighborValueG * Filter[filterY * FilterSize + filterX];
                            b += neighborValueB * Filter[filterY * FilterSize + filterX];
                        }
                    }

                    // Normalization
                    outputBitmap[Math.Clamp(pixelIndex, 0, outputBitmap.Length - 1)] = (byte)Math.Clamp(r / Math.Clamp(FilterSum, 1, int.MaxValue), 0, 255);
                    outputBitmap[Math.Clamp(pixelIndex + 1, 0, outputBitmap.Length - 1)] = (byte)Math.Clamp(g / Math.Clamp(FilterSum, 1, int.MaxValue), 0, 255);
                    outputBitmap[Math.Clamp(pixelIndex + 2, 0, outputBitmap.Length - 1)] = (byte)Math.Clamp(b / Math.Clamp(FilterSum, 1, int.MaxValue), 0, 255);

                    if (bytesPerPixel == 4) outputBitmap[pixelIndex + 3] = pixelData[pixelIndex + 3];
                }
            }

            return outputBitmap;
        }
    }
}
