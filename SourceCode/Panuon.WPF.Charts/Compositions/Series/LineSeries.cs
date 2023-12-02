using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class LineSeries
        : SeriesBase
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
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineSeries), new PropertyMetadata(Brushes.Black));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineSeries), new PropertyMetadata(1d));
        #endregion

        #region Radius
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(LineSeries), new PropertyMetadata(4d));
        #endregion

        #region ToggleStroke
        public Brush ToggleStroke
        {
            get { return (Brush)GetValue(ToggleStrokeProperty); }
            set { SetValue(ToggleStrokeProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeProperty =
            DependencyProperty.Register("ToggleStroke", typeof(Brush), typeof(LineSegment), new PropertyMetadata(null));
        #endregion

        #region ToggleStrokeThickness
        public double ToggleStrokeThickness
        {
            get { return (double)GetValue(ToggleStrokeThicknessProperty); }
            set { SetValue(ToggleStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeThicknessProperty =
            DependencyProperty.Register("ToggleStrokeThickness", typeof(double), typeof(LineSeries), new PropertyMetadata(1d));
        #endregion

        #region ToggleFill
        public Brush ToggleFill
        {
            get { return (Brush)GetValue(ToggleFillProperty); }
            set { SetValue(ToggleFillProperty, value); }
        }

        public static readonly DependencyProperty ToggleFillProperty =
            DependencyProperty.Register("ToggleFill", typeof(Brush), typeof(LineSeries), new PropertyMetadata(Brushes.Black));
        #endregion

        #region ToggleRadius
        public double ToggleRadius
        {
            get { return (double)GetValue(ToggleRadiusProperty); }
            set { SetValue(ToggleRadiusProperty, value); }
        }

        public static readonly DependencyProperty ToggleRadiusProperty =
            DependencyProperty.Register("ToggleRadius", typeof(double), typeof(LineSeries), new PropertyMetadata(2d));
        #endregion


        #endregion

        #region Overrides
        protected override void OnRendering(IDrawingContext drawingContext,
            ICanvasContext canvasContext,
            IEnumerable<ICoordinate> coordinates)
        {
            ICoordinate lastCoordinate = null;
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);
                var offsetX = canvasContext.GetOffsetX(coordinate.Index);
                var offsetY = canvasContext.GetOffsetY(value);

                if (lastCoordinate != null)
                {
                    var lastValue = lastCoordinate.GetValue(this);

                    drawingContext.DrawLine(Stroke, StrokeThickness,
                        canvasContext.GetOffsetX(lastCoordinate.Index),
                        canvasContext.GetOffsetY(lastValue),
                        offsetX,
                        offsetY);

                }

                drawingContext.DrawEllipse(ToggleStroke,
                    ToggleStrokeThickness,
                    ToggleFill,
                    ToggleRadius,
                    ToggleRadius,
                    offsetX,
                    offsetY);

                lastCoordinate = coordinate;
            }
        }
        #endregion
    }
}