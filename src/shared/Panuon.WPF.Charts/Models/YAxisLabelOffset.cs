using System;

namespace Panuon.WPF.Charts
{
    internal class YAxisLabelOffset
    {
        public YAxisLabelOffset(
            string label,
            double value,
            Func<double> getOffsetY)
        {
            Label = label;
            Value = value;
            GetOffsetY = getOffsetY;
        }

        public string Label { get; set; }

        public double Value { get; set; }

        public Func<double> GetOffsetY { get; set; }
    }
}
