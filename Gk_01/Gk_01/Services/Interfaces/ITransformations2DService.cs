using Gk_01.Models;
using System.Windows;

namespace Gk_01.Services.Interfaces
{
    public interface ITransformations2DService
    {
        void ScaleShape(CustomPath shape, Point scalePoint, double scaleXValue, double scaleYValue);
        void RotateShape(CustomPath shape, Point rotationPoint, double angle);
        void TranslateShape(CustomPath shape, Vector translateVector);
        void EndShapeTransformation(CustomPath shape);
    }
}
