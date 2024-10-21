using Gk_01.Enums;
using Gk_01.Helpers.DTO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gk_01.Services.Interfaces
{
    public interface IFileService
    {
        void SerializeToFile(string filePath, UIElementCollection objects);
        IEnumerable<ShapeDto> DeserializeFromFile(string filePath);
        Task<Image> LoadImage(string filePath);
        void SaveImage(Image image, string filePath, FileType fileType, int? compressionLevel);
    }
}
