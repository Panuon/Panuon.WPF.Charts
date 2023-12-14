using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class ValueProviderSeriesBase
        : SeriesBase, IChartValueProvider
    {
        #region Properties

        #region ValueMemberPath
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(ValueProviderSeriesBase));
        #endregion

        #endregion

        #region Event Handlers
        #endregion
    }
}
