using Gk_01.Core.GraphicFileLoaders;
using Gk_01.Core.Serialize;
using Gk_01.Enums;
using Gk_01.Exceptions;
using Gk_01.Helpers.DTO;
using Gk_01.Core.GraphicFileLoaders;
using Gk_01.Core.Serialize;
using Gk_01.Services.Interfaces;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gk_01.Services.Services
{
    public class FileService : IFileService
    {
        public async Task<Image> LoadImage(string filePath)
        {
            GraphicFileManager manager = GetGraphicFileManager(filePath);
            return await manager.LoadDataFromFile(filePath);
        }
        public void SaveImage(Image image, string filePath, FileType fileType, int? compressionLevel)
        {
            GraphicFileManager manager = GetGraphicFileManager(filePath, fileType);
            manager.SaveDataToFile(image, filePath, compressionLevel);
        }

        private GraphicFileManager GetGraphicFileManager(string filePath, FileType? fileType = null)
        {
            if(fileType != null)
            {
                return fileType switch 
                {
                    FileType.JPEG => new Manager_JPEG(),
                    FileType.PPM_P3 => new Manager_PPM_P3(),
                    FileType.PPM_P6 => new Manager_PPM_P6(),
                    _ => throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.PPM}, {FileType.JPEG}")
                };
            }

            var extensionString = Path.GetExtension(filePath).Replace(".", "").ToUpper();

            if (Enum.TryParse(typeof(FileType), extensionString, true, out var extension))
            {
                return extension switch
                {
                    FileType.PPM => GetPPMLoader(filePath),
                    FileType.JPEG => new Manager_JPEG(),
                    FileType.JPG => new Manager_JPEG(),
                    _ => throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.PPM}, {FileType.JPEG}")
                };
            }
            else
                throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.PPM}, {FileType.JPEG}");
        }

        private GraphicFileManager GetPPMLoader(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] header = new byte[2];
                fs.Read(header, 0, 2);
                var fileHeaderString = System.Text.Encoding.ASCII.GetString(header);
                if (Enum.TryParse(typeof(PPMType), fileHeaderString, false, out var fileHeader))
                {
                    return fileHeader switch
                    {
                        PPMType.P3 => new Manager_PPM_P3(),
                        PPMType.P6 => new Manager_PPM_P6(),
                        _ => throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.PPM}, {FileType.JPEG}")
                    };
                }
                else throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.PPM}, {FileType.JPEG}");
            }
        }

        public void SerializeToFile(string filePath, UIElementCollection objects)
        {
            try
            {
                AbstractSerializer serializer = GetSerializerByFilePathExtension(filePath);
                var serializedString = serializer.Serialize(objects);
                File.WriteAllText(filePath, serializedString);
            }
            catch (Exception) { throw; }
        }

        public IEnumerable<ShapeDto> DeserializeFromFile(string filePath)
        {
            try
            {
                AbstractSerializer serializer = GetSerializerByFilePathExtension(filePath);
                var readingString = File.ReadAllText(filePath);
                return serializer.Deserialize(readingString);
            }
            catch (Exception) { throw; }
        }

        private AbstractSerializer GetSerializerByFilePathExtension(string filePath)
        {
            var extensionString = Path.GetExtension(filePath).Replace(".", "").ToUpper();

            if (Enum.TryParse(typeof(FileType), extensionString, true, out var extension))
            {
                return extension switch
                {
                    FileType.JSON => new SerializerJSON(),
                    FileType.TXT => new SerializerTXT(),
                    FileType.XML => new SerializerXML(),
                    _ => throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.JSON}, {FileType.TXT}, {FileType.XML}")
                };
            }
            else
                throw new BadFileException($"Nieobsługiwany typ pliku. Obsługiwane formaty plików to: {FileType.JSON}, {FileType.TXT}, {FileType.XML}");
        }
    }
}
