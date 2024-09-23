using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 1, DisplayName = "LineChart")]
    public partial class LineChartView
        : Grid
    {
        #region Ctor
        public LineChartView()
        {
            InitializeComponent();

            var itemsSource = new object[]
            {
                new { Label = "Long Data Column Name 1", Value1 = 25, Value2 = 4, Value3 = 2, },
                new { Label = "Long Data Column Name 2", Value1 = 9, Value2 = 26, Value3 = 3, },
                new { Label = "Long Data Column Name 3", Value1 = 17, Value2 = 4, Value3 = 1, },
                new { Label = "Long Data Column Name 4", Value1 = 18, Value2 = 5, Value3 = 24, },
                new { Label = "Long Data Column Name 5", Value1 = 9, Value2 = 16, Value3 = 60, },
            };

            chart.ItemsSource = itemsSource;
        }
        #endregion
    }
}