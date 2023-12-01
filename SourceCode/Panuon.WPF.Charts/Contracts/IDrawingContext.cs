using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public interface IDrawingContext
    {

        double AreaWidth { get; }

        double AreaHeight { get; }

        double GetOffsetX(int index);

        double GetOffsetY(double value);

        void DrawLine(Brush stroke,
            double strokeThickness,
            double startX,
            double startY,
            double endX,
            double endY);

        void DrawGeometry(Geometry geometry,
            Brush stroke,
            double strokeThickness);

        void DrawText(FormattedText text,
            double offsetX,
            double offsetY);
    }
}
