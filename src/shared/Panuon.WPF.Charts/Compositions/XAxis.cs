using Panuon.WPF.Charts.Implements;
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

        internal readonly Dictionary<CartesianCoordinateImpl, FormattedText> _formattedTexts =
            new Dictionary<CartesianCoordinateImpl, FormattedText>();
        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            _formattedTexts.Clear();

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

                _formattedTexts.Add(coordinate, formattedText);
            }

            if (!_formattedTexts.Any())
            {
                return new Size(0, 0);
            }
            return new Size(0, _formattedTexts.Values.Max(x => x.Height) + Spacing + TicksSize + StrokeThickness);
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

            drawingContext.DrawLine(
                Stroke,
                StrokeThickness,
                -StrokeThickness / 2,
                0,
                ActualWidth,
                0
            );
            foreach (var coordinateText in _formattedTexts)
            {
                var coordinate = coordinateText.Key;
                var text = coordinateText.Value;

                var offsetX = coordinate.OffsetX;

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
                    Spacing + TicksSize + StrokeThickness
                );
            }
        }
        #endregion

        #endregion
    }
}