using System.Reflection.Metadata.Ecma335;
using System.Windows.Media.Imaging;

namespace Gk_01.Helpers.ImageProcessors.ImageBinarization
{
    public sealed class EntropyBinarizationProcessor : ImageAutoBinarizationProcessor
    {
        public sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            threshold = 0;
            double maxEntropy = 0;

            threshold = (int)CalculateEntropy(pixelData, bytesPerPixel);

            // Binaryzacja obrazu na podstawie wybranego progu
            for (var i = 0; i < pixelData.Length; i += bytesPerPixel)
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
                    if (i + 2 < pixelData.Length)
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

        private double CalculateEntropy(byte[] pixelData, int bytesPerPixel)
        {
            int[] histogram = new int[256];
            int totalPixels = pixelData.Length / bytesPerPixel;

            // Oblicz histogram
            for (int i = 0; i < pixelData.Length; i += bytesPerPixel)
            {
                byte grayScale = (byte)(0.299 * pixelData[i + 2] + 0.587 * pixelData[i + 1] + 0.114 * pixelData[i]);
                histogram[grayScale]++;
            }

            int bestThreshold = 0;
            double maxEntropy = 0;

            // Znajdź próg maksymalizujący entropię
            for (int t = 0; t < 256; t++)
            {
                int sumBelow = 0, sumAbove = 0;
                double entropy1 = 0, entropy2 = 0;

                // Suma pikseli poniżej i powyżej progu
                for (int i = 0; i <= t; i++) sumBelow += histogram[i];
                for (int i = t + 1; i < 256; i++) sumAbove += histogram[i];

                if (sumBelow == 0 || sumAbove == 0) continue;

                // Obliczenie entropii klas
                for (int i = 0; i <= t; i++)
                {
                    double p = (double)histogram[i] / sumBelow;
                    if (p > 0) entropy1 -= p * Math.Log(p, 2);
                }
                for (int i = t + 1; i < 256; i++)
                {
                    double p = (double)histogram[i] / sumAbove;
                    if (p > 0) entropy2 -= p * Math.Log(p, 2);
                }

                // Obliczenie całkowitej entropii
                double p1 = (double)sumBelow / totalPixels;
                double p2 = 1 - p1;
                double totalEntropy = p1 * entropy1 + p2 * entropy2;

                if (totalEntropy > maxEntropy)
                {
                    maxEntropy = totalEntropy;
                    bestThreshold = t;
                }
            }
            return bestThreshold;
        }
    }
}
