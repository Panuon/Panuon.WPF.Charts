namespace Panuon.WPF.Charts
{
    public interface ICanvasContext
    {
        double AreaWidth { get; }

        double AreaHeight { get; }

        double MinValue { get; }

        double MaxValue { get; }

        double GetOffset(double value);


    }
}
