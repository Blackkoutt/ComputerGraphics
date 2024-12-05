using Gk_01.Services.Interfaces;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gk_01.Models
{
    public class Curve : CustomPath
    {
        private IBezierCurveCalculatorService? bezierCurveCalculatorService;
        public IBezierCurveCalculatorService BezierCurveCalculatorService 
        {
            set { bezierCurveCalculatorService = value; }
        }

        private int curvePointsCount = 20;
        public int CurvePointsCount
        {
            set 
            { 
                curvePointsCount = value;
                InvalidateVisual();
            }
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                shapeType = ShapeTypeEnum.Curve.ToString();

                if (CharacteristicPoints.Count < 2)
                    return Geometry.Empty;

                var bezierCurve = new Polyline();
                
                if(bezierCurveCalculatorService != null)
                {
                    var bezierPoints = bezierCurveCalculatorService.CalculateBezierPoints(curvePointsCount, CharacteristicPoints.Values.ToList());
                    bezierCurve.Points = new PointCollection(bezierPoints);
                }

                var streamGeometry = new StreamGeometry();
                using (var context = streamGeometry.Open())
                {
                    context.BeginFigure(bezierCurve.Points[0], false, false);
                    context.PolyLineTo(bezierCurve.Points, true, true);
                }

                return streamGeometry;
            }
        }
    }
}
