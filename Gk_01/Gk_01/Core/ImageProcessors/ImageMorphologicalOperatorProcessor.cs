using Gk_01.Enums;
using SkiaSharp;

namespace Gk_01.Core.ImageProcessors
{
    public abstract class ImageMorphologicalOperatorProcessor : ImageProcessor
    {
        protected int size = 3;
        protected int[,] structuringElement = new int[,]
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 },
        };

        public virtual void SetStructuringElement(StructuringElementType structuringElementType, int size)
        {
            structuringElement = new int[size, size];
            int center = size / 2;
            this.size = size;
            switch (structuringElementType)
            {
                case StructuringElementType.Square:
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            structuringElement[y, x] = 1;
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
                                structuringElement[y, x] = 1;
                            }
                        }
                    }
                    break;
            }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public int[,] StructuringElement
        {
            get { return structuringElement; }
            set { structuringElement = value; }
        }

        public override abstract byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0);
    }
}
