using Gk_01.Helpers.DTO;
using System.Windows.Controls;

namespace Gk_01.Services.Interfaces
{
    public interface IFileService
    {
        void SerializeToFile(string filePath, UIElementCollection objects);
        IEnumerable<ShapeDto> DeserializeFromFile(string filePath);
    }
}
