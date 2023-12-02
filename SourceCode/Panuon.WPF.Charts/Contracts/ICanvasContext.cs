namespace Panuon.WPF.Charts
{
    public interface ICanvasContext
    {
        double AreaWidth { get; }

        double AreaHeight { get; }

        double GetOffsetX(int index);

        double GetOffsetY(double value);

    }
}
