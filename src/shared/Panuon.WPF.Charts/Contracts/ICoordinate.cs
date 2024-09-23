namespace Panuon.WPF.Charts
{
    public interface ICoordinate
    {
        string Title { get; }

        int Index { get; }

        double GetValue(IChartValueProvider seriesOrSegment);

        double OffsetX { get; }
    }
}
