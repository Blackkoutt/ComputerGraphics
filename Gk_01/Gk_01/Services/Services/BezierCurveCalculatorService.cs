using Gk_01.Services.Interfaces;
using System.Windows;

namespace Gk_01.Services.Services
{
    public class BezierCurveCalculatorService : IBezierCurveCalculatorService
    {
        private double BinomialCoefficient(int n, int i)
        {
            double result = 1;
            for (int j = 1; j <= i; j++)
            {
                result *= (n - (i - j)) / (double)j;
            }
            return result;
        }

        private double CalculateBerensteinPolynomial(int n, int i, double t)
        {
            var binominal = BinomialCoefficient(n, i);
            return binominal * Math.Pow(t, i) * Math.Pow((1 - t), (n - i));
        }

        // DeCastelajau Algorithm
        private Point CalculateBezierPointByDeCastelajouAlgorithm(double t, List<Point> controlPoints)
        {
            int n = controlPoints.Count - 1;
            Point curvePoint = new Point(); 
            for(int i = 0; i <= n; i++)
            {
                var berenstein = CalculateBerensteinPolynomial(n, i, t);
                curvePoint.X += berenstein * controlPoints[i].X;
                curvePoint.Y += berenstein * controlPoints[i].Y;
            }
            return curvePoint;
        }

        public List<Point> CalculateBezierPoints(int curvePointsCount, List<Point> controlPoints)
        {
            List<Point> bezierPoints = [];
            for(int i = 0; i < curvePointsCount; i++)
            {
                double t = (double)i / (double)(curvePointsCount - 1);
                var bezierPoint = CalculateBezierPointByDeCastelajouAlgorithm(t, controlPoints);
                bezierPoints.Add(bezierPoint);
            }
            return bezierPoints;
        }


    }
}
