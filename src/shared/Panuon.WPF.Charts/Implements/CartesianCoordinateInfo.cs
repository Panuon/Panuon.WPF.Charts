using System.Windows;

namespace Panuon.WPF.Charts
{
    internal class CartesianCoordinateInfo
    {
        public CartesianCoordinateInfo(
            Point point, 
            double value)
        {
            Point = point;
            Value = value;
        }

        public Point Point { get; set; }

        public double Value { get; set; }
    }
}
