﻿using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 3, DisplayName = "BarChart")]
    public partial class BarChartView
        : Grid
    {
        #region Ctor
        public BarChartView()
        {
            InitializeComponent();

            var itemsSource = new object[]
            {
                new { Label = "Data 1", Value = 5 },
                new { Label = "Data 2", Value = 9 },
                new { Label = "Data 3", Value = 2 },
                new { Label = "Data 4", Value = 8 },
                new { Label = "Data 5", Value = 9 },
            };

            chart.ItemsSource = itemsSource;
        }
        #endregion
    }
}