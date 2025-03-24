using Panuon.WPF;
using Samples.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Samples.Views
{
    [ExampleView(Index = 1, DisplayName = "LineChart", Type = "Cartesian")]
    public partial class LineChartView
        : UserControl, ICartesianChartView
    {
        #region Ctor
        public LineChartView()
        {
            InitializeComponent();
        }

        public void Generate()
        {
            var itemsSource = new List<object>();
            for(var i = 0; i < 5; i++)
            {
                itemsSource.Add(new
                {
                    Label = $"Data {i + 1}",
                    Value1 = RandomUtil.Next(0, 100),
                    Value2 = RandomUtil.Next(0, 100),
                    Value3 = RandomUtil.Next(0, 100),
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