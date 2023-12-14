using Panuon.WPF.Charts;
using System.Windows;

namespace Panuon.WPF.Chart
{
    public abstract class ValueProviderSegmentBase
        : SegmentBase, IChartValueProvider
    {
        #region Properties

        #region ValueMemberPath
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentBase));
        #endregion

        #endregion
    }
}
