using System;
using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts.Implements
{
    internal class RadialChartContextImpl
        : ChartContextImplBase, IRadialChartContext
    {
        #region Fields
        #endregion

        #region Ctor
        internal RadialChartContextImpl(RadialChart chart)
            : base (chart)
        {
        }
        #endregion

        #region Properties
        public new RadialChart Chart => (RadialChart)base.Chart;

        public IEnumerable<IRadialCoordinate> Coordinates => Chart.Coordinates;

        public override ICoordinate RetrieveCoordinate(Point position)
        {
            var coordinates = Coordinates;

            var areaWidth = Math.Max(0, CanvasWidth - (Chart.LabelSpacing + Chart.FontSize) * 2);
            var areaHeight = Math.Max(0, CanvasHeight - (Chart.LabelSpacing + Chart.FontSize) * 2);

            var radius = Math.Min(areaWidth, areaHeight) / 2;
            var centerX = CanvasWidth / 2;
            var centerY = CanvasHeight / 2;

            foreach (var coordinate in coordinates)
            {
                var startAngle = coordinate.StartAngle;
                var angle = coordinate.Angle;

                if (IsPointInsideSector(
                    position,
                    centerX, centerY,
                    radius,
                    startAngle, startAngle + angle))
                {
                    return coordinate;
                }
            }
            return null;
        }

        IRadialCoordinate IRadialChartContext.RetrieveCoordinate(Point offset)
        {
            return (IRadialCoordinate)RetrieveCoordinate(offset);
        }
        #endregion

        #region Functions
        private bool IsPointInsideSector(Point point,
            double centerX,
            double centerY,
            double radius,
            double startAngle,
            double endAngle
        )
        {
            var distance = Math.Sqrt(Math.Pow(point.X - centerX, 2) + Math.Pow(point.Y - centerY, 2));
            var angle = Math.Atan2(point.Y - centerY, point.X - centerX) * (180 / Math.PI);

            angle += 90;
            if (angle < 0)
            {
                angle += 360;
            }

            return distance <= radius && angle >= startAngle && angle <= endAngle;
        }
        #endregion
    }
}
