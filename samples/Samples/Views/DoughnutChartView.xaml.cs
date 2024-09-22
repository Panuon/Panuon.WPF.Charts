using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 6, DisplayName = "DoughnutChart")]
    public partial class DoughnutChartView
        : Grid
    {
        #region Ctor
        public DoughnutChartView()
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