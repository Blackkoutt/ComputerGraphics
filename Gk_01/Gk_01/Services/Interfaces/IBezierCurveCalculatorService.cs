using System.Windows;

namespace Gk_01.Services.Interfaces
{
    public interface IBezierCurveCalculatorService
    {
        List<Point> CalculateBezierPoints(int curvePointsCount, List<Point> controlPoints);
    }
}
