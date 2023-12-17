using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ColumnSeries
        : ValueProviderSeriesBase
    {
        #region Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ColumnSeries), new PropertyMetadata(Brushes.Black, OnRenderPropertyChanged));

        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ColumnSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ColumnSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region Width
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(ColumnSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto), OnRenderPropertyChanged));
        #endregion

        #endregion

        #region Overrides
        protected override void OnRendering(IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            var coordinates = chartContext.Coordinates;

            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);
                var columnWidth = chartContext.CalculateWidth(Width);
                var offsetX = coordinate.Offset;
                var offsetY = chartContext.GetOffset(value);

                drawingContext.DrawRectangle(Stroke, StrokeThickness, Fill, offsetX - columnWidth / 2, offsetY, columnWidth, chartContext.AreaHeight - offsetY);
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

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffset(value);
                drawingContext.DrawEllipse(Fill, 2, Brushes.White, 5, 5, coordinate.Offset, offsetY);
                tooltips.Add(new SeriesTooltip(Fill, Title ?? coordinate.Title, value.ToString()));
            }

        }
        #endregion
    }
}