namespace Panuon.WPF.Charts
{
    public interface ICoordinate
    {
        string Title { get; }

        int Index { get; }

        double GetValue(IChartArgument seriesOrSegment);

        double OffsetX { get; }
    }
}
