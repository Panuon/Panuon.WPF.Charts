using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts
{
    public class ScaleHighlightLayer
        : HighlightLayerBase
    {
        #region Ctor
        public ScaleHighlightLayer()
        {
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        public static void Regist<TSeries>(SeriesHighlightEventHandler seriesHighlightHandler)
            where TSeries : SeriesBase
        {
            RegistHighlightHandler<ScaleHighlightLayer, TSeries>(seriesHighlightHandler);
        }
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