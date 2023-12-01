namespace Panuon.WPF.Charts
{
    public interface ICoordinate
    {
        string Title { get; }

        int Index { get; }

        double GetValue(SeriesBase series);
    }
}
