using Gk_01.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gk_01.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ImagePointProcessingDialog.xaml
    /// </summary>
    public partial class ImagePointProcessingDialog : Window
    {
        public ImagePointProcessingDialog(string dialogName, string labelText)
        {
            InitializeComponent();
            Title = dialogName;
            Label.Content = labelText;
        }
    }
}
