using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 5, DisplayName = "PieChart", Type = "Radial")]
    public partial class PieChartView
        : Grid
    {
        #region Ctor
        public PieChartView()
        {
            InitializeComponent();

            var itemsSource = new object[]
            {
                new { Label = "Data 1", Value = 25},
                new { Label = "Data 2", Value = 9 },
                new { Label = "Data 3", Value = 17 },
                new { Label = "Data 4", Value = 18 },
                new { Label = "Data 5", Value = 9 },
            };

            chart.ItemsSource = itemsSource;
        }
        #endregion
    }
}