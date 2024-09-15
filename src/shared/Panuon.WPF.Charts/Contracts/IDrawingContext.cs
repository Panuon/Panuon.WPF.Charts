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

        void DrawGeometry(Brush stroke,
            double strokeThickness,
            Brush fill,
            Geometry geometry);

        void DrawText(FormattedText text,
            double offsetX,
            double offsetY);

        void DrawText(FormattedText text,
            Brush fill,
            Brush stroke,
            double strokeThickness,
            double offsetX,
            double offsetY);

        void DrawRectangle(Brush stroke,
            double strokeThickness,
            Brush fill,
            double startX,
            double startY,
            double width,
            double height);

        void DrawArc(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            double centerX,
            double centerY,
            double radius,
            double startAngle,
            double endAngle
        );

        void DrawArc(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            double centerX,
            double centerY,
            double innerRadius,
            double outterRadius,
            double startAngle,
            double endAngle
        );
    }
}
