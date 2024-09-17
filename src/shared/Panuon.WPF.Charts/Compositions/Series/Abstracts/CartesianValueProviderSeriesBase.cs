using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class CartesianValueProviderSeriesBase
        : CartesianSeriesBase, IChartValueProvider
    {
        #region Properties

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CartesianValueProviderSeriesBase), new PropertyMetadata(null));
        #endregion

        #region TitleMemberPath
        public string TitleMemberPath
        {
            get { return (string)GetValue(TitleMemberPathProperty); }
            set { SetValue(TitleMemberPathProperty, Title); }
        }

        public static readonly DependencyProperty TitleMemberPathProperty =
            DependencyProperty.Register("TitleMemberPath", typeof(string), typeof(CartesianValueProviderSeriesBase), new PropertyMetadata(null));
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
    }
}
