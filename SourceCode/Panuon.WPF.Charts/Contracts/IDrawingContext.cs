using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public interface IDrawingContext
    {
        void DrawLine(Brush stroke,
            double strokeThickness,
            double startX,
            double startY,
            double endX,
            double endY);

        void DrawEllipse(Brush stroke,
            double strokeThickness,
            Brush fill,
            double radiusX,
            double radiusY,
            double startX,
            double startY);

        void DrawGeometry(Geometry geometry,
            Brush stroke,
            double strokeThickness);

        void DrawText(FormattedText text,
            double offsetX,
            double offsetY);
    }
}
