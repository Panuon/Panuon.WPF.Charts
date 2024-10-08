﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Panuon.WPF.Charts.Implements
{
    internal class CartesianChartContextImpl
        : ChartContextImplBase, ICartesianChartContext
    {
        #region Fields
        private CartesianChart _chart;
        #endregion

        #region Ctor
        internal CartesianChartContextImpl(CartesianChart chart)
            : base(chart)
        {
            _chart = chart;
        }
        #endregion

        #region Properties
        public bool SwapXYAxes => _chart.SwapXYAxes;

        public double MinValue => _chart.ActualMinValue;

        public double MaxValue => _chart.ActualMaxValue;

        public IEnumerable<ICartesianCoordinate> Coordinates => _chart.Coordinates;
        #endregion

        #region Methods
        public override ICoordinate RetrieveCoordinate(Point position)
        {
            if (!SwapXYAxes)
            {
                if (position.X < 0 ||
                    position.X > AreaWidth)
                {
                    return null;
                }
                var leftCoordinate = Coordinates.LastOrDefault(x => x.Offset <= position.X);
                var rightCoordinate = Coordinates.FirstOrDefault(y => y.Offset >= position.X);
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
                return Math.Abs(leftCoordinate.Offset - position.X) <= Math.Abs(rightCoordinate.Offset - position.X)
                    ? leftCoordinate
                    : rightCoordinate;
            }
            else
            {
                if (position.Y < 0 ||
                    position.Y > AreaHeight)
                {
                    return null;
                }
                var topCoordinate = Coordinates.LastOrDefault(x => x.Offset <= position.Y);
                var bottomCoordinate = Coordinates.FirstOrDefault(y => y.Offset >= position.Y);
                if (topCoordinate == null &&
                    bottomCoordinate == null)
                {
                    return null;
                }
                if (topCoordinate == null)
                {
                    return bottomCoordinate;
                }
                if (bottomCoordinate == null)
                {
                    return topCoordinate;
                }
                return Math.Abs(topCoordinate.Offset - position.Y) <= Math.Abs(bottomCoordinate.Offset - position.Y)
                    ? topCoordinate
                    : bottomCoordinate;
            }
        }

        ICartesianCoordinate ICartesianChartContext.RetrieveCoordinate(Point position)
        {
            return (ICartesianCoordinate)RetrieveCoordinate(position);
        }

        public double GetOffsetY(double value)
        {
            var minMaxDelta = MaxValue - MinValue;
            if (!_chart.SwapXYAxes)
            {
                return AreaHeight - AreaHeight * ((value - MinValue) / minMaxDelta);
            }
            else
            {
                return AreaWidth * ((value - MinValue) / minMaxDelta);
            }
        }


        #endregion
    }
}
