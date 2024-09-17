using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class PieSeries
        : RadialValueProviderSegmentsSeriesBase<PieSeriesSegment>
    {
        #region Structs
        private struct PieSeriesSegmentInfo
        {
            public double StartAngle { get; set; }

            public double Angle { get; set; }

            public FormattedText Title { get; set; }
        }
        #endregion

        #region Fields
        private Dictionary<PieSeriesSegment, PieSeriesSegmentInfo> _segmentInfos;
        #endregion

        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(PieSeries), new PropertyMetadata(5d));
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

        #region Events
        public event GeneratingTitleEventHandler GeneratingTitle;
        #endregion

        #region Overrides

        #region OnRenderBegin
        protected override void OnRenderBegin(IDrawingContext drawingContext, IChartContext chartContext)
        {
            base.OnRenderBegin(drawingContext, chartContext);

            _segmentInfos = new Dictionary<PieSeriesSegment, PieSeriesSegmentInfo>();

            var chartPanel = chartContext.ChartPanel;
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

                var title = generatingTitleArgs.Title;
                _segmentInfos[segment] = new PieSeriesSegmentInfo()
                {
                    StartAngle = totalAngle,
                    Angle = angle,
                    Title = title == null
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
            var chartPanel = chartContext.ChartPanel;
            var coordinates = chartContext.Coordinates;

            var areaWidth = chartContext.AreaWidth - Spacing * 2;
            var areaHeight = chartContext.AreaHeight - Spacing * 2;

            var radius = Math.Min(areaWidth, areaHeight) / 2;
            var centerX = areaWidth / 2 + Spacing;
            var centerY = areaHeight / 2 + Spacing;

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
                        radius,
                        startAngle,
                        startAngle + angle);
            }

            foreach (var segmentInfo in _segmentInfos)
            {
                var segment = segmentInfo.Key;
                var startAngle = segmentInfo.Value.StartAngle * animationProgress;
                var angle = Math.Round(segmentInfo.Value.Angle, 2) * animationProgress;

                var formattedText = segmentInfo.Value.Title;

                if(formattedText == null)
                {
                    continue;
                }

                var currentAngle = startAngle + angle / 2;
                var radian = (currentAngle - 90) * Math.PI / 180.0;
                var rayLength = CalculateRayLength(formattedText.Width, formattedText.Height, currentAngle + 90);

                var halfPoint = new Point(
                    centerX + (radius + Spacing + rayLength) * Math.Cos(radian),
                    centerY + (radius + Spacing + rayLength) * Math.Sin(radian)
                );

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
        #endregion

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
