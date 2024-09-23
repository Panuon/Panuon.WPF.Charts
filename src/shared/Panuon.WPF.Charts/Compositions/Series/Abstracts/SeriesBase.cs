using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Panuon.WPF.Charts
{
    public abstract class SeriesBase
        : FrameworkElement
    {
        #region Fields
        private ChartBase _chart;

        private bool _isAnimationCompleted;

        private bool _isAnimationBeginCalled = false;

        private AnimationProgressObject _loadAnimationProgressObject;
        #endregion

        #region Ctor
        public SeriesBase()
        {
            Loaded += Series_Loaded;
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        public IEnumerable<SeriesLegendEntry> RetrieveLegendEntries()
        {
            if (_chart == null
                || _chart.Coordinates == null
                || !_chart.IsCanvasReady()
                || _loadAnimationProgressObject?.Progress == null)
            {
                return Enumerable.Empty<SeriesLegendEntry>(); ;
            }

            var chartContext = _chart.GetCanvasContext();
            var layerContext = _chart.CreateLayerContext();

            return OnRetrieveLegendEntries(
                chartContext: chartContext,
                layerContext: layerContext
            );
        }

        public ICoordinate RetrieveCoordinate(
            IChartContext chartContext,
            ILayerContext layerContext,
            Point position
        )
        {
            return OnRetrieveCoordinate(
                chartContext,
                layerContext,
                position
            );
        }
        #endregion

        #region Internal Methods
        internal void OnAttached(ChartBase chart)
        {
            _chart = chart;
        }
        #endregion

        #region Overrides
        protected sealed override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_chart == null
                || _chart.Coordinates == null
                || !_chart.IsCanvasReady()
                || _loadAnimationProgressObject?.Progress == null)
            {
                return;
            }

            var context = _chart.CreateDrawingContext(drawingContext);
            var chartContext = _chart.GetCanvasContext();
            var layerContext = _chart.CreateLayerContext();

            if (!_isAnimationBeginCalled
                || _loadAnimationProgressObject.Progress == 1)
            {
                OnRenderBegin(
                    context,
                    chartContext
                );
                _isAnimationBeginCalled = true;
            }

            OnRendering(
                drawingContext: context,
                chartContext: chartContext,
                animationProgress: (double)_loadAnimationProgressObject.Progress
            );

            if (_loadAnimationProgressObject.Progress == 1)
            {
                OnRenderCompleted(
                    drawingContext: context,
                    chartContext: chartContext
                );
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        #endregion

        #region Abstract Methods
        protected virtual void OnRenderBegin(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
        }

        /// <summary>
        /// Call this method during rendering chart.
        /// </summary>
        /// <param name="drawingContext">DrawingContext.</param>
        /// <param name="chartContext">ChartContext</param>
        /// <param name="animationProgress">Animation progress. From 0 to 1.</param>
        protected abstract void OnRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        );

        protected virtual void OnRenderCompleted(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
        }

        protected abstract ICoordinate OnRetrieveCoordinate(
            IChartContext chartContext,
            ILayerContext layerContext,
            Point position
        );

        protected abstract IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries (
            IChartContext chartContext,
            ILayerContext layerContext
        );
        #endregion

        #region Event Handlers
        private static void OnAnimationPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (SeriesBase)d;
            series.InvalidateVisual();
        }

        private void Series_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Series_Loaded;

            _loadAnimationProgressObject = new AnimationProgressObject();
            _loadAnimationProgressObject.ProgressChanged += LoadAnimationProgressObject_ProgressChanged;

            _loadAnimationProgressObject.Progress = 0;
            if (_chart.AnimationDuration is TimeSpan duration
                && duration.TotalMilliseconds > 0)
            {
                var animation = new DoubleAnimation(0, 1, duration)
                {
                    EasingFunction = AnimationUtil.CreateEasingFunction(_chart.AnimationEasing)
                };
                animation.Completed += delegate
                {
                    _loadAnimationProgressObject.Progress = 1;
                    _isAnimationCompleted = true;
                };

                _loadAnimationProgressObject.BeginAnimation(AnimationProgressObject.ProgressProperty, animation);
            }
            else
            {
                _loadAnimationProgressObject.Progress = 1;
                _isAnimationCompleted = true;
            }
        }

        private void LoadAnimationProgressObject_ProgressChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
        #endregion

    }

}