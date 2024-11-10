namespace Gk_01.Helpers.ImageProcessors.ImageFilters
{
    public sealed class HorizontalSobelFilter : ImageFilter
    {
        private int[] Filter =  [1, 2, 1,
                                 0, 0, 0,
                                -1, -2, -1];

        private int FilterSize = 3;
        protected sealed override(int[] Filter, int FilterSize) GetFilter() => (Filter, FilterSize);

    }
}
