using System;
using System.Windows;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace Panuon.WPF.Charts
{
    internal class WPFDrawingContextImpl
        : IDrawingContext
    {
        #region Fields
        private DrawingContext _drawingContext;
        #endregion

        #region Ctor
        internal WPFDrawingContextImpl(DrawingContext drawingContext)
        {
            _drawingContext = drawingContext;
        }
        #endregion

        #region Methods
        public void DrawGeometry(Geometry geometry, Brush stroke, double strokeThickness)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Brush stroke,
            double strokeThickness,
            double startX,
            double startY,
            double endX,
            double endY)
        {
            var pen = stroke == null || strokeThickness <= 0
                ? null
                : new Pen(stroke, strokeThickness);
            pen?.Freeze();
            _drawingContext.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
        }

        public void DrawEllipse(Brush stroke,
            double strokeThickness,
            Brush fill,
            double radiusX,
            double radiusY,
            double offsetX,
            double offsetY)
        {
            var pen = stroke == null || strokeThickness <= 0
                ? null
                : new Pen(stroke, strokeThickness);
            pen?.Freeze();
            _drawingContext.DrawEllipse(fill, pen, new Point(offsetX, offsetY), radiusX, radiusY);
        }

        public void DrawText(FormattedText text, 
            double offsetX,
            double offsetY)
        {
            _drawingContext.DrawText(text, new Point(offsetX, offsetY));
        }

        public void DrawRectangle(Brush stroke,
            double strokeThickness,
            Brush fill,
            double startX,
            double startY,
            double width,
            double height)
        {
            var pen = stroke == null || strokeThickness <= 0
                ? null
                : new Pen(stroke, strokeThickness);
            pen?.Freeze();
            _drawingContext.DrawRectangle(fill, pen, new Rect(startX, startY, width, height));
        }
        #endregion
    }
}
