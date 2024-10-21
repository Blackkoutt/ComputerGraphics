using Gk_01.Controls;
using Gk_01.Helpers.GraphicFileReaders;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Gk_01.Helpers.GraphicFileLoaders
{
    public sealed class Manager_PPM_P3 : GraphicFileManager
    {
        public sealed override async Task<Image> LoadDataFromFile(string filePath)
        {
            // Image info
            int? width = null;
            int? height = null;
            double? colorScale = null;

            const byte color8BitLength = 255;
            byte[] colorArray = [];
            var colorComponentIndex = 0;

            var readingText = await File.ReadAllTextAsync(filePath);
            var lines = readingText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string lineContent = string.Empty;
            List<string> values = [];
            var imageDataStartIndex = 0;

            foreach (var line in lines)
            {
                // Pomijanie linii pustych
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Usuwanie komentarzy
                var commentIndex = line.IndexOf('#');
                if (commentIndex != -1)
                {
                    lineContent = line.Substring(0, commentIndex).Trim();
                    if (string.IsNullOrWhiteSpace(lineContent)) continue;
                }
                else lineContent = line;

                // Podział na wartości
                var lineValues = lineContent.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                values.AddRange(lineValues);
            }
            for (var i = 0; i < values.Count; i++)
            {
                var stringValue = values[i];
                if (int.TryParse(stringValue.Trim(), out var value))
                {
                    // If the image info has not been set
                    if (!width.HasValue) width = value;
                    else if (!height.HasValue) height = value;
                    else if (!colorScale.HasValue)
                    {
                        colorArray = new byte[(int)(width * height * 3)];
                        colorScale = (double)color8BitLength / (double)value;
                    }
                    // If image info has been set - read image data
                    else
                    {
                        imageDataStartIndex = i;
                        break;
                    }
                }
            }
            Parallel.For(0, colorArray.Length, j =>
            {
                colorArray[j] = (byte)(int.Parse(values[imageDataStartIndex + j]) * colorScale)!;
            });

            return ConvertDataToImage((int)width!, (int)height!, colorArray);
        }

        public sealed override void SaveDataToFile(Image image, string filePath, int? compressionLevel)
        {
            BitmapSource? bitmapSource = image.Source as BitmapSource;
            if (bitmapSource == null) return;

            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            const byte maxColor = 255;

            int bytesPerPixel = 0;
            if (bitmapSource.Format == PixelFormats.Bgr32)
            {
                bytesPerPixel = 4; 
            }
            else if (bitmapSource.Format == PixelFormats.Bgr24)
            {
                bytesPerPixel = 3; 
            }
            else
            {
                return;
            }

            WriteableBitmap writableBitmap = new WriteableBitmap(bitmapSource);
            writableBitmap.Lock();

            byte[] pixelData = new byte[width * height * bytesPerPixel];

            Marshal.Copy(writableBitmap.BackBuffer, pixelData, 0, pixelData.Length);

            writableBitmap.Unlock();

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("P3"); 
                sw.WriteLine($"{width} {height}");
                sw.WriteLine(maxColor);

                for (int i = 0; i < height; i++)
                {
                    int rowOffset = i * width * bytesPerPixel;
                    for (int j = 0; j < width; j++)
                    {
                        byte B = pixelData[rowOffset + (j * bytesPerPixel)]; 
                        byte G = pixelData[rowOffset + (j * bytesPerPixel) + 1];
                        byte R = pixelData[rowOffset + (j * bytesPerPixel) + 2];

                        // Write BGR values to the file in PPM format
                        sw.Write($"{R,3} {G,3} {B,3} ");
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}
