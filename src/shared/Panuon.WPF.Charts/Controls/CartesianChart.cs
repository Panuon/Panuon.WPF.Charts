using Panuon.WPF.Charts;
using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using Panuon.WPF.Charts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
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

        private CartesianChartContextImpl _chartContext;
        #endregion

        #region Ctor
        public CartesianChart()
        {
            _gridLinesPanel = new GridLinesPanel(this);
            _seriesPanel = new SeriesPanel(this);

            SetCurrentValue(SeriesProperty, new SeriesCollection<CartesianSeriesBase>());
            SetCurrentValue(XAxisProperty, new XAxis());
            SetCurrentValue(YAxisProperty, new YAxis());

            _children.Insert(0, _gridLinesPanel);
            _children.Insert(1, _seriesPanel);
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
            DependencyProperty.Register("SwapXYAxes", typeof(bool), typeof(CartesianChart), new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
                OnAffectsSchemaPropertyChanged
            ));
        #endregion

        #region Series
        public SeriesCollection<CartesianSeriesBase> Series
        {
            get { return (SeriesCollection<CartesianSeriesBase>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection<CartesianSeriesBase>), typeof(CartesianChart), new PropertyMetadata(null, OnSeriesChanged));
        #endregion

        #region GridLinesVisibility
        public CartesianChartGridLinesVisibility GridLinesVisibility
        {
            get { return (CartesianChartGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(CartesianChartGridLinesVisibility), typeof(CartesianChart),
                new FrameworkPropertyMetadata(CartesianChartGridLinesVisibility.Both, OnGridLinesPropertyChanged));
        #endregion

        #region GridLinesBrush
        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(CartesianChart),
                new FrameworkPropertyMetadata(Brushes.LightGray, OnGridLinesPropertyChanged));
        #endregion

        #region GridLinesThickness
        public double GridLinesThickness
        {
            get { return (double)GetValue(GridLinesThicknessProperty); }
            set { SetValue(GridLinesThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register("GridLinesThickness", typeof(double), typeof(CartesianChart),
                new FrameworkPropertyMetadata(1d, OnGridLinesPropertyChanged));
        #endregion

        #region GridLinesDashArray
        public DoubleCollection GridLinesDashArray
        {
            get { return (DoubleCollection)GetValue(GridLinesDashArrayProperty); }
            set { SetValue(GridLinesDashArrayProperty, value); }
        }

        public static readonly DependencyProperty GridLinesDashArrayProperty =
            DependencyProperty.Register("GridLinesDashArray", typeof(DoubleCollection), typeof(CartesianChart),
                new FrameworkPropertyMetadata(null, OnGridLinesPropertyChanged));
        #endregion

        #region CurrentOffset
        public double CurrentOffset
        {
            get { return (double)GetValue(CurrentOffsetProperty); }
            set { SetValue(CurrentOffsetProperty, value); }
        }

        public static readonly DependencyProperty CurrentOffsetProperty =
            DependencyProperty.Register("CurrentOffset", typeof(double), typeof(CartesianChart), new PropertyMetadata(OnCurrentOffsetChanged));
        #endregion

        #endregion

        #region Events 
        public event DrawingHorizontalGridLineRoutedEventHandler DrawingHorizontalGridLine
        {
            add { AddHandler(DrawingHorizontalGridLineEvent, value); }
            remove { RemoveHandler(DrawingHorizontalGridLineEvent, value); }
        }

        public static readonly RoutedEvent DrawingHorizontalGridLineEvent =
            EventManager.RegisterRoutedEvent("DrawingHorizontalGridLine", RoutingStrategy.Bubble, typeof(DrawingHorizontalGridLineRoutedEventHandler), typeof(CartesianChart));
        #endregion

        #region Internal Properties

        internal List<CartesianCoordinateImpl> Coordinates { get; private set; }


        internal double ActualMinValue
        {
            get
            {
                if (YAxis?.MinValue != null)
                {
                    return (double)YAxis.MinValue;
                }
                else if (YAxis.Labels != null && YAxis.Labels.Any())
                {
                    return YAxis.Labels.Min(l => l.Value);
                }
                else
                {
                    return _measuredMinValue;
                }
            }
        }
        private double _measuredMinValue;

        internal double ActualMaxValue
        {
            get
            {
                if (YAxis?.MaxValue != null)
                {
                    return (double)YAxis.MaxValue;
                }
                else if (YAxis.Labels != null && YAxis.Labels.Any())
                {
                    return YAxis.Labels.Max(l => l.Value);
                }
                else
                {
                    return _measuredMaxValue;
                }
            }
        }
        private double _measuredMaxValue;

        internal double CanvasWidth { get; private set; }

        internal double CanvasHeight { get; private set; }

        internal double SliceWidth
        {
            get
            {
                var width = RenderSize.Width;
                if (!SwapXYAxes
                    && YAxis != null)
                {
                    width -= YAxis.RenderSize.Width;
                }
                else if(SwapXYAxes
                    && XAxis != null)
                {
                    width -= XAxis.RenderSize.Width;
                }
                return width;
            }
        }

        internal double SliceHeight
        {
            get
            {
                var height = RenderSize.Height;
                if (!SwapXYAxes
                    && XAxis != null)
                {
                    height -= XAxis.RenderSize.Height;
                }
                else if (SwapXYAxes
                    && YAxis != null)
                {
                    height -= YAxis.RenderSize.Height;
                }
                return height;
            }
        }

        public double ScrollableWidth
        {
            get
            {
                return Math.Max(0, CanvasWidth - SliceWidth);
            }
        }

        public double ScrollableHeight
        {
            get
            {
                return Math.Max(0, CanvasHeight - SliceHeight);
            }
        }
        #endregion

        #region Internal Methods
        internal override void OnClearValues()
        {
            Coordinates = null;
        }

        internal void RaiseDrawingHorizontalGridLine(
            double value,
            ref Brush stroke,
            ref double? strokeThickness,
            ref DoubleCollection dashArray)
        {
            var eventArgs = new DrawingHorizontalGridLineRoutedEventArgs(
                DrawingHorizontalGridLineEvent, 
                value, 
                stroke, 
                strokeThickness,
                dashArray);
            RaiseEvent(eventArgs);
            stroke = eventArgs.Stroke;
            strokeThickness = eventArgs.StrokeThickness;
        }
        #endregion

        #region Overrides
        public override IEnumerable<SeriesBase> GetSeries() => Series ?? Enumerable.Empty<SeriesBase>();

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
                    string label = null;
                    if (!string.IsNullOrEmpty(LabelMemberPath))
                    {
                        var labelValue = PropertyAccessor.GetValue(loopItem, LabelMemberPath);
                        label = labelValue is string
                            ? (string)labelValue
                            : labelValue?.ToString();
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
                                loopItem = PropertyAccessor.GetValue(loopItem, valuesMemberPath);
                                if (!typeof(IEnumerable).IsAssignableFrom(loopItem.GetType()))
                                {
                                    throw new InvalidOperationException($"Property named '{valuesMemberPath}' in {loopItem} must be of a collection type.");
                                }
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

                    coordinates.Add(new CartesianCoordinateImpl(this)
                    {
                        Label = label,
                        Values = values,
                        Index = index
                    });
                    index++;
                }
            }

            Coordinates = coordinates;
            var coordinateValues = coordinates.SelectMany(x => x.Values.Values);
            if (coordinates.Any()
                && coordinateValues.Any())
            {
                CheckMinMaxValue(coordinateValues.Min(),
                    coordinateValues.Max(),
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

            XAxis?.Measure(availableSize);
            YAxis?.Measure(availableSize);
            _gridLinesPanel.Measure(availableSize);

            var size = base.MeasureOverride(availableSize);

            return size;
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderWidth = finalSize.Width - Padding.Left - Padding.Right;
            var renderHeight = finalSize.Height - Padding.Top - Padding.Bottom;

            var hAxisHeight = SwapXYAxes
                ? YAxis?.DesiredSize.Height ?? 0
                : XAxis?.DesiredSize.Height ?? 0;
            var vAxisWidth = SwapXYAxes
                ? XAxis?.DesiredSize.Width ?? 0
                : YAxis?.DesiredSize.Width ?? 0;

            var coordinateDelta = XAxis.CoordinateMinWidth.IsAuto
                ? (SwapXYAxes
                    ? (Math.Max(0, renderHeight - hAxisHeight)) / Coordinates.Count
                    : (Math.Max(0, renderWidth - vAxisWidth)) / Coordinates.Count)
                : GridLengthUtil.GetActualValue(XAxis.CoordinateMinWidth, renderWidth - vAxisWidth);

            for (int i = 0; i < Coordinates.Count; i++)
            {
                var coordinate = Coordinates[i];
                coordinate.Offset = (i + 0.5) * coordinateDelta;
            }

            _gridLinesPanel.Arrange(new Rect(
                Padding.Left + vAxisWidth, 
                Padding.Top, 
                Math.Max(0, renderWidth - vAxisWidth), 
                Math.Max(0, renderHeight - hAxisHeight)
            ));
            Debug.WriteLine(renderWidth - vAxisWidth);
            _seriesPanel.Arrange(new Rect(Padding.Left + vAxisWidth, Padding.Top, Math.Max(0, renderWidth - vAxisWidth), Math.Max(0, renderHeight - hAxisHeight)));
            _layersPanel.Arrange(new Rect(Padding.Left + vAxisWidth, Padding.Top, Math.Max(0, renderWidth - vAxisWidth), Math.Max(0, renderHeight - hAxisHeight)));
            _legendPanel.Arrange(new Rect(Padding.Left + vAxisWidth, Padding.Top, Math.Max(0, renderWidth - vAxisWidth), Math.Max(0, renderHeight - hAxisHeight)));

            var hAxis = SwapXYAxes ? (AxisBase)YAxis : (AxisBase)XAxis;
            var vAxis = SwapXYAxes ? (AxisBase)XAxis : (AxisBase)YAxis;

            if (!SwapXYAxes)
            {
                var xAxisWidth = XAxis.CoordinateMinWidth.IsAuto
                    ? Math.Max(0, renderWidth - vAxisWidth)
                    : coordinateDelta * Coordinates.Count;

                CanvasWidth = xAxisWidth;
                CanvasHeight = _seriesPanel.RenderSize.Height;

                XAxis?.Arrange(new Rect(
                    Padding.Left + vAxisWidth - XAxis.StrokeThickness / 2,
                    Math.Max(0, renderHeight - hAxisHeight),
                    Math.Max(0, renderWidth - vAxisWidth),
                    hAxisHeight)
                );
                YAxis?.Arrange(new Rect(
                    Padding.Left,
                    Padding.Top,
                    vAxisWidth,
                    Math.Max(0, renderHeight - hAxisHeight))
                );
            }
            else
            {
                var xAxisHeight = XAxis.CoordinateMinWidth.IsAuto
                    ? Math.Max(0, renderHeight - hAxisHeight)
                    : coordinateDelta * Coordinates.Count;

                CanvasWidth = _seriesPanel.RenderSize.Width;
                CanvasHeight = xAxisHeight;

                YAxis?.Arrange(new Rect(
                    Padding.Left + vAxisWidth,
                    Math.Max(0, renderHeight - hAxisHeight),
                    Math.Max(0, renderWidth - vAxisWidth),
                    hAxisHeight)
                );
                XAxis?.Arrange(new Rect(
                    Padding.Left,
                    Padding.Top,
                    vAxisWidth,
                    Math.Max(0, renderHeight - hAxisHeight))
                );
            }

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


        private double _offset = 0;
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            e.Handled = true;
            base.OnPreviewMouseWheel(e);

            _offset = Math.Max(0, Math.Min(CanvasWidth - SliceWidth, _offset - e.Delta));
            SetCurrentValue(CurrentOffsetProperty, _offset);
        }

        #endregion

        #region Event Handlers

        private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            foreach (SeriesBase series in chart.GetSeries())
            {
                series.OnAttached(chart);
            }
            if (e.OldValue is SeriesCollection<CartesianSeriesBase> oldSeries)
            {
                oldSeries.CollectionChanged -= chart.Series_CollectionChanged;
            }
            if (e.NewValue is SeriesCollection<CartesianSeriesBase> newSeries)
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

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            if(e.OldValue is AxisBase oldAxis)
            {
                chart._children.Remove(oldAxis);
            }
            if (e.NewValue is AxisBase newAxis)
            {
                newAxis.OnAttached(chart);
                chart._children.Insert(1, newAxis);
            }
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            if (e.OldValue is AxisBase oldAxis)
            {
                chart._children.Remove(oldAxis);
            }
            if (e.NewValue is AxisBase newAxis)
            {
                newAxis.OnAttached(chart);
                newAxis.SetBinding(AxisBase.DataContextProperty, new Binding()
                {
                    Path = new PropertyPath(DataContextProperty),
                    Source = chart
                });
                chart._children.Insert(2, newAxis);
            }
        }

        private static void OnCurrentOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            chart.OnCurrentOffsetChanged();
        }

        private static void OnGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (CartesianChart)d;
            chart._gridLinesPanel.InvalidateVisual();
        }
        #endregion

        #region Functions
        private void OnCurrentOffsetChanged()
        {
            foreach (var series in Series)
            {
                series.Offset = -CurrentOffset;
            }

            if (XAxis != null)
            {
                XAxis.Offset = Math.Min(0, Math.Max(-ScrollableWidth, -CurrentOffset));
            }
            if (YAxis != null)
            {
                YAxis.Offset = Math.Min(0, Math.Max(-ScrollableHeight, -CurrentOffset));
            }
        }

        private void CheckMinMaxValue(double minValue,
            double maxValue,
            out int resultMin,
            out int resultMax)
        {
            var min = (int)Math.Floor(minValue);
            var max = (int)Math.Ceiling(maxValue * 1.5) ;

            var digit = Math.Max(1, max.ToString().Length);
            var baseValue = Math.Pow(10d, digit - 1) / 2;

            resultMin = (int)Math.Floor(Math.Floor(min / baseValue) * baseValue);
            resultMax = (int)Math.Ceiling(Math.Ceiling(max / baseValue) * baseValue);
        }

        #endregion
    }

    

}
