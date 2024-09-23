using System.Collections.Generic;

namespace Panuon.WPF.Charts.Implements
{
    internal class RadialCoordinateImpl
        : IRadialCoordinate
    {
        #region Properties
        public string Title { get; set; }

        public int Index { get; set; }

        public double Offset { get; set; }

        public double StartAngle { get; set; }

        public double Angle { get; set; }
        #endregion

        #region Internal Properties
        internal Dictionary<IChartArgument, (double, double)> Angles { get; set; }

        internal Dictionary<IChartArgument, double> Values { get; set; }
        #endregion
        public (double, double) GetAngle(IChartArgument seriesOrSegment)
        {
            return Angles[seriesOrSegment];
        }

        public double GetValue(IChartArgument seriesOrSegment)
        {
            return Values[seriesOrSegment];
        }

    }
}
