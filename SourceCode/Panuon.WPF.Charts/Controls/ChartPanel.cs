using Panuon.WPF.Charts.Controls.Internals;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Series))]
    public class ChartPanel
        : FrameworkElement
    {
        #region Fields

        private readonly UIElementCollection _children;

        internal SeriesPresenter _seriesPresenter;
        internal XAxisPresenter _xAxisPresenter;
        internal YAxisPresenter _yAxisPresenter;
        #endregion

        #region Ctor
        public ChartPanel()
        {
            _children = new UIElementCollection(this, this);

            XAxis = new XAxis();

            _xAxisPresenter = new XAxisPresenter(this);
            _children.Add(_xAxisPresenter);

            _yAxisPresenter = new YAxisPresenter(this);
            _children.Add(_yAxisPresenter);

            _seriesPresenter = new SeriesPresenter(this);
            _children.Add(_seriesPresenter);

            Series = new SeriesCollection();
        }
        #endregion

        #region Properties

        #region ItemsSource
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ChartPanel));
        #endregion

        #region Padding
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(ChartPanel), new FrameworkPropertyMetadata(new Thickness(),
                FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region XAxis
        public XAxis XAxis
        {
            get { return (XAxis)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(XAxis), typeof(ChartPanel), new PropertyMetadata(null));
        #endregion

        #region YAxis
        public YAxis YAxis
        {
            get { return (YAxis)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(YAxis), typeof(ChartPanel), new PropertyMetadata(null));
        #endregion

        #region Series
        public SeriesCollection Series
        {
            get { return (SeriesCollection)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection), typeof(ChartPanel), new PropertyMetadata(null));
        #endregion

        #region GridLinesVisibility
        public ChartPanelGridLinesVisibility GridLinesVisibility
        {
            get { return (ChartPanelGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(ChartPanelGridLinesVisibility), typeof(ChartPanel), new PropertyMetadata(ChartPanelGridLinesVisibility.Both));
        #endregion

        #region GridLinesBrush
        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(ChartPanel), new PropertyMetadata(Brushes.LightGray));
        #endregion

        #region GridLinesThickness
        public double GridLinesThickness
        {
            get { return (double)GetValue(GridLinesThicknessProperty); }
            set { SetValue(GridLinesThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register("GridLinesThickness", typeof(double), typeof(ChartPanel), new PropertyMetadata(1d));
        #endregion

        #endregion

        #region Overrides

        #region VisualChildrenCount
        protected override int VisualChildrenCount => _children.Count;
        #endregion

        #region GetVisualChild
        protected override Visual GetVisualChild(int index) => _children[index];
        #endregion

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            _xAxisPresenter.Measure(availableSize);
            _yAxisPresenter.Measure(availableSize);

            _seriesPresenter.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderWidth = finalSize.Width - Padding.Left - Padding.Right;
            var renderHeight = finalSize.Height - Padding.Top - Padding.Bottom;

            var xAxisHeight = _xAxisPresenter.DesiredSize.Height;
            var yAxisWidth = _yAxisPresenter.DesiredSize.Width;

            _xAxisPresenter.Arrange(new Rect(Padding.Left, renderHeight - xAxisHeight, renderWidth, xAxisHeight));

            _yAxisPresenter.Arrange(new Rect(Padding.Left, Padding.Top, yAxisWidth, renderHeight));

            _seriesPresenter.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, renderWidth - yAxisWidth, renderHeight - xAxisHeight));

            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #endregion

        #region Event Handlers
       
        #endregion

        #region Functions

        #endregion
    }
}
