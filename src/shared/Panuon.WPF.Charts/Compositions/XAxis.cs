using Panuon.WPF.Charts.Implements;
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
        internal readonly Dictionary<FormattedText, Func<double>> _formattedTextOffsets =
            new Dictionary<FormattedText, Func<double>>();
        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            _formattedTextOffsets.Clear();

            if (!_chart.SwapXYAxes)
            {
                foreach (var coordinate in _chart.Coordinates)
                {
                    if (coordinate.Label == null)
                    {
                        continue;
                    }
                    var formattedText = new FormattedText(coordinate.Label,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                        FontSize,
                        Foreground
#if NET452
#else
                        , VisualTreeHelper.GetDpi(this).PixelsPerDip
#endif
                    )
                    {
                        MaxLineCount = LabelMaxLineCount,
                        MaxTextWidth = LabelMaxWidth,
                        Trimming = TextTrimming.CharacterEllipsis
                    };

                    _formattedTextOffsets.Add(formattedText, () => coordinate.Offset);
                }
            }
            else
            {
                var deltaX = (_chart.ActualMaxValue - _chart.ActualMinValue) / 5;

                for (int i = 0; i <= 5; i++)
                {
                    var value = _chart.ActualMinValue + deltaX * i;
                    var formattedText = new FormattedText(
                        value.ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                        FontSize,
                        Foreground
#if NET452
#else
                        , VisualTreeHelper.GetDpi(this).PixelsPerDip
#endif
                    )
                    {
                        MaxLineCount = LabelMaxLineCount,
                        MaxTextWidth = LabelMaxWidth,
                        Trimming = TextTrimming.CharacterEllipsis
                    };
                    _formattedTextOffsets.Add(formattedText, () => (_chart.GetCanvasContext() as ICartesianChartContext).GetOffsetY(value));
                }
            }

            if (!_formattedTextOffsets.Any())
            {
                return new Size(0, 0);
            }


            return new Size(
                0,
                _formattedTextOffsets.Keys.Max(x => x.Height) + Spacing + TicksSize + StrokeThickness
            );
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(finalSize.Width, DesiredSize.Height);
        }
        #endregion

        #region OnRender
        protected override void OnRender(DrawingContext context)
        {
            if (!_chart.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chart.CreateDrawingContext(context);
            var chartContext = _chart.GetCanvasContext() as ICartesianChartContext;

            drawingContext.DrawLine(
                Stroke,
                StrokeThickness,
                -StrokeThickness / 2,
                0,
                ActualWidth,
                0
            );

            foreach (var coordinateText in _formattedTextOffsets)
            {
                var text = coordinateText.Key;
                var offsetX = coordinateText.Value();

                if (_chart.SwapXYAxes)
                {

                }

                drawingContext.DrawLine(
                     TicksBrush,
                     StrokeThickness,
                     offsetX,
                     StrokeThickness / 2,
                     offsetX,
                     StrokeThickness / 2 + TicksSize
                 );
                drawingContext.DrawText(
                    text,
                    offsetX - text.Width / 2,
                    StrokeThickness + Spacing + TicksSize
                );
            }
        }
        #endregion

        #endregion
    }
}