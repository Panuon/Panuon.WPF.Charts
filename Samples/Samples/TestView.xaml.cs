using Panuon.WPF;
using Panuon.WPF.Charts;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Samples
{
    /// <summary>
    /// TestView.xaml 的交互逻辑
    /// </summary>
    public partial class TestView : Window
    {
        public TestView()
        {
            InitializeComponent();

            var viewModel = new TestViewModel();
            DataContext = viewModel;

            
            chart.ItemsSource = new object[]
            {
                new
                {
                    Label = "1",
                    Value = 0.7,
                },
                new
                {
                    Label = "2",
                    Value = (decimal?)null,
                },
                new
                {
                    Label = "3",
                    Value = 0.2,
                },
                new
                {
                    Label = "4",
                    Value = (decimal?)null,
                },
                new
                {
                    Label = "5",
                    Value = -0.8,
                },
                new
                {
                    Label = "6",
                    Value = -3,
                },
                new
                {
                    Label = "7",
                    Value = -2,
                }
            };
        }

        private void chart_DrawingHorizontalGridLine(object sender, Panuon.WPF.Charts.DrawingHorizontalGridLineRoutedEventArgs e)
        {
            if(e.Value == 1
                || e.Value == -1)
            {
                e.Stroke = Brushes.Green;
            } else if (e.Value == 2
                || e.Value == -2)
            {
                e.Stroke = Brushes.Yellow;
            } else if (e.Value == 3
                || e.Value == -3)
            {
                e.Stroke = Brushes.Red;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as TestViewModel).YAxisLabels = new Collection<YAxisLabel>()
            {
                new YAxisLabel(){ Label = "3", Value = 3 },
                new YAxisLabel(){ Label = "-3", Value = -3 },
            };
        }
    }

    public class TestViewModel
        : NotifyPropertyChangedBase
    {
        private Collection<YAxisLabel> myVar;

        public Collection<YAxisLabel> YAxisLabels
        {
            get { return myVar; }
            set { myVar = value; NotifyOfPropertyChange("YAxisLabels"); }
        }

    }
}
