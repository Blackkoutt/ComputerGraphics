using Gk_01.Enums;
using Gk_01.Helpers.ImagePointProcessing;
using System.Windows.Shapes;

namespace Gk_01.Helpers.ImageProcessors
{
    public abstract class ImageOperatorProcessor : ImageProcessor
    {
        protected int size = 3;
        protected bool[,] structuringElement = 
        {
            { true, true, true },
            { true, true, true },
            { true, true, true },
        };

        public void SetStructuringElement(StructuringElementType structuringElementType, int size)
        {
            structuringElement = new bool[size, size];
            int center = size / 2;
            this.size = size;
            switch (structuringElementType)
            {
                case StructuringElementType.Square:
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            structuringElement[y, x] = true;
                        }
                    }
                    break;

                case StructuringElementType.Circle:
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            int dx = x - center;
                            int dy = y - center;
                            if (dx * dx + dy * dy <= center * center)
                            {
                                structuringElement[y, x] = true;
                            }
                        }
                    }
                    break;
            }
        }
        protected abstract byte ProcessPixelWithMorphologicalOperator(byte[] pixelData, int width, int height, int imageX, int imageY);
        protected override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
           // int filterOffset = (size - 1) / 2; // Radius of the filter
            var copyPixelData = new byte[pixelData.Length];
            pixelData.CopyTo(copyPixelData, 0);

            byte[] resultPixels = new byte[pixelData.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte processedPixel = ProcessPixelWithMorphologicalOperator(copyPixelData, width, height, x, y);
                    

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
