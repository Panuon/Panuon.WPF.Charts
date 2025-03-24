using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class XAxis
        : AxisBase
    {
        #region Fields
        internal readonly List<(string, Func<double>)> _labelOffsets =
            new List<(string, Func<double>)>();
        #endregion

        #region Ctor
        public XAxis()
        {
            ClipToBounds = true;
        }
        #endregion

        #region Properties

        #region CoordinateMinWidth
        public GridLength CoordinateMinWidth
        {
            get { return (GridLength)GetValue(CoordinateMinWidthProperty); }
            set { SetValue(CoordinateMinWidthProperty, value); }
        }

        public static readonly DependencyProperty CoordinateMinWidthProperty =
            DependencyProperty.Register("CoordinateMinWidth", typeof(GridLength), typeof(XAxis), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            _labelOffsets.Clear();

            foreach (var coordinate in _chart.Coordinates)
            {
                if (coordinate.Label == null)
                {
                    continue;
                }
                _labelOffsets.Add((coordinate.Label, () => coordinate.Offset));
            }

            if (!_labelOffsets.Any())
            {
                return new Size(0, 0);
            }

            var maxText = _labelOffsets.OrderByDescending(lc => lc.Item1?.Length ?? 0).First().Item1;

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
                    0,
                    (maxFormattedText?.Height ?? 0) + Spacing + TicksSize + StrokeThickness
                );
            }
            else
            {
                return new Size(
                    (maxFormattedText?.Width ?? 0) + Spacing + TicksSize + StrokeThickness,
                    0
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
                    finalSize.Width, 
                    DesiredSize.Height
                );
            }
            else
            {
                return new Size(
                    DesiredSize.Width,
                    finalSize.Height
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
                    new Point(0, StrokeThickness),
                    new Point(chartContext.CanvasWidth, StrokeThickness));
            }
            else
            {
                drawingContext.DrawLine(
                    Stroke,
                    StrokeThickness,
                    new Point(ActualWidth - StrokeThickness / 2, -StrokeThickness / 2),
                    new Point(ActualWidth - StrokeThickness / 2, ActualHeight + StrokeThickness / 2));
            }

            foreach (var coordinateText in _labelOffsets)
            {
                if (!_chart.SwapXYAxes)
                {
                    var text = coordinateText.Item1;
                    var offsetX = coordinateText.Item2();

                    drawingContext.DrawLine(
                        TicksBrush,
                        StrokeThickness,
                        new Point(offsetX, StrokeThickness),
                        new Point(offsetX, StrokeThickness + TicksSize));

                    var formattedText = CreateFormattedText(
                        text,
                        maxLineCount: LabelMaxLineCount,
                        maxTextWidth: LabelMaxWidth);

                    drawingContext.DrawText(
                        formattedText,
                        new Point(offsetX - formattedText.Width / 2, StrokeThickness + Spacing + TicksSize));
                }
                else
                {
                    var text = coordinateText.Item1;
                    var offsetY = coordinateText.Item2();

                    drawingContext.DrawLine(
                        TicksBrush,
                        StrokeThickness,
                        new Point(ActualWidth - StrokeThickness / 2, offsetY),
                        new Point(ActualWidth - StrokeThickness / 2 - TicksSize, offsetY));

                    var formattedText = CreateFormattedText(
                        text,
                        maxLineCount: LabelMaxLineCount,
                        maxTextWidth: LabelMaxWidth);

                    drawingContext.DrawText(
                        formattedText,
                        new Point(ActualWidth - StrokeThickness - Spacing - TicksSize - formattedText.Width, offsetY - formattedText.Height / 2));
                }
            }
        }
        #endregion

        #endregion
    }
}