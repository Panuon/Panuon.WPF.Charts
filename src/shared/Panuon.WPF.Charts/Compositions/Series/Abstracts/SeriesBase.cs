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
        : FrameworkElement, IChartArgument
    {
        #region Fields
        private ChartBase _chart;

        internal bool _isAnimationCompleted;

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
                || !_chart.IsCanvasReady()
                || _loadAnimationProgressObject?.Progress == null)
            {
                return Enumerable.Empty<SeriesLegendEntry>(); ;
            }

            var chartContext = _chart.GetCanvasContext();

            return OnInternalRetrieveLegendEntries(
                chartContext: chartContext
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
                || !_chart.IsCanvasReady()
                || _loadAnimationProgressObject?.Progress == null)
            {
                return;
            }

            var context = _chart.CreateDrawingContext(drawingContext);
            var chartContext = _chart.GetCanvasContext();

            if (!_isAnimationBeginCalled
                || _loadAnimationProgressObject.Progress == 1)
            {
                OnInternalRenderBegin(
                    context,
                    chartContext
                );
                _isAnimationBeginCalled = true;
            }

            OnInternalRendering(
                drawingContext: context,
                chartContext: chartContext,
                animationProgress: (double)_loadAnimationProgressObject.Progress
            );

            if (_loadAnimationProgressObject.Progress == 1)
            {
                OnInternalRenderCompleted(
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
        internal protected virtual void OnInternalRenderBegin(
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
        internal protected abstract void OnInternalRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        );

        internal protected virtual void OnInternalRenderCompleted(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
        }

        internal protected abstract IEnumerable<SeriesLegendEntry> OnInternalRetrieveLegendEntries(
            IChartContext chartContext
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