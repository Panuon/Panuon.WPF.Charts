namespace Panuon.WPF.Charts
{
    internal class CanvasContextImpl
        : ICanvasContext
    {
        #region Fields


        private double _deltaX;
        private double _minMaxDelta;
        #endregion

        #region Ctor
        internal CanvasContextImpl(double areaWidth,
            double areaHeight,
            int _coordinatesCount,
            double minValue,
            double maxValue)
        {
            AreaWidth = areaWidth;
            AreaHeight = areaHeight;
            CoordinatesCount = _coordinatesCount;
            MinValue = minValue;
            MaxValue = maxValue;

            _deltaX = AreaWidth / _coordinatesCount;

            _minMaxDelta = MaxValue - MinValue;
        }
        #endregion

        #region Properties
        public double AreaWidth { get; }

        public double AreaHeight { get; }

        public int CoordinatesCount { get; }

        public double MinValue { get; }

        public double MaxValue { get; }

        #endregion

        #region Methods
        public double GetOffset(double value)
        {
            return AreaHeight - AreaHeight * ((value - MinValue) / _minMaxDelta);
        }
        #endregion
    }
}
