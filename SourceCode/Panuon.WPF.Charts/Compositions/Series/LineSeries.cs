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

        #endregion

        #region Overrides
        protected override void OnRendering(IDrawingContext drawingContext,
            IEnumerable<ICoordinate> coordinates)
        {
            ICoordinate lastCoordinate = null;
            foreach (var coordinate in coordinates)
            {
                if (lastCoordinate != null)
                {
                    var lastValue = lastCoordinate.GetValue(this);
                    var value = coordinate.GetValue(this);

                    drawingContext.DrawLine(Stroke, StrokeThickness,
                        drawingContext.GetOffsetX(lastCoordinate.Index),
                        drawingContext.GetOffsetY(lastValue),
                        drawingContext.GetOffsetX(coordinate.Index),
                        drawingContext.GetOffsetY(value));
                }

                lastCoordinate = coordinate;
            }
        }
        #endregion
    }
}