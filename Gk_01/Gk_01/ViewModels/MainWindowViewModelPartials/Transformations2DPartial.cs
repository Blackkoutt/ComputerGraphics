using Gk_01.Enums;
using Gk_01.Models;
using System.Windows;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Rotate shape
        private int rotationPoint_X;
        private int rotationPoint_Y;
        private double rotationAngle;

        private bool rotatePointSet = false;
        private bool scalingPointSet = false;
        private CustomPath? _rotationPoint = null;
        private CustomPath? _scalingPoint = null;




        private void AddRotationOrScalingPoint(Point clickPosition, TransformationPoint transformationPoint)
        {
            if (transformationPoint == TransformationPoint.Rotation)
            {
                _rotationPoint = _drawingService.DrawRotationOrScallingPoint(clickPosition);
                RotationPoint_X = (int)clickPosition.X;
                RotationPoint_Y = (int)clickPosition.Y;
                rotatePointSet = true;
            }
            else if (transformationPoint == TransformationPoint.Scaling)
            {
                _scalingPoint = _drawingService.DrawRotationOrScallingPoint(clickPosition);
                ScalingPoint_X = (int)clickPosition.X;
                ScalingPoint_Y = (int)clickPosition.Y;
                scalingPointSet = true;
            }

        }


        private void StartRotation(Point clickPosition)
        {
            if (_currentShape != null)
            {
                if (!rotatePointSet) AddRotationOrScalingPoint(clickPosition, TransformationPoint.Rotation);
                else
                {
                    _isRotateing = true;
                    _defaultRotationPosition = clickPosition;
                }
            }
        }
        private void RotateShape(Point currentMousePosition)
        {
            _canvas!.CaptureMouse();
            var angleInRadians = Math.Atan2(currentMousePosition.Y - _defaultRotationPosition.Y, currentMousePosition.X - _defaultRotationPosition.X);
            double angleDegrees = angleInRadians * (180 / Math.PI);
            RotationAngle = angleDegrees;
        }
        private void PerformRotation(double angleInRadians)
        {
            if (_currentShape != null)
            {
                var rotationPoint = new Point(RotationPoint_X, RotationPoint_Y);
                _transformations2DService.RotateShape(_currentShape!, rotationPoint, angleInRadians);
                // _currentShape!.RotateShape(rotationPoint, RotationAngle);
                _isSaved = false;
            }
        }
        private void EndRotation()
        {
            _isRotateing = false;
            rotatePointSet = false;
            _canvas!.Children.Remove(_rotationPoint);
            _rotationPoint = null;
            _transformations2DService.EndShapeTransformation(_currentShape!);
            //_currentShape!.EndShapeTransformation();
            _canvas!.ReleaseMouseCapture();
        }
        public double RotationAngle
        {
            get { return rotationAngle; }
            set
            {
                rotationAngle = value;
                double rotationAngleInRadians = rotationAngle * (Math.PI / 180);
                PerformRotation(rotationAngleInRadians);
                OnPropertyChanged();
            }
        }
        public int RotationPoint_X
        {
            get { return rotationPoint_X; }
            set
            {
                rotationPoint_X = value;
                OnPropertyChanged();
            }
        }
        public int RotationPoint_Y
        {
            get { return rotationPoint_Y; }
            set
            {
                rotationPoint_Y = value;
                OnPropertyChanged();
            }
        }





        // Scale shape
        private int scalingPoint_X;
        private int scalingPoint_Y;
        private double scale_X;
        private double scale_Y;
        private void StartScalling(Point clickPosition)
        {
            if (_currentShape != null)
            {
                if (!scalingPointSet) AddRotationOrScalingPoint(clickPosition, TransformationPoint.Scaling);
                else
                {
                    _isScaling = true;
                    _defaultScalingPosition = clickPosition;
                }
            }
        }
        private void ScaleShape(Point currentMousePosition)
        {
            _canvas!.CaptureMouse();
            Scale_X = Math.Abs(ScalingPoint_X - currentMousePosition.X) / Math.Abs(ScalingPoint_X - _defaultScalingPosition.X);
            Scale_Y = Math.Abs(ScalingPoint_Y - currentMousePosition.Y) / Math.Abs(ScalingPoint_Y - _defaultScalingPosition.Y);
        }
        private void PerformScaling()
        {
            if (_currentShape != null)
            {
                var scalingPoint = new Point(ScalingPoint_X, ScalingPoint_Y);
                _transformations2DService.ScaleShape(_currentShape, scalingPoint, Scale_X, scale_Y);
                _isSaved = false;
            }
        }
        private void EndScaling()
        {
            _isScaling = false;
            scalingPointSet = false;
            _canvas!.Children.Remove(_scalingPoint);
            _scalingPoint = null;
            _transformations2DService.EndShapeTransformation(_currentShape!);
            //_currentShape!.EndShapeTransformation();
            _canvas!.ReleaseMouseCapture();

        }
        public int ScalingPoint_X
        {
            get { return scalingPoint_X; }
            set
            {
                scalingPoint_X = value;
                OnPropertyChanged();
            }
        }
        public int ScalingPoint_Y
        {
            get { return scalingPoint_Y; }
            set
            {
                scalingPoint_Y = value;
                OnPropertyChanged();
            }
        }
        public double Scale_X
        {
            get { return scale_X; }
            set
            {
                scale_X = value;
                PerformScaling();
                OnPropertyChanged();
            }
        }
        public double Scale_Y
        {
            get { return scale_Y; }
            set
            {
                scale_Y = value;
                PerformScaling();
                OnPropertyChanged();
            }
        }





        // Translate shape
        private int translation_X;
        private int translation_Y;
        private void StartTranslation(Point clickPosition)
        {
            if (_currentShape != null)
            {
                _isTranslateing = true;
                _mouseClickOnShapePosition = clickPosition;
                //CanvasCursor = Cursors.SizeAll;
            }
        }
        private void TranslateShape(Point currentMousePosition)
        {
            _currentShape!.CaptureMouse();
            Translation_X = (int)(currentMousePosition.X - _mouseClickOnShapePosition.X);
            Translation_Y = (int)(currentMousePosition.Y - _mouseClickOnShapePosition.Y);
        }
        private void PerformTranslation()
        {
            if (_currentShape != null)
            {
                var translateVector = new Vector(Translation_X, Translation_Y);
                _transformations2DService.TranslateShape(_currentShape, translateVector);
                // _currentShape!.TranslateShape(translateVector);
                _isSaved = false;
            }
        }
        private void EndTranslation()
        {
            _isTranslateing = false;
            _transformations2DService.EndShapeTransformation(_currentShape!);
            //_currentShape!.EndShapeTransformation();
            _currentShape!.ReleaseMouseCapture();
        }
        public int Translation_X
        {
            get { return translation_X; }
            set
            {
                translation_X = value;
                PerformTranslation();
                OnPropertyChanged();
            }
        }
        public int Translation_Y
        {
            get { return translation_Y; }
            set
            {
                translation_Y = value;
                PerformTranslation();
                OnPropertyChanged();
            }
        }
    }
}
