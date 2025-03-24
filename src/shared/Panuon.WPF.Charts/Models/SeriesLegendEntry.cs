using System;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class SeriesLegendEntry
    {
        public SeriesLegendEntry(
            string title,
            MarkerShape markerShape,
            Brush markerStroke,
            double markerStrokeThickness,
            Brush markerFill)
        {
            Title = title;
            MarkerShape = markerShape;
            MarkerStroke = markerStroke;
            MarkerStrokeThickness = markerStrokeThickness;
            MarkerFill = markerFill;
        }

        public SeriesLegendEntry(
            string title,
            Action<DrawingMarkerEventArgs> onDrawingMarker)
        {
            Title = title;
            OnDrawingMarker = onDrawingMarker;
        }

        public string Title { get; set; }

        public MarkerShape MarkerShape { get; set; }

        public Brush MarkerStroke { get; set; }

        public double MarkerStrokeThickness { get; set; }

        public Brush MarkerFill { get; set; }

        public Action<DrawingMarkerEventArgs> OnDrawingMarker { get; set; }

    }

}
