using Gk_01.Core.ImageProcessors;
using Gk_01.Observable;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        private Image? _currentImage;
        private Image? _defaultImage;
        private WriteableBitmap? _originalImageWritableBitmap;

        // Commands
        public ICommand ResetImageCommand { get; set; }
        public ICommand RedoCommand { get; set; }

        private void AddImageHandlers()
        {
            ResetImageCommand = new RelayCommand(ResetImage);
            RedoCommand = new RelayCommand(Redo);
        }
        public RelayCommand SetImageProcessingCommandHandler(ImageProcessor processor, bool isDialog = false, string title = "", string labelText = "", int minValue = int.MinValue, int maxValue = int.MaxValue, int defaultValue = 0)
        {
            if (isDialog)
            {
                return new RelayCommand(param =>
                    _imagePointProcessingHandler!.ShowDialog(param,
                    imageProcessor: processor,
                    defaultImage: _defaultImage,
                    currentImage: _currentImage,
                    title: title,
                    labelText: labelText,
                    minValue: minValue,
                    maxValue: maxValue,
                    defaultValue: defaultValue
                ));
            }
            else
            {
                return new RelayCommand(param =>
                   _imagePointProcessingHandler!.ProcessImage(param,
                   imageProcessor: processor,
                   defaultImage: _defaultImage,
                   currentImage: _currentImage
               ));
            }
        }

        private void Redo(object parameter)
        {
            // TO DO
        }

        private void ResetImage(object parameter)
        {
            if (_originalImageWritableBitmap != null)
            {
                _currentImage.Source = _originalImageWritableBitmap;
                _defaultImage.Source = _originalImageWritableBitmap;
                _canvas.Children.Clear();
                _canvas.Children.Add(_currentImage);
            }
        }

        private void EndImageProcessing(object? sender, EventArgs e)
        {
            if (_defaultImage != null && _currentImage != null)
            {
                _defaultImage = new Image
                {
                    Source = (_currentImage.Source as BitmapSource)?.Clone()
                };
            }
        }

    }
}
