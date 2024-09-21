using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class SeriesLegendEntry
    {
        public SeriesLegendEntry(
            Brush brush,
            string title
        )
        {
            Brush = brush;
            Title = title;
        }

        public SeriesLegendEntry(
            Brush highlightBrush,
            string title,
            string value
        )
            : this(highlightBrush, title)
        {
            Value = value;
        }

        public string Title { get; set; }

        public string Value { get; set; }

        public Brush Brush { get; set; }

    }

}
