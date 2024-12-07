using Gk_01.Core.ImageProcessors;
using Gk_01.Enums;
using Gk_01.Core.ImageProcessors.MorphologicalOperators;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Structuring Element
        private int structuringElementSize = 3;
        private string _selectedStructuringObject;
        private StructuringElementType _currentStructuringElementType = StructuringElementType.Square;

        // Morphological Operators Commands
        public ICommand DilatationOperatorCommand { get; set; }
        public ICommand ErosionOperatorCommand { get; set; }
        public ICommand OpenOperatorCommand { get; set; }
        public ICommand CloseOperatorCommand { get; set; }
        public ICommand ThinningOperatorCommand { get; set; }
        public ICommand ThickeningOperatorCommand { get; set; }

        // Morphological Operators
        private ImageMorphologicalOperatorProcessor dilatationOperatorProcessor = new DilatationOperatorProcessor();
        private ImageMorphologicalOperatorProcessor erosionOperatorProcessor = new ErosionOperatorProcessor();
        private ImageMorphologicalOperatorProcessor openingOperatorProcessor = new OpeningOperatorProcessor();
        private ImageMorphologicalOperatorProcessor closingOperatorProcessor =  new ClosingOperatorProcessor();
        private ImageMorphologicalOperatorProcessor thinningOperatorProcessor = new ThinningOperatorProcessor();
        private ImageMorphologicalOperatorProcessor thickeningOperatorProcessor = new ThickeningOperatorProcessor();

        // Structuring elements
        public ObservableCollection<string> StructuringObjects { get; set; } =
        new ObservableCollection<string>
        {
            "Kwadrat",
            "Koło",
        };

        public void AddMorphologicalOperatorsHandlers()
        {
            DilatationOperatorCommand = SetImageProcessingCommandHandler(processor: dilatationOperatorProcessor);
            ErosionOperatorCommand = SetImageProcessingCommandHandler(processor: erosionOperatorProcessor);
            OpenOperatorCommand = SetImageProcessingCommandHandler(processor: openingOperatorProcessor);
            CloseOperatorCommand = SetImageProcessingCommandHandler(processor: closingOperatorProcessor);
            ThinningOperatorCommand = SetImageProcessingCommandHandler(processor: thinningOperatorProcessor);
            ThickeningOperatorCommand = SetImageProcessingCommandHandler(processor: thickeningOperatorProcessor);
        }

        public StructuringElementType CurrentStructuringElementType
        {
            get => _currentStructuringElementType;
            set
            {
                _currentStructuringElementType = value;
                dilatationOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                erosionOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                openingOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                closingOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                thickeningOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                thinningOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                OnPropertyChanged();
            }
        }

        public int StructuringElementSize
        {
            get => structuringElementSize;
            set
            {
                structuringElementSize = value;
                dilatationOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                erosionOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                openingOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                closingOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                thickeningOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                thinningOperatorProcessor.SetStructuringElement(_currentStructuringElementType, structuringElementSize);
                OnPropertyChanged();
            }
        }

        public string SelectedStructuringObject
        {
            get => _selectedStructuringObject;
            set
            {
                if (_selectedStructuringObject != value)
                {
                    _selectedStructuringObject = value;
                    switch (_selectedStructuringObject)
                    {
                        case "Kwadrat":
                            _currentStructuringElementType = StructuringElementType.Square;
                            break;
                        case "Koło":
                            _currentStructuringElementType = StructuringElementType.Circle;
                            break;
                    }
                    OnPropertyChanged();
                }
            }
        }
    }
}
