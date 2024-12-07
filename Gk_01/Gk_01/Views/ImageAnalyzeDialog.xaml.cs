using Gk_01.ViewModels.MainWindowViewModelPartials;
using System.Windows;

namespace Gk_01.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ImageAnalyzeDialog.xaml
    /// </summary>
    public partial class ImageAnalyzeDialog : Window
    {
        public ImageAnalyzeDialog()
        {
            InitializeComponent();
            DataContext = MainWindowViewModel.Instance;
        }
    }
}
