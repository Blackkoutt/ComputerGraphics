using Gk_01.DI;
using Gk_01.Handlers;
using Gk_01.Observable;
using Gk_01.Services.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private static MainWindowViewModel? _instance = null;
        private ImagePointProcessingHandler _imagePointProcessingHandler;
        private HistogramViewModel _histogramViewModel;
        private Canvas? _canvas;
        private bool _isSaved = true;

        // Services
        private readonly IFileService _fileService;
        private readonly IDrawingService _drawingService;
        private readonly ITransformations2DService _transformations2DService;

        // Commands
        public ICommand NewFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
  
        public MainWindowViewModel(IFileService fileService, IDrawingService drawingService, ITransformations2DService transformations2DService)
        {
            // Services
            _fileService = fileService;
            _drawingService = drawingService;
            _transformations2DService = transformations2DService;
            
            // Handlers
            NewFileCommand = new RelayCommand(NewFile);
            CloseCommand = new RelayCommand(Close);
            AddDrawingHandlers();
            AddImageHandlers();
            AddCanvasHandlers();
            AddSerializationHandlers();
            AddGraphicFileHandlers();
            AddImagePointProcessingHandlers();
            AddFiltersHandlers();
            AddMorphologicalOperatorsHandlers();
            AddHistogramHandler();
            AddBinarizationHandlers();
            AddImageAnalyzeHandler();

            // Canvas Transformations
            AddCanvasTransformations();
        }
        
        public void OnInit()
        {
            _imagePointProcessingHandler = ImagePointProcessingHandler.Instance;
            _histogramViewModel = HistogramViewModel.Instance;
            _imagePointProcessingHandler.Canvas = _canvas;
            _imagePointProcessingHandler.CloseProcessingDialogEvent += EndImageProcessing;
            _imagePointProcessingHandler.ProcessedImageEvent += EndImageProcessing;
        }

        private void Close(object parameter)
        {
            if (!_isSaved)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Czy na pewno chcesz zakończyć działanie programu? Masz niezapisane zmiany.",
                    "Potwierdzenie zamknięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }
            Application.Current.Shutdown();
        }

        private void NewFile(object parameter)
        {
            _drawingService.ClearCanvas();
            _currentImage = null;
            _defaultImage = null;
            P_X = 0;
            P_Y = 0;
            LineThickness = 1;
            SelectedLineColor = Brushes.Black;
            SelectedFillColor = Brushes.Transparent;
            _currentCharacteriticsPointID = null;
            _currentShape = null;
            _isResizing = false;
            _isTranslateing = false;
            _isSaved = true;
        }

        public Canvas Canvas
        {
            set
            {
                _canvas = value;
                _drawingService.Canvas = value;
            }
        }

        public static MainWindowViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = DIContainer.GetContainer().Resolve<MainWindowViewModel>();
                    return _instance;
                }
                else
                    return _instance;
            }
        }
    }
}
