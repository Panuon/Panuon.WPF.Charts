using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DrawingHorizontalGridLineRoutedEventArgs
        : RoutedEventArgs
    {
        public DrawingHorizontalGridLineRoutedEventArgs(
            RoutedEvent @event,
            double value,
            Brush stroke,
            double? strokeThickness,
            DoubleCollection dashArray)
            : base(@event)
        {
            Value = value;
            Stroke = stroke;
            StrokeThickness = strokeThickness;
            DashArray = dashArray;
        }

        public double Value { get; }

        public double? StrokeThickness { get; set; }

        public Brush Stroke { get; set; }

        public DoubleCollection DashArray { get; set; }

    }
}
