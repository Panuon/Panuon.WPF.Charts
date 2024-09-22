using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 4, DisplayName = "Clustered")]
    public partial class ClusteredColumnChartView
        : Grid
    {
        #region Ctor
        public ClusteredColumnChartView()
        {
            InitializeComponent();

            var itemsSource = new object[]
            {
                new { Title = "Data 1", Values = new int[] { 25, 4, 72 } },
                new { Title = "Data 2", Values = new int[] { 9, 86, 35 } },
                new { Title = "Data 3", Values = new int[] { 67, 4, 1 } },
                new { Title = "Data 4", Values = new int[] { 18, 5, 24 } },
                new { Title = "Data 5", Values = new int[] { 89, 16, 60 } },
            };

            chart.ItemsSource = itemsSource;
        }
        #endregion
    }
}