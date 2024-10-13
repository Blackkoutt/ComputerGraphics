using Gk_01.Helpers.DTO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Windows.Controls;

namespace Gk_01.Helpers.Serialize
{
    public sealed class SerializerJSON : AbstractSerializer
    {
        public sealed override IEnumerable<ShapeDto> Deserialize(string stringToDeserialize)
        {
            try
            {
                var shapeDtosObjects = JsonSerializer.Deserialize<IEnumerable<ShapeDto>>(stringToDeserialize);
                return shapeDtosObjects ?? new List<ShapeDto>();
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Wystąpił błąd podczas deserializacji pliku .json. Sprawdź czy plik zwiera poprawny format. Błąd: {ex.Message}");
            }
            
        }

        public sealed override string Serialize(UIElementCollection shapeObjects)
        {
            var shapeListDto = GetShapeDtos(shapeObjects);
            try
            {
                return JsonSerializer.Serialize(shapeListDto);
            }
            catch(Exception ex)
            {
                throw new SerializationException($"Wystąpił błąd podczas serializacji do pliku .json. Błąd: {ex.Message}");
            }   
        }
    }
}
