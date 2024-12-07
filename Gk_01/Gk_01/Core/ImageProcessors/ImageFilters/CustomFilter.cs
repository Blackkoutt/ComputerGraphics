using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.ImageFilters
{
    public sealed class CustomFilter(int[] Filter, int FilterSize) : ImageFilter
    {
        protected sealed override (int[] Filter, int FilterSize) GetFilter() => (Filter, FilterSize);
    }
}
