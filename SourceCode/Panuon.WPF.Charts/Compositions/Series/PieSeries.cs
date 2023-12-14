using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        #region Overrides
        protected override void OnRendering(IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            var coordinates = chartContext.Coordinates;

            var radius = Math.Min(chartContext.AreaWidth, chartContext.AreaHeight) / 2;
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
        }

        protected override void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinates = chartContext.Coordinates;
                var radius = Math.Min(chartContext.AreaWidth, chartContext.AreaHeight) / 2;
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
                        centerX,
                        centerY,
                        radius,
                        totalAngle,
                        totalAngle + angle))
                    {
                        drawingContext.DrawArc(Brushes.Gold,
                                4,
                                null,
                                centerX,
                                centerY,
                                radius,
                                totalAngle,
                                totalAngle + angle);
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
