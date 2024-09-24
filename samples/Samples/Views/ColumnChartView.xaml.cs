using Panuon.WPF;
using Panuon.WPF.Charts;
using Samples.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 3, DisplayName = "ColumnChart", Type = "Cartesian")]
    public partial class ColumnChartView
        : Grid, ICartesianChartView
    {
        #region Ctor
        public ColumnChartView()
        {
            InitializeComponent();
        }

        public void Generate()
        {
            var itemsSource = new List<object>();
            for (var i = 0; i < 5; i++)
            {
                itemsSource.Add(new
                {
                    Label = $"Data {i + 1}",
                    Value = RandomUtil.Next(0, 100),
                });
            }
            chart.ItemsSource = itemsSource;
        }

        public void SetAnimation(
            AnimationEasing animationEasing,
            TimeSpan? animationTime
        )
        {
            chart.AnimationEasing = animationEasing;
            chart.AnimationDuration = animationTime;
        }
        #endregion
    }
}