using Gk_01.Helpers.DTO;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Gk_01.Helpers.Serialize
{
    public sealed class SerializerXML : AbstractSerializer
    {
        public sealed override IEnumerable<ShapeDto> Deserialize(string stringToDeserialize)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ShapeDto>));
                using (var reader = new StringReader(stringToDeserialize))
                {
                    var shapes = serializer.Deserialize(reader);
                    if (shapes != null) return (List<ShapeDto>)shapes;
                    else return new List<ShapeDto>();
                }
            }
            catch(Exception ex)
            {
                throw new SerializationException($"Wystąpił błąd podczas deserializacji pliku .xml. Sprawdź czy plik zwiera poprawny format. Błąd: {ex.Message}");
            }
        }

        public sealed override string Serialize(UIElementCollection shapeObjects)
        {
            var shapeListDto = GetShapeDtos(shapeObjects);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ShapeDto>));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, shapeListDto);
                string xmlString = writer.ToString();
                writer.Close();
                return xmlString;
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Wystąpił błąd podczas serializacji do pliku .xml. Błąd: {ex.Message}");
            }  
        }
    }
}
