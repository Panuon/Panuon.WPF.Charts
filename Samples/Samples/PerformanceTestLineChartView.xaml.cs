using Samples.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Samples.Views
{
    public partial class PerformanceTestLineChartView
        : Grid
    {
        private Stopwatch _stopwatch;

        #region Ctor
        public PerformanceTestLineChartView()
        {
            InitializeComponent();
        }
        #endregion

        private void DataCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = int.Parse((DataCountComboBox.SelectedItem as ComboBoxItem).Content.ToString());

            var quality = 100;
            if(count >= 10000)
            {
                quality = 80;
            }
            else if (count >= 5000)
            {
                quality = 90;
            }

            lineSeries1.RenderQuality = quality;
            lineSeries2.RenderQuality = quality;
            lineSeries3.RenderQuality = quality;

            var itemsSource = new List<object>();

            var labelDelta = count / 5;
            var lastValue1 = RandomUtil.Next(10, 90);
            var lastValue2 = RandomUtil.Next(10, 90);
            var lastValue3 = RandomUtil.Next(10, 90);
            for (int i = 0; i < count; i++)
            {
                var value1 = Math.Max(10, Math.Min(90, lastValue1 + RandomUtil.Next(-3, 4)));
                var value2 = Math.Max(10, Math.Min(90, lastValue2 + RandomUtil.Next(-3, 4)));
                var value3 = Math.Max(10, Math.Min(90, lastValue3 + RandomUtil.Next(-3, 4)));
                itemsSource.Add(new
                {
                    Label = i % labelDelta == 0 ? $"Data {i + 1}" : null,
                    Value1 = value1,
                    Value2 = value2,
                    Value3 = value3
                });
                lastValue1 = value1;
                lastValue2 = value2;
                lastValue3 = value3;
            }

            if (IsLoaded)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }
            chart.ItemsSource = itemsSource;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            if (_stopwatch != null)
            {
                _stopwatch.Stop();
                RunTime.Text = $"Render Time: {_stopwatch.ElapsedMilliseconds}ms";
            }
        }

    }
}