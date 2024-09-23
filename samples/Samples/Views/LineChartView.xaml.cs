using Samples.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Samples.Views
{
    public partial class LineChartView
        : Grid
    {
        #region Ctor
        public LineChartView()
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

            if (quality < 100)
            {
                RunQuality.Text = $"To improve animation smoothness and rendering speed, the render quality has been reduced to {quality} (0~100).";
            }
            else
            {
                RunQuality.Text = null;
            }

            var itemsSource = new List<object>();

            var labelDelta = count / 5;
            var lastValue1 = RandomUtil.Next(10, 50);
            var lastValue2 = RandomUtil.Next(10, 50);
            var lastValue3 = RandomUtil.Next(10, 50);
            for (int i = 0; i < count; i++)
            {
                var value1 = Math.Max(10, Math.Min(100, lastValue1 + RandomUtil.Next(-3, 3)));
                var value2 = Math.Max(10, Math.Min(100, lastValue2 + RandomUtil.Next(-3, 3)));
                var value3 = Math.Max(10, Math.Min(100, lastValue3 + RandomUtil.Next(-3, 3)));
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
            chart.ItemsSource = itemsSource;
        }
    }
}