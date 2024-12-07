using Gk_01.Enums;
using Gk_01.Views;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using Gk_01.Observable;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        public ICommand LoadFileCommand { get; set; }
        public ICommand SaveFileCommand { get; set; }
        private void AddGraphicFileHandlers()
        {
            LoadFileCommand = new RelayCommand(LoadGraphicFile);
            SaveFileCommand = new RelayCommand(SaveFile);
        }
        private void SaveFile(object parameter)
        {
            if (_currentImage != null)
            {
                Dictionary<FileType, string> fileTypeDictionary = new Dictionary<FileType, string>
                {
                    { FileType.PPM_P3, "PPM P3 files (*.ppm)|*.ppm" },
                    { FileType.PPM_P6, "PPM P6 files (*.ppm)|*.ppm" },
                    { FileType.JPEG, "JPEG files (*.jpeg;*.jpg)|*.jpeg;*.jpg" }
                };
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Zapisz jako",
                    Filter = string.Join("|", fileTypeDictionary.Values),
                    FilterIndex = 1 // default 
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var filePath = saveFileDialog.FileName;
                        var fileType = fileTypeDictionary.ElementAt(saveFileDialog.FilterIndex - 1).Key;
                        int? compressionLevel = null;
                        if (fileType == FileType.JPEG)
                        {
                            SelectCompressionLevelDialog optionDialog = new SelectCompressionLevelDialog();
                            var viewModel = optionDialog.DataContext as SelectCompressionLevelDialogViewModel;
                            if (optionDialog.ShowDialog() == true && viewModel != null)
                                compressionLevel = viewModel.SelectedOption;
                        }
                        _fileService.SaveImage(_currentImage, filePath, fileType, compressionLevel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                           "Błąd zapisu pliku graficznego",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Płótno nie zawiera żadnych obrazów",
                                "",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private async void LoadGraphicFile(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Otwórz plik graficzny",
                Filter = "PPM files (*.ppm)|*.ppm|JPEG files (*.jpeg;*.jpg)|*.jpeg;*.jpg",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    var image = await _fileService.LoadImage(filePath);
                    _currentImage = image;
                    _defaultImage = new Image
                    {
                        Source = (image.Source as BitmapSource)?.Clone()
                    };

                    var originalImageBitmapSource = _currentImage.Source as BitmapSource;
                    _originalImageWritableBitmap = new WriteableBitmap(originalImageBitmapSource);

                    _drawingService.ClearCanvas();
                    _canvas!.Children.Add(image);
                    imageDefaultLeft = 0;
                    imageDefaultTop = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                       "Błąd wczytywania pliku graficznego",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);
                }
            }
        }
    }
}
