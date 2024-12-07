using Gk_01.Core.ImageProcessors;
using Gk_01.Enums;

namespace Gk_01.Core.ImageProcessors.MorphologicalOperators
{
    public sealed class HitOrMissOperatorProcessor : ImageMorphologicalOperatorProcessor
    {
        private HitOrMissType hitOrMissType = HitOrMissType.Hit;
        public HitOrMissType HitOrMissType
        {
            get { return hitOrMissType; }
            set
            {
                hitOrMissType = value;
            }
        }

        public override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            byte[] binaryPixelData = new byte[pixelData.Length / 4];
            for (int i = 0; i < binaryPixelData.Length; i++)
            {
                binaryPixelData[i] = pixelData[i * 4] > 127 ? (byte)255 : (byte)0;
            }

            byte[] resultPixels = new byte[pixelData.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int halfSize = (structuringElement.GetLength(1) - 1) / 2;

                    bool hit = false;
                    int hitCount = 0;
                    for (int ky = -halfSize; ky <= halfSize; ky++)
                    {
                        for (int kx = -halfSize; kx <= halfSize; kx++)
                        {
                            int nx = x + kx;
                            int ny = y + ky;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                int imageIndex = ny * width + nx;
                                int pixelIdx = imageIndex;

                                if (binaryPixelData[pixelIdx] == 255 && structuringElement[ky + halfSize, kx + halfSize] == 1)
                                {
                                    // Thickening - stuructiring element must hit object
                                    if (HitOrMissType == HitOrMissType.Hit)
                                    {
                                        hit = true;
                                        break;
                                    }
                                    // Thinning - stuructiring element must fit in object
                                    else if (HitOrMissType == HitOrMissType.Fit)
                                    {
                                        hitCount++;
                                    }
                                }
                            }
                        }
                        if (hit) break;
                    }

                    int pixelIndex = (y * width + x) * 4;

                    // Thickening - stuructiring element must hit object
                    if (HitOrMissType == HitOrMissType.Hit)
                    {
                        if (!hit)
                        {
                            resultPixels[pixelIndex] = 0; // Black
                            resultPixels[pixelIndex + 1] = 0;
                            resultPixels[pixelIndex + 2] = 0;
                            resultPixels[pixelIndex + 3] = pixelData[pixelIndex + 3];
                        }
                        else
                        {
                            resultPixels[pixelIndex] = 255; // White
                            resultPixels[pixelIndex + 1] = 255;
                            resultPixels[pixelIndex + 2] = 255;
                            resultPixels[pixelIndex + 3] = pixelData[pixelIndex + 3];
                        }
                    }

                    // Thinning - stuructiring element must fit in object
                    else if (HitOrMissType == HitOrMissType.Fit)
                    {
                        bool fit = hitCount == structuringElement.Cast<int>().Select(el => el == 1).Count();
                        if (!fit)
                        {
                            resultPixels[pixelIndex] = 0; // Black
                            resultPixels[pixelIndex + 1] = 0;
                            resultPixels[pixelIndex + 2] = 0;
                            resultPixels[pixelIndex + 3] = pixelData[pixelIndex + 3];
                        }
                        else
                        {
                            resultPixels[pixelIndex] = 255; // White
                            resultPixels[pixelIndex + 1] = 255;
                            resultPixels[pixelIndex + 2] = 255;
                            resultPixels[pixelIndex + 3] = pixelData[pixelIndex + 3];
                        }
                    }
                }
            }
            return resultPixels;
        }
    }
}
