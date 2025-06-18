using Panuon.WPF.Charts;
using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Series))]
    public class RadialChart
        : ChartBase
    {
        #region Fields
        private RadialChartContextImpl _chartContext;
        #endregion

        #region Ctor
        public RadialChart()
        {
            _seriesPanel = new SeriesPanel(this);

            SetCurrentValue(SeriesProperty, new SeriesCollection<RadialSeriesBase>());

            _children.Insert(0, _seriesPanel);
        }
        #endregion

        #region Properties

        #region Series
        public SeriesCollection<RadialSeriesBase> Series
        {
            get { return (SeriesCollection<RadialSeriesBase>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection<RadialSeriesBase>), typeof(RadialChart), new PropertyMetadata(null, OnSeriesChanged));
        #endregion

        #region Spacing
        public double LabelSpacing
        {
            get { return (double)GetValue(LabelSpacingProperty); }
            set { SetValue(LabelSpacingProperty, value); }
        }

        public static readonly DependencyProperty LabelSpacingProperty =
            DependencyProperty.Register("LabelSpacing", typeof(double), typeof(RadialChart), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Internal Properties
        internal List<RadialCoordinateImpl> Coordinates { get; private set; }
        #endregion

        #region Internal Methods
        internal override void OnClearValues()
        {
            Coordinates = null;
        }
        #endregion

        #region Overrides
        public override IEnumerable<SeriesBase> GetSeries() => Series;

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            var coordinates = new List<RadialCoordinateImpl>();
            if (ItemsSource != null)
            {
                var index = 0;
                // 获取可枚举的集合
                IEnumerable items;
                if (ItemsSource is DataTable dataTable)
                {
                    items = dataTable.Rows;
                }
                else if (ItemsSource is IEnumerable enumerable)
                {
                    items = enumerable;
                }
                else
                {
                    throw new InvalidOperationException("ItemsSource must be IEnumerable or DataTable.");
                }

                foreach (var item in items)
                {
                    var loopItem = item;
                    string label = null;

                    if (!string.IsNullOrEmpty(LabelMemberPath))
                    {
                        object labelValue;
                        if (loopItem is DataRow dataRow)
                        {
                            // 对于DataRow，使用索引器获取值
                            labelValue = dataRow[LabelMemberPath];
                        }
                        else
                        {
                            // 对于其他类型，使用反射
                            var itemType = loopItem.GetType();
                            var labelProperty = itemType.GetProperty(LabelMemberPath);
                            if (labelProperty == null)
                            {
                                throw new InvalidOperationException($"Property {LabelMemberPath} does not exists.");
                            }
                            labelValue = labelProperty.GetValue(loopItem);
                        }

                        label = labelValue is string
                            ? (string)labelValue
                            : labelValue.ToString();
                    }

                    var values = new Dictionary<IChartArgument, decimal?>();

                    foreach (RadialSeriesBase series in GetSeries())
                    {
                        if (series is IChartValueProvider valueProvider)
                        {
                            var value = GetValueFromValueProvider(valueProvider, loopItem);
                            values.Add(valueProvider, value);
                        }
                        if (series is RadialSegmentsSeriesBase radialSeries)
                        {
                            var valuesMemberPath = radialSeries.ValuesMemberPath;
                            if (!string.IsNullOrEmpty(valuesMemberPath))
                            {
                                object valuesCollection;
                                if (loopItem is DataRow dataRow)
                                {
                                    valuesCollection = dataRow[valuesMemberPath];
                                    if (!(valuesCollection is IEnumerable))
                                    {
                                        throw new InvalidOperationException($"Value at '{valuesMemberPath}' must be of a collection type.");
                                    }
                                }
                                else
                                {
                                    valuesCollection = PropertyAccessor.GetValue(loopItem, valuesMemberPath);
                                }
                                loopItem = valuesCollection;
                            }

                            var radialSegments = radialSeries.GetSegments().ToList();
                            for (int i = 0; i < radialSegments.Count; i++)
                            {
                                var segment = radialSegments[i] as ValueProviderSegmentBase;
                                var value = GetValueFromValueProvider(segment, loopItem, i);
                                values.Add(segment, value);
                            }
                        }
                    }

                    coordinates.Add(new RadialCoordinateImpl(this)
                    {
                        Label = label,
                        Values = values,
                        Index = index,
                    });
                    index++;
                }

                var totalValue = (decimal)coordinates.SelectMany(c => c.Values.Select(v => v.Value)).Where(v => v != null).Sum();
                if (totalValue > 0)
                {
                    var angleDelta = (double)(360m / totalValue);
                    var startAngle = 0d;
                    foreach (var coordinate in coordinates)
                    {
                        var argumentAngle = 0d;
                        foreach (var value in coordinate.Values)
                        {
                            var angle = value.Value == null ? 0 : (double)value.Value * angleDelta;
                            argumentAngle += angle;
                        }
                        coordinate.StartAngle = startAngle;
                        coordinate.Angle = argumentAngle;
                        startAngle += argumentAngle;
                    }
                }
            }

            Coordinates = coordinates;

            return size;
        }


        #region GetCanvasContext
        internal override IChartContext GetCanvasContext()
        {
            if (_chartContext == null)
            {
                _chartContext = new RadialChartContextImpl(this);
            }

            return _chartContext;
        }
        #endregion

        #endregion

        #region Event Handlers
        private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (RadialChart)d;
            foreach (SeriesBase series in chart.GetSeries())
            {
                series.OnAttached(chart);
            }
            if (e.OldValue is SeriesCollection<RadialSeriesBase> oldSeries)
            {
                oldSeries.CollectionChanged -= chart.Series_CollectionChanged;
            }
            if (e.NewValue is SeriesCollection<RadialSeriesBase> newSeries)
            {
                newSeries.CollectionChanged += chart.Series_CollectionChanged;
            }
            chart.Rerender();
        }

        private void Series_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SeriesBase series in e.NewItems)
                {
                    series.OnAttached(this);
                }
            }
            Rerender();
        }

        #endregion
    }
}