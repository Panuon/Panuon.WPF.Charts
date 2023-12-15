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

            var itemsSource = new object[]
            {
                new { Title = "数据1", Value1 = 5, Value2 = 4, Value3 = 2, },
                new { Title = "数据2", Value1 = 9, Value2 = 6, Value3 = 3, },
                new { Title = "数据3", Value1 = 7, Value2 = 4, Value3 = 1, },
                new { Title = "数据4", Value1 = 8, Value2 = 5, Value3 = 4, },
                new { Title = "数据5", Value1 = 9, Value2 = 6, Value3 = 60, },
            };

            chartPanel1.ItemsSource = itemsSource;
            chartPanel2.ItemsSource = itemsSource;
            chartPanel3.ItemsSource = itemsSource;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //xAxis.Background = Brushes.PaleGreen;
        }
    }
}
