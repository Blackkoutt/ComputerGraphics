using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Gk_01.Helpers.GraphicFileReaders
{
    public abstract class GraphicFileManager
    {
        public abstract Task<Image> LoadDataFromFile(string filePath);
        public abstract void SaveDataToFile(Image image, string filePath, int? compressionLevel);

        protected Image ConvertDataToImage(int width, int height, byte[] data)
        {
            BitmapSource bitmapSource = BitmapSource.Create(
                width, height,
                80, 80,           // DPI
                PixelFormats.Rgb24,  
                null,             // Color pallete
                data,             // Image data
                width * 3         // Bytes per line count
            );

            var image = new Image { Source = bitmapSource };

            return image;
        }
    }
}
