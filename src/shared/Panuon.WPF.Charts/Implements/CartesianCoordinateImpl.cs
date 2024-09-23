using System.Collections.Generic;

namespace Panuon.WPF.Charts.Implements
{
    internal class CartesianCoordinateImpl
        : ICartesianCoordinate
    {
        #region Properties
        public string Label { get; set; }

        public int Index { get; set; }

        public double Offset { get; set; }

        internal Dictionary<IChartArgument, double> Values { get; set; }
        #endregion

        public double GetValue(IChartArgument seriesOrSegment)
        {
            return Values[seriesOrSegment];
        }

    }
}
