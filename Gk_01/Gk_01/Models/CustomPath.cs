
using Gk_01.Enums;
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

        public void MoveShape(Vector vector)
        {
            foreach(var pair in CharacteristicPoints)
            {
                var defaultPoint = DefaultCharacteristicPoints[pair.Key];
                var x = vector.X + defaultPoint.X;
                var y = vector.Y + defaultPoint.Y;
                CharacteristicPoints[pair.Key] = new Point(x, y);
            }
            InvalidateVisual();
        }

        public void EndMovingShape()
        {
            DefaultCharacteristicPoints = CharacteristicPoints.ToDictionary(
                   pair => pair.Key,
                   pair => new Point(pair.Value.X, pair.Value.Y)
               );
        }
    }
}
