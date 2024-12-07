using Gk_01.Core.ImageProcessors.ImageBinarization;
using Gk_01.Observable;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Binarization
        public ICommand BinarizationThresholdCommand { get; set; }
        public ICommand BinarizationBlackSelectionCommand { get; set; }
        public ICommand BinarizationMeanIterativeSelectionCommand { get; set; }
        public ICommand BinarizationEntropySelectionCommand { get; set; }

        public void AddBinarizationHandlers()
        {
            BinarizationThresholdCommand = SetImageProcessingCommandHandler(
                                   processor: new ThresholdBinarizationProcessor(),
                                   isDialog: true,
                                   title: "Binaryzacja",
                                   labelText: "Wartość progowa: ",
                                   minValue: 0,
                                   maxValue: 255,
                                   defaultValue: 127);

            BinarizationBlackSelectionCommand = SetImageProcessingCommandHandler(
                                    processor: new BlackPercentSelectionBinarizationProcessor(),
                                    isDialog: true,
                                    title: "Binaryzacja",
                                    labelText: "Procent czarnych pikseli (%): ",
                                    minValue: 0,
                                    maxValue: 100,
                                    defaultValue: 50);

            BinarizationMeanIterativeSelectionCommand = new RelayCommand(param =>
                                                        _imagePointProcessingHandler!.AutoBinarizeImage(param,
                                                        imageProcessor: new MeanIterativeSelectionBinarizationProcessor(),
                                                        defaultImage: _defaultImage,
                                                        currentImage: _currentImage));

            BinarizationEntropySelectionCommand = new RelayCommand(param =>
                                                        _imagePointProcessingHandler!.AutoBinarizeImage(param,
                                                        imageProcessor: new EntropyBinarizationProcessor(),
                                                        defaultImage: _defaultImage,
                                                        currentImage: _currentImage));
        }
    }
}
