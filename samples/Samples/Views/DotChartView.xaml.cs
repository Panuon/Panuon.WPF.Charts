using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 2, DisplayName = "DotChartView")]
    public partial class DotChartView
        : Grid
    {
        #region Ctor
        public DotChartView()
        {
            InitializeComponent();

            var itemsSource = new object[]
            {
                new { Title = "Data 1", Value1 = 25, Value2 = 4, Value3 = 2, },
                new { Title = "Data 2", Value1 = 9, Value2 = 26, Value3 = 3, },
                new { Title = "Data 3", Value1 = 17, Value2 = 4, Value3 = 1, },
                new { Title = "Data 4", Value1 = 18, Value2 = 5, Value3 = 24, },
                new { Title = "Data 5", Value1 = 9, Value2 = 16, Value3 = 60, },
            };

            chart.ItemsSource = itemsSource;
        }
        #endregion
    }
}