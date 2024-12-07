using Gk_01.Controls;
using Gk_01.Core.ImageProcessors;
using Gk_01.Core.ImageProcessors.ImageFilters;
using Gk_01.Observable;
using Gk_01.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gk_01.Handlers
{
    public class ImagePointProcessingHandler : ObservableObject
    {
        private Canvas? _canvas;
        private ImagePointProcessingDialog? processingDialog;
        private CustomFilterDialog? customFilterDialog;
        private Grid? filterMask;
        private Image? _defaultImage;
        private Image? _currentImage;
        private ImageProcessor? _imageProcessor;
        private int value;
        private int filterSize;
        public event EventHandler CloseProcessingDialogEvent;
        public event EventHandler ProcessedImageEvent;

        private static ImagePointProcessingHandler? _instance = null;
        public ICommand CustomFilterCommand { get; set; }
        
        private ImagePointProcessingHandler() { }

        public void AutoBinarizeImage(object parameter, ImageAutoBinarizationProcessor imageProcessor, Image? defaultImage, Image? currentImage)
        {
            if (_canvas == null)
            {
                MessageBox.Show($"Przekształcenie nie ma dostępu do elementu canvas.",
                "Błąd przekształceń",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            if (currentImage == null || defaultImage == null)
            {
                MessageBox.Show($"Płótno nie zawiera żadnych obrazów",
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
            _defaultImage = defaultImage;
            _currentImage = currentImage;
            _imageProcessor = imageProcessor;
            if (processingDialog != null)
            {
                processingDialog.Close();
                processingDialog = null;
            }
            if (customFilterDialog != null)
            {
                customFilterDialog.Close();
                customFilterDialog = null;
            }
            imageProcessor!.ProcessImage(
                currentImage: _currentImage!,
                defaultImage: _defaultImage!);

            int threshold = imageProcessor.Threshold;
            MessageBox.Show($"Wyznaczona wartość progowa: {threshold}",
                "Wartość progowa",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        public void ShowDialog(object parameter, ImageProcessor imageProcessor, Image? defaultImage, Image? currentImage, string title, string labelText, int minValue = int.MinValue, int maxValue = int.MaxValue, int defaultValue = 0)
        {
            value = defaultValue;
            if (_canvas == null)
            {
                MessageBox.Show($"Przekształcenie nie ma dostępu do elementu canvas.",
                "Błąd przekształceń",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            if (currentImage == null || defaultImage == null)
            {
                MessageBox.Show($"Płótno nie zawiera żadnych obrazów",
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
            _defaultImage = defaultImage;
            _currentImage = currentImage;
            _imageProcessor = imageProcessor;
            if (processingDialog != null)
            {
                processingDialog.Close();
                processingDialog = null;
            }
            if (customFilterDialog != null)
            {
                customFilterDialog.Close();
                customFilterDialog = null;
            }
            processingDialog = new ImagePointProcessingDialog(title, labelText);
            processingDialog.Input.MinValue = minValue;
            processingDialog.Input.MaxValue = maxValue;
            processingDialog.DataContext = this;
            processingDialog.Closed += delegate { CloseProcessingDialogEvent?.Invoke(this, EventArgs.Empty); };
            processingDialog.ShowDialog();
        }


        public void CustomFilter(object parameter, Image? defaultImage, Image? currentImage)
        {
            _defaultImage = defaultImage;
            _currentImage = currentImage;
            if (processingDialog != null)
            {
                processingDialog.Close();
                processingDialog = null;
            }
            if(customFilterDialog != null)
            {
                customFilterDialog.Close();
                customFilterDialog = null;  
            }
            customFilterDialog = new CustomFilterDialog();
            filterMask = customFilterDialog.FilterMask;
            customFilterDialog.DataContext = this;
            CustomFilterCommand = new RelayCommand(ApplyCustomFilter);
            customFilterDialog.ShowDialog();
        }

        private void ApplyCustomFilter(object parameter)
        {
            if (_canvas == null)
            {
                MessageBox.Show($"Przekształcenie nie ma dostępu do elementu canvas.",
                "Błąd przekształceń",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            if (_currentImage == null || _defaultImage == null)
            {
                MessageBox.Show($"Płótno nie zawiera żadnych obrazów",
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }

            if (filterMask == null) return;
            var filterSize = FilterSize;
            int[] filter = new int[FilterSize * FilterSize];

            for (int row = 0; row < FilterSize; row++)
            {
                for (int col = 0; col < FilterSize; col++)
                {
                    var element = filterMask.Children
                        .Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                    if (element is InputTypeNumber inputControl)
                    {
                        int value = inputControl.InputValue;
                        filter[row * filterSize + col] = value;
                    }
                }
            }

            var processor = new CustomFilter(filter, filterSize);
            processor.ProcessImage(_defaultImage, _currentImage);
            customFilterDialog!.Close();
            ProcessedImageEvent?.Invoke(this, EventArgs.Empty);
        }

        public void ProcessImage(object parameter, ImageProcessor imageProcessor, Image? defaultImage, Image? currentImage)
        {
            if (_canvas == null)
            {
                MessageBox.Show($"Przekształcenie nie ma dostępu do elementu canvas.",
                "Błąd przekształceń",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            if (currentImage == null || defaultImage == null)
            {
                MessageBox.Show($"Płótno nie zawiera żadnych obrazów",
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
            _defaultImage = defaultImage;
            _currentImage = currentImage;
            _imageProcessor = imageProcessor;
            if (processingDialog != null)
            {
                processingDialog.Close();
                processingDialog = null;
            }
            if (customFilterDialog != null)
            {
                customFilterDialog.Close();
                customFilterDialog = null;
            }
            _imageProcessor.ProcessImage(_defaultImage, _currentImage);
            ProcessedImageEvent?.Invoke(this, EventArgs.Empty);
        }

        public void AddMask()
        {
            if (filterMask == null) return;
            filterMask.Children.Clear();
            filterMask.RowDefinitions.Clear();
            filterMask.ColumnDefinitions.Clear();

            for (int i = 0; i < FilterSize; i++)
            {
                filterMask.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                filterMask.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int row = 0; row < FilterSize; row++)
            {
                for (int col = 0; col < FilterSize; col++)
                {
                    var input = new InputTypeNumber
                    {
                        Width = 50,
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        InputValue = 0,
                        MinValue = int.MinValue
                    };

                    Grid.SetRow(input, row);
                    Grid.SetColumn(input, col);

                    filterMask.Children.Add(input);
                }
            }
        }


        public Canvas? Canvas
        {
            get { return _canvas; }
            set { _canvas = value; }
        }

        public static ImagePointProcessingHandler Instance
        {
            get
            {
                _instance ??= new ImagePointProcessingHandler();
                return _instance;
            }
        }

        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
                _imageProcessor!.ProcessImage(
                    currentImage: _currentImage!,
                    defaultImage: _defaultImage!,
                    value: this.value);
            }
        }
        
        public int FilterSize
        {
            get { return filterSize; }
            set
            {
                this.filterSize = value;
                OnPropertyChanged();
                AddMask();
            }
        }
    }
}
