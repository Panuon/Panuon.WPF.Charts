using System;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public class GeneratingTitleRoutedEventArgs
        : RoutedEventArgs
    {
        #region Ctor
        public GeneratingTitleRoutedEventArgs(
            RoutedEvent @event,
            double value,
            string label
        ) : base(@event)
        {
            Value = value;
            Label = label;
        }
        #endregion

        #region Properties
        public double Value { get; }

        public string Label { get; set; }
        #endregion
    }
}
