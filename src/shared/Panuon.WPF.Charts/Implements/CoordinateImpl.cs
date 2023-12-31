﻿using System.Collections.Generic;

namespace Panuon.WPF.Charts.Implements
{
    internal class CoordinateImpl
        : ICoordinate
    {
        #region Properties
        public string Title { get; set; }

        public int Index { get; set; }

        public double Offset { get; set; }

        internal Dictionary<IChartValueProvider, double> Values { get; set; }
        #endregion

        public double GetValue(IChartValueProvider seriesOrSegment)
        {
            return Values[seriesOrSegment];
        }

    }
}
