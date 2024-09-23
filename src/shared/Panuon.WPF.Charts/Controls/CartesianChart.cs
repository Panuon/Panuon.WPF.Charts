using Panuon.WPF.Chart;
using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Series))]
    public class CartesianChart
        : ChartBase
    {
        #region Fields
        private GridLinesPanel _gridLinesPanel;
        private Decorator _xAxisDecorator;
        private Decorator _yAxisDecorator;

        private CartesianChartContextImpl _chartContext;
        #endregion

        #region Ctor
        public CartesianChart()
        {
            Series = new SeriesCollection<CartesianSeriesBase>();

            _gridLinesPanel = new GridLinesPanel(this);
            _children.Insert(0, _gridLinesPanel);

            _xAxisDecorator = new Decorator();
            _children.Insert(1, _xAxisDecorator);

            _yAxisDecorator = new Decorator();
            _children.Insert(2, _yAxisDecorator);

            _seriesPanel = new SeriesPanel(this);
            _children.Insert(3, _seriesPanel);

            XAxis = new XAxis();
            YAxis = new YAxis();
        }
        #endregion

        #region Properties

        #region XAxis
        public XAxis XAxis
        {
            get { return (XAxis)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(XAxis), typeof(CartesianChart), new PropertyMetadata(OnXAxisChanged));
        #endregion

        #region YAxis
        public YAxis YAxis
        {
            get { return (YAxis)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(YAxis), typeof(CartesianChart), new PropertyMetadata(OnYAxisChanged));
        #endregion

        #region SwapXYAxes
        public bool SwapXYAxes
        {
            get { return (bool)GetValue(SwapXYAxesProperty); }
            set { SetValue(SwapXYAxesProperty, value); }
        }

        public static readonly DependencyProperty SwapXYAxesProperty =
            DependencyProperty.Register("SwapXYAxes", typeof(bool), typeof(CartesianChart), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Series
        public SeriesCollection<CartesianSeriesBase> Series
        {
            get { return (SeriesCollection<CartesianSeriesBase>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection<CartesianSeriesBase>), typeof(CartesianChart), new PropertyMetadata(null));
        #endregion

        #region GridLinesVisibility
        public CartesianChartGridLinesVisibility GridLinesVisibility
        {
            get { return (CartesianChartGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            GridLinesPanel.GridLinesVisibilityProperty.AddOwner(typeof(CartesianChart), new FrameworkPropertyMetadata(CartesianChartGridLinesVisibility.Both,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region GridLinesBrush
        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLinesBrushProperty =
            GridLinesPanel.GridLinesBrushProperty.AddOwner(typeof(CartesianChart), new FrameworkPropertyMetadata(Brushes.LightGray,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region GridLinesThickness
        public double GridLinesThickness
        {
            get { return (double)GetValue(GridLinesThicknessProperty); }
            set { SetValue(GridLinesThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLinesThicknessProperty =
            GridLinesPanel.GridLinesThicknessProperty.AddOwner(typeof(CartesianChart), new FrameworkPropertyMetadata(1d,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #endregion

        #region Internal Properties

        internal List<CartesianCoordinateImpl> Coordinates { get; private set; }


        internal double ActualMinValue
        {
            get
            {
                return YAxis?.MinValue ?? _measuredMinValue;
            }
        }
        private double _measuredMinValue;

        internal double ActualMaxValue
        {
            get
            {
                return YAxis?.MaxValue ?? _measuredMaxValue;
            }
        }
        private double _measuredMaxValue;
        #endregion

        #region Overrides
        public override IEnumerable<SeriesBase> GetSeries() => Series;

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            #region Measure Coordinates
            var coordinates = new List<CartesianCoordinateImpl>();
            if (ItemsSource != null)
            {
                var index = 0;
                foreach (var item in ItemsSource)
                {
                    var loopItem = item;
                    var itemType = loopItem.GetType();
                    string label = null;
                    if (!string.IsNullOrEmpty(LabelMemberPath))
                    {
                        var labelProperty = itemType.GetProperty(LabelMemberPath);
                        if (labelProperty == null)
                        {
                            throw new System.InvalidOperationException($"Property {LabelMemberPath} does not exists.");
                        }

                        var labelValue = labelProperty.GetValue(loopItem);
                        label = labelValue is string
                            ? (string)labelValue
                            : labelValue.ToString();
                    }

                    var values = new Dictionary<IChartArgument, double>();
                    foreach (CartesianSeriesBase series in GetSeries())
                    {
                        if (series is IChartValueProvider valueProvider)
                        {
                            var value = GetValueFromValueProvider(valueProvider, loopItem);
                            values.Add(valueProvider, value);
                        }
                        if (series is CartesianSegmentsSeriesBase cartesianSeries)
                        {
                            var valuesMemberPath = cartesianSeries.ValuesMemberPath;
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
                            var cartesianSegments = cartesianSeries.GetSegments()
                                .ToList();
                            for (int i = 0; i < cartesianSegments.Count; i++)
                            {
                                var segment = cartesianSegments[i] as ValueProviderSegmentBase;
                                var value = GetValueFromValueProvider(segment, loopItem, i);
                                values.Add(segment, value);
                            }
                        }
                    }

                    coordinates.Add(new CartesianCoordinateImpl()
                    {
                        Label = label,
                        Values = values,
                        Index = index
                    });
                    index++;
                }
            }

            Coordinates = coordinates;
            if (coordinates.Any())
            {
                CheckMinMaxValue(coordinates.SelectMany(x => x.Values.Values).Min(),
                    coordinates.SelectMany(x => x.Values.Values).Max(),
                    out int minValue,
                    out int maxValue);

                _measuredMinValue = minValue;
                _measuredMaxValue = maxValue;
            }
            else
            {
                _measuredMinValue = 0;
                _measuredMaxValue = 10;
            }
            #endregion

            var size = base.MeasureOverride(availableSize);

            XAxis?.Measure(availableSize);
            YAxis?.Measure(availableSize);
            _gridLinesPanel.Measure(availableSize);

            return size;
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderWidth = finalSize.Width - Padding.Left - Padding.Right;
            var renderHeight = finalSize.Height - Padding.Top - Padding.Bottom;

            var xAxisHeight = XAxis?.DesiredSize.Height ?? 0;
            var yAxisWidth = YAxis?.DesiredSize.Width ?? 0;

            var delta = SwapXYAxes
                ? (Math.Max(0, renderHeight - xAxisHeight)) / Coordinates.Count
                : (Math.Max(0, renderWidth - yAxisWidth)) / Coordinates.Count;
            for (int i = 0; i < Coordinates.Count; i++)
            {
                var coordinate = Coordinates[i];
                coordinate.Offset = (i + 0.5) * delta;
            }

            _gridLinesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, Math.Max(0, renderWidth - yAxisWidth), Math.Max(0, renderHeight - xAxisHeight)));
            _seriesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, Math.Max(0, renderWidth - yAxisWidth), Math.Max(0, renderHeight - xAxisHeight)));
            _layersPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, Math.Max(0, renderWidth - yAxisWidth), Math.Max(0, renderHeight - xAxisHeight)));
            XAxis?.Arrange(new Rect(Padding.Left + yAxisWidth, Math.Max(0, renderHeight - xAxisHeight), Math.Max(0, renderWidth - yAxisWidth), xAxisHeight));
            YAxis?.Arrange(new Rect(Padding.Left, Padding.Top, yAxisWidth, Math.Max(0, renderHeight - xAxisHeight)));

            _gridLinesPanel.InvalidateVisual();

            return finalSize;
        }
        #endregion

        #region GetCanvasContext
        internal override IChartContext GetCanvasContext()
        {
            if (_chartContext == null)
            {
                _chartContext = new CartesianChartContextImpl(this);
            }

            return _chartContext;
        }
        #endregion

        #endregion

        #region Event Handlers
        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            if(e.OldValue != null)
            {
                chart._xAxisDecorator.Child = null;
            }
            if (e.NewValue is XAxis xAxis)
            {
                xAxis.OnAttached(chart);
                chart._xAxisDecorator.Child = xAxis;
            }
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            if (e.OldValue != null)
            {
                chart._yAxisDecorator.Child = null;
            }
            if (e.NewValue is YAxis yAxis)
            {
                yAxis.OnAttached(chart);
                chart._yAxisDecorator.Child = yAxis;
            }
        }
        #endregion


        #region Functions
        private void CheckMinMaxValue(double minValue,
            double maxValue,
            out int resultMin,
            out int resultMax)
        {
            var min = (int)Math.Floor(minValue);
            var max = (int)Math.Ceiling(maxValue);

            var digit = Math.Max(1, max.ToString().Length - 1);
            var baseValue = Math.Pow(10d, digit);

            resultMin = (int)Math.Floor(min / baseValue) * (int)baseValue;
            resultMax = (int)Math.Ceiling(max / baseValue) * (int)baseValue;
        }

        #endregion
    }
}
