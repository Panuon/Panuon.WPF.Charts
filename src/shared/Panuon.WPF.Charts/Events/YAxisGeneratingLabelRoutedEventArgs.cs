using System.Windows;

namespace Panuon.WPF.Charts
{
    public class YAxisGeneratingLabelRoutedEventArgs
        : RoutedEventArgs
    {
        #region Ctor
        public YAxisGeneratingLabelRoutedEventArgs(
            RoutedEvent @event,
            decimal value,
            decimal minValue,
            decimal maxValue,
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

        public decimal Value { get; }

        public decimal MinValue { get; }

        public decimal MaxValue { get; }

        #endregion
    }
}
