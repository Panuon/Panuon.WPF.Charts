using System;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ToggleHighlightLayer
        : HighlightLayerBase
    {
        #region Ctor
        public ToggleHighlightLayer()
        {
        }
        #endregion

        #region Properties

        #region HighlightToggleRadius
        public double HighlightToggleRadius
        {
            get { return (double)GetValue(HighlightToggleRadiusProperty); }
            set { SetValue(HighlightToggleRadiusProperty, value); }
        }

        public static readonly DependencyProperty HighlightToggleRadiusProperty =
            DependencyProperty.Register("HighlightToggleRadius", typeof(double), typeof(ToggleHighlightLayer), new PropertyMetadata(6d));
        #endregion

        #region HighlightToggleStrokeThickness
        public double HighlightToggleStrokeThickness
        {
            get { return (double)GetValue(HighlightToggleStrokeThicknessProperty); }
            set { SetValue(HighlightToggleStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty HighlightToggleStrokeThicknessProperty =
            DependencyProperty.Register("HighlightToggleStrokeThickness", typeof(double), typeof(ToggleHighlightLayer), new PropertyMetadata(2d));
        #endregion

        #region HighlightToggleFill
        public Brush HighlightToggleFill
        {
            get { return (Brush)GetValue(HighlightToggleFillProperty); }
            set { SetValue(HighlightToggleFillProperty, value); }
        }

        public static readonly DependencyProperty HighlightToggleFillProperty =
            DependencyProperty.Register("HighlightToggleFill", typeof(Brush), typeof(ToggleHighlightLayer), new PropertyMetadata(Brushes.White));
        #endregion

        #endregion

        #region Methods
        public static void Regist<TSeries>(SeriesHighlightHandler<ToggleHighlightLayer, TSeries> seriesHighlightHandler)
            where TSeries : SeriesBase
        {
            RegistHighlightHandler(seriesHighlightHandler);
        }
        #endregion

        #region Event Handlers
        private void AnimationProgressObject_ProgressChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
        #endregion
    }
}