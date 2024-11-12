namespace Gk_01.Helpers.ImageProcessors.ImageBinarization
{
    public sealed class MeanIterativeSelectionBinarizationProcessor : ImageAutoBinarizationProcessor
    {
        protected sealed override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            threshold = 128;
            int newThreshold;
            bool thresholdChanged;

            do
            {
                thresholdChanged = false;
                int sumBlack = 0;
                int sumWhite = 0;
                int countBlack = 0;
                int countWhite = 0;

                for (var i = 0; i < pixelData.Length; i += bytesPerPixel)
                {
                    var b = pixelData[i];
                    var g = pixelData[i + 1];
                    var r = pixelData[i + 2];
                    var grayScale = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                    if (grayScale < threshold)
                    {
                        sumBlack += grayScale;
                        countBlack++;
                    }
                    else
                    {
                        sumWhite += grayScale;
                        countWhite++;
                    }
                }

                int meanBlack = countBlack > 0 ? sumBlack / countBlack : 0;
                int meanWhite = countWhite > 0 ? sumWhite / countWhite : 255;

                newThreshold = (meanBlack + meanWhite) / 2;

                if (newThreshold != threshold)
                {
                    threshold = newThreshold;
                    thresholdChanged = true;
                }
            }
            while (thresholdChanged);

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
    }
}
