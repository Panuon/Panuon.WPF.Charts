namespace Panuon.WPF.Charts
{
    public interface IRadialCoordinate
        : ICoordinate
    {
        (double, double) GetAngle(IChartArgument seriesOrSegment);

        double StartAngle { get; }

        double Angle { get; }
    }
}
