using Gk_01.Core.ImageProcessors.ImageAnalyze;
using Gk_01.Enums;
using Gk_01.Observable;
using Gk_01.Views;
using System.Windows;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
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
        private int maxValue= 100;

        private ImageAnalyzeDialog dialog;

        // Visibility
        private Visibility defaultHueVisibility = Visibility.Visible;
        private Visibility redHueVisibility = Visibility.Collapsed;
        public Visibility DefaultHueVisibility
        {
            get => defaultHueVisibility; 
            set
            {
                defaultHueVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility RedHueVisibility
        {
            get => redHueVisibility;
            set
            {
                redHueVisibility = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand ImageAnalyzeCommand { get; set; }
        public ICommand AnalyzeImageButtonClickCommand { get; set; }
        public ICommand ChangeAnalyzingColorCommand { get; set; }

        private ImageAnalyzeProcessor imageAnalyzeProcessor;

        public void AddImageAnalyzeHandler()
        {
            imageAnalyzeProcessor = new ImageAnalyzeProcessor();
            ImageAnalyzeCommand = new RelayCommand(OpenDialog);
            AnalyzeImageButtonClickCommand = new RelayCommand(AnalyzeImage);
            ChangeAnalyzingColorCommand = new RelayCommand(ChangeAnalyzingColor);
        }

        private void ChangeAnalyzingColor(object parameter)
        {
            if (parameter is string colorName)
            {
                if (Enum.TryParse(typeof(ColorEnum), colorName, out var parseResult))
                {
                    CurrentAnalyzingColor = (ColorEnum)parseResult;
                    switch (CurrentAnalyzingColor)
                    {
                        case ColorEnum.Red:
                            RedHueVisibility = Visibility.Visible;
                            DefaultHueVisibility = Visibility.Collapsed;
                            MinBottomRedHue = 0;
                            MaxBottomRedHue = 20;
                            MinTopRedHue = 335;
                            MaxTopRedHue = 360;

                            break;
                        case ColorEnum.Green:
                            RedHueVisibility = Visibility.Collapsed;
                            DefaultHueVisibility = Visibility.Visible;
                            MinHue = 65;
                            MaxHue = 160;
                            break;
                        case ColorEnum.Blue:
                            RedHueVisibility = Visibility.Collapsed;
                            DefaultHueVisibility = Visibility.Visible;
                            MinHue = 170;
                            MaxHue = 255;
                            break;
                    }
                    MinSaturation = 15;
                    MaxSaturation = 100;
                    MinValue = 20;
                    MaxValue = 100;
                }
            }
        }

        private void OpenDialog(object parameter)
        {
            dialog = new ImageAnalyzeDialog();
            RedHueVisibility = Visibility.Collapsed;
            DefaultHueVisibility = Visibility.Visible;
            MinHue = 65;
            MaxHue = 160;
            MinSaturation = 15;
            MaxSaturation = 100;
            MinValue = 20;
            MaxValue = 100;
            dialog.ShowDialog();
        }
        private void AnalyzeImage(object parameter)
        {
            _imagePointProcessingHandler!.ProcessImage(parameter,
                   imageProcessor: imageAnalyzeProcessor,
                   defaultImage: _defaultImage,
                   currentImage: _currentImage);
            var analyzeResult = Math.Round(imageAnalyzeProcessor.PixelColorPercentage, 2);
            string colorName = "";
            if (_currentAnalyzingColor == ColorEnum.Green) colorName = "zielonym";
            else if (_currentAnalyzingColor == ColorEnum.Red) colorName = "czerwonym";
            else if (_currentAnalyzingColor == ColorEnum.Blue) colorName = "niebieskim";

            MessageBoxResult result = MessageBox.Show(
            $"Obraz zawiera {analyzeResult}% pikseli w kolorze {colorName}.",
            "Wynik analizy obrazu",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

            if(result == MessageBoxResult.OK)
            {
                dialog.Close();
            }
        }

        public ColorEnum CurrentAnalyzingColor
        {
            get => _currentAnalyzingColor;
            set 
            {
                _currentAnalyzingColor = value;
                imageAnalyzeProcessor.CurrentAnalyzingColor = _currentAnalyzingColor;
            }
        }

        public int MinHue
        {
            get => minHue;
            set
            {
                minHue = value;
                imageAnalyzeProcessor.MinHue = minHue;
                OnPropertyChanged();
            }
        }
        public int MaxHue
        {
            get => maxHue;
            set
            {
                maxHue = value;
                imageAnalyzeProcessor.MaxHue = maxHue;
                OnPropertyChanged();
            }
        }
        public int MinBottomRedHue
        {
            get => minBottomRedHue;
            set
            {
                minBottomRedHue = value;
                imageAnalyzeProcessor.MinBottomRedHue = minBottomRedHue;
                OnPropertyChanged();
            }
        }
        public int MaxBottomRedHue
        {
            get => maxBottomRedHue;
            set
            {
                maxBottomRedHue = value;
                imageAnalyzeProcessor.MaxBottomRedHue = maxBottomRedHue;
                OnPropertyChanged();
            }
        }
        public int MinTopRedHue
        {
            get => minTopRedHue;
            set
            {
                minTopRedHue = value;
                imageAnalyzeProcessor.MinTopRedHue = minTopRedHue;
                OnPropertyChanged();
            }
        }
        public int MaxTopRedHue
        {
            get => maxTopRedHue;
            set
            {
                maxTopRedHue = value;
                imageAnalyzeProcessor.MaxTopRedHue = maxTopRedHue;
                OnPropertyChanged();
            }
        }
        public int MinSaturation
        {
            get => minSaturation;
            set
            {
                minSaturation = value;
                imageAnalyzeProcessor.MinSaturation = minSaturation;
                OnPropertyChanged();
            }
        }
        public int MaxSaturation
        {
            get => maxSaturation;
            set
            {
                maxSaturation = value;
                imageAnalyzeProcessor.MaxSaturation = maxSaturation;
                OnPropertyChanged();
            }
        }
        public int MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
                imageAnalyzeProcessor.MinValue = minValue;
                OnPropertyChanged();
            }
        }
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                imageAnalyzeProcessor.MaxValue = maxValue;
                OnPropertyChanged();
            }
        }
    }
}
