using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

        #region AnimationDuration
        public TimeSpan? AnimationDuration
        {
            get { return (TimeSpan?)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan?), typeof(ToggleHighlightLayer));
        #endregion

        #region AnimationEasing
        public AnimationEasing AnimationEasing
        {
            get { return (AnimationEasing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        public static readonly DependencyProperty AnimationEasingProperty =
            DependencyProperty.Register("AnimationEasing", typeof(AnimationEasing), typeof(ToggleHighlightLayer));
        #endregion

        #region HighlightEffect
        public HighlightEffect HighlightEffect
        {
            get { return (HighlightEffect)GetValue(HighlightEffectProperty); }
            set { SetValue(HighlightEffectProperty, value); }
        }

        public static readonly DependencyProperty HighlightEffectProperty =
            DependencyProperty.Register("HighlightEffect", typeof(HighlightEffect), typeof(ToggleHighlightLayer), new PropertyMetadata(HighlightEffect.Scale));
        #endregion

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

        #region Overrides
        protected override void OnRender(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            foreach (var seriesAnimationObjects in GetHighlightProgress())
            {
                var series = seriesAnimationObjects.Key;
                series.Highlight(
                    drawingContext,
                    chartContext,
                    layerContext,
                    seriesAnimationObjects.Value.ToDictionary(
                        kv => chartContext.Coordinates.First(c => c.Index == kv.Key),
                        kv => kv.Value
                    )
                );
            }
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