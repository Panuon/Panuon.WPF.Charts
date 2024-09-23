using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class SeriesLegendEntry
    {
        public SeriesLegendEntry(
            Brush brush,
            string label
        )
        {
            Brush = brush;
            Label = label;
        }

        public SeriesLegendEntry(
            Brush highlightBrush,
            string label,
            string value
        )
            : this(highlightBrush, label)
        {
            Value = value;
        }

        public string Label { get; set; }

        public string Value { get; set; }

        public Brush Brush { get; set; }

    }

}
