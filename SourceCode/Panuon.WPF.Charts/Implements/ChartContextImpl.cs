using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Panuon.WPF.Charts
{
    internal class ChartContextImpl
        : IChartContext
    {
        #region Fields
        private double _deltaX;
        private double _minMaxDelta;
        #endregion

        #region Ctor
        internal ChartContextImpl(double areaWidth,
            double areaHeight,
            double minValue,
            double maxValue,
            IEnumerable<SeriesBase> series,
            IEnumerable<ICoordinate> coordinates)
        {
            AreaWidth = areaWidth;
            AreaHeight = areaHeight;
            MinValue = minValue;
            MaxValue = maxValue;
            Series = series;

            _deltaX = AreaWidth / coordinates.Count();

            _minMaxDelta = MaxValue - MinValue;

            Coordinates = coordinates;
        }
        #endregion

        #region Properties
        public double AreaWidth { get; }

        public double AreaHeight { get; }

        public double MinValue { get; }

        public double MaxValue { get; }

        public IEnumerable<ICoordinate> Coordinates { get; }

        public IEnumerable<SeriesBase> Series { get; }
        #endregion

        #region Methods
        public double GetOffset(double value)
        {
            return AreaHeight - AreaHeight * ((value - MinValue) / _minMaxDelta);
        }

        public double CalculateWidth(GridLength width)
        {
            if (width.IsAbsolute)
            {
                return width.Value;
            }
            else if (width.IsStar)
            {
                return (_deltaX * width.Value);
            }

            else
            {
                return _deltaX / 2;
            }
        }
        #endregion
    }
}
