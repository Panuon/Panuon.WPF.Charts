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
        public void DrawGeometry(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Geometry geometry,
            DoubleCollection dashArray = null
        )
        {
            _drawingContext.DrawGeometry(
                fill,
                GetPen(stroke, strokeThickness, dashArray),
                geometry);
        }

        public void DrawLine(
            Brush stroke,
            double strokeThickness,
            Point startPoint,
            Point endPoint,
            DoubleCollection dashArray = null
        )
        {
            _drawingContext.DrawLine(
                GetPen(stroke, strokeThickness, dashArray),
                startPoint,
                endPoint);
        }

        public void DrawEllipse(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            DoubleCollection dashArray = null)
        {
            _drawingContext.DrawEllipse(
                fill,
                GetPen(stroke, strokeThickness, dashArray),
                centerPoint,
                Math.Max(0, size.Width / 2),
                Math.Max(0, size.Height / 2)
            );
        }

        public void DrawRectangle(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            Size? radius = null,
            DoubleCollection dashArray = null)
        {
            _drawingContext.DrawRoundedRectangle(
                fill,
                GetPen(stroke, strokeThickness, dashArray),
                new Rect(centerPoint.X - size.Width / 2, centerPoint.Y - size.Height / 2, size.Width, size.Height),
                radius?.Width ?? 0,
                radius?.Height ?? 0
            );
        }

        public void DrawTriangle(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            DoubleCollection dashArray = null)
        {
            var top = new Point(centerPoint.X, centerPoint.Y - size.Height / 2);
            var bottomLeft = new Point(centerPoint.X - size.Width / 2, centerPoint.Y + size.Height / 2);
            var bottomRight = new Point(centerPoint.X + size.Width / 2, centerPoint.Y + size.Height / 2);

            var triangleGeometry = new PathGeometry();
            var figure = new PathFigure
            {
                StartPoint = top,
                IsClosed = true
            };

            figure.Segments.Add(new LineSegment(bottomLeft, true));
            figure.Segments.Add(new LineSegment(bottomRight, true));
            triangleGeometry.Figures.Add(figure);

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, triangleGeometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = GetPen(stroke, strokeThickness, dashArray);
                _drawingContext.DrawGeometry(null, pen, triangleGeometry);
            }
        }

        public void DrawDiamond(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            DoubleCollection dashArray = null)
        {
            var top = new Point(centerPoint.X, centerPoint.Y - size.Height / 2);
            var right = new Point(centerPoint.X + size.Width / 2, centerPoint.Y);
            var bottom = new Point(centerPoint.X, centerPoint.Y + size.Height / 2);
            var left = new Point(centerPoint.X - size.Width / 2, centerPoint.Y);

            var diamondGeometry = new PathGeometry();
            var figure = new PathFigure
            {
                StartPoint = top,
                IsClosed = true,
                IsFilled = fill != null
            };

            figure.Segments.Add(new LineSegment(right, true));
            figure.Segments.Add(new LineSegment(bottom, true));
            figure.Segments.Add(new LineSegment(left, true));
            diamondGeometry.Figures.Add(figure);

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, diamondGeometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = GetPen(stroke, strokeThickness, dashArray);
                _drawingContext.DrawGeometry(null, pen, diamondGeometry);
            }
        }

        public void DrawCross(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            DoubleCollection dashArray = null)
        {
            var line1Start = new Point(centerPoint.X - size.Width / 2, centerPoint.Y - size.Height / 2);
            var line1End = new Point(centerPoint.X + size.Width / 2, centerPoint.Y + size.Height / 2);
            var line2Start = new Point(centerPoint.X + size.Width / 2, centerPoint.Y - size.Height / 2);
            var line2End = new Point(centerPoint.X - size.Width / 2, centerPoint.Y + size.Height / 2);

            var geometry = new GeometryGroup();
            geometry.Children.Add(new LineGeometry(line1Start, line1End));
            geometry.Children.Add(new LineGeometry(line2Start, line2End));

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, geometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = new Pen(stroke, strokeThickness) { DashStyle = dashArray != null ? new DashStyle(dashArray, 0) : DashStyles.Solid };
                _drawingContext.DrawGeometry(null, pen, geometry);
            }
        }

        public void DrawStar(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            DoubleCollection dashArray = null)
        {
            var points = new PointCollection();
            var outerRadius = Math.Min(size.Width, size.Height) / 2;
            var innerRadius = outerRadius * 0.4;

            for (var i = 0; i < 10; i++)
            {
                var angle = Math.PI * 2 * i / 10 - Math.PI / 2;
                var radius = i % 2 == 0 ? outerRadius : innerRadius;
                points.Add(new Point(
                    centerPoint.X + radius * Math.Cos(angle),
                    centerPoint.Y + radius * Math.Sin(angle)));
            }

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = points[0], IsClosed = true };

            for (var i = 1; i < points.Count; i++)
            {
                figure.Segments.Add(new LineSegment(points[i], true));
            }

            geometry.Figures.Add(figure);

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, geometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = GetPen(stroke, strokeThickness, dashArray);
                _drawingContext.DrawGeometry(null, pen, geometry);
            }
        }

        public void DrawArrowUp(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point targetPoint,
            DoubleCollection dashArray = null)
        {
            var tip = targetPoint;
            var left = new Point(tip.X - size.Width / 2, tip.Y + size.Height);
            var right = new Point(tip.X + size.Width / 2, tip.Y + size.Height);

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = tip, IsClosed = true };
            figure.Segments.Add(new LineSegment(left, true));
            figure.Segments.Add(new LineSegment(right, true));
            geometry.Figures.Add(figure);

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, geometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = GetPen(stroke, strokeThickness, dashArray);
                _drawingContext.DrawGeometry(null, pen, geometry);
            }
        }

        public void DrawArrowDown(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point targetPoint,
            DoubleCollection dashArray = null)
        {
            var tip = targetPoint;
            var left = new Point(tip.X - size.Width / 2, tip.Y - size.Height);
            var right = new Point(tip.X + size.Width / 2, tip.Y - size.Height);

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = tip, IsClosed = true };
            figure.Segments.Add(new LineSegment(left, true));
            figure.Segments.Add(new LineSegment(right, true));
            geometry.Figures.Add(figure);

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, geometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = GetPen(stroke, strokeThickness, dashArray);
                _drawingContext.DrawGeometry(null, pen, geometry);
            }
        }

        public void DrawPlus(
            Brush stroke,
            double strokeThickness,
            Brush fill,
            Size size,
            Point centerPoint,
            DoubleCollection dashArray = null)
        {
            var verticalLine = new LineGeometry(
                new Point(centerPoint.X, centerPoint.Y - size.Height / 2),
                new Point(centerPoint.X, centerPoint.Y + size.Height / 2));

            var horizontalLine = new LineGeometry(
                new Point(centerPoint.X - size.Width / 2, centerPoint.Y),
                new Point(centerPoint.X + size.Width / 2, centerPoint.Y));

            var geometry = new GeometryGroup();
            geometry.Children.Add(verticalLine);
            geometry.Children.Add(horizontalLine);

            if (fill != null)
            {
                _drawingContext.DrawGeometry(fill, null, geometry);
            }

            if (stroke != null && strokeThickness > 0)
            {
                var pen = new Pen(stroke, strokeThickness) { DashStyle = dashArray != null ? new DashStyle(dashArray, 0) : DashStyles.Solid };
                _drawingContext.DrawGeometry(null, pen, geometry);
            }
        }

        public void DrawText(
            FormattedText text,
            Point startPoint)
        {
            _drawingContext.DrawText(
                text,
                startPoint);
        }

        public void DrawText(
            FormattedText text,
            Point startPoint,
            Brush fill,
            Brush stroke,
            double strokeThickness)
        {
            _drawingContext.DrawGeometry(fill,
                new Pen(stroke, strokeThickness),
                text.BuildGeometry(startPoint));
        }

        public void DrawArc(Brush stroke,
            double strokeThickness,
            Brush fill,
            Point centerPoint,
            double outterRadius,
            double startAngle,
            double endAngle)
        {
            var angleRadians = (startAngle - 90) * Math.PI / 180.0;
            var endAngleRadians = (endAngle - 90) * Math.PI / 180.0;

            var startPoint = new Point(centerPoint.X + outterRadius * Math.Cos(angleRadians), centerPoint.Y + outterRadius * Math.Sin(angleRadians));
            var endPoint = new Point(centerPoint.X + outterRadius * Math.Cos(endAngleRadians), centerPoint.Y + outterRadius * Math.Sin(endAngleRadians));

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
                Point = new Point(centerPoint.X, centerPoint.Y),
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
           Point centerPoint,
           double innerRadius,
           double outterRadius,
           double startAngle,
           double endAngle)
        {
            var angleRadians = (startAngle - 90) * Math.PI / 180.0;
            var endAngleRadians = (endAngle - 90) * Math.PI / 180.0;

            var outterStartPoint = new Point(centerPoint.X + outterRadius * Math.Cos(angleRadians), centerPoint.Y + outterRadius * Math.Sin(angleRadians));
            var outterEndPoint = new Point(centerPoint.X + outterRadius * Math.Cos(endAngleRadians), centerPoint.Y + outterRadius * Math.Sin(endAngleRadians));
            var innerStartPoint = new Point(centerPoint.X + innerRadius * Math.Cos(angleRadians), centerPoint.Y + innerRadius * Math.Sin(angleRadians));
            var innerEndPoint = new Point(centerPoint.X + innerRadius * Math.Cos(endAngleRadians), centerPoint.Y + innerRadius * Math.Sin(endAngleRadians));

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

        public void PushTranslate(double offsetX, double offsetY)
        {
            _drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));
        }

        public void PushClip(double offsetX, double offsetY, double width, double height)
        {
            _drawingContext.PushClip(new RectangleGeometry(new Rect(offsetX, offsetY, width, height)));
        }
        #endregion

        #region Functions
        private Pen GetPen(Brush stroke,
            double strokeThickness,
            DoubleCollection dashArray = null)
        {
            if (stroke == null
                || !(strokeThickness > 0))
            {
                return null;
            }
            var pen = new Pen(stroke, strokeThickness)
            {
                LineJoin = PenLineJoin.Round
            };
            if (dashArray != null)
            {
                pen.DashStyle = new DashStyle(dashArray, 0);
            }
            pen.Freeze();
            return pen;
        }
        #endregion
    }
}
