using Gk_01.Core.ImageProcessors.ImagePointProcessors;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Image points processing
        public ICommand ImageAdditionCommand { get; set; }
        public ICommand ImageSubtractionCommand { get; set; }
        public ICommand ImageMultiplicationCommand { get; set; }
        public ICommand ImageDivisionCommand { get; set; }
        public ICommand ImageChangeBrightnessCommand { get; set; }
        public ICommand ImageGrayscaleAverageMethodCommand { get; set; }
        public ICommand ImageGrayscaleLuminosityMethodCommand { get; set; }

        public void AddImagePointProcessingHandlers()
        {
            // Image points processing
            ImageAdditionCommand = SetImageProcessingCommandHandler(
                                    processor: new AdditionProcessor(),
                                    isDialog: true,
                                    title: "Dodawanie",
                                    labelText: "Wartość składnika: ");

            ImageSubtractionCommand = SetImageProcessingCommandHandler(
                                    processor: new SubtractionProcessor(),
                                    isDialog: true,
                                    title: "Odejmowanie",
                                    labelText: "Wartość odjemnika: ");

            ImageMultiplicationCommand = SetImageProcessingCommandHandler(
                                    processor: new MultiplicationProcessor(),
                                    isDialog: true,
                                    title: "Mnożenie",
                                    labelText: "Wartość mnożnika: ");

            ImageDivisionCommand = SetImageProcessingCommandHandler(
                                    processor: new DivisionProcessor(),
                                    isDialog: true,
                                    title: "Dzielenie",
                                    labelText: "Wartość dzielnika: ");

            ImageChangeBrightnessCommand = SetImageProcessingCommandHandler(
                                    processor: new BrightnessProcessor(),
                                    isDialog: true,
                                    title: "Zmiana jasności",
                                    labelText: "Wartość piksela: ");
            ImageGrayscaleAverageMethodCommand = SetImageProcessingCommandHandler(processor: new GrayscaleAverageMethodProcessor());
            ImageGrayscaleLuminosityMethodCommand = SetImageProcessingCommandHandler(processor: new GrayscaleLuminosityProcessor());

        }
    }
}
