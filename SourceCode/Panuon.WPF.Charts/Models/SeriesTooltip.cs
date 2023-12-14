using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class SeriesTooltip
    {
        public SeriesTooltip(Brush highlightBrush,
            string title)
        {
            HighlightBrush = highlightBrush;
            Title = title;
        }

        public SeriesTooltip(Brush highlightBrush,
            string title,
            string value)
            : this(highlightBrush, title)
        {
            Value = value;
        }

        public string Title { get; set; }

        public string Value { get; set; }

        public Brush HighlightBrush { get; set; }

    }

}
