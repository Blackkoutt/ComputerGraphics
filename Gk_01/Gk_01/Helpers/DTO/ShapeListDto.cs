using System.Xml.Serialization;

namespace Gk_01.Helpers.DTO
{
    [XmlRoot("Shapes")]
    public class ShapeListDto
    {
        [XmlElement("Shape")]
        public ICollection<ShapeDto> Shapes { get; set; } = new List<ShapeDto>();
    }
}
