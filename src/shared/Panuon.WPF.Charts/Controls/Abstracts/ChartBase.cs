using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Implements;
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


        private static TypeConverter _doubleTypeConverter = 
            TypeDescriptor.GetConverter(typeof(double));
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

        #region LabelMemberPath
        public string LabelMemberPath
        {
            get { return (string)GetValue(TitleMemberPathProperty); }
            set { SetValue(TitleMemberPathProperty, value); }
        }

        public static readonly DependencyProperty TitleMemberPathProperty =
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

            double value;
            if (string.IsNullOrEmpty(valueProvider.ValueMemberPath))
            {
                try
                {
                    if (index != -1
                        && item is IEnumerable enumerableItem)
                    {
                        var enumerator = enumerableItem.GetEnumerator();
                        for (int i = 0; i <= index; i++)
                        {
                            enumerator.MoveNext();
                        }
                        value = Convert.ToDouble(enumerator.Current);
                    }
                    else
                    {
                        value = Convert.ToDouble(item);
                    }
                }
                catch
                {
                    throw new InvalidOperationException($"Type '{itemType}' cannot be converted to double. To specify the value property, use the ValueMemberPath property.");
                }
            }
            else
            {
                var valueProperty = itemType.GetProperty(valueProvider.ValueMemberPath);
                if (valueProperty == null)
                {
                    throw new System.InvalidOperationException($"Property named '{valueProvider.ValueMemberPath}' does not exists in {item}.");
                }
                var valueValue = valueProperty.GetValue(item);
                value = Convert.ToDouble(valueValue);
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
        #endregion

        #endregion

        #region Event Handlers
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
        #endregion

        #region Function
        private void Rerender()
        {
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();

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
        #endregion

    }
}
