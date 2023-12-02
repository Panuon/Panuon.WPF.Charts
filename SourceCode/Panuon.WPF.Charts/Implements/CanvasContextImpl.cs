using System;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    internal class CanvasContextImpl
        : ICanvasContext
    {
        #region Fields

        private int _totalIndex;
        private double _minValue;
        private double _maxValue;
        #endregion

        #region Ctor
        internal CanvasContextImpl(double areaWidth,
            double areaHeight,
            int totalIndex,
            double minValue,
            double maxValue)
        {
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
        #endregion
    }
}
