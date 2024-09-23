using System.Collections.Generic;

namespace Panuon.WPF.Charts.Implements
{
    internal class RadialCoordinateImpl
        : IRadialCoordinate
    {
        #region Properties
        public string Title { get; set; }

        public int Index { get; set; }

        public double OffsetX { get; set; }

        public double StartAngle { get; set; }

        public double Angle { get; set; }

        internal Dictionary<IChartValueProvider, double> Values { get; set; }
        #endregion

        public double GetValue(IChartValueProvider seriesOrSegment)
        {
            return Values[seriesOrSegment];
        }

    }
}
