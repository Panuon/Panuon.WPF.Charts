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

        public void DrawText(FormattedText text,
            Brush fill,
            Brush stroke,
            double strokeThickness,
            double offsetX,
            double offsetY)
        {
            _drawingContext.DrawGeometry(fill,
                new Pen(stroke, strokeThickness),
                text.BuildGeometry(new Point(offsetX, offsetY)));
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
            double outterRadius,
            double startAngle,
            double endAngle)
        {
            var angleRadians = (startAngle - 90) * Math.PI / 180.0;
            var endAngleRadians = (endAngle - 90) * Math.PI / 180.0;

            var startPoint = new Point(centerX + outterRadius * Math.Cos(angleRadians), centerY + outterRadius * Math.Sin(angleRadians));
            var endPoint = new Point(centerX + outterRadius * Math.Cos(endAngleRadians), centerY + outterRadius * Math.Sin(endAngleRadians));

            var figure = new PathFigure
            {
                StartPoint = startPoint,
            };
            figure.Segments.Add(new ArcSegment
            {
                Point = endPoint,
                Size = new Size(outterRadius, outterRadius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = (endAngle - startAngle) > 180,
            });
            figure.Segments.Add(new LineSegment
            {
                Point = new Point(centerX, centerY),
                IsSmoothJoin = true,
            });
            figure.Segments.Add(new LineSegment
            {
                Point = startPoint,
                IsSmoothJoin = true,
            });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            _drawingContext.DrawGeometry(fill, 
                GetPen(stroke, strokeThickness), 
                geometry);
        }

        public void DrawArc(Brush stroke,
           double strokeThickness,
           Brush fill,
           double centerX,
           double centerY,
           double innerRadius,
           double outterRadius,
           double startAngle,
           double endAngle)
        {
            var angleRadians = (startAngle - 90) * Math.PI / 180.0;
            var endAngleRadians = (endAngle - 90) * Math.PI / 180.0;

            var outterStartPoint = new Point(centerX + outterRadius * Math.Cos(angleRadians), centerY + outterRadius * Math.Sin(angleRadians));
            var outterEndPoint = new Point(centerX + outterRadius * Math.Cos(endAngleRadians), centerY + outterRadius * Math.Sin(endAngleRadians));
            var innerStartPoint = new Point(centerX + innerRadius * Math.Cos(angleRadians), centerY + innerRadius * Math.Sin(angleRadians));
            var innerEndPoint = new Point(centerX + innerRadius * Math.Cos(endAngleRadians), centerY + innerRadius * Math.Sin(endAngleRadians));

            var figure = new PathFigure
            {
                StartPoint = outterStartPoint,
            };
            figure.Segments.Add(new ArcSegment
            {
                Point = outterEndPoint,
                Size = new Size(outterRadius, outterRadius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = (endAngle - startAngle) > 180,
            });
            figure.Segments.Add(new LineSegment
            {
                Point = innerEndPoint,
                IsSmoothJoin = true,
            });
            figure.Segments.Add(new ArcSegment
            {
                Point = innerStartPoint,
                Size = new Size(innerRadius, innerRadius),
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = (endAngle - startAngle) > 180,
            });
            figure.Segments.Add(new LineSegment
            {
                Point = outterStartPoint,
                IsSmoothJoin = true,
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
