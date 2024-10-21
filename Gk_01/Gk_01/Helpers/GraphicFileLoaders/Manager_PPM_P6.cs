using Gk_01.Controls;
using Gk_01.Helpers.GraphicFileReaders;
using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gk_01.Helpers.GraphicFileLoaders
{
    public sealed class Manager_PPM_P6 : GraphicFileManager
    {
        public sealed override async Task<Image> LoadDataFromFile(string filePath)
        {
            // Special signs
            const byte commentSign = (byte)'#';
            const byte spaceSign = (byte)' ';
            const byte endLineSign = (byte)'\n';
            const byte tabSign = (byte)'\t';

            // Image info
            int? width = null;
            int? height = null;
            double? colorScale = null;

            const byte maxColor = 255;
            bool allInfoValuesSet = false;
            var stringValueBuilder = new StringBuilder();

            byte[] colorArray = [];
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            var imageDataStartIndex = 0;

            // Ommit file Header
            for (var i = 3; i< fileBytes.Length; i++)
            {
                // If the image info has not been set
                if (!allInfoValuesSet)
                {

                    if(fileBytes[i] == spaceSign || fileBytes[i] == endLineSign || fileBytes[i] == tabSign)
                    {
                        while (i < fileBytes.Length && (fileBytes[i + 1] == spaceSign || fileBytes[i + 1] == tabSign))
                        {
                            i++;
                        }
                        if (int.TryParse(stringValueBuilder.ToString().Trim(), out var value))
                        {
                            if (!width.HasValue) width = value;
                            else if (!height.HasValue) height = value;
                            else if (!colorScale.HasValue)
                            {
                                colorArray = new byte[(int)(width * height * 3)!];
                                colorScale = (double)maxColor / (double)value;
                                allInfoValuesSet = true;
                                continue;
                            }
                            stringValueBuilder.Clear();
                        }
                    }
                    else if (fileBytes[i] != commentSign)
                    {
                        stringValueBuilder.Append((char)fileBytes[i]);
                    }
                    if (fileBytes[i] == commentSign)
                    {
                        while (i < fileBytes.Length && fileBytes[i] != endLineSign)
                        {
                            i++;
                        }
                        continue;
                    }
                }
                // If image info has been set - read image binary data
                else
                {
                    while (i < fileBytes.Length && fileBytes[i - 1] != endLineSign)
                    {
                        i++;
                    }
                    imageDataStartIndex = i;
                    break;
                }
            }

            Parallel.For(0, colorArray.Length, j =>
            {
                colorArray[j] = (byte)(fileBytes[imageDataStartIndex + j] * colorScale)!; 
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

            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine("P6");
                sw.WriteLine($"{width} {height}");
                sw.WriteLine(maxColor);
                sw.Flush();
            }
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                using (var binaryWriter = new BinaryWriter(fs))
                {
                    for (int i = 0; i < height; i++)
                    {
                        int rowOffset = i * width * bytesPerPixel;
                        for (int j = 0; j < width; j++)
                        {
                            byte B = pixelData[rowOffset + (j * bytesPerPixel)];
                            byte G = pixelData[rowOffset + (j * bytesPerPixel) + 1];
                            byte R = pixelData[rowOffset + (j * bytesPerPixel) + 2];

                            binaryWriter.Write(R);
                            binaryWriter.Write(G);
                            binaryWriter.Write(B);
                        }
                    }
                }
            }
        }
    }
}
