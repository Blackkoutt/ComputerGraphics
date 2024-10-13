
using Gk_01.Enums;
using System.Windows;
using System.Windows.Media;

namespace Gk_01.Models
{
    public abstract class CustomPath : System.Windows.Shapes.Shape
    {
        protected string shapeType = string.Empty;
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }
      
        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }
        public string ShapeType => shapeType;

        protected double CharacteristicPointR => 8;
        private Dictionary<ShapeElement, GeometryDrawing> drawingDictionary = [];
        public Dictionary<ShapeElement, GeometryDrawing> DrawingDictionary => drawingDictionary;

        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
            nameof(StartPoint), typeof(Point), typeof(CustomPath), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));
        
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
          nameof(EndPoint), typeof(Point), typeof(CustomPath), new FrameworkPropertyMetadata(new Point(100, 100), FrameworkPropertyMetadataOptions.AffectsRender));

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

            AddOrReplaceInDictionary(ShapeElement.Figure, drawingShape);

            var p0_point = AddCharacteristicPoint(drawingGroup, StartPoint);
            AddOrReplaceInDictionary(ShapeElement.P0_Point, p0_point);

            var p1_point = AddCharacteristicPoint(drawingGroup, EndPoint);
            AddOrReplaceInDictionary(ShapeElement.P1_Point, p1_point);

            return drawingGroup;
        }
        private void AddOrReplaceInDictionary(ShapeElement key, GeometryDrawing drawingShape)
        {
            if (drawingDictionary.ContainsKey(key))
                drawingDictionary[key] = drawingShape;
            else
                drawingDictionary.Add(key, drawingShape);
        }

        private GeometryDrawing AddCharacteristicPoint(DrawingGroup drawingGroup, Point position)
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

        public void SetStartPointX(double x)
        {
            var startPointY = StartPoint.Y;
            StartPoint = new Point(x, startPointY);
            InvalidateVisual();
        }

        public void SetStartPointY(double y)
        {
            var startPointX = StartPoint.X;
            StartPoint = new Point(startPointX, y);
            InvalidateVisual();
        }

        public void SetEndPointX(double x)
        {
            var endPointY = EndPoint.Y;
            EndPoint = new Point(x, endPointY);
            InvalidateVisual();
        }
        public void SetEndPointY(double y)
        {
            var endPointX = EndPoint.X;
            EndPoint = new Point(endPointX, y);
            InvalidateVisual();
        }
    }
}
