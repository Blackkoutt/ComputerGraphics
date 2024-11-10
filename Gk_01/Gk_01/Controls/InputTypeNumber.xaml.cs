using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Gk_01.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy InputTypeNumber.xaml
    /// </summary>
    public partial class InputTypeNumber : UserControl, INotifyPropertyChanged
    {
        private bool _isButtonUpPressed;
        private bool _isButtonDownPressed;

        private DispatcherTimer _autoIncrementTimer;
        private const int _incrementStep = 1;
        private const int _interval = 25;

        private DispatcherTimer _delayTimer;
        private const int _delayInterval = 200;
        public InputTypeNumber()
        {
            InitializeComponent();
            DataContext = this;
            InputValue = 0;
            InputTypeNumberTextBox.LostFocus += InputTypeNumberTextBox_LostFocus;

            _autoIncrementTimer = new DispatcherTimer();
            _autoIncrementTimer.Interval = TimeSpan.FromMilliseconds(_interval);
            _autoIncrementTimer.Tick += AutoIncrementTimer_Tick;

            _delayTimer = new DispatcherTimer();
            _delayTimer.Interval = TimeSpan.FromMilliseconds(_delayInterval);
            _delayTimer.Tick += DelayTimer_Tick;
        }

        private void DelayTimer_Tick(object? sender, EventArgs e)
        {
            _delayTimer.Stop();

            if (_isButtonUpPressed || _isButtonDownPressed)
                _autoIncrementTimer.Start();
        }

        private void AutoIncrementTimer_Tick(object? sender, EventArgs e)
        {
            if (_isButtonUpPressed && InputValue < MaxValue)
                InputValue += _incrementStep;

            else if (_isButtonDownPressed && InputValue > MinValue)
                InputValue -= _incrementStep;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty InputValueProperty =
            DependencyProperty.Register(nameof(InputValue), typeof(int), typeof(InputTypeNumber),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int InputValue
        {
            get { return (int)GetValue(InputValueProperty); }
            set
            {
                SetValue(InputValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueProperty =
           DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(InputTypeNumber),
           new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueProperty =
          DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(InputTypeNumber),
          new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Up_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isButtonUpPressed = true;
            _delayTimer.Start();
        }

        private void Button_Up_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isButtonUpPressed = false;
            _delayTimer.Stop();
            _autoIncrementTimer.Stop();
        }

        private void Button_Down_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isButtonDownPressed = true;
            _delayTimer.Start();
        }

        private void Button_Down_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isButtonDownPressed = false;
            _delayTimer.Stop();
            _autoIncrementTimer.Stop();
        }

        private void InputTypeNumberTextBox_preview_text_input(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void InputTypeNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputTypeNumberTextBox.Text, out int value))
            {
                if (value > MaxValue) InputValue = MaxValue;
                else if (value < MinValue) InputValue = MinValue;
                else InputValue = value;
            }
            else
            {
                InputTypeNumberTextBox.Text = InputValue.ToString();
            }
        }

        private void Button_Down_Click(object sender, RoutedEventArgs e)
        {
           if ( InputValue > MinValue) InputValue -= _incrementStep;
        }

        private void Button_Up_Click(object sender, RoutedEventArgs e)
        {
            if (InputValue < MaxValue)
            {
                InputValue += _incrementStep;
            }
        }
    }
}
