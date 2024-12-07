using Gk_01.Observable;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        public ICommand SerializeCommand { get; set; }
        public ICommand DeserializeCommand { get; set; }
        private void AddSerializationHandlers()
        {
            SerializeCommand = new RelayCommand(SerializeObjects);
            DeserializeCommand = new RelayCommand(DeserializeObjects);
        }
        private void SerializeObjects(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Serializuj jako",
                Filter = "Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml",
                FilterIndex = 1 // default txt
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    _fileService.SerializeToFile(filePath, _canvas!.Children);
                    _isSaved = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                                   "Błąd serializacji",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Error);
                }

            }
        }

        private void DeserializeObjects(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Deserializuj z",
                Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    var shapeDtoList = _fileService.DeserializeFromFile(filePath);
                    var badShapeTypes = _drawingService.DrawShapes(shapeDtoList);
                    if (badShapeTypes.Any())
                    {
                        MessageBox.Show($"Kilka figur zostało niewczytanych z powodu nieobsługiwanych typów: " +
                                        $"{string.Join(", ", badShapeTypes)}",
                                        "Błąd wczytywania",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}",
                                    "Błąd deserializacji",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }
    }
}
