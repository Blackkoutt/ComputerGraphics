using Gk_01.Observable;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Histogram
        public ICommand HistogramCommand { get; set; }

        public void AddHistogramHandler()
        {
            HistogramCommand = new RelayCommand(param => _histogramViewModel!.ShowHistogram(param, currentImage: _currentImage));
        }
    }
}
