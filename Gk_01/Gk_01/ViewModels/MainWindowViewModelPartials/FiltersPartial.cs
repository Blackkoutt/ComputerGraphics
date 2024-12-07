using Gk_01.Core.ImageProcessors.ImageFilters;
using Gk_01.Observable;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Image filters
        public ICommand FilterAverageCommand { get; set; }
        public ICommand FilterMedianCommand { get; set; }
        public ICommand FilterVerticalSobelCommand { get; set; }
        public ICommand FilterHorizontalSobelCommand { get; set; }
        public ICommand FilterHighPassCommand { get; set; }
        public ICommand FilterGaussianCommand { get; set; }
        public ICommand FilterCustomCommand { get; set; }

        public void AddFiltersHandlers()
        {
            FilterAverageCommand = SetImageProcessingCommandHandler(processor: new AverageFilter());
            FilterMedianCommand = SetImageProcessingCommandHandler(processor: new MedianFilter(3));
            FilterVerticalSobelCommand = SetImageProcessingCommandHandler(processor: new VerticalSobelFilter());
            FilterHorizontalSobelCommand = SetImageProcessingCommandHandler(processor: new HorizontalSobelFilter());
            FilterHighPassCommand = SetImageProcessingCommandHandler(processor: new HighPassFilter());
            FilterGaussianCommand = SetImageProcessingCommandHandler(processor: new GaussianFilter());
            FilterCustomCommand = new RelayCommand(param =>
                                  _imagePointProcessingHandler!.CustomFilter(param,
                                  defaultImage: _defaultImage,
                                  currentImage: _currentImage));
        }
    }
}
