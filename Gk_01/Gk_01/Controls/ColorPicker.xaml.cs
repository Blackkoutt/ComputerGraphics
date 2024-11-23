using Gk_01.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gk_01.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl, INotifyPropertyChanged
    {
        //RGBA
        private int rValue;
        private int gValue;
        private int bValue;
        private int aValue;

        //CMYK
        private int cValue;
        private int mValue;
        private int yValue;
        private int kValue;

        private Point _mouseClickPosition;
        private bool _isSaturationMoving = false;
        private bool _isColorSpectrumMoving = false;

        private LinearGradientBrush colorSpectrumGradient;
        private bool isUpdateing = false;

        private string hexColorValue;
        public ColorPicker()
        {
            InitializeComponent();
            InitializeColorSpectrum();
            DataContext = this;

            //CMYK
            CValue = 0;
            MValue = 0;
            YValue = 0;
            KValue = 100;
        }

        private void ConvertRGBToCMYK()
        {
            if (isUpdateing) return;
            isUpdateing = true;

            KValue = (int)((1 - Math.Max(Math.Max((double)RValue / 255, (double)GValue / 255), (double)BValue / 255)) * 100);
            if (KValue != 100)
            {
                CValue = (int)((1 - (double)RValue / 255 - ((double)KValue / 100)) / (1 - ((double)KValue / 100)) * 100);
                MValue = (int)((1 - (double)GValue / 255 - ((double)KValue / 100)) / (1 - ((double)KValue / 100)) * 100);
                YValue = (int)((1 - (double)BValue / 255 - ((double)KValue / 100)) / (1 - ((double)KValue / 100)) * 100);
            }
            else
            {
                CValue = MValue = YValue = 0;
            }


            isUpdateing = false;
        }
        private void ConvertCMYKToRGB()
        {
            if (isUpdateing) return;
            isUpdateing = true;
            RValue = (int)(255 * (1 - ((double)CValue / 100)) * (1 - ((double)KValue / 100)));
            GValue = (int)(255 * (1 - ((double)MValue / 100)) * (1 - ((double)KValue / 100)));
            BValue = (int)(255 * (1 - ((double)YValue / 100)) * (1 - ((double)KValue / 100)));
            isUpdateing = false;
        }

        private void InitializeColorSpectrum()
        {
            colorSpectrumGradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            colorSpectrumGradient.GradientStops.Clear(); // Czyścimy istniejące przystanki

            double offsetStep = 1.0 / (6.0 * 256.0);
            double offset = 0.0;

            // G+
            for (int i = 0; i <= 255; i++)
            {
                colorSpectrumGradient.GradientStops.Add(new GradientStop(Color.FromRgb(255, (byte)i, 0), offset));
                offset += offsetStep;
            }

            // R-
            for (int i = 0; i <= 255; i++)
            {
                colorSpectrumGradient.GradientStops.Add(new GradientStop(Color.FromRgb((byte)(255 - i), 255, 0), offset));
                offset += offsetStep;
            }

            // B+
            for (int i = 0; i <= 255; i++)
            {
                colorSpectrumGradient.GradientStops.Add(new GradientStop(Color.FromRgb(0, 255, (byte)i), offset));
                offset += offsetStep;
            }

            // G-
            for (int i = 0; i <= 255; i++)
            {
                colorSpectrumGradient.GradientStops.Add(new GradientStop(Color.FromRgb(0, (byte)(255 - i), 255), offset));
                offset += offsetStep;
            }

            // R+
            for (int i = 0; i <= 255; i++)
            {
                colorSpectrumGradient.GradientStops.Add(new GradientStop(Color.FromRgb((byte)i, 0, 255), offset));
                offset += offsetStep;
            }

            // B-
            for (int i = 0; i <= 255; i++)
            {
                colorSpectrumGradient.GradientStops.Add(new GradientStop(Color.FromRgb(255, 0, (byte)(255 - i)), offset));
                offset += offsetStep;
            }

            ColorSpectrum.Fill = colorSpectrumGradient;
        }


        private void SaturationGradient_MouseDown(object sender, MouseEventArgs e)
        {
            Point clickPosition = e.GetPosition(SaturationGradient);
            _isSaturationMoving = true;
            _mouseClickPosition = clickPosition;
            SaturationGradient.CaptureMouse();
        }

        private void SaturationGradient_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSaturationMoving)
            {
                Point currentMousePosition = e.GetPosition(SaturationGradient);
                if(currentMousePosition.X > SaturationGradient.ActualWidth ||
                   currentMousePosition.Y > SaturationGradient.ActualHeight ||
                   currentMousePosition.X < 0 ||
                   currentMousePosition.Y < 0)
                {
                    return;
                } 
                    
                IndicatorPosition.X = currentMousePosition.X - (ColorPickerIndicator.Width / 2);
                IndicatorPosition.Y = currentMousePosition.Y - (ColorPickerIndicator.Height / 2);

                CalculateSaturationAndBrightness(currentMousePosition);
            }
        }

        private void CalculateSaturationAndBrightness(Point point)
        {
            double saturationValue = point.X / SaturationGradient.ActualWidth;
            saturationValue = Math.Clamp(saturationValue, 0.0, 1.0);

            double brightnessValue = 1 - (point.Y / SaturationGradient.ActualHeight);
            brightnessValue = Math.Clamp(brightnessValue, 0.0, 1.0);

            // set saturation and brightnessValue
            Color baseColor = SecondGradientColor.Color;
            RValue = (int)((baseColor.R * saturationValue + 255 * (1 - saturationValue)) * brightnessValue);
            GValue = (int)((baseColor.G * saturationValue + 255 * (1 - saturationValue)) * brightnessValue);
            BValue = (int)((baseColor.B * saturationValue + 255 * (1 - saturationValue)) * brightnessValue);

            Color selectedColor = Color.FromArgb(255, (byte)RValue, (byte)GValue, (byte)BValue);

            var newColor = new SolidColorBrush(selectedColor);
            SelectedColor = newColor;
            HexColorValue = SelectedColor.Color.ToString();
        }

        private void SaturationGradient_MouseUp(object sender, MouseEventArgs e)
        {
            _isSaturationMoving = false;
            SaturationGradient.ReleaseMouseCapture();
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sliderValue = e.NewValue;
            var colorsInGradient = colorSpectrumGradient.GradientStops.Count;
            int colorIndex = (int)((colorsInGradient - 1) * sliderValue);
            if(colorIndex < colorsInGradient && colorIndex > 0)
            {
                var newColor = colorSpectrumGradient.GradientStops[colorIndex].Color;
                SecondGradientColor.Color = newColor;
                _isColorSpectrumMoving = true;
                CalculateSaturationAndBrightness(new Point(IndicatorPosition.X, IndicatorPosition.Y));
                _isColorSpectrumMoving = false;
            }
        }
        private void ColorBlock_Click(object sender, RoutedEventArgs e)
        {
           ColorPopup.IsOpen = !ColorPopup.IsOpen; 
        }

        private void SetColorFromArgb()
        {
            var newColor = new SolidColorBrush(Color.FromArgb(255, (byte)RValue, (byte)GValue, (byte)BValue));
            SelectedColor = newColor;
            HexColorValue = SelectedColor.Color.ToString();
            if(!_isSaturationMoving && !_isColorSpectrumMoving) SecondGradientColor.Color = newColor.Color;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new RGBCubeWindow();
            window.Show();
        }

        public string HexColorValue
        {
            get { return hexColorValue; }
            set
            {
                hexColorValue = value;
                OnPropertyChanged();
            }
        }

        public int RValue
        {
            get { return rValue;  }
            set
            {
                rValue = value;
                OnPropertyChanged();

                ConvertRGBToCMYK();
                SetColorFromArgb();
            }
        }

        public int GValue
        {
            get { return gValue; }
            set
            {
                gValue = value;
                OnPropertyChanged();

                ConvertRGBToCMYK();
                SetColorFromArgb();
            }
        }

        public int BValue
        {
            get { return bValue; }
            set
            {
                bValue = value;
                OnPropertyChanged();

                ConvertRGBToCMYK();
                SetColorFromArgb();
            }
        }

        public int AValue
        {
            get { return aValue; }
            set
            {
                aValue = value;
                OnPropertyChanged();
            }
        }
        public int CValue
        {
            get { return cValue; }
            set
            {
                cValue = value;
                OnPropertyChanged();

                ConvertCMYKToRGB();
                SetColorFromArgb();
            }
        }
        public int MValue
        {
            get { return mValue; }
            set
            {
                mValue = value;
                OnPropertyChanged();

                ConvertCMYKToRGB();
                SetColorFromArgb();
            }
        }
        public int YValue
        {
            get { return yValue; }
            set
            {
                yValue = value;
                OnPropertyChanged();

                ConvertCMYKToRGB();
                SetColorFromArgb();
            }
        }
        public int KValue
        {
            get { return kValue; }
            set
            {
                kValue = value;
                OnPropertyChanged();

                ConvertCMYKToRGB();
                SetColorFromArgb();
            }
        }
        public SolidColorBrush SelectedColor
        {
            get { return (SolidColorBrush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); OnPropertyChanged(); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(Brushes.Black));

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
