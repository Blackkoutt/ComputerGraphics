
using Gk_01.Enums;
using ScottPlot;
using System.Windows;
using System.Windows.Media;

namespace Gk_01.Models
{
    public abstract class CustomPath : System.Windows.Shapes.Shape
    {
        protected string shapeType = string.Empty;

        public Dictionary<Guid, Point> CharacteristicPoints
        {
            get => (Dictionary<Guid, Point>)GetValue(CharacteristicPointsProperty);
            set {
                SetValue(CharacteristicPointsProperty, value);
            } 
        }

        public static readonly DependencyProperty CharacteristicPointsProperty = DependencyProperty.Register(
          nameof(CharacteristicPoints), typeof(Dictionary<Guid, Point>), typeof(CustomPath), new FrameworkPropertyMetadata(new Dictionary<Guid, Point>(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Dictionary<Guid, Point> DefaultCharacteristicPoints { get; set; }

       
        public Point StartPoint
        {
            get => CharacteristicPoints.First().Value;
        }
      
        public Point EndPoint
        {
            get => CharacteristicPoints.Last().Value;
        }

        public string ShapeType => shapeType;

        protected double CharacteristicPointR => 8;
        private Dictionary<Guid, (ShapeElement, GeometryDrawing)> drawingDictionary = [];
        public Dictionary<Guid, (ShapeElement Type, GeometryDrawing Figure)> DrawingDictionary => drawingDictionary;

        protected abstract override Geometry DefiningGeometry { get; }

        protected DrawingGroup CreateDrawing()
        {
            var drawingGroup = new DrawingGroup();

            var drawingShape = new GeometryDrawing
            {
                Geometry = DefiningGeometry,
                Brush = Fill,
                Pen = new Pen(Stroke, StrokeThickness)
            };
            drawingGroup.Children.Add(drawingShape);

            AddOrReplaceInDictionary(Guid.NewGuid(), ShapeElement.Figure, drawingShape);

            foreach(var (pointId, point) in CharacteristicPoints)
            {
                var p0_point = CreateCharacteristicPoint(drawingGroup, point);
                AddOrReplaceInDictionary(pointId, ShapeElement.CharacteristicPoint, p0_point);
            }

            return drawingGroup;
        }
        private void AddOrReplaceInDictionary(Guid figureGuid, ShapeElement type, GeometryDrawing drawingShape)
        {
            if (drawingDictionary.ContainsKey(figureGuid))
                drawingDictionary[figureGuid] = (type, drawingShape);
            else
                drawingDictionary.Add(figureGuid, (type, drawingShape));
        }

        private GeometryDrawing CreateCharacteristicPoint(DrawingGroup drawingGroup, Point position)
        {
            var circleGeometry = new EllipseGeometry(position, CharacteristicPointR, CharacteristicPointR);
            var circleDrawing = new GeometryDrawing
            {
                Geometry = circleGeometry,
                Brush = Brushes.OrangeRed,
                Pen = new Pen(Brushes.OrangeRed, 1)
            };

            drawingGroup.Children.Add(circleDrawing);
            return circleDrawing;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var drawing = CreateDrawing();
            drawingContext.DrawDrawing(drawing);
        }

        public void SetPointX(Guid pointId, double x)
        {
            if(CharacteristicPoints.TryGetValue(pointId, out var point))
            {
                CharacteristicPoints[pointId] = new Point(x, point.Y);
                DefaultCharacteristicPoints[pointId] = new Point(x, point.Y);
                InvalidateVisual();
            }
        }

        public void SetPointY(Guid pointId, double y)
        {
            if (CharacteristicPoints.TryGetValue(pointId, out var point))
            {
                CharacteristicPoints[pointId] = new Point(point.X, y);
                DefaultCharacteristicPoints[pointId] = new Point(point.X, y);
                InvalidateVisual();
            }
        }

        /*public void RotateShape(Point rotationPoint, double angle)
        {
            var rotationPointTranslate1 = rotationPoint.X * (1 - Math.Cos(angle)) + rotationPoint.Y * Math.Sin(angle);
            var rotationPointTranslate2 = rotationPoint.Y * (1 - Math.Cos(angle)) - rotationPoint.X * Math.Sin(angle);
            double[,] rotateMatrix =
            {
                { Math.Cos(angle), -Math.Sin(angle), rotationPointTranslate1 },
                { Math.Sin(angle), Math.Cos(angle), rotationPointTranslate2 },
                { 0, 0, 1 }
            };

            TransformShape(rotateMatrix);             
            InvalidateVisual();
        }

        public void TranslateShape(Vector translateVector)
        {
            double[,] translateMatrix =
            {
                { 1.0, 0, translateVector.X },
                { 0, 1.0, translateVector.Y },
                { 0, 0, 1.0 }
            };

            TransformShape(translateMatrix);
            InvalidateVisual();
        }


        private void TransformShape(double[,] transformationMatrix)
        {
            double[] homogeneousCoordinates = new double[3];
            double[] translatedValues = new double[3];
            foreach (var pair in CharacteristicPoints)
            {
                var defaultPoint = DefaultCharacteristicPoints[pair.Key];
                homogeneousCoordinates[0] = defaultPoint.X;
                homogeneousCoordinates[1] = defaultPoint.Y;
                homogeneousCoordinates[2] = 1;

                for (int i = 0; i < 3; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        sum += homogeneousCoordinates[j] * transformationMatrix[i, j];
                    }
                    translatedValues[i] = sum;
                }

                var x = translatedValues[0]; //vector.X + defaultPoint.X;
                var y = translatedValues[1]; //vector.Y + defaultPoint.Y;

                CharacteristicPoints[pair.Key] = new Point(x, y);
            }
        }

        public void EndShapeTransformation()
        {
            DefaultCharacteristicPoints = CharacteristicPoints.ToDictionary(
                   pair => pair.Key,
                   pair => new Point(pair.Value.X, pair.Value.Y)
               );
        }*/
    }
}
