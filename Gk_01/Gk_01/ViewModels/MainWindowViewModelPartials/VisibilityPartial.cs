using System.Windows;

namespace Gk_01.ViewModels.MainWindowViewModelPartials
{
    public partial class MainWindowViewModel
    {
        // Visibility
        private Visibility _curveDegreeVisibility = Visibility.Collapsed;
        private Visibility _curvePointsVisibility = Visibility.Collapsed;
        private Visibility _anglesCountVisibility = Visibility.Collapsed;
        private Visibility _translationVectorVisibility = Visibility.Collapsed;
        private Visibility _characteristicsPointVisibility = Visibility.Visible;
        private Visibility _rotationVectorVisibility = Visibility.Collapsed;
        private Visibility _rotationAngleVisibility = Visibility.Collapsed;
        private Visibility _scaleVisibility = Visibility.Collapsed;

        public Visibility RotationAngleVisibility
        {
            get => _rotationAngleVisibility;
            set
            {
                _rotationAngleVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility RotationVectorVisibility
        {
            get => _rotationVectorVisibility;
            set
            {
                _rotationVectorVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility ScaleVisibility
        {
            get => _scaleVisibility;
            set
            {
                _scaleVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility CharacteristicsPointVisibility
        {
            get => _characteristicsPointVisibility;
            set
            {
                _characteristicsPointVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility TranslationVectorVisibility
        {
            get => _translationVectorVisibility;
            set
            {
                _translationVectorVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility CurveDegreeVisibility
        {
            get => _curveDegreeVisibility;
            set
            {
                _curveDegreeVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility CurvePointsVisibility
        {
            get => _curvePointsVisibility;
            set
            {
                _curvePointsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility AnglesCountVisibility
        {
            get => _anglesCountVisibility;
            set
            {
                _anglesCountVisibility = value;
                OnPropertyChanged();
            }
        }

    }
}
