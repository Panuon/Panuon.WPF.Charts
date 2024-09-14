using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class SegmentBase
        : DependencyObject
    {

        #region Properties

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SegmentBase), new PropertyMetadata(null));
        #endregion

        #region TitleMemberPath
        public string TitleMemberPath
        {
            get { return (string)GetValue(TitleMemberPathProperty); }
            set { SetValue(TitleMemberPathProperty, Title); }
        }

        public static readonly DependencyProperty TitleMemberPathProperty =
            DependencyProperty.Register("TitleMemberPath", typeof(string), typeof(SegmentBase), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Methods
        protected static void OnRenderPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var segments = (SegmentBase)d;
            //segments.InvalidRender();
        }
        #endregion
    }
}
