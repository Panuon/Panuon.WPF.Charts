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
        : ChartElementBase, IChartArgument
    {
        #region Fields
        private ChartBase _chart;

        internal bool _isAnimationBeginCalled = false;

        private AnimationProgressObject _loadAnimationProgressObject;

        private bool _isRenderEverCalled;
        #endregion

        #region Ctor
        public SeriesBase()
        {
            Loaded += SeriesBase_Loaded;
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

        public void InvalidateLayout()
        {
            if (IsLoaded
                && (_loadAnimationProgressObject == null || _chart.AnimationMode == AnimationTriggerMode.Always))
            {
                _isAnimationBeginCalled = false;
                BeginLoadAnimation();
                return;
            }

            if (_loadAnimationProgressObject == null
                || !_isRenderEverCalled)
            {
                return;
            }

            _isAnimationBeginCalled = false;
            _loadAnimationProgressObject.BeginAnimation(AnimationProgressObject.ProgressProperty, null);
            _loadAnimationProgressObject.Progress = 1;
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
            if (_chart == null
                || !_chart.IsCanvasReady()
                || _chart.ItemsSource == null)
            {
                return;
            }

            if (_loadAnimationProgressObject == null)
            {
                return;
            }

            _isRenderEverCalled = true;

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
                animationProgress: Math.Max(0, Math.Min(1, (double)_loadAnimationProgressObject.Progress))
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
        private void SeriesBase_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SeriesBase_Loaded;

            if(_chart.ItemsSource != null
                && _loadAnimationProgressObject == null)
            {
                BeginLoadAnimation();
            }
        }

        private static void OnAnimationPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (SeriesBase)d;
            series.InvalidateVisual();
        }

        private void BeginLoadAnimation()
        {
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
                };

                _loadAnimationProgressObject.BeginAnimation(AnimationProgressObject.ProgressProperty, animation);
            }
            else
            {
                _loadAnimationProgressObject.Progress = 1;
            }
        }

        private void LoadAnimationProgressObject_ProgressChanged(object sender, EventArgs e)
        {
            base.InvalidateVisual();
        }
        #endregion

    }

}