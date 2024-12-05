using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Gk_01.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy InputTypeDouble.xaml
    /// </summary>
    public partial class InputTypeDouble : UserControl, INotifyPropertyChanged
    {
        private bool _isButtonUpPressed;
        private bool _isButtonDownPressed;

        private DispatcherTimer _autoIncrementTimer;
        private const int _interval = 25;

        private DispatcherTimer _delayTimer;
        private const int _delayInterval = 200;
        public InputTypeDouble()
        {
            InitializeComponent();
            DataContext = this;
            InputDoubleValue = 0;
            InputTypeNumberTextBox.LostFocus += InputTypeNumberTextBox_LostFocus;

            _autoIncrementTimer = new DispatcherTimer();
            _autoIncrementTimer.Interval = TimeSpan.FromMilliseconds(_interval);
            _autoIncrementTimer.Tick += AutoIncrementTimer_Tick;

            _delayTimer = new DispatcherTimer();
            _delayTimer.Interval = TimeSpan.FromMilliseconds(_delayInterval);
            _delayTimer.Tick += DelayTimer_Tick;
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(double), typeof(InputTypeNumber),
            new FrameworkPropertyMetadata(0.1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        private void DelayTimer_Tick(object? sender, EventArgs e)
        {
            _delayTimer.Stop();

            if (_isButtonUpPressed || _isButtonDownPressed)
                _autoIncrementTimer.Start();
        }

        private void AutoIncrementTimer_Tick(object? sender, EventArgs e)
        {
            if (_isButtonUpPressed && InputDoubleValue < MaxDoubleValue)
                InputDoubleValue += Step;

            else if (_isButtonDownPressed && InputDoubleValue > MinDoubleValue)
                InputDoubleValue -= Step;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public static readonly DependencyProperty InputDoubleValueProperty =
            DependencyProperty.Register(nameof(InputDoubleValue), typeof(double), typeof(InputTypeDouble),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double InputDoubleValue
        {
            get { return (double)GetValue(InputDoubleValueProperty); }
            set
            {
                SetValue(InputDoubleValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxDoubleValueProperty =
           DependencyProperty.Register(nameof(MaxDoubleValue), typeof(int), typeof(InputTypeDouble),
           new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MaxDoubleValue
        {
            get { return (int)GetValue(MaxDoubleValueProperty); }
            set
            {
                SetValue(MaxDoubleValueProperty, value);
            }
        }

        public static readonly DependencyProperty MinDoubleValueProperty =
          DependencyProperty.Register(nameof(MinDoubleValue), typeof(int), typeof(InputTypeDouble),
          new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MinDoubleValue
        {
            get { return (int)GetValue(MinDoubleValueProperty); }
            set
            {
                SetValue(MinDoubleValueProperty, value);
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
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void InputTypeNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(InputTypeNumberTextBox.Text, out double value))
            {
                if (value > MaxDoubleValue) InputDoubleValue = MaxDoubleValue;
                else if (value < MinDoubleValue) InputDoubleValue = MinDoubleValue;
                else InputDoubleValue = value;
            }
            else
            {
                InputTypeNumberTextBox.Text = InputDoubleValue.ToString();
            }
        }

        private void Button_Down_Click(object sender, RoutedEventArgs e)
        {
            if (InputDoubleValue > MinDoubleValue) InputDoubleValue -= Step;
        }

        private void Button_Up_Click(object sender, RoutedEventArgs e)
        {
            if (InputDoubleValue < MaxDoubleValue)
            {
                InputDoubleValue += Step;
            }
        }
    }
}
