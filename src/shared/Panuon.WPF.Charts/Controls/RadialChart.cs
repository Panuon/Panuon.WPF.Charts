using Panuon.WPF.Chart;
using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System.Collections;
using System;
using System.Collections.Generic;
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
            Series = new SeriesCollection<RadialSeriesBase>();

            _seriesPanel = new SeriesPanel(this);
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
            DependencyProperty.Register("Series", typeof(SeriesCollection<RadialSeriesBase>), typeof(RadialChart), new PropertyMetadata(null));
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

        #region Overrides
        public override IEnumerable<SeriesBase> GetSeries() => Series;

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            var coordinates = new List<RadialCoordinateImpl>();
            if (ItemsSource != null)
            {
                var index = 0;
                foreach (var item in ItemsSource)
                {
                    var loopItem = item;
                    var itemType = loopItem.GetType();
                    string title = null;
                    if (!string.IsNullOrEmpty(TitleMemberPath))
                    {
                        var titleProperty = itemType.GetProperty(TitleMemberPath);
                        if (titleProperty == null)
                        {
                            throw new InvalidOperationException($"Property {TitleMemberPath} does not exists.");
                        }

                        var titleValue = titleProperty.GetValue(loopItem);
                        title = titleValue is string
                            ? (string)titleValue
                            : titleValue.ToString();
                    }

                    var values = new Dictionary<IChartArgument, double>();

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
                                var valuesProperty = itemType.GetProperty(valuesMemberPath);
                                if (valuesProperty == null)
                                {
                                    throw new InvalidOperationException($"Property named '{valuesMemberPath}' does not exists in {loopItem}.");
                                }
                                if (!typeof(IEnumerable).IsAssignableFrom(valuesProperty.PropertyType))
                                {
                                    throw new InvalidOperationException($"Property named '{valuesMemberPath}' in {loopItem} must be of a collection type.");
                                }
                                loopItem = valuesProperty.GetValue(loopItem);
                            }
                            var radialSegments = radialSeries.GetSegments()
                                .ToList();
                            for (int i = 0; i < radialSegments.Count; i++)
                            {
                                var segment = radialSegments[i] as ValueProviderSegmentBase;
                                var value = GetValueFromValueProvider(segment, loopItem, i);
                                values.Add(segment, value);
                            }
                        }
                    }

                    coordinates.Add(new RadialCoordinateImpl()
                    {
                        Title = title,
                        Values = values,
                        Index = index,
                        Angles = new Dictionary<IChartArgument, (double, double)>(),
                    });
                    index++;
                }

                var totalValue = coordinates.SelectMany(c => c.Values.Select(v => v.Value)).Sum();
                if (totalValue > 0)
                {
                    var angleDelta = 360d / totalValue;
                    var startAngle = 0d;
                    foreach (var coordinate in coordinates)
                    {
                        var argumentAngle = 0d;
                        foreach (var value in coordinate.Values)
                        {
                            var angle = value.Value * angleDelta;
                            coordinate.Angles.Add(value.Key, (startAngle + argumentAngle, angle));
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
    }
}