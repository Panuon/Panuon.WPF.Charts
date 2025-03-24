using Panuon.WPF.Charts.Controls.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        internal LegendPanel _legendPanel;

        private static TypeConverter _doubleTypeConverter = 
            TypeDescriptor.GetConverter(typeof(double));
        #endregion

        #region Ctor
        public ChartBase()
        {
            _children = new UIElementCollection(this, this);

            SetCurrentValue(LayersProperty, new LayerCollection());

            _layersPanel = new LayersPanel(this);
            _children.Add(_layersPanel);

            var itemTemplate = FindResource(LegendItemTemplateKey) as DataTemplate;
            SetCurrentValue(LegendItemTemplateProperty, itemTemplate);

            var labelStyle = FindResource(LegendLabelStyleKey) as Style;
            SetCurrentValue(LegendLabelStyleProperty, labelStyle);

            _legendPanel = new LegendPanel(this);
            _children.Add(_legendPanel);
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

        #region LabelMemberPath
        public string LabelMemberPath
        {
            get { return (string)GetValue(LabelMemberPathProperty); }
            set { SetValue(LabelMemberPathProperty, value); }
        }

        public static readonly DependencyProperty LabelMemberPathProperty =
            DependencyProperty.Register("LabelMemberPath", typeof(string), typeof(ChartBase));
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

        #region ShowLegend
        public bool ShowLegend
        {
            get { return (bool)GetValue(ShowLegendProperty); }
            set { SetValue(ShowLegendProperty, value); }
        }

        public static readonly DependencyProperty ShowLegendProperty =
           DependencyProperty.Register("ShowLegend", typeof(bool), typeof(ChartBase), new FrameworkPropertyMetadata(false));
        #endregion

        #region LegendLabelStyle
        public Style LegendLabelStyle
        {
            get { return (Style)GetValue(LegendLabelStyleProperty); }
            set { SetValue(LegendLabelStyleProperty, value); }
        }

        public static readonly DependencyProperty LegendLabelStyleProperty =
           DependencyProperty.Register("LegendLabelStyle", typeof(Style), typeof(ChartBase), new FrameworkPropertyMetadata(null));
        #endregion

        #region LegendItemTemplate
        public DataTemplate LegendItemTemplate
        {
            get { return (DataTemplate)GetValue(LegendItemTemplateProperty); }
            set { SetValue(LegendItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty LegendItemTemplateProperty =
           DependencyProperty.Register("LegendItemTemplate", typeof(DataTemplate), typeof(ChartBase), new FrameworkPropertyMetadata(null));
        #endregion

        #region LegendPosition
        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        public static readonly DependencyProperty LegendPositionProperty =
           DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(ChartBase), new FrameworkPropertyMetadata(LegendPosition.TopRight, OnLegendPositionChanged));
        #endregion

        #region LegendOffsetX
        public double LegendOffsetX
        {
            get { return (double)GetValue(LegendOffsetXProperty); }
            set { SetValue(LegendOffsetXProperty, value); }
        }

        public static readonly DependencyProperty LegendOffsetXProperty =
           DependencyProperty.Register("LegendOffsetX", typeof(double), typeof(ChartBase), new FrameworkPropertyMetadata(0d, OnLegendPositionChanged));
        #endregion

        #region LegendOffsetY
        public double LegendOffsetY
        {
            get { return (double)GetValue(LegendOffsetYProperty); }
            set { SetValue(LegendOffsetYProperty, value); }
        }

        public static readonly DependencyProperty LegendOffsetYProperty =
           DependencyProperty.Register("LegendOffsetY", typeof(double), typeof(ChartBase), new FrameworkPropertyMetadata(0d, OnLegendPositionChanged));
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

        #region AnimationMode
        public AnimationTriggerMode AnimationMode
        {
            get { return (AnimationTriggerMode)GetValue(AnimationModeProperty); }
            set { SetValue(AnimationModeProperty, value); }
        }

        public static readonly DependencyProperty AnimationModeProperty =
            DependencyProperty.Register("AnimationMode", typeof(AnimationTriggerMode), typeof(ChartBase));
        #endregion

        #endregion

        #region ComponentStyleKeys
        public static ComponentResourceKey LegendItemTemplateKey { get; } =
            new ComponentResourceKey(typeof(ChartBase), nameof(LegendItemTemplateKey));

        public static ComponentResourceKey LegendLabelStyleKey { get; } =
            new ComponentResourceKey(typeof(ChartBase), nameof(LegendLabelStyleKey));
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
            _seriesPanel.Measure(availableSize);
            _layersPanel.Measure(availableSize);

            _legendPanel.UpdateEntries();
            _legendPanel.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderWidth = finalSize.Width - Padding.Left - Padding.Right;
            var renderHeight = finalSize.Height - Padding.Top - Padding.Bottom;

            _seriesPanel.Arrange(new Rect(Padding.Left, Padding.Top, renderWidth, renderHeight));
            _layersPanel.Arrange(new Rect(Padding.Left, Padding.Top, renderWidth, renderHeight));
            _legendPanel.Arrange(new Rect(Padding.Left, Padding.Top, renderWidth, renderHeight));

            _seriesPanel.InvalidateVisual();
            _layersPanel.InvalidateVisual();

            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #endregion

        #region Methods

        public abstract IEnumerable<SeriesBase> GetSeries();

        #region Protected Methods
        protected double GetValueFromValueProvider(
            IChartValueProvider valueProvider,
            object item,
            int index = -1
        )
        {
            var itemType = item.GetType();

            double value = 0d;
            try
            {
                if (index != -1
                    && item is IEnumerable enumerableItem)
                {
                    var enumerator = enumerableItem.GetEnumerator();
                    for (int i = 0; i <= index; i++)
                    {
                        enumerator.MoveNext();
                        if (string.IsNullOrEmpty(valueProvider.ValueMemberPath))
                        {
                            value = Convert.ToDouble(enumerator.Current);
                        }
                        else
                        {
                            var valueValue = PropertyAccessor.GetValue(enumerator.Current, valueProvider.ValueMemberPath);
                            value = Convert.ToDouble(valueValue);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(valueProvider.ValueMemberPath))
                    {
                        value = Convert.ToDouble(item);
                    }
                    else
                    {
                        var valueValue = PropertyAccessor.GetValue(item, valueProvider.ValueMemberPath);
                        value = Convert.ToDouble(valueValue);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException($"Type '{itemType}' cannot be converted to double. To specify the value property, use the ValueMemberPath property.");
            }

            return value;
        }
        #endregion

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

        internal abstract IChartContext GetCanvasContext();

        internal abstract void OnClearValues();
        #endregion

        #endregion

        #region Event Handlers
        protected static void OnAffectsSchemaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (ChartBase)d;
            chart.Rerender();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (ChartBase)d;
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= chart.ObservableItemsSource_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged -= chart.ObservableItemsSource_CollectionChanged;
                newCollection.CollectionChanged += chart.ObservableItemsSource_CollectionChanged;
            }

            if (!chart.IsLoaded)
            {
                return;
            }

            chart.Rerender();
        }

        private void ObservableItemsSource_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            Rerender();
        }

        private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (ChartBase)d;
            chart.OnLegendPositionChanged();
        }
        #endregion

        #region Function
        protected void Rerender()
        {
            OnClearValues();

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();

            _seriesPanel.ResetSeries();

            foreach (UIElement child in _children)
            {
                if (child is Decorator decorator)
                {
                    decorator.Child.InvalidateMeasure();
                    decorator.Child.InvalidateArrange();
                    decorator.Child.InvalidateVisual();
                }
                else
                {
                    child.InvalidateMeasure();
                    child.InvalidateArrange();
                    child.InvalidateVisual();
                }
            }
        }

        private void OnLegendPositionChanged()
        {
            _legendPanel.InvalidateArrange();
        }
        #endregion

    }
}
