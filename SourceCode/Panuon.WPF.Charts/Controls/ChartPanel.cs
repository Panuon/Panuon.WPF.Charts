using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        internal SeriesPanel _seriesPanel;
        internal XAxisPresenter _xAxisPresenter;
        internal YAxisPresenter _yAxisPresenter;
        #endregion

        #region Ctor
        public ChartPanel()
        {
            _children = new UIElementCollection(this, this);

            Series = new SeriesCollection();

            XAxis = new XAxis();

            _xAxisPresenter = new XAxisPresenter(this);
            _children.Add(_xAxisPresenter);

            _yAxisPresenter = new YAxisPresenter(this);
            _children.Add(_yAxisPresenter);

            _seriesPanel = new SeriesPanel(this);
            _children.Add(_seriesPanel);

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

        #region TitleMemberPath
        public string TitleMemberPath
        {
            get { return (string)GetValue(TitleMemberPathProperty); }
            set { SetValue(TitleMemberPathProperty, value); }
        }

        public static readonly DependencyProperty TitleMemberPathProperty =
            DependencyProperty.Register("TitleMemberPath", typeof(string), typeof(ChartPanel));
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

        #region Internal Properties

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
            var coordinates = new List<CoordinateImpl>();
            if (ItemsSource != null)
            {
                var index = 0;
                foreach (var item in ItemsSource)
                {
                    if (string.IsNullOrEmpty(TitleMemberPath))
                    {
                        throw new System.NullReferenceException("Property TitleMemberPath of ChartPanel can not be null.");
                    }
                    var itemType = item.GetType();
                    var titleProperty = itemType.GetProperty(TitleMemberPath);
                    var titleValue = titleProperty.GetValue(item);
                    var title = titleValue is string
                        ? (string)titleValue
                        : titleValue.ToString();

                    var values = new Dictionary<SeriesBase, double>();
                    foreach (var series in Series)
                    {
                        if (string.IsNullOrEmpty(series.ValueMemberPath))
                        {
                            throw new NullReferenceException("Property ValueMemberPath of Series can not be null.");
                        }

                        var valueProperty = itemType.GetProperty(series.ValueMemberPath);
                        var valueValue = valueProperty.GetValue(item);
                        var value = Convert.ToDouble(valueValue);

                        values.Add(series, value);
                    }

                    coordinates.Add(new CoordinateImpl()
                    {
                        Title = title,
                        Values = values,
                        Index = index
                    });
                    index++;
                }
            }

            CheckMinMaxValue(coordinates.SelectMany(x => x.Values.Values).Min(),
                coordinates.SelectMany(x => x.Values.Values).Max(),
                out int minValue,
                out int maxValue);

            _xAxisPresenter.Coordinates = coordinates;
            _xAxisPresenter.Measure(availableSize);

            _yAxisPresenter.Coordinates = coordinates;
            _yAxisPresenter.MinValue = minValue;
            _yAxisPresenter.MaxValue = maxValue;
            _yAxisPresenter.Measure(availableSize);

            _seriesPanel.Coordinates = coordinates;
            _seriesPanel.MinValue = minValue;
            _seriesPanel.MaxValue = maxValue;
            _seriesPanel.Measure(availableSize);

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

            _xAxisPresenter.Arrange(new Rect(Padding.Left + yAxisWidth, renderHeight - xAxisHeight, renderWidth - yAxisWidth, xAxisHeight));

            _yAxisPresenter.Arrange(new Rect(Padding.Left, Padding.Top, yAxisWidth, renderHeight - xAxisHeight));

            _seriesPanel.Arrange(new Rect(Padding.Left + yAxisWidth, Padding.Top, renderWidth - yAxisWidth, renderHeight - xAxisHeight));

            _xAxisPresenter.InvalidateVisual();
            _yAxisPresenter.InvalidateVisual();
            _seriesPanel.InvalidateVisual();

            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #endregion

        #region Methods

        #region Internal Methods
        internal bool CanCreateDrawingContext()
        {
            return _seriesPanel.RenderSize.Width != 0
                && _seriesPanel.RenderSize.Height != 0;
        }

        internal IDrawingContext CreateDrawingContext(DrawingContext context)
        {
            var drawingContext = new WPFDrawingContextImpl(context,
                _seriesPanel.RenderSize.Width,
                _seriesPanel.RenderSize.Height,
                _seriesPanel.Coordinates.Count(),
                _seriesPanel.MinValue,
                _seriesPanel.MaxValue);

            return drawingContext;
        }
        #endregion

        #endregion

        #region Event Handlers

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
