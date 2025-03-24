using System.Windows;

namespace Panuon.WPF.Charts
{
    public class YAxisGeneratingLabelRoutedEventArgs
        : RoutedEventArgs
    {
        #region Ctor
        public YAxisGeneratingLabelRoutedEventArgs(
            RoutedEvent @event,
            double value,
            double minValue,
            double maxValue,
            string label
        ) : base(@event)
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
            Label = label;
        }
        #endregion

        #region Properties
        public string Label { get; set; }

        public double Value { get; }

        public double MinValue { get; }

        public double MaxValue { get; }

        #endregion
    }
}
