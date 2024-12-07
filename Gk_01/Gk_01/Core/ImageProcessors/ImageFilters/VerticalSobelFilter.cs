using Gk_01.Core.ImageProcessors;

namespace Gk_01.Core.ImageProcessors.ImageFilters
{
    public sealed class VerticalSobelFilter : ImageFilter
    {
        private int[] Filter = [-1, 0, 1,
                                -2, 0, 2,
                                -1, 0, 1];

        private int FilterSize = 3;
        protected sealed override (int[] Filter, int FilterSize) GetFilter() => (Filter, FilterSize);
    }
}
