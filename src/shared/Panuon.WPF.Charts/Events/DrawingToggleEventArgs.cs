using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DrawingMarkerEventArgs
        : CancelEventArgs
    {

        public DrawingMarkerEventArgs(
            IDrawingContext drawingContext,
            SeriesBase series,
            ICartesianCoordinate coordinate,
            double value,
            Point centerPoint,
            double size,
            Brush stroke,
            double strokeThickness,
            Brush fill)
        {
            DrawingContext = drawingContext;
            Coordinate = coordinate;
            Value = value;
            Series = series;
            CenterPoint = centerPoint;
            Stroke = stroke;
            StrokeThickness = strokeThickness;
            Fill = fill;
            Size = size;
        }
        public IDrawingContext DrawingContext { get; }

        public ICartesianCoordinate Coordinate { get; }

        public double Value { get; }

        public SeriesBase Series { get; }

        public Point CenterPoint { get; set; }

        public double Size { get; set; }

        public Brush Stroke { get; set; }

        public double StrokeThickness { get; set; }

        public Brush Fill { get; set; }

    }
}
