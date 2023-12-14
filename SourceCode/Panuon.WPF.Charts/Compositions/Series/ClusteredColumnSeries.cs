using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ClusteredColumnSeries
        : SegmentsSeriesBase<ClusteredColumnSeriesSegment>
    {
        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ClusteredColumnSeries), new PropertyMetadata(5d));
        #endregion

        #region Width
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(ClusteredColumnSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto), OnRenderPropertyChanged));
        #endregion

        #endregion

        #region Overrides
        protected override void OnRendering(IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            var coordinates = chartContext.Coordinates;

            foreach (var coordinate in coordinates)
            {
                var offsetX = coordinate.Offset;
                var totalWidth = chartContext.CalculateWidth(Width);

                var left = offsetX - totalWidth / 2;
                var barWidth = CalculateBarWidth(totalWidth);
                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffset(value);

                    drawingContext.DrawRectangle(segment.Stroke,
                        segment.StrokeThickness,
                        segment.Fill,
                        left,
                        offsetY,
                        barWidth,
                        chartContext.AreaHeight - offsetY);

                    left += (barWidth + Spacing);
                }
            }
        }

        protected override void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);

                var offsetX = coordinate.Offset;
                var totalWidth = chartContext.CalculateWidth(Width);
                var left = offsetX - totalWidth / 2;
                var barWidth = CalculateBarWidth(totalWidth);

                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffset(value);
                    drawingContext.DrawEllipse(segment.Fill,
                        2,
                        Brushes.White,
                        5,
                        5,
                        left + barWidth / 2, offsetY);
                    tooltips.Add(new SeriesTooltip(segment.Fill, coordinate.Title, value.ToString()));
                    left += (barWidth + Spacing);
                }
            }
        }
        #endregion

        #region Functions
        private double CalculateBarWidth(double totalWidth)
        {
            return (totalWidth - (Segments.Count - 1) * Spacing) / Segments.Count;
        }
        #endregion

    }
}
