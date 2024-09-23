using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class YAxis
        : AxisBase
    {
        #region Fields
        internal readonly Dictionary<FormattedText, Func<double>> _formattedTextOffsets =
            new Dictionary<FormattedText, Func<double>>();
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

        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            _formattedTextOffsets.Clear();

            if (!_chart.SwapXYAxes)
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
#if NET452 || NET462 || NET472 || NET48
#else
                        , VisualTreeHelper.GetDpi(this).PixelsPerDip
#endif
                    );
                    _formattedTextOffsets.Add(formattedText, () => (_chart.GetCanvasContext() as ICartesianChartContext).GetOffsetY(value));
                }
            }
            else
            {
                foreach (var coordinate in _chart.Coordinates)
                {
                    if (coordinate.Title == null)
                    {
                        continue;
                    }
                    var formattedText = new FormattedText(coordinate.Title,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                        FontSize,
                        Foreground
#if NET452 || NET462 || NET472 || NET48
                    );
#else
                        , VisualTreeHelper.GetDpi(this).PixelsPerDip);
#endif

                    _formattedTextOffsets.Add(formattedText, () => coordinate.Offset);
                }
            }

            if (!_formattedTextOffsets.Any())
            {
                return new Size(0, 0);
            }

            return new Size(
                _formattedTextOffsets.Keys.Max(x => x.Width) + Spacing + TicksSize + StrokeThickness,
                0
            );
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(DesiredSize.Width, finalSize.Height);
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
                ActualWidth,
                -StrokeThickness / 2,
                ActualWidth,
                ActualHeight
            );

            foreach (var valueText in _formattedTextOffsets)
            {
                var text = valueText.Key;
                var offsetY = valueText.Value();

                drawingContext.DrawLine(
                    TicksBrush,
                    StrokeThickness,
                    ActualWidth - StrokeThickness / 2,
                    offsetY,
                    ActualWidth - StrokeThickness / 2 - TicksSize,
                    offsetY
                );
                drawingContext.DrawText(
                    text,
                    ActualWidth - StrokeThickness - Spacing - TicksSize - text.Width,
                    offsetY - text.Height / 2
                );
            }
        }
        #endregion

        #endregion
    }
}
