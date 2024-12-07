using Gk_01.Core.ImageProcessors;
using Gk_01.Enums;
using System.Windows.Media;


namespace Gk_01.Core.ImageProcessors.ImageAnalyze
{
    public class ImageAnalyzeProcessor : ImageProcessor
    {
        private ColorEnum _currentAnalyzingColor = ColorEnum.Green;

        private int minHue = 65;
        private int maxHue = 160;

        private int minBottomRedHue = 0;
        private int maxBottomRedHue = 20;
        private int minTopRedHue = 335;
        private int maxTopRedHue = 360;

        private int minSaturation = 15;
        private int maxSaturation = 100;

        private int minValue = 20;
        private int maxValue = 100;

        private double pixelColorPercentage = 0;

        public override byte[] ProcessImageBitmap(byte[] pixelData, int width, int height, int bytesPerPixel, int value = 0)
        {
            object lockObject = new object();
            Color markColor = Color.FromRgb(252, 225, 247);
            int pixelColorCount = 0;
            bool analyzingCondition = false;

            var pixelsCount = pixelData.Length / bytesPerPixel;

            Parallel.For(0, pixelsCount, i =>
            {
                int index = i * bytesPerPixel;

                byte blue = pixelData[index];
                byte green = pixelData[index + 1];
                byte red = pixelData[index + 2];

                var (hue, saturation, value) = RgbToHsv(red, green, blue);

                switch (CurrentAnalyzingColor)
                {
                    case ColorEnum.Green:
                    case ColorEnum.Blue:
                        analyzingCondition = hue >= MinHue && hue <= MaxHue && saturation > MinSaturation / 100.0 && value > MinValue / 100.0;
                        break;
                    case ColorEnum.Red:
                        analyzingCondition = (hue >= MinBottomRedHue && hue <= MaxBottomRedHue || hue >= MinTopRedHue && hue <= MaxTopRedHue) && saturation > MinSaturation / 100.0 && value > MinValue / 100.0;
                        break;
                }

                if (analyzingCondition)
                {
                    lock (lockObject)
                    {
                        pixelData[index] = markColor.R;
                        pixelData[index + 1] = markColor.G;
                        pixelData[index + 2] = markColor.B;
                        pixelColorCount++;
                    }
                }
            });
            PixelColorPercentage = (double)pixelColorCount / (pixelData.Length / bytesPerPixel) * 100;

            return pixelData;
        }

        private (double H, double S, double V) RgbToHsv(byte r, byte g, byte b)
        {
            double red = r / 255.0;
            double green = g / 255.0;
            double blue = b / 255.0;

            double max = Math.Max(red, Math.Max(green, blue));
            double min = Math.Min(red, Math.Min(green, blue));
            double delta = max - min;

            double hue = 0.0;
            if (delta > 0)
            {
                if (max == red)
                    hue = 60 * ((green - blue) / delta % 6);
                else if (max == green)
                    hue = 60 * ((blue - red) / delta + 2);
                else if (max == blue)
                    hue = 60 * ((red - green) / delta + 4);

                if (hue < 0) hue += 360;
            }

            double saturation = max == 0 ? 0 : delta / max;
            double value = max;

            return (hue, saturation, value);
        }

        public double PixelColorPercentage
        {
            get => pixelColorPercentage;
            set => pixelColorPercentage = value;
        }

        public int MinHue
        {
            get => minHue;
            set
            {
                minHue = value;
            }
        }
        public int MaxHue
        {
            get => maxHue;
            set
            {
                maxHue = value;
            }
        }
        public int MinBottomRedHue
        {
            get => minBottomRedHue;
            set
            {
                minBottomRedHue = value;
            }
        }
        public int MaxBottomRedHue
        {
            get => maxBottomRedHue;
            set
            {
                maxBottomRedHue = value;
            }
        }
        public int MinTopRedHue
        {
            get => minTopRedHue;
            set
            {
                minTopRedHue = value;
            }
        }
        public int MaxTopRedHue
        {
            get => maxTopRedHue;
            set
            {
                maxTopRedHue = value;
            }
        }
        public int MinSaturation
        {
            get => minSaturation;
            set
            {
                minSaturation = value;
            }
        }
        public int MaxSaturation
        {
            get => maxSaturation;
            set
            {
                maxSaturation = value;
            }
        }
        public int MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
            }
        }
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
            }
        }
        public ColorEnum CurrentAnalyzingColor
        {
            get => _currentAnalyzingColor;
            set
            {
                _currentAnalyzingColor = value;

            }
        }
    }
}
