using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Series))]
    public class ChartPanel
        : Control
    {
        #region Fields

        private readonly UIElementCollection _children;

        internal GridLinesPanel _gridLinesPanel;
        internal SeriesPanel _seriesPanel;
        internal LayersPanel _layersPanel;
        internal XAxisPresenter _xAxisPresenter;
        internal YAxisPresenter _yAxisPresenter;

        private ChartContextImpl _chartContext;
        private LayerContextImpl _layerContext;
        #endregion

        #region Ctor
        public ChartPanel()
        {
            _children = new UIElementCollection(this, this);

            Series = new SeriesCollection();
            Layers = new LayerCollection();

            XAxis = new XAxis();
            YAxis = new YAxis();

            _gridLinesPanel = new GridLinesPanel(this);
            _children.Add(_gridLinesPanel);

            _seriesPanel = new SeriesPanel(this);
            _children.Add(_seriesPanel);

            _layersPanel = new LayersPanel(this);
            _children.Add(_layersPanel);
            
            _xAxisPresenter = new XAxisPresenter(this);
            _children.Add(_xAxisPresenter);

            _yAxisPresenter = new YAxisPresenter(this);
            _children.Add(_yAxisPresenter);

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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ChartPanel), new FrameworkPropertyMetadata(null, 
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
                OnItemsSourceChanged,
                null));
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

        #region Layers
        public LayerCollection Layers
        {
            get { return (LayerCollection)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        public static readonly DependencyProperty LayersProperty =
            DependencyProperty.Register("Layers", typeof(LayerCollection), typeof(ChartPanel), new PropertyMetadata(null));
        #endregion

        #region GridLinesVisibility
        public ChartPanelGridLinesVisibility GridLinesVisibility
        {
            get { return (ChartPanelGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            GridLinesPanel.GridLinesVisibilityProperty.AddOwner(typeof(ChartPanel), new FrameworkPropertyMetadata(ChartPanelGridLinesVisibility.Both,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region GridLinesBrush
        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLinesBrushProperty =
            GridLinesPanel.GridLinesBrushProperty.AddOwner(typeof(ChartPanel), new FrameworkPropertyMetadata(Brushes.LightGray,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region GridLinesThickness
        public double GridLinesThickness
        {
            get { return (double)GetValue(GridLinesThicknessProperty); }
            set { SetValue(GridLinesThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLinesThicknessProperty =
            GridLinesPanel.GridLinesThicknessProperty.AddOwner(typeof(ChartPanel), new FrameworkPropertyMetadata(1d,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region AnimationEasing
        public AnimationEasing AnimationEasing
        {
            get { return (AnimationEasing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        public static readonly DependencyProperty AnimationEasingProperty =
            DependencyProperty.Register("AnimationEasing", typeof(AnimationEasing), typeof(ChartPanel), new PropertyMetadata(AnimationEasing.None));
        #endregion

        #region AnimationDuration
        public TimeSpan? AnimationDuration
        {
            get { return (TimeSpan?)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan?), typeof(ChartPanel), new PropertyMetadata(TimeSpan.FromSeconds(1)));
        #endregion

        #endregion

        #region Internal Properties

        internal List<CoordinateImpl> Coordinates { get; private set; }

        internal double MinValue { get; private set; }

        internal double MaxValue { get; private set; }
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
                    if (titleProperty == null)
                    {
                        throw new System.NullReferenceException($"Property {TitleMemberPath} does not exists.");
                    }
                    var titleValue = titleProperty.GetValue(item);
                    var title = titleValue is string
                        ? (string)titleValue
                        : titleValue.ToString();

                    var values = new Dictionary<IChartValueProvider, double>();
                    foreach (var series in Series)
                    {
                        if (series is ValueProviderSeriesBase singleSeries)
                        {
                            if (string.IsNullOrEmpty(singleSeries.ValueMemberPath))
                            {
                                throw new NullReferenceException("Property ValueMemberPath of Series can not be null.");
                            }

                            var valueProperty = itemType.GetProperty(singleSeries.ValueMemberPath);
                            if (valueProperty == null)
                            {
                                throw new System.NullReferenceException($"Property {singleSeries.ValueMemberPath} does not exists.");
                            }
                            var valueValue = valueProperty.GetValue(item);
                            var value = Convert.ToDouble(valueValue);

                            values.Add(singleSeries, value);
                        }
                        if(series is SegmentsSeriesBase segmentsSeries)
                        {
                            foreach(var segment in segmentsSeries.GetSegments())
                            {
                                if (string.IsNullOrEmpty(segment.ValueMemberPath))
                                {
                                    throw new NullReferenceException("Property ValueMemberPath of Segment can not be null.");
                                }

                                var valueProperty = itemType.GetProperty(segment.ValueMemberPath);
                                if (valueProperty == null)
                                {
                                    throw new System.NullReferenceException($"Property {segment.ValueMemberPath} does not exists.");
                                }
                                var valueValue = valueProperty.GetValue(item);
                                var value = Convert.ToDouble(valueValue);

                                values.Add(segment, value);
                            }
                        }
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

            Coordinates = coordinates;
            if (coordinates.Any())
            {
                CheckMinMaxValue(coordinates.SelectMany(x => x.Values.Values).Min(),
                    coordinates.SelectMany(x => x.Values.Values).Max(),
                    out int minValue,
                    out int maxValue);

                MinValue = minValue;
                MaxValue = maxValue;
            }
            else
            {
                MinValue = 0;
                MaxValue = 10;
            }

            _xAxisPresenter.Measure(availableSize);
            _yAxisPresenter.Measure(availableSize);
            _seriesPanel.Measure(availableSize);
            _layersPanel.Measure(availableSize);
            _gridLinesPanel.Measure(availableSize);

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

            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #endregion

        #region Methods

        #region Internal Methods
        internal bool IsCanvasReady()
        {
            return _seriesPanel.RenderSize.Width != 0
                && _seriesPanel.RenderSize.Height != 0;
        }

        internal IDrawingContext CreateDrawingContext(DrawingContext context)
        {
            var drawingContext = new WPFDrawingContextImpl(context);
            return drawingContext;
        }

        internal IChartContext GetCanvasContext()
        {
            if(_chartContext == null)
            {
                _chartContext = new ChartContextImpl(this);
            }

            return _chartContext;
        }

        internal ILayerContext CreateLayerContext()
        {
            if (_layerContext == null)
            {
                _layerContext = new LayerContextImpl(this);
            }
            return _layerContext;
        }
        #endregion

        #endregion

        #region Event Handlers
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartPanel = (ChartPanel)d;
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= chartPanel.ObservableItemsSource_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged -= chartPanel.ObservableItemsSource_CollectionChanged;
                newCollection.CollectionChanged += chartPanel.ObservableItemsSource_CollectionChanged;
            }
        }

        private void ObservableItemsSource_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
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
