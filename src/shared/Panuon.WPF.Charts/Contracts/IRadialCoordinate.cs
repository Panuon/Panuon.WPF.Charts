namespace Panuon.WPF.Charts
{
    public interface IRadialCoordinate
        : ICoordinate
    {
        double StartAngle { get; }

        double Angle { get; }
    }
}
