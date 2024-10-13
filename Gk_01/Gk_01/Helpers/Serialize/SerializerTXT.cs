using Gk_01.Exceptions;
using Gk_01.Helpers.DTO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Gk_01.Helpers.Serialize
{
    public sealed class SerializerTXT : AbstractSerializer
    {
        public sealed override string Serialize(UIElementCollection shapeObjects)
        {
            var shapeListDto = GetShapeDtos(shapeObjects);
            StringBuilder stringBuilder = new StringBuilder();  
            foreach(var shapeDto in shapeListDto)
            {
                stringBuilder.AppendLine($"{nameof(shapeDto.ShapeType)}: {shapeDto.ShapeType}");
                stringBuilder.AppendLine($"{nameof(shapeDto.StartPoint)}: X:{shapeDto.StartPoint.X} Y:{shapeDto.StartPoint.Y}");
                stringBuilder.AppendLine($"{nameof(shapeDto.EndPoint)}: X:{shapeDto.EndPoint.X} Y:{shapeDto.EndPoint.Y}");
                stringBuilder.AppendLine($"{nameof(shapeDto.Stroke)}: {shapeDto.Stroke}");
                stringBuilder.AppendLine($"{nameof(shapeDto.Fill)}: {shapeDto.Fill}");
                stringBuilder.AppendLine($"{nameof(shapeDto.StrokeTickness)}: {shapeDto.StrokeTickness}");
                if(shapeListDto.Last() != shapeDto)stringBuilder.AppendLine($"");
            }
            return stringBuilder.ToString();
        }

        public sealed override IEnumerable<ShapeDto> Deserialize(string stringToDeserialize)
        {
            List<ShapeDto> shapeOutputList = [];
            var shapeList = stringToDeserialize.Trim().Split("\r\n\r\n");
            foreach (var shape in shapeList)
            {
                var shapeProperties = shape.Trim().Split("\n");
                var shapeDto = new ShapeDto();
                foreach (var property in shapeProperties)
                {
                    var propertyTab = property.Trim().Split(": ");
                    var propertyName = propertyTab[0].Trim();
                    var propertyValue = propertyTab[1].Trim();
                    if (string.IsNullOrEmpty(propertyValue))
                        throw new SerializationException("Wystąpił błąd podczas deserializacji pliku txt. Sprawdź czy plik zawiera poprawny format.");

                    var shapeProperty = shapeDto.GetType().GetProperty(propertyName);
                    if (shapeProperty != null)
                    {
                        try
                        {
                            var convertedValue = ConvertPropertyValue(shapeProperty, shapeDto, propertyValue);
                            shapeProperty.SetValue(shapeDto, convertedValue);
                        }
                        catch(ConversionException)
                        {
                            throw;
                        }       
                    }
                    else throw new SerializationException("Wystąpił błąd podczas deserializacji pliku txt. Sprawdź czy plik zawiera poprawny format.");
                }
                shapeOutputList.Add(shapeDto);
            }
            return shapeOutputList;
        }

        private object ConvertPropertyValue(PropertyInfo property, ShapeDto shapeDto, string propertyValue)
        {
            if (property.PropertyType == typeof(string))
            {
                return propertyValue;
            }
            else if (property.PropertyType == typeof(Point))
            {
                var xyTab = propertyValue.Split(' ');
                if (double.TryParse(xyTab[0].Replace("X", "").Replace(":", ""), out var resultX) && double.TryParse(xyTab[1].Replace("Y", "").Replace(":", ""), out var resultY))
                    return new Point(x: resultX, y: resultY);
            }
            else if (property.PropertyType == typeof(int))
            {
                if (int.TryParse(propertyValue, out var intResult))
                    return intResult;
            }
            throw new ConversionException("Błąd konwersji podczas parsowania pliku txt.");
        }
    }
}
