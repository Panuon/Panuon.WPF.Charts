using System.Windows;

namespace Panuon.WPF.Charts.Utils
{
    internal static class GridLengthUtil
    {
        public static double GetActualValue(
            GridLength length,
            double value,
            double autoPercent = 0.5
        )
        {
            if (length.IsAbsolute)
            {
                return length.Value;
            }
            else if (length.IsStar)
            {
                return (value * length.Value);
            }

            else
            {
                return value * autoPercent;
            }
        }
    }
}
