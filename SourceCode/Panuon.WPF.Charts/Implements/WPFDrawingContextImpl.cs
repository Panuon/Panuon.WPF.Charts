using System;
using System.Windows;
using System.Windows.Media;

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
        public void DrawGeometry(Brush stroke, 
            double strokeThickness,
            Brush fill,
            Geometry geometry)
        {
            _drawingContext.DrawGeometry(fill,
                GetPen(stroke, strokeThickness),
                geometry);
        }

        public void DrawLine(Brush stroke,
            double strokeThickness,
            double startX,
            double startY,
            double endX,
            double endY)
        {
            _drawingContext.DrawLine(GetPen(stroke, strokeThickness),
                new Point(startX, startY), 
                new Point(endX, endY));
        }

        public void DrawEllipse(Brush stroke,
            double strokeThickness,
            Brush fill,
            double radiusX,
            double radiusY,
            double offsetX,
            double offsetY)
        {
            _drawingContext.DrawEllipse(fill, 
                GetPen(stroke, strokeThickness), 
                new Point(offsetX, offsetY), 
                radiusX, 
                radiusY);
        }

        public void DrawText(FormattedText text, 
            double offsetX,
            double offsetY)
        {
            _drawingContext.DrawText(text, 
                new Point(offsetX, offsetY));
        }

        public void DrawRectangle(Brush stroke,
            double strokeThickness,
            Brush fill,
            double startX,
            double startY,
            double width,
            double height)
        {
            _drawingContext.DrawRectangle(fill, 
                GetPen(stroke, strokeThickness), 
                new Rect(startX, startY, width, height));
        }

        public void DrawArc(Brush stroke,
            double strokeThickness,
            Brush fill,
            double centerX,
            double centerY,
            double radius,
            double startAngle,
            double endAngle)
        {
            var angleRadians = (startAngle - 90) * Math.PI / 180.0;
            var endAngleRadians = (endAngle - 90) * Math.PI / 180.0;

            var startPoint = new Point(centerX + radius * Math.Cos(angleRadians), centerY + radius * Math.Sin(angleRadians));
            var endPoint = new Point(centerX + radius * Math.Cos(endAngleRadians), centerY + radius * Math.Sin(endAngleRadians));

            var figure = new PathFigure
            {
                StartPoint = startPoint,

            };
            figure.Segments.Add(new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = (endAngle - startAngle) > 180,
            });
            figure.Segments.Add(new LineSegment
            {
                Point = new Point(centerX, centerY),
            });
            figure.Segments.Add(new LineSegment
            {
                Point = startPoint,
            });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            _drawingContext.DrawGeometry(fill, 
                GetPen(stroke, strokeThickness), 
                geometry);
        }
        #endregion

        #region Functions
        private Pen GetPen(Brush stroke,
            double strokeThickness)
        {
            if(stroke == null
                || !(strokeThickness > 0))
            {
                return null;
            }
            var pen = new Pen(stroke, strokeThickness);
            pen.Freeze();
            return pen;
        }
        #endregion
    }
}
