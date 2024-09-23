using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Panuon.WPF.Charts.Implements
{
    internal class CartesianChartContextImpl
        : ChartContextImplBase, ICartesianChartContext
    {
        #region Fields
        #endregion

        #region Ctor
        internal CartesianChartContextImpl(CartesianChart chart)
            : base(chart)
        {
        }
        #endregion

        #region Properties
        public new CartesianChart Chart => (CartesianChart)base.Chart;

        public double MinValue => Chart.ActualMinValue;

        public double MaxValue => Chart.ActualMaxValue;

        public IEnumerable<ICartesianCoordinate> Coordinates => Chart.Coordinates;
        #endregion

        #region Methods
        public override ICoordinate RetrieveCoordinate(Point position)
        {
            if (position.X < 0 ||
                position.X > AreaWidth)
            {
                return null;
            }
            var leftCoordinate = Coordinates.LastOrDefault(x => x.OffsetX <= position.X);
            var rightCoordinate = Coordinates.FirstOrDefault(y => y.OffsetX >= position.X);
            if (leftCoordinate == null &&
                rightCoordinate == null)
            {
                return null;
            }
            if (leftCoordinate == null)
            {
                return rightCoordinate;
            }
            if (rightCoordinate == null)
            {
                return leftCoordinate;
            }
            return Math.Abs(leftCoordinate.OffsetX - position.X) <= Math.Abs(rightCoordinate.OffsetX - position.X)
                ? leftCoordinate
                : rightCoordinate;
        }

        ICartesianCoordinate ICartesianChartContext.RetrieveCoordinate(Point position)
        {
            return (ICartesianCoordinate)RetrieveCoordinate(position);
        }

        public double GetOffsetY(double value)
        {
            var minMaxDelta = MaxValue - MinValue;
            return AreaHeight - AreaHeight * ((value - MinValue) / minMaxDelta);
        }


        #endregion
    }
}
