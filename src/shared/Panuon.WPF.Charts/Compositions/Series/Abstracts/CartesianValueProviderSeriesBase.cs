using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class CartesianValueProviderSeriesBase
        : CartesianSeriesBase, IChartValueProvider
    {
        #region Properties

        #region Label
        public string Label
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(CartesianValueProviderSeriesBase), new PropertyMetadata(null));
        #endregion

        #region LabelMemberPath
        public string LabelMemberPath
        {
            get { return (string)GetValue(TitleMemberPathProperty); }
            set { SetValue(TitleMemberPathProperty, Label); }
        }

        public static readonly DependencyProperty TitleMemberPathProperty =
            DependencyProperty.Register("LabelMemberPath", typeof(string), typeof(CartesianValueProviderSeriesBase), new PropertyMetadata(null));
        #endregion

        #region ValueMemberPath
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(CartesianValueProviderSeriesBase));
        #endregion

        #endregion

        #region Methods
        #endregion
    }
}