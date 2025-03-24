using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class YAxis
        : AxisBase
    {
        #region Fields
        internal readonly List<YAxisLabelOffset> _labelOffsets =
            new List<YAxisLabelOffset>();
        #endregion

        #region Ctor
        public YAxis()
        {
            Labels = new Collection<YAxisLabel>();
        }
        #endregion

        #region Events
        public event YAxisGeneratingLabelRoutedEventHandler GeneratingLabel
        {
            add { AddHandler(GeneratingLabelEvent, value); }
            remove { RemoveHandler(GeneratingLabelEvent, value); }
        }

        public static readonly RoutedEvent GeneratingLabelEvent =
            EventManager.RegisterRoutedEvent("GeneratingLabel", RoutingStrategy.Bubble, typeof(YAxisGeneratingLabelRoutedEventHandler), typeof(YAxis));
        #endregion

        #region Properties

        #region MinValue
        public double? MinValue
        {
            get { return (double?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double?), typeof(YAxis));
        #endregion

        #region MinValue
        public double? MaxValue
        {
            get { return (double?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double?), typeof(YAxis));
        #endregion

        #region Labels
        public Collection<YAxisLabel> Labels
        {
            get { return (Collection<YAxisLabel>)GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }

        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(Collection<YAxisLabel>), typeof(YAxis), new PropertyMetadata(OnLabelsChanged));
        #endregion

        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            var canvasContext = _chart.GetCanvasContext() as ICartesianChartContext;
            _labelOffsets.Clear();

            if (Labels != null
                && Labels.Any())
            {
                foreach (var label in Labels)
                {
                    _labelOffsets.Add(new YAxisLabelOffset(
                        label.Label,
                        label.Value,
                        () => canvasContext.GetOffsetY(label.Value)
                    ));
                }
            }
            else
            {
                var deltaX = (_chart.ActualMaxValue - _chart.ActualMinValue) / 5;

                for (int i = 0; i <= 5; i++)
                {
                    var value = _chart.ActualMinValue + deltaX * i;
                    var label = value.ToString();
                    var eventArgs = new YAxisGeneratingLabelRoutedEventArgs(
                        GeneratingLabelEvent,
                        value,
                        canvasContext.MinValue,
                        canvasContext.MaxValue,
                        label);
                    RaiseEvent(eventArgs);
                    label = eventArgs.Label;

                    _labelOffsets.Add(new YAxisLabelOffset(
                        label,
                        value,
                        () => canvasContext.GetOffsetY(value)
                    ));
                }
            }

            if (!_labelOffsets.Any())
            {
                return new Size(0, 0);
            }

            var maxText = _labelOffsets.OrderByDescending(lc => lc.Label?.Length ?? 0).First().Label;

            FormattedText maxFormattedText = null;
            if (!string.IsNullOrEmpty(maxText))
            {
                maxFormattedText = CreateFormattedText(
                    maxText,
                    maxLineCount: LabelMaxLineCount,
                    maxTextWidth: LabelMaxWidth
                );
            }

            if (!_chart.SwapXYAxes)
            {
                return new Size(
                    (maxFormattedText?.Width ?? 0) + Spacing + TicksSize + StrokeThickness,
                    0
                );
            }
            else
            {
                return new Size(
                    0,
                    (maxFormattedText?.Height ?? 0) + Spacing + TicksSize + StrokeThickness
                );
            }
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_chart.SwapXYAxes)
            {
                return new Size(
                    DesiredSize.Width, 
                    finalSize.Height
                );
            }
            else
            {
                return new Size(
                    finalSize.Width,
                    DesiredSize.Height
                );
            }
        }
        #endregion

        #region OnRender
        protected override void OnRender(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            if (!_chart.SwapXYAxes)
            {
                drawingContext.DrawLine(
                    Stroke,
                    StrokeThickness,
                    new Point(ActualWidth, -StrokeThickness / 2),
                    new Point(ActualWidth, ActualHeight + StrokeThickness / 2));
            }
            else
            {
                drawingContext.DrawLine(
                    Stroke,
                    StrokeThickness,
                    new Point(-StrokeThickness / 2, 0),
                    new Point(ActualWidth, 0));
            }

            for(var index = 0; index < _labelOffsets.Count; index++)
            {
                var coordinateText = _labelOffsets.ElementAt(index);
                if (!_chart.SwapXYAxes)
                {
                    var text = coordinateText.Label;
                    var offsetY = coordinateText.GetOffsetY();
                    if (index != 0)
                    {
                        drawingContext.DrawLine(
                            TicksBrush,
                            StrokeThickness,
                            new Point(ActualWidth - StrokeThickness / 2, offsetY),
                            new Point(ActualWidth - StrokeThickness / 2 - TicksSize, offsetY));
                    }

                    var formattedText = CreateFormattedText(
                        text,
                        maxLineCount: LabelMaxLineCount,
                        maxTextWidth: LabelMaxWidth);

                    drawingContext.DrawText(
                        text: formattedText,
                        startPoint: new Point(ActualWidth - StrokeThickness - Spacing - TicksSize - formattedText.Width, offsetY - formattedText.Height / 2));
                }
                else
                {
                    var text = coordinateText.Label;
                    var offsetX = coordinateText.GetOffsetY();
                    if (index != 0)
                    {
                        drawingContext.DrawLine(
                             TicksBrush,
                             StrokeThickness,
                             new Point(offsetX, StrokeThickness / 2),
                             new Point(offsetX, StrokeThickness / 2 + TicksSize)
                         );
                    }

                    var formattedText = CreateFormattedText(
                        text,
                        maxLineCount: LabelMaxLineCount,
                        maxTextWidth: LabelMaxWidth
                    );
                    drawingContext.DrawText(
                        formattedText,
                        new Point(offsetX - formattedText.Width / 2, StrokeThickness + Spacing + TicksSize)
                    );
                }
            }
        }
        #endregion

        #endregion

        #region Event Handlers
        private static void OnLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as YAxis;

            if (e.OldValue is ObservableCollection<YAxisLabel> oldCollection)
            {
                oldCollection.CollectionChanged -= axis.Labels_CollectionChanged;
            }

            if (e.NewValue is ObservableCollection<YAxisLabel> newCollection)
            {
                newCollection.CollectionChanged -= axis.Labels_CollectionChanged;
                newCollection.CollectionChanged += axis.Labels_CollectionChanged;
            }
            axis.InvalidateMeasure();
            axis.InvalidateArrange();
            axis.InvalidateVisual();
        }

        private void Labels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }
        #endregion
    }
}
