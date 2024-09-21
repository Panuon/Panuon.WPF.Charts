using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DoughnutSeries
        : RadialValueProviderSegmentsSeriesBase<DoughnutSeriesSegment>
    {
        #region Structs
        private struct DoughnutSeriesSegmentInfo
        {
            public double StartAngle { get; set; }

            public double Angle { get; set; }

            public FormattedText Title { get; set; }
        }
        #endregion

        #region Fields
        private Dictionary<DoughnutSeriesSegment, DoughnutSeriesSegmentInfo> _segmentInfos;
        #endregion

        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(DoughnutSeries), new PropertyMetadata(5d));
        #endregion

        #region Thickness
        public GridLength Thickness
        {
            get { return (GridLength)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(GridLength), typeof(DoughnutSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto)));
        #endregion

        #endregion

        #region Events
        public event GeneratingTitleEventHandler GeneratingTitle;
        #endregion

        #region Overrides

        #region OnRenderBegin
        protected override void OnRenderBegin(IDrawingContext drawingContext, IChartContext chartContext)
        {
            base.OnRenderBegin(drawingContext, chartContext);

            _segmentInfos = new Dictionary<DoughnutSeriesSegment, DoughnutSeriesSegmentInfo>();

            var chartPanel = chartContext.Chart;
            var coordinates = chartContext.Coordinates;

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

                var generatingTitleArgs = new GeneratingTitleEventArgs(
                    value: value,
                    title: segment.Title ?? coordinate.Title
                );
                GeneratingTitle?.Invoke(this, generatingTitleArgs);

                _segmentInfos[segment] = new DoughnutSeriesSegmentInfo()
                {
                    StartAngle = totalAngle,
                    Angle = angle,
                    Title = string.IsNullOrEmpty(generatingTitleArgs.Title)
                        ? null
                        : new FormattedText(
                            generatingTitleArgs.Title,
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface(chartPanel.FontFamily, chartPanel.FontStyle, chartPanel.FontWeight, chartPanel.FontStretch),
                            chartPanel.FontSize,
                            chartPanel.Foreground
        #if NET452 || NET462 || NET472 || NET48
        #else
                            , VisualTreeHelper.GetDpi(chartPanel).PixelsPerDip
        #endif
                        )
                            {
                                TextAlignment = TextAlignment.Center
                            }
                };

                totalAngle += angle;
                index++;
            }
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        )
        {
            var chartPanel = chartContext.Chart;
            var coordinates = chartContext.Coordinates;

            var areaWidth = chartContext.AreaWidth - Spacing * 2 - chartContext.Chart.FontSize * 2;
            var areaHeight = chartContext.AreaHeight - Spacing * 2 - chartContext.Chart.FontSize * 2;

            var outterRadius = Math.Min(areaWidth, areaHeight) / 2;
            var thickness = GridLengthUtil.GetActualValue(Thickness, outterRadius, 0.2);

            var centerX = chartContext.AreaWidth / 2;
            var centerY = chartContext.AreaHeight / 2;

            var totalValue = coordinates.Select(c => c.GetValue(this)).Sum();
            var angleDelta = 360d / totalValue;

            foreach (var segmentInfo in _segmentInfos)
            {
                var segment = segmentInfo.Key;
                var startAngle = segmentInfo.Value.StartAngle * animationProgress;
                var angle = Math.Round(segmentInfo.Value.Angle, 2) * animationProgress;

                drawingContext.DrawArc(segment.Stroke,
                        segment.StrokeThickness,
                        segment.Fill,
                        centerX,
                        centerY,
                        outterRadius - thickness,
                        outterRadius,
                        startAngle,
                        startAngle + angle);
            }

            foreach (var segmentInfo in _segmentInfos)
            {
                var segment = segmentInfo.Key;
                var formattedText = segmentInfo.Value.Title;
                var startAngle = segmentInfo.Value.StartAngle * animationProgress;
                var angle = Math.Round(segmentInfo.Value.Angle, 2) * animationProgress;

                var currentAngle = startAngle + angle / 2;
                var radian = (currentAngle - 90) * Math.PI / 180.0;
                var rayLength = CalculateRayLength(formattedText.Width, formattedText.Height, currentAngle + 90);

                var halfPoint = new Point(
                    centerX + (outterRadius + Spacing + rayLength) * Math.Cos(radian),
                    centerY + (outterRadius + Spacing + rayLength) * Math.Sin(radian)
                );

                if (formattedText != null)
                {
                    if (segment.LabelStroke == null && segment.LabelForeground == null)
                    {
                        drawingContext.DrawText(
                            formattedText,
                            halfPoint.X,
                            halfPoint.Y - formattedText.Height / 2
                        );
                    }
                    else
                    {
                        drawingContext.DrawText(
                            formattedText,
                            segment.LabelForeground,
                            segment.LabelStroke,
                            segment.LabelStrokeThickness,
                            halfPoint.X,
                            halfPoint.Y - formattedText.Height / 2
                        );
                    }
                }
            }
        }
        #endregion

        protected override ICoordinate OnRetrieveCoordinate(
            IChartContext chartContext,
            ILayerContext layerContext, 
            Point position
        )
        {
            return null;
        }

        protected override void OnHighlighting(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            IDictionary<ICoordinate, double> coordinatesProgress
        )
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinates = chartContext.Coordinates;

                var areaWidth = chartContext.AreaWidth - Spacing * 2 - chartContext.Chart.FontSize * 2;
                var areaHeight = chartContext.AreaHeight - Spacing * 2 - chartContext.Chart.FontSize * 2;

                var outterRadius = Math.Min(areaWidth, areaHeight) / 2;
                var thickness = GridLengthUtil.GetActualValue(Thickness, outterRadius, 0.2);

                var centerX = chartContext.AreaWidth / 2;
                var centerY = chartContext.AreaHeight / 2;

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
                        outterRadius,
                        totalAngle, totalAngle + angle))
                    {
                        drawingContext.DrawArc(
                            Brushes.Gold,
                            2,
                            null,
                            centerX,
                            centerY,
                            outterRadius - thickness,
                            outterRadius,
                            totalAngle,
                            totalAngle + angle
                        );
                        return;
                    }

                    totalAngle += angle;
                    index++;
                }
            }

        }

        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries (
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            yield break;
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

        private static double CalculateRayLength(
            double width,
            double height,
            double angle
        )
        {
            var theta = angle * Math.PI / 180;

            var cx = width / 2;
            var cy = height / 2;

            var left = 0;
            var right = width;
            var top = height;
            var bottom = 0;

            var dx = Math.Cos(theta);
            var dy = Math.Sin(theta);

            var intersectionDistance = double.MaxValue;

            if (dx != 0)
            {
                if (dx > 0)
                {
                    var t = (right - cx) / dx;
                    var y = cy + t * dy;
                    if (y >= bottom && y <= top)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
                else
                {
                    var t = (left - cx) / dx;
                    var y = cy + t * dy;
                    if (y >= bottom && y <= top)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
            }

            if (dy != 0)
            {
                if (dy > 0)
                {
                    var t = (top - cy) / dy;
                    var x = cx + t * dx;
                    if (x >= left && x <= right)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
                else
                {
                    var t = (bottom - cy) / dy;
                    var x = cx + t * dx;
                    if (x >= left && x <= right)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
            }

            return intersectionDistance;
        }

        #endregion

    }
}
