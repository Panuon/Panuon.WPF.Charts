using Panuon.WPF.Charts.Controls.Internals;
using System.Collections.Generic;
using System.Windows;
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
        internal XAxisPresenter _xAxisPresenter;
        internal YAxisPresenter _yAxisPresenter;
        #endregion

        #region Ctor
        public CartesianChart()
        {
            Series = new SeriesCollection<CartesianSeriesBase>();

            XAxis = new XAxis();
            YAxis = new YAxis();

            _seriesPanel = new SeriesPanel(this);
            _children.Add(_seriesPanel);

            _gridLinesPanel = new GridLinesPanel(this);
            _children.Insert(0, _gridLinesPanel);

            _xAxisPresenter = new XAxisPresenter(this);
            _children.Add(_xAxisPresenter);

            _yAxisPresenter = new YAxisPresenter(this);
            _children.Add(_yAxisPresenter);
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
            DependencyProperty.Register("XAxis", typeof(XAxis), typeof(CartesianChart), new PropertyMetadata(null));
        #endregion

        #region YAxis
        public YAxis YAxis
        {
            get { return (YAxis)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(YAxis), typeof(CartesianChart), new PropertyMetadata(null));
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

            _xAxisPresenter.Measure(availableSize);
            _yAxisPresenter.Measure(availableSize);
            _gridLinesPanel.Measure(availableSize);

            return size;
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderWidth = finalSize.Width - Padding.Left - Padding.Right;
            var renderHeight = finalSize.Height - Padding.Top - Padding.Bottom;

            var xAxisHeight = _xAxisPresenter.DesiredSize.Height;
            var yAxisWidth = _yAxisPresenter.DesiredSize.Width;

            var deltaX = (renderWidth - yAxisWidth) / Coordinates.Count;

            for (int i = 0; i < Coordinates.Count; i++)
            {
                var coordinate = Coordinates[i];
                coordinate.Offset = (i + 0.5) * deltaX;
            }

            _gridLinesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, renderWidth - yAxisWidth, renderHeight - xAxisHeight));
            _seriesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, renderWidth - yAxisWidth, renderHeight - xAxisHeight));
            _layersPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, renderWidth - yAxisWidth, renderHeight - xAxisHeight));
            _xAxisPresenter.Arrange(new Rect(Padding.Left + yAxisWidth, renderHeight - xAxisHeight, renderWidth - yAxisWidth, xAxisHeight));
            _yAxisPresenter.Arrange(new Rect(Padding.Left, Padding.Top, yAxisWidth, renderHeight - xAxisHeight));

            _gridLinesPanel.InvalidateVisual();
            _seriesPanel.InvalidateVisual();
            _layersPanel.InvalidateVisual();
            _xAxisPresenter.InvalidateVisual();
            _yAxisPresenter.InvalidateVisual();

            return finalSize;
        }
        #endregion

        #endregion
    }
}
