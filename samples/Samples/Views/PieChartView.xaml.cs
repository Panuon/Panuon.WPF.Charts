using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 5, DisplayName = "PieChart")]
    public partial class PieChartView
        : Grid
    {
        #region Ctor
        public PieChartView()
        {
            InitializeComponent();

            var itemsSource = new object[]
            {
                new { Title = "Data 1", Value = 25},
                new { Title = "Data 2", Value = 9 },
                new { Title = "Data 3", Value = 17 },
                new { Title = "Data 4", Value = 18 },
                new { Title = "Data 5", Value = 9 },
            };

            chart.ItemsSource = itemsSource;
        }
        #endregion
    }
}