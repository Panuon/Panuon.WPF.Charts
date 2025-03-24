using Panuon.WPF;
using Panuon.WPF.Charts;
using Samples.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Samples.Views
{
    [ExampleView(Index = 4, DisplayName = "ClusteredColumnChart", Type = "Cartesian")]
    public partial class ClusteredColumnChartView
        : UserControl, ICartesianChartView
    {
        #region Ctor
        public ClusteredColumnChartView()
        {
            InitializeComponent();
        }

        public void Generate()
        {
            var averageCostSeries = new ClusteredColumnSeries()
            {
                Spacing = 2,
                ValuesMemberPath = "Values", //对应第一层的属性名称
                ShowValueLabels = true,
                InvertForeground = Brushes.White,
            };
            for (int i = 0; i < 3; i++)
            {
                var averageCostSegment = new ClusteredColumnSeriesSegment()
                {
                    Title = $"Line {i + 1}",
                    ValueMemberPath = "Value",
                    BackgroundFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1a0b7bda")),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0b7bda")),
                };
                averageCostSeries.Segments.Add(averageCostSegment);
            }
            var AverageCostSeries = new SeriesCollection<CartesianSeriesBase>()
            {
                averageCostSeries,
            };
            chart.Series = AverageCostSeries;

            var itemsSource = new List<object>();
            for (var i = 0; i < 5; i++)
            {
                itemsSource.Add(new
                {
                    Label = $"Data {i + 1}",
                    Values = new object[]
                    {
                        new
                        {
                            Title = "Column1",
                            Value = RandomUtil.Next(0, 100)
                        },
                        new
                        {
                            Title = "Column2",
                            Value = RandomUtil.Next(0, 100)
                        },
                        new
                        {
                            Title = "Column3",
                            Value = RandomUtil.Next(0, 100)
                        },
                    }
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