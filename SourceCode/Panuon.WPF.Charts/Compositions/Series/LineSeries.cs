using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class LineSeries
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
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LineSeries), new PropertyMetadata(null));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineSeries), new PropertyMetadata(Brushes.Black, OnRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region Radius
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(LineSeries), new PropertyMetadata(4d, OnRenderPropertyChanged));
        #endregion

        #region ToggleStroke
        public Brush ToggleStroke
        {
            get { return (Brush)GetValue(ToggleStrokeProperty); }
            set { SetValue(ToggleStrokeProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeProperty =
            DependencyProperty.Register("ToggleStroke", typeof(Brush), typeof(LineSegment), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region ToggleStrokeThickness
        public double ToggleStrokeThickness
        {
            get { return (double)GetValue(ToggleStrokeThicknessProperty); }
            set { SetValue(ToggleStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeThicknessProperty =
            DependencyProperty.Register("ToggleStrokeThickness", typeof(double), typeof(LineSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region ToggleFill
        public Brush ToggleFill
        {
            get { return (Brush)GetValue(ToggleFillProperty); }
            set { SetValue(ToggleFillProperty, value); }
        }

        public static readonly DependencyProperty ToggleFillProperty =
            DependencyProperty.Register("ToggleFill", typeof(Brush), typeof(LineSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region ToggleRadius
        public double ToggleRadius
        {
            get { return (double)GetValue(ToggleRadiusProperty); }
            set { SetValue(ToggleRadiusProperty, value); }
        }

        public static readonly DependencyProperty ToggleRadiusProperty =
            DependencyProperty.Register("ToggleRadius", typeof(double), typeof(LineSeries), new PropertyMetadata(3d, OnRenderPropertyChanged));
        #endregion


        #endregion

        #region Overrides
        protected override void OnRendering(IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            var coordinates = chartContext.Coordinates;

            ICoordinate lastCoordinate = null;
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);
                var offsetX = coordinate.Offset;
                var offsetY = chartContext.GetOffset(value);

                if (lastCoordinate != null)
                {
                    var lastValue = lastCoordinate.GetValue(this);

                    drawingContext.DrawLine(Stroke, StrokeThickness,
                        lastCoordinate.Offset,
                        chartContext.GetOffset(lastValue),
                        offsetX,
                        offsetY);

                }

                var toggleFill = ToggleFill ?? ((ToggleStroke == null || ToggleStrokeThickness == 0) ? Stroke : null);

                drawingContext.DrawEllipse(ToggleStroke,
                    ToggleStrokeThickness,
                    toggleFill,
                    ToggleRadius,
                    ToggleRadius,
                    offsetX,
                    offsetY);

                lastCoordinate = coordinate;
            }
        }

        protected override void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffset(value);
                drawingContext.DrawEllipse(Stroke, 2, Brushes.White, 5, 5, coordinate.Offset, offsetY);
            }
        }
        #endregion
    }
}