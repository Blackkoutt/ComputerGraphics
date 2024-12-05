using Gk_01.Models;
using Gk_01.Services.Interfaces;
using System.Windows;

namespace Gk_01.Services.Services
{
    public class Transformations2DService : ITransformations2DService
    {
        public void ScaleShape(CustomPath shape, Point scalePoint, double scaleXValue, double scaleYValue)
        {
            double[,] scaleMatrix =
            {
                { scaleXValue, 0, scalePoint.X * (1 - scaleXValue)},
                { 0, scaleYValue, scalePoint.Y * (1 - scaleYValue)},
                { 0, 0, 1},
            };

            TransformShape(shape, scaleMatrix);
            shape.InvalidateVisual();
        }

        public void RotateShape(CustomPath shape, Point rotationPoint, double angle)
        {
            var rotationPointTranslate1 = rotationPoint.X * (1 - Math.Cos(angle)) + rotationPoint.Y * Math.Sin(angle);
            var rotationPointTranslate2 = rotationPoint.Y * (1 - Math.Cos(angle)) - rotationPoint.X * Math.Sin(angle);
            double[,] rotateMatrix =
            {
                { Math.Cos(angle), -Math.Sin(angle), rotationPointTranslate1 },
                { Math.Sin(angle), Math.Cos(angle), rotationPointTranslate2 },
                { 0, 0, 1 }
            };

            TransformShape(shape, rotateMatrix);
            shape.InvalidateVisual();
        }

        public void TranslateShape(CustomPath shape, Vector translateVector)
        {
            double[,] translateMatrix =
            {
                { 1.0, 0, translateVector.X },
                { 0, 1.0, translateVector.Y },
                { 0, 0, 1.0 }
            };

            TransformShape(shape, translateMatrix);
            shape.InvalidateVisual();
        }
        public void EndShapeTransformation(CustomPath shape)
        {
            shape.DefaultCharacteristicPoints = shape.CharacteristicPoints.ToDictionary(
                   pair => pair.Key,
                   pair => new Point(pair.Value.X, pair.Value.Y)
               );
        }


        private void TransformShape(CustomPath shape, double[,] transformationMatrix)
        {
            double[] homogeneousCoordinates = new double[3];
            double[] translatedValues = new double[3];
            foreach (var pair in shape.CharacteristicPoints)
            {
                var defaultPoint = shape.DefaultCharacteristicPoints[pair.Key];
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

                shape.CharacteristicPoints[pair.Key] = new Point(x, y);
            }
        }
    }
}
