using Gk_01.Extenstions;
using Gk_01.Helpers.DTO;
using Gk_01.Models;
using System.Windows.Controls;

namespace Gk_01.Helpers.Serialize
{
    public abstract class AbstractSerializer
    {     
        public abstract string Serialize(UIElementCollection objects);
        public abstract IEnumerable<ShapeDto> Deserialize(string stringToDeserialize);

        protected IEnumerable<ShapeDto> GetShapeDtos(UIElementCollection geometryObjects)
        {
            ICollection<ShapeDto> shapeDtoList = [];
            foreach (var geometryObject in geometryObjects)
            {
                if (geometryObject is CustomPath geometryPath)
                {
                    var geometryDto = geometryPath.AsDto();
                    shapeDtoList.Add(geometryDto);
                }
            }
            return shapeDtoList;
        }
    }
}
