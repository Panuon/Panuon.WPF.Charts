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

        private int _totalIndex;
        private double _minValue;
        private double _maxValue;
        #endregion

        #region Ctor
        internal WPFDrawingContextImpl(DrawingContext drawingContext,
            double areaWidth,
            double areaHeight,
            int totalIndex,
            double minValue,
            double maxValue)
        {
            _drawingContext = drawingContext;
            AreaWidth = areaWidth;
            AreaHeight = areaHeight;
            _totalIndex = totalIndex;
            _minValue = minValue;
            _maxValue = maxValue;
        }
        #endregion

        #region Properties
        public double AreaWidth { get; set; }

        public double AreaHeight { get; set; }
        #endregion

        #region Methods
        public double GetOffsetX(int index)
        {
            var deltaX = AreaWidth / _totalIndex;
            return (index + 0.5) * deltaX;
        }

        public double GetOffsetY(double value)
        {
            return AreaHeight - AreaHeight * ((value - _minValue) / (_maxValue - _minValue));
        }

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
            var pen = new Pen(stroke, strokeThickness);
            pen.Freeze();
            _drawingContext.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
        }

        public void DrawText(FormattedText text, 
            double offsetX,
            double offsetY)
        {
            _drawingContext.DrawText(text, new Point(offsetX, offsetY));
        }

        #endregion
    }
}
