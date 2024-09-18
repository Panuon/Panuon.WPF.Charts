using Panuon.WPF.Chart;
using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class ChartBase
        : Control
    {
        #region Fields
        protected readonly UIElementCollection _children;

        internal SeriesPanel _seriesPanel;
        internal LayersPanel _layersPanel;

        private ChartContextImpl _chartContext;
        private LayerContextImpl _layerContext;
        #endregion

        #region Ctor
        public ChartBase()
        {
            _children = new UIElementCollection(this, this);

            Layers = new LayerCollection();

            _layersPanel = new LayersPanel(this);
            _children.Add(_layersPanel);
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ChartBase), new FrameworkPropertyMetadata(null, 
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
            DependencyProperty.Register("TitleMemberPath", typeof(string), typeof(ChartBase));
        #endregion

        #region Layers
        public LayerCollection Layers
        {
            get { return (LayerCollection)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        public static readonly DependencyProperty LayersProperty =
            DependencyProperty.Register("Layers", typeof(LayerCollection), typeof(ChartBase), new PropertyMetadata(null));
        #endregion

        #region AnimationEasing
        public AnimationEasing AnimationEasing
        {
            get { return (AnimationEasing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        public static readonly DependencyProperty AnimationEasingProperty =
            DependencyProperty.Register("AnimationEasing", typeof(AnimationEasing), typeof(ChartBase), new PropertyMetadata(AnimationEasing.None));
        #endregion

        #region AnimationDuration
        public TimeSpan? AnimationDuration
        {
            get { return (TimeSpan?)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan?), typeof(ChartBase), new PropertyMetadata(TimeSpan.FromSeconds(1)));
        #endregion

        #endregion

        #region Internal Properties

        internal List<CoordinateImpl> Coordinates { get; private set; }

        internal virtual double ActualMinValue
        {
            get
            {
                return _measuredMinValue;
            }
        }
        private double _measuredMinValue;

        internal virtual double ActualMaxValue
        {
            get
            {
                return _measuredMaxValue;
            }
        }
        private double _measuredMaxValue;
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
                    var itemType = item.GetType();
                    string title = null;
                    if (!string.IsNullOrEmpty(TitleMemberPath))
                    {
                        var titleProperty = itemType.GetProperty(TitleMemberPath);
                        if (titleProperty == null)
                        {
                            throw new System.NullReferenceException($"Property {TitleMemberPath} does not exists.");
                        }

                        var titleValue = titleProperty.GetValue(item);
                        title = titleValue is string
                            ? (string)titleValue
                            : titleValue.ToString();
                    }

                    var values = new Dictionary<IChartValueProvider, double>();
                    foreach (var series in GetSeries())
                    {
                        if (series is IChartValueProvider valueProvider)
                        {
                            var value = GetValueFromValueProvider(valueProvider, item);
                            values.Add(valueProvider, value);
                        }
                        if (series is CartesianSegmentsSeriesBase cartesianSeries)
                        {
                            foreach (ValueProviderSegmentBase segment in cartesianSeries.GetSegments())
                            {
                                var value = GetValueFromValueProvider(segment, item);
                                values.Add(segment, value);
                            }
                        }
                        if (series is RadialSegmentsSeriesBase radialSeries)
                        {
                            foreach (ValueProviderSegmentBase segment in radialSeries.GetSegments())
                            {
                                var value = GetValueFromValueProvider(segment, item);
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

                _measuredMinValue = minValue;
                _measuredMaxValue = maxValue;
            }
            else
            {
                _measuredMinValue = 0;
                _measuredMaxValue = 10;
            }

            _seriesPanel.Measure(availableSize);
            _layersPanel.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderWidth = finalSize.Width - Padding.Left - Padding.Right;
            var renderHeight = finalSize.Height - Padding.Top - Padding.Bottom;

            var deltaX = renderWidth / Coordinates.Count;

            for (int i = 0; i < Coordinates.Count; i++)
            {
                var coordinate = Coordinates[i];
                coordinate.Offset = (i + 0.5) * deltaX;
            }

            _seriesPanel.Arrange(new Rect(Padding.Left, Padding.Top, renderWidth, renderHeight));
            _layersPanel.Arrange(new Rect(Padding.Left, Padding.Top, renderWidth, renderHeight));

            _seriesPanel.InvalidateVisual();
            _layersPanel.InvalidateVisual();

            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #endregion

        #region Methods

        public abstract IEnumerable<SeriesBase> GetSeries();

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
            var chartPanel = (ChartBase)d;
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

        private double GetValueFromValueProvider(
            IChartValueProvider valueProvider,
            object item
        )
        {
            if (string.IsNullOrEmpty(valueProvider.ValueMemberPath))
            {
                throw new NullReferenceException("Property ValueMemberPath of Series can not be null.");
            }

            var itemType = item.GetType();

            var valueProperty = itemType.GetProperty(valueProvider.ValueMemberPath);
            if (valueProperty == null)
            {
                throw new System.NullReferenceException($"Property named '{valueProvider.ValueMemberPath}' does not exists.");
            }
            var valueValue = valueProperty.GetValue(item);
            var value = Convert.ToDouble(valueValue);

            return value;
        }
        #endregion
    }
}
