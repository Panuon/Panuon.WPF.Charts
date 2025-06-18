using System.Windows;

namespace Panuon.WPF.Charts
{
    internal class CartesianCoordinateInfo
    {
        public CartesianCoordinateInfo(
            Point point, 
            decimal value)
        {
            Point = point;
            Value = value;
        }

        public Point Point { get; set; }

        public decimal Value { get; set; }
    }
}
