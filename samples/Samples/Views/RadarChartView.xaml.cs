using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 6, DisplayName = "RadarChartView")]
    public partial class RadarChartView
        : Grid
    {
        #region Ctor
        public RadarChartView()
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