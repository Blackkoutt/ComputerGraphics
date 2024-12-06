using Gk_01.Enums;
using Gk_01.Helpers.ImagePointProcessing;

namespace Gk_01.Helpers.ImageProcessors
{
    public abstract class ImageMorphologicalOperatorProcessor : ImageProcessor
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

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public bool[,] StructuringElement
        {
            get { return structuringElement; }
            set { structuringElement = value; }
        }

        public override abstract byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0);
    }
}
