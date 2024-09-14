using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class PieSeries
        : ValueProviderSegmentsSeriesBase<PieSeriesSegment>
    {
        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(PieSeries), new PropertyMetadata(20d));
        #endregion

        #region Width
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(PieSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto), OnRenderPropertyChanged));
        #endregion

        #endregion

        #region Overrides
        protected override void OnRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        )
        {
            var chartPanel = chartContext.ChartPanel;
            var coordinates = chartContext.Coordinates;

            var areaWidth = chartContext.AreaWidth - Spacing * 2;
            var areaHeight = chartContext.AreaHeight - Spacing * 2;

            var radius = Math.Min(areaWidth, areaHeight) / 2;
            var centerX = areaWidth / 2 + Spacing;
            var centerY = areaHeight / 2 + Spacing;

            var totalValue = coordinates.Select(c => c.GetValue(this)).Sum();
            var angleDelta = 360d / totalValue;

            var index = 0;
            var totalAngle = 0d;
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);
                var angle = Math.Round(angleDelta * value, 2);
                if (index >= Segments.Count)
                {
                    break;
                }
                var segment = Segments[index];

                drawingContext.DrawArc(segment.Stroke,
                        segment.StrokeThickness,
                        segment.Fill,
                        centerX,
                        centerY,
                        radius,
                        totalAngle,
                        totalAngle + angle);

                totalAngle += angle;
                index++;
            }

            index = 0;
            totalAngle = 0d;
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);
                var angle = Math.Round(angleDelta * value, 2);
                if (index >= Segments.Count)
                {
                    break;
                }
                var segment = Segments[index];

                var formattedText = new FormattedText(coordinate.Title,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(chartPanel.FontFamily, chartPanel.FontStyle, chartPanel.FontWeight, chartPanel.FontStretch),
                    chartPanel.FontSize,
                    chartPanel.Foreground
#if NET452 || NET462 || NET472 || NET48
                    );
#else
                    ,VisualTreeHelper.GetDpi(chartPanel).PixelsPerDip);
#endif

                double radian = (totalAngle + angle / 2 - 90) * Math.PI / 180.0;

                var halfPoint = new Point(centerX + radius * Math.Cos(radian) + (2 + formattedText.Width / 2) * Math.Cos(radian),
                    centerY + radius * Math.Sin(radian) + (1 + formattedText.Height / 2) * Math.Sin(radian));

                if (segment.LabelStroke == null && segment.LabelForeground == null)
                {
                    drawingContext.DrawText(formattedText, halfPoint.X - formattedText.Width / 2, halfPoint.Y - formattedText.Height / 2);
                }
                else
                {
                    drawingContext.DrawText(formattedText, segment.LabelForeground, segment.LabelStroke, segment.LabelStrokeThickness, halfPoint.X - formattedText.Width / 2, halfPoint.Y - formattedText.Height / 2);
                }

                totalAngle += angle;
                index++;
            }
        }

        protected override void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinates = chartContext.Coordinates;

                var areaWidth = chartContext.AreaWidth - Spacing * 2;
                var areaHeight = chartContext.AreaHeight - Spacing * 2;

                var radius = Math.Min(areaWidth, areaHeight) / 2;
                var centerX = areaWidth / 2 + Spacing;
                var centerY = areaHeight / 2 + Spacing;

                var totalValue = coordinates.Select(c => c.GetValue(this)).Sum();
                var angleDelta = 360d / totalValue;

                var index = 0;
                var totalAngle = 0d;
                foreach (var coordinate in coordinates)
                {
                    var value = coordinate.GetValue(this);
                    var angle = Math.Round(angleDelta * value, 2);
                    if (index >= Segments.Count)
                    {
                        return;
                    }
                    var segment = Segments[index];

                    if (IsPointInsideSector(position,
                        centerX, centerY,
                        radius,
                        totalAngle, totalAngle + angle))
                    {
                        drawingContext.DrawArc(Brushes.Gold, 2,
                                null,
                                centerX, centerY,
                                radius,
                                totalAngle, totalAngle + angle);
                        tooltips.Add(new SeriesTooltip(segment.Fill, segment.Title ?? coordinate.Title, value.ToString()));
                        return;
                    }

                    totalAngle += angle;
                    index++;
                }
            }
        }
        #endregion

        #region Functions
        private bool IsPointInsideSector(Point point, 
            double centerX, 
            double centerY,
            double radius,
            double startAngle, 
            double endAngle)
        {
            var distance = Math.Sqrt(Math.Pow(point.X - centerX, 2) + Math.Pow(point.Y - centerY, 2));
            var angle = Math.Atan2(point.Y - centerY, point.X - centerX) * (180 / Math.PI);

            angle += 90;
            if (angle < 0)
            {
                angle += 360;
            }

            return distance <= radius && angle >= startAngle && angle <= endAngle;
        }
        #endregion

    }
}
