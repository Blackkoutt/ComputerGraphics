using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;

namespace Gk_01.Core.ImageProcessors
{
    public abstract class ImageProcessor
    {
        public abstract byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0);
        public void ProcessImage(Image defaultImage, Image currentImage, int value = 0)
        {
            var defaultImageBitmapSource = defaultImage.Source as BitmapSource;
            var defaultImageWritableBitmap = new WriteableBitmap(defaultImageBitmapSource);

            defaultImageWritableBitmap.Lock();

            int width = defaultImageWritableBitmap.PixelWidth;
            int height = defaultImageWritableBitmap.PixelHeight;
            int bytesPerPixel = defaultImageWritableBitmap.Format == PixelFormats.Bgr32 ? 4 : 3;
            int stride = defaultImageWritableBitmap.BackBufferStride;
            byte[] pixelData = new byte[height * stride];

            Marshal.Copy(defaultImageWritableBitmap.BackBuffer, pixelData, 0, pixelData.Length);
            defaultImageWritableBitmap.Unlock();

            var processedBitmap = ProcessImageBitmap(pixelData, width, height, bytesPerPixel, value);

            var currentImageBitmapSource = currentImage.Source as BitmapSource;
            var currentImageWritableBitmap = new WriteableBitmap(currentImageBitmapSource);

            currentImageWritableBitmap.Lock();
            currentImageWritableBitmap.WritePixels(new Int32Rect(0, 0, width, height), processedBitmap, stride, 0);
            currentImageWritableBitmap.Unlock();
            currentImage.Source = currentImageWritableBitmap;
        }
    }
}
