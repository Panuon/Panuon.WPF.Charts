using Panuon.WPF.Charts.Controls.Internals;
using System;
using System.Collections.Generic;
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
        internal GridLinesPanel _gridLinesPanel;

        private Decorator _xAxisDecorator;

        private Decorator _yAxisDecorator;
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
        internal override double ActualMinValue
        {
            get
            {
                return YAxis?.MinValue ?? base.ActualMinValue;
            }
        }

        internal override double ActualMaxValue
        {
            get
            {
                return YAxis?.MaxValue ?? base.ActualMaxValue;
            }
        }
        #endregion

        #region Overrides

        public override IEnumerable<SeriesBase> GetSeries() => Series;

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
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

            var deltaX = (Math.Max(0, renderWidth - yAxisWidth)) / Coordinates.Count;

            for (int i = 0; i < Coordinates.Count; i++)
            {
                var coordinate = Coordinates[i];
                coordinate.Offset = (i + 0.5) * deltaX;
            }

            _gridLinesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, Math.Max(0, renderWidth - yAxisWidth), Math.Max(0, renderHeight - xAxisHeight)));
            _seriesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, Math.Max(0, renderWidth - yAxisWidth), Math.Max(0, renderHeight - xAxisHeight)));
            _layersPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, Math.Max(0, renderWidth - yAxisWidth), Math.Max(0, renderHeight - xAxisHeight)));
            XAxis?.Arrange(new Rect(Padding.Left + yAxisWidth, Math.Max(0, renderHeight - xAxisHeight), Math.Max(0, renderWidth - yAxisWidth), xAxisHeight));
            YAxis?.Arrange(new Rect(Padding.Left, Padding.Top, yAxisWidth, Math.Max(0, renderHeight - xAxisHeight)));

            return finalSize;
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
    }
}
