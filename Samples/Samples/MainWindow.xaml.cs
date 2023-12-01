using Panuon.WPF.Charts;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            chartPanel.ItemsSource = new object[]
            {
                new { Title = "1", Value1 = 5, Value2 = 4, },
                new { Title = "2", Value1 = 1, Value2 = 2, },
                new { Title = "3", Value1 = 7, Value2 = 4, },
                new { Title = "4", Value1 = 8, Value2 = 5 },
                new { Title = "5", Value1 = 4, Value2 = 6 },
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //xAxis.Background = Brushes.PaleGreen;
        }
    }
}
