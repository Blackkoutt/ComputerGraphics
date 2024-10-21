using Gk_01.DI;
using Unity;

namespace Gk_01.ViewModels
{
    public class SelectCompressionLevelDialogViewModel : BaseViewModel
    {
        private int _selectedOption;

        public SelectCompressionLevelDialogViewModel() 
        {
            SelectedOption = 0;
        }
        public int SelectedOption
        {
            get => _selectedOption;
            set
            {
                _selectedOption = value;
                OnPropertyChanged();
            }
        }
    }
}
