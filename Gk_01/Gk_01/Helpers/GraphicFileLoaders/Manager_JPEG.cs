using Gk_01.Helpers.GraphicFileReaders;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gk_01.Helpers.GraphicFileLoaders
{
    public sealed class Manager_JPEG : GraphicFileManager
    {
        public sealed override Task<Image> LoadDataFromFile(string filePath)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(filePath, UriKind.Absolute));

            var image = new Image
            {
                Source = bitmapImage,
                Width = bitmapImage.PixelWidth,
                Height = bitmapImage.PixelHeight
            };

            return Task.FromResult(image);   
        }

        public override void SaveDataToFile(Image image, string filePath, int? compressionLevel)
        {
            BitmapSource? bitmapSource = image.Source as BitmapSource;
            if (bitmapSource == null) return;

            if (compressionLevel == null) compressionLevel = 0;
            int quality = 100 - (int)compressionLevel;

            var jpegEncoder = new JpegBitmapEncoder();
            jpegEncoder.QualityLevel = quality;

            jpegEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                jpegEncoder.Save(fileStream);
            }
        }
    }
}
