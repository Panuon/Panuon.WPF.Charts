using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public interface IDrawingContext
    {
        void DrawLine(Brush stroke, double strokeThickness, Point startPoint, Point endPoint, DoubleCollection dashArray = null);

        void DrawEllipse(Brush stroke, double strokeThickness, Brush fill, Size size, Point centerPoint, DoubleCollection dashArray = null);

        void DrawRectangle(Brush stroke, double strokeThickness, Brush fill, Size size, Point centerPoint, Size? radius = null, DoubleCollection dashArray = null);

        void DrawTriangle(Brush stroke, double strokeThickness, Brush fill, Size size, Point centerPoint, DoubleCollection dashArray = null);

        void DrawCross(Brush stroke, double strokeThickness, Brush fill, Size size, Point startPoint, DoubleCollection dashArray = null);

        void DrawDiamond(Brush stroke, double strokeThickness, Brush fill, Size size, Point centerPoint, DoubleCollection dashArray = null);

        void DrawStar(Brush stroke, double strokeThickness, Brush fill, Size size, Point centerPoint, DoubleCollection dashArray = null);

        void DrawArrowUp(Brush stroke, double strokeThickness, Brush fill, Size size, Point targetPoint, DoubleCollection dashArray = null);

        void DrawArrowDown(Brush stroke, double strokeThickness, Brush fill, Size size, Point targetPoint, DoubleCollection dashArray = null);

        void DrawPlus(Brush stroke, double strokeThickness, Brush fill, Size size, Point centerPoint, DoubleCollection dashArray = null);

        void DrawGeometry(Brush stroke, double strokeThickness, Brush fill, Geometry geometry, DoubleCollection dashArray = null);

        void DrawText(FormattedText text, Point startPoint);

        void DrawText(FormattedText text, Point startPoint, Brush fill, Brush stroke, double strokeThickness);

        void DrawArc(Brush stroke, double strokeThickness, Brush fill, Point centerPoint, double radius, double startAngle, double endAngle);

        void DrawArc(Brush stroke, double strokeThickness, Brush fill, Point centerPoint, double innerRadius, double outterRadius, double startAngle, double endAngle);

        void PushTranslate(double offsetX, double offsetY);

        void PushClip(double offsetX, double offsetY, double width, double height);
    }
}
