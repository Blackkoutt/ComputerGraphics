using Gk_01.Enums;
using Gk_01.Exceptions;
using Gk_01.Helpers.DTO;
using Gk_01.Helpers.Serialize;
using Gk_01.Services.Interfaces;
using System.IO;
using System.Windows.Controls;

namespace Gk_01.Services.Services
{
    public class FileService : IFileService
    {
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
