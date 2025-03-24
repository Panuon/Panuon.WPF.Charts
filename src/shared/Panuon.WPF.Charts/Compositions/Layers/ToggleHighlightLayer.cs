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

        #region HighlightMarkerSize
        public double HighlightMarkerSize
        {
            get { return (double)GetValue(HighlightMarkerSizeProperty); }
            set { SetValue(HighlightMarkerSizeProperty, value); }
        }

        public static readonly DependencyProperty HighlightMarkerSizeProperty =
            DependencyProperty.Register("HighlightMarkerSize", typeof(double), typeof(ToggleHighlightLayer), new PropertyMetadata(6d));
        #endregion

        #region HighlightMarkerStrokeThickness
        public double HighlightMarkerStrokeThickness
        {
            get { return (double)GetValue(HighlightMarkerStrokeThicknessProperty); }
            set { SetValue(HighlightMarkerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty HighlightMarkerStrokeThicknessProperty =
            DependencyProperty.Register("HighlightMarkerStrokeThickness", typeof(double), typeof(ToggleHighlightLayer), new PropertyMetadata(2d));
        #endregion

        #region HighlightMarkerFill
        public Brush HighlightMarkerFill
        {
            get { return (Brush)GetValue(HighlightMarkerFillProperty); }
            set { SetValue(HighlightMarkerFillProperty, value); }
        }

        public static readonly DependencyProperty HighlightMarkerFillProperty =
            DependencyProperty.Register("HighlightMarkerFill", typeof(Brush), typeof(ToggleHighlightLayer), new PropertyMetadata(Brushes.White));
        #endregion

        #endregion

        #region Methods
        public static void Regist<TSeries, TChartContext>(SeriesHighlightHandler<ToggleHighlightLayer, TSeries, TChartContext> seriesHighlightHandler)
            where TSeries : SeriesBase
            where TChartContext : IChartContext
        {
            RegistHighlightHandler(seriesHighlightHandler);
        }

        public static void Regist<TSeries>(SeriesHighlightHandler<ToggleHighlightLayer, TSeries, ICartesianChartContext> seriesHighlightHandler)
            where TSeries : CartesianSeriesBase
        {
            RegistHighlightHandler(seriesHighlightHandler);
        }

        public static void Regist<TSeries>(SeriesHighlightHandler<ToggleHighlightLayer, TSeries, IRadialChartContext> seriesHighlightHandler)
            where TSeries : RadialSeriesBase
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